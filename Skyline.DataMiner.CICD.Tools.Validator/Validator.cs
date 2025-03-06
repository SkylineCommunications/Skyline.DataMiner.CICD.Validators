namespace Skyline.DataMiner.CICD.Tools.Validator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Skyline.DataMiner.CICD.Common;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Parsers.Common.Xml;
    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Common.Suppressions;
    using Skyline.DataMiner.CICD.Validators.Common.Tools;
    using Skyline.DataMiner.XmlSchemas.Protocol;

    internal class Validator
    {
        /// <summary>
        /// Gets the minimum DataMiner version with its build number for which support is still given.
        /// </summary>
        private static string MinimumSupportedDataMinerVersionWithBuildNumber => "10.3.0.0 - 12752";

        /// <summary>
        /// Gets the minimum DataMiner version with its build number for which support is still given.
        /// </summary>
        /// <value>The minimum DataMiner version with its build number for which support is still given.</value>
        private static DataMinerVersion MinSupportedDataMinerVersionWithBuildNumber { get; } = DataMinerVersion.Parse(MinimumSupportedDataMinerVersionWithBuildNumber);

        internal async Task<ValidatorResults> ValidateProtocolSolution(string solutionFilePath, bool includeSuppressed)
        {
            string protocolCode = GetProtocolCode(solutionFilePath);

            return await ValidateProtocolSolution(solutionFilePath, protocolCode, includeSuppressed);
        }

        private static string GetProtocolCode(string protocolFolderPath)
        {
            var solutionDirectoryPath = Path.GetDirectoryName(protocolFolderPath);

            string protocolFilePath = Path.GetFullPath(Path.Combine(solutionDirectoryPath, "protocol.xml"));

            if (!File.Exists(protocolFilePath))
            {
                throw new InvalidOperationException($"protocol.xml not found. Expected location '${protocolFilePath}'.");
            }

            string protocolCode = File.ReadAllText(protocolFilePath);
            return protocolCode;
        }

        private static async Task<ValidatorResults> ValidateProtocolSolution(string solutionFilePath, string protocolCode, bool includeSuppressed)
        {
            DateTime timestamp = DateTime.Now;

            var uom = Resources.uom;
            XDocument uomDoc = XDocument.Parse(uom);

            using CancellationTokenSource cts = new CancellationTokenSource();

            var workspace = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionFilePath, cancellationToken: cts.Token);

            var parser = new Parser(protocolCode);
            var document = parser.Document;
            var model = new ProtocolModel(document);
            var inputData = new ProtocolInputData(model, document, new QActionCompilationModel(model, solution));

            ValidatorSettings settings = new ValidatorSettings(MinSupportedDataMinerVersionWithBuildNumber, new UnitList(uomDoc));

            Task<IList<IValidationResult>>[] tasks =
            {
                Task.Factory.StartNew(() =>
                {
                    // Legacy validator.
                    var validator = new Validators.Protocol.Legacy.Validator();

                    return validator.RunValidate(inputData, settings, cts.Token);
                }),
                Task.Factory.StartNew(() =>
                {
                    // New validator.
                    var validator = new Validators.Protocol.Validator();

                    return validator.RunValidate(inputData, settings, cts.Token);
                })
            };

            // Run validator tasks and combine results.
            IList<IValidationResult> validatorResults = Task.WhenAll(tasks).Result.SelectMany(x => x).ToList();

            var lineInfoProvider = new SimpleLineInfoProvider(protocolCode);

            CommentSuppressionManager suppressionManager = new CommentSuppressionManager(document, lineInfoProvider);

            var results = ConvertResults(validatorResults, suppressionManager, lineInfoProvider);

            ProcessSubresults(results, results.Issues, includeSuppressed);

            results.Protocol = model.Protocol?.Name?.Value;
            results.Version = model.Protocol?.Version?.Value;

            results.ValidatorVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            results.ValidationTimeStamp = timestamp;

            results.SuppressedIssuesIncluded = includeSuppressed;

            return results;
        }

        private static void ProcessSubresults(ValidatorResults validatorResults, List<ValidatorResult> subResults, bool includeSuppressed)
        {
            foreach (var validatorResult in subResults)
            {
                if (!validatorResult.Suppressed)
                {
                    if (validatorResult.SubResults?.Count > 0)
                    {
                        ProcessSubresults(validatorResults, validatorResult.SubResults, includeSuppressed);

                        if (!validatorResult.SubResults.Any(validatorResult => !validatorResult.Suppressed))
                        {
                            validatorResult.Suppressed = true;
                        }
                    }
                    else
                    {
                        AddIssueToCounter(validatorResults, validatorResult.Severity, false);
                    }
                }
                else
                {
                    AddIssueToCounter(validatorResults, validatorResult.Severity, true);
                }
            }

            if (subResults.Count > 0 && !includeSuppressed)
            {
                subResults.RemoveAll(v => v.Suppressed);
            }
        }

        private static string BuildIssueMessage(IValidationResult result)
        {
            if (result.DescriptionParameters != null)
            {
                string[] parameters = result.DescriptionParameters.Select(p => p?.ToString()).ToArray();
                return String.Format(result.DescriptionFormat, parameters);
            }

            if (result.DescriptionFormat != null)
            {
                // case when <d2p1:DescriptionParameters i:nil="true" />
                return result.DescriptionFormat;
            }

            return result.Description;
        }

        private static ValidatorResults ConvertResults(IList<IValidationResult> results, CommentSuppressionManager suppressionManager, ILineInfoProvider lineInfoProvider)
        {
            ValidatorResults validatorResults = new ValidatorResults();
            validatorResults.Issues = new List<ValidatorResult>();

            foreach (var result in results)
            {
                bool isSuppressed = suppressionManager.IsSuppressed(result);

                var validatorResult = CreateValidatorResult(result, isSuppressed, lineInfoProvider);

                if (result?.SubResults?.Count > 0)
                {
                    AddSubResults(validatorResult, result, isSuppressed, suppressionManager, lineInfoProvider);
                }

                validatorResults.Issues.Add(validatorResult);
            }

            return validatorResults;
        }

        private static void AddSubResults(ValidatorResult validatorResult, IValidationResult result, bool isSuppressed, CommentSuppressionManager suppressionManager, ILineInfoProvider lineInfoProvider)
        {
            validatorResult.SubResults = new List<ValidatorResult>();

            foreach (var subresult in result.SubResults)
            {
                bool isSubResultSuppressed;
                if (!isSuppressed)
                {
                    isSubResultSuppressed = suppressionManager.IsSuppressed(subresult);
                }
                else
                {
                    isSubResultSuppressed = true;
                }

                var r = CreateValidatorResult(subresult, isSubResultSuppressed, lineInfoProvider);

                validatorResult.SubResults.Add(r);

                if (subresult.SubResults != null && subresult.SubResults.Count > 0)
                {
                    AddSubResults(r, subresult, isSubResultSuppressed, suppressionManager, lineInfoProvider);
                }
            }
        }

        private static void AddIssueToCounter(ValidatorResults validatorResults, Severity severity, bool isSuppressed)
        {
            if (isSuppressed)
            {
                switch (severity)
                {
                    case Severity.Critical:
                        validatorResults.SuppressedCriticalIssueCount++;
                        break;
                    case Severity.Major:
                        validatorResults.SuppressedMajorIssueCount++;
                        break;
                    case Severity.Minor:
                        validatorResults.SuppressedMinorIssueCount++;
                        break;
                    case Severity.Warning:
                        validatorResults.SuppressedWarningIssueCount++;
                        break;
                }
            }
            else
            {
                switch (severity)
                {
                    case Severity.Critical:
                        validatorResults.CriticalIssueCount++;
                        break;
                    case Severity.Major:
                        validatorResults.MajorIssueCount++;
                        break;
                    case Severity.Minor:
                        validatorResults.MinorIssueCount++;
                        break;
                    case Severity.Warning:
                        validatorResults.WarningIssueCount++;
                        break;
                }
            }
        }

        private static ValidatorResult CreateValidatorResult(IValidationResult result, bool isSuppressed, ILineInfoProvider lineInfoProvider)
        {
            string message = BuildIssueMessage(result);

            var validatorResult = new ValidatorResult
            {
                Certainty = result.Certainty,
                FixImpact = result.FixImpact,
                Category = result.Category,
                Description = message,
                Severity = result.Severity,
                Id = result.FullId ?? Convert.ToString(result.ErrorId),
                Dve = result.DveExport != null ? $"{result.DveExport.Value.Name} [{result.DveExport.Value.TablePid}]" : null,
                Suppressed = isSuppressed
            };

            GetPosition(result, validatorResult, lineInfoProvider);

            return validatorResult;
        }

        private static void GetPosition(IValidationResult result, ValidatorResult validatorResult, ILineInfoProvider lineInfoProvider)
        {
            if (result.Position > 0 && result.Line <= 0)
            {
                try
                {
                    lineInfoProvider.GetLineAndColumn(result.Position, out int line, out int col);
                    validatorResult.Line = line + 1;
                    validatorResult.Column = col + 1;
                }
                catch (Exception)
                {
                    // Do nothing.
                }
            }
            else if (result.Position <= 0 && result.Line > 0)
            {
                try
                {
                    int pos = lineInfoProvider.GetOffset(result.Line - 1, 0);
                    pos += lineInfoProvider.GetFirstNonWhitespaceOffset(result.Line - 1) ?? 0;

                    lineInfoProvider.GetLineAndColumn(pos, out int line, out int col);
                    validatorResult.Line = line + 1;
                    validatorResult.Column = col + 1;
                }
                catch (Exception)
                {
                    // Do nothing.
                }
            }
        }
    }
}

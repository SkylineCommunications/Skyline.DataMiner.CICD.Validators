using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Build.Locator;
using Skyline.DataMiner.CICD.Common;
using Skyline.DataMiner.CICD.Models.Protocol;
using Skyline.DataMiner.CICD.Models.Protocol.Read;
using Skyline.DataMiner.CICD.Parsers.Common.Xml;
using Skyline.DataMiner.CICD.Validators.Common.Data;
using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
using Skyline.DataMiner.CICD.Validators.Common.Model;
using Skyline.DataMiner.CICD.Validators.Common.Suppressions;
using Skyline.DataMiner.CICD.Validators.Common.Tools;
using Skyline.DataMiner.Net.SLDataGateway.API.Collections.Concurrent;
using Skyline.DataMiner.XmlSchemas.Protocol;


namespace Skyline.DataMiner.CICD.Tools.Validator
{
    internal class MajorChangeChecker
    {
        private static string MinimumSupportedDataMinerVersionWithBuildNumber => "10.5.5.0 - 15690";
        private static DataMinerVersion MinSupportedDataMinerVersionWithBuildNumber { get; } = DataMinerVersion.Parse(MinimumSupportedDataMinerVersionWithBuildNumber);


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
        internal async Task<MajorChangeCheckerResults> CheckMajorChanges(string newSolutionFilePath, string oldProtocolCode, bool includeSuppressed)
        {
            string newProtocolCode = GetProtocolCode(newSolutionFilePath);
            return await CheckMajorChanges(newSolutionFilePath, newProtocolCode, oldProtocolCode, includeSuppressed);
        }


        private static async Task<MajorChangeCheckerResults> CheckMajorChanges(string newSolutionFilePath, string newProtocolCode, string oldProtocolCode, bool includeSuppressed)
        {
            if (!MSBuildLocator.IsRegistered)
            {
                MSBuildLocator.RegisterDefaults();
            }

            DateTime timestamp = DateTime.Now;

            var uom = Resources.uom;
            XDocument uomDoc = XDocument.Parse(uom);

            using CancellationTokenSource cts = new CancellationTokenSource();

            
            var newParser = new Parser(newProtocolCode);
            var newDocument = newParser.Document;
            var newModel = new ProtocolModel(newDocument);

            var oldParser = new Parser(oldProtocolCode);
            var oldDocument = oldParser.Document;
            var oldModel = new ProtocolModel(oldDocument);

            var workspace = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(newSolutionFilePath, cancellationToken: cts.Token);

            var newInputData = new ProtocolInputData(newModel, newDocument, new QActionCompilationModel(newModel, solution));
            var oldInputData = new ProtocolInputData(oldModel, oldDocument);

            ValidatorSettings settings = new ValidatorSettings(MinSupportedDataMinerVersionWithBuildNumber, new UnitList(uomDoc));

            var checker = new Validators.Protocol.Validator();
            IList<IValidationResult> MajorChangeCheckerResults = checker.RunCompare(newInputData, oldInputData, settings, cts.Token);

            var lineInfoProvider = new SimpleLineInfoProvider(newProtocolCode);
            CommentSuppressionManager suppressionManager = new CommentSuppressionManager(newDocument, lineInfoProvider);

            var MajorChangeCheckerresults = ConvertResults(MajorChangeCheckerResults, suppressionManager, lineInfoProvider);
            ProcessSubresults(MajorChangeCheckerresults, MajorChangeCheckerresults.Issues, includeSuppressed);

            MajorChangeCheckerresults.NewProtocol = newModel.Protocol?.Name?.Value;
            MajorChangeCheckerresults.NewVersion = newModel.Protocol?.Version?.Value;
            MajorChangeCheckerresults.OldProtocol = oldModel.Protocol?.Name?.Value;
            MajorChangeCheckerresults.OldVersion = oldModel.Protocol?.Version?.Value;
            MajorChangeCheckerresults.MCCVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            MajorChangeCheckerresults.ValidationTimeStamp = timestamp;
            MajorChangeCheckerresults.SuppressedIssuesIncluded = includeSuppressed;
            return MajorChangeCheckerresults;
        }

        private static void ProcessSubresults(MajorChangeCheckerResults majorChangeCheckerResults, List<MajorChangeCheckerResult> subResults, bool includeSuppressed)
        {
            foreach (var majorChangeCheckerResult in subResults)
            {
                if (!majorChangeCheckerResult.Suppressed)
                {
                    if (majorChangeCheckerResult.SubResults?.Count > 0)
                    {
                        ProcessSubresults(majorChangeCheckerResults, majorChangeCheckerResult.SubResults, includeSuppressed);

                        if (!majorChangeCheckerResult.SubResults.Any(result => !result.Suppressed))
                        {
                            majorChangeCheckerResult.Suppressed = true;
                        }
                    }
                    else
                    {
                        AddIssueToCounter(majorChangeCheckerResults, majorChangeCheckerResult.Severity, false);
                    }
                }
                else
                {
                    AddIssueToCounter(majorChangeCheckerResults, majorChangeCheckerResult.Severity, true);
                }
            }

            if (subResults.Count > 0 && !includeSuppressed)
            {
                subResults.RemoveAll(v => v.Suppressed);
            }
        }
        private static void AddIssueToCounter(MajorChangeCheckerResults majorChangeCheckerResults, Severity severity, bool isSuppressed)
        {
            if (isSuppressed)
            {
                switch (severity)
                {
                    case Severity.Critical:
                        majorChangeCheckerResults.SuppressedCriticalIssueCount++;
                        break;
                    case Severity.Major:
                        majorChangeCheckerResults.SuppressedMajorIssueCount++;
                        break;
                    case Severity.Minor:
                        majorChangeCheckerResults.SuppressedMinorIssueCount++;
                        break;
                    case Severity.Warning:
                        majorChangeCheckerResults.SuppressedWarningIssueCount++;
                        break;
                }
            }
            else
            {
                switch (severity)
                {
                    case Severity.Critical:
                        majorChangeCheckerResults.CriticalIssueCount++;
                        break;
                    case Severity.Major:
                        majorChangeCheckerResults.MajorIssueCount++;
                        break;
                    case Severity.Minor:
                        majorChangeCheckerResults.MinorIssueCount++;
                        break;
                    case Severity.Warning:
                        majorChangeCheckerResults.WarningIssueCount++;
                        break;
                }
            }
        }
        private static MajorChangeCheckerResults ConvertResults(IList<IValidationResult> results, CommentSuppressionManager suppressionManager, ILineInfoProvider lineInfoProvider)
        {
            MajorChangeCheckerResults majorChangeCheckerResults = new MajorChangeCheckerResults();
            majorChangeCheckerResults.Issues = new List<MajorChangeCheckerResult>();

            foreach (var result in results)
            {
                bool isSuppressed = suppressionManager.IsSuppressed(result);

                var majorChangeCheckerResult = CreateMajorChangeCheckerResult(result, isSuppressed, lineInfoProvider);

                if (result?.SubResults?.Count > 0)
                {
                    AddSubResults(majorChangeCheckerResult, result, isSuppressed, suppressionManager, lineInfoProvider);
                }

                majorChangeCheckerResults.Issues.Add(majorChangeCheckerResult);
            }

            return majorChangeCheckerResults;
        }
        private static MajorChangeCheckerResult CreateMajorChangeCheckerResult(IValidationResult result, bool isSuppressed, ILineInfoProvider lineInfoProvider)
        {
            string message = BuildIssueMessage(result);
            
            var majorChangeCheckerResult = new MajorChangeCheckerResult
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

            GetPosition(result, majorChangeCheckerResult, lineInfoProvider);

            return majorChangeCheckerResult;
        }
        private static void AddSubResults(MajorChangeCheckerResult majorChangeCheckerResult, IValidationResult result, bool isSuppressed, CommentSuppressionManager suppressionManager, ILineInfoProvider lineInfoProvider)
        {
            majorChangeCheckerResult.SubResults = new List<MajorChangeCheckerResult>();

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

                var r = CreateMajorChangeCheckerResult(subresult, isSubResultSuppressed, lineInfoProvider);

                majorChangeCheckerResult.SubResults.Add(r);

                if (subresult.SubResults != null && subresult.SubResults.Count > 0)
                {
                    AddSubResults(r, subresult, isSubResultSuppressed, suppressionManager, lineInfoProvider);
                }
            }
        }
        private static void GetPosition(IValidationResult result, MajorChangeCheckerResult majorChangeCheckerResult, ILineInfoProvider lineInfoProvider)
        {
            if (result.Position > 0 && result.Line <= 0)
            {
                try
                {
                    lineInfoProvider.GetLineAndColumn(result.Position, out int line, out int col);
                    majorChangeCheckerResult.Line = line + 1;
                    majorChangeCheckerResult.Column = col + 1;
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
                    majorChangeCheckerResult.Line = line + 1;
                    majorChangeCheckerResult.Column = col + 1;
                }
                catch (Exception)
                {
                    // Do nothing.
                }
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
                
                return result.DescriptionFormat;
            }

            return result.Description;
        }
    }
}

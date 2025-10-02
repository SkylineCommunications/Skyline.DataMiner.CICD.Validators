namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.Results
{
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Common.Suppressions;

    internal static class ResultsConverter
    {
        public static void ConvertResults(ValidatorResults validatorResults, IList<IValidationResult> results, ISuppressionManager suppressionManager, ILineInfoProvider lineInfoProvider)
        {
            foreach (var result in results)
            {
                bool isSuppressed = suppressionManager.IsSuppressed(result);

                var validatorResult = CreateValidatorResult(result, isSuppressed, lineInfoProvider);

                if (result.SubResults?.Count > 0)
                {
                    AddSubResults(validatorResult, result, isSuppressed, suppressionManager, lineInfoProvider);
                }

                validatorResults.Issues.Add(validatorResult);
            }
        }

        public static void ProcessSubResults(ValidatorResults validatorResults, List<ValidatorResult> subResults, bool includeSuppressed)
        {
            foreach (var validatorResult in subResults)
            {
                if (!validatorResult.Suppressed)
                {
                    if (validatorResult.SubResults?.Count > 0)
                    {
                        ProcessSubResults(validatorResults, validatorResult.SubResults, includeSuppressed);

                        if (validatorResult.SubResults.All(result => result.Suppressed))
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

        private static string BuildIssueMessage(IValidationResult result)
        {
            if (result.DescriptionParameters != null)
            {
                object?[] parameters = result.DescriptionParameters.Select(p => p?.ToString()).ToArray<object?>();
                return String.Format(result.DescriptionFormat, parameters);
            }

            if (result.DescriptionFormat != null)
            {
                // case when <d2p1:DescriptionParameters i:nil="true" />
                return result.DescriptionFormat;
            }

            return result.Description;
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
            if (result is { Position: > 0, Line: <= 0 })
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
            else if (result is { Position: <= 0, Line: > 0 })
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

        private static void AddSubResults(ValidatorResult validatorResult, IValidationResult result, bool isSuppressed, ISuppressionManager suppressionManager, ILineInfoProvider lineInfoProvider)
        {
            validatorResult.SubResults = [];

            foreach (var validationResult in result.SubResults)
            {
                bool isSubResultSuppressed = isSuppressed || suppressionManager.IsSuppressed(validationResult);

                var r = CreateValidatorResult(validationResult, isSubResultSuppressed, lineInfoProvider);

                validatorResult.SubResults.Add(r);

                if (validationResult.SubResults is { Count: > 0 })
                {
                    AddSubResults(r, validationResult, isSubResultSuppressed, suppressionManager, lineInfoProvider);
                }
            }
        }
    }
}
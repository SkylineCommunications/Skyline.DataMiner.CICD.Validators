namespace Skyline.DataMiner.CICD.Validators.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    /// <inheritdoc cref="IValidator"/>
    public class Validator : IValidator
    {
        /// <inheritdoc cref="IValidator.RunValidate"/>
        public IList<IValidationResult> RunValidate(IProtocolInputData input, ValidatorSettings validatorSettings, CancellationToken cancellationToken)
        {
            var results = new List<IValidationResult>();

            var allTests = TestCollector.GetAllValidateTests();

            if (validatorSettings.TestsToExecute != null && validatorSettings.TestsToExecute.Count > 0)
            {
                allTests = allTests.Filter(validatorSettings.TestsToExecute);
            }

            // validate main protocol
            results.AddRange(ValidateProtocol(input, cancellationToken, validatorSettings, allTests));

            // validate exported protocols, if any
            foreach (var ep in input.Model.GetAllExportedProtocols())
            {
                // No QActionCompilationModel needed as exported protocols don't have QActions anyway.
                var exportInput = new ProtocolInputData(ep.Model, ep.Document);
                var exportResults = ValidateProtocol(exportInput, cancellationToken, validatorSettings, allTests);

                // indicate for which DVE these results were created
                foreach (IValidationResult exportResult in exportResults)
                {
                    exportResult.WithDveExport(ep.TablePid, ep.Name);
                }

                results.AddRange(exportResults);
            }

            // bubble up
            foreach (var result in results.OfType<ISeverityBubbleUpResult>())
            {
                result.DoSeverityBubbleUp();
            }

            return results;
        }

        private static IList<IValidationResult> ValidateProtocol(IProtocolInputData input, CancellationToken cancellationToken, ValidatorSettings validatorSettings, TestCollection<IValidate> tests)
        {
            var results = new List<IValidationResult>();

            var context = new ValidatorContext(input, validatorSettings);

            if (context.ProtocolModel == null)
            {
                results.Add(new ValidationResult
                {
                    Severity = Severity.Critical,
                    Description = "ProtocolModel was null. Try again.",
                    FullId = "0.0.0"
                });

                return results;
            }

            if (!context.CompiledQActions.Any() && context.ProtocolModel?.Protocol?.QActions?.Any() == true)
            {
                // No compiled QActions are found (no compilation model provider available) but there are actually QActions.
                results.Add(new ValidationResult
                {
                    Severity = Severity.Warning,
                    Description = "C# checks could not be executed.",
                    FullId = "0.0.1"
                });
            }

            try
            {
                foreach ((IValidate test, TestAttribute _) in tests.Tests)
                {
                    RunValidateTest(test, context, results);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                results.Add(new ValidationResult
                {
                    Severity = Severity.Critical,
                    Description = $"Unexpected exception: {ex}",
                    FullId = "0.0.0"
                });
            }

            return results;
        }

        /// <inheritdoc cref="IValidator.RunCompare"/>
        public IList<IValidationResult> RunCompare(IProtocolInputData newData, IProtocolInputData oldData, ValidatorSettings validatorSettings, CancellationToken cancellationToken)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            MajorChangeCheckContext context = new MajorChangeCheckContext(newData, oldData, validatorSettings);

            try
            {
                TestCollection<ICompare> allTests = TestCollector.GetAllCompareTests();

                foreach ((ICompare test, TestAttribute _) in allTests.Tests)
                {
                    RunCompareTest(test, context, results);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                results.Add(new ValidationResult
                {
                    Description = $"Unexpected exception: {ex}",
                    Severity = Severity.Critical,
                    FullId = "0.0.0",
                });
            }

            foreach (ISeverityBubbleUpResult result in results.OfType<ISeverityBubbleUpResult>())
            {
                result.DoSeverityBubbleUp();
            }

            return results;
        }

        /// <inheritdoc cref="IValidator.ExecuteCodeFix"/>
        public ICodeFixResult ExecuteCodeFix(Skyline.DataMiner.CICD.Parsers.Common.XmlEdit.XmlDocument document, Skyline.DataMiner.CICD.Models.Protocol.Edit.Protocol protocol, IValidationResult result, ValidatorSettings validatorSettings)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (protocol == null)
            {
                throw new ArgumentNullException(nameof(protocol));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            ICodeFixResult codeFixResult = null;
            string message = null;

            if (result is ValidationResult validationResult)
            {
                var context = new CodeFixContext(document, protocol, validationResult, validatorSettings);

                if (validationResult.Test is ICodeFix codeFix)
                {
                    try
                    {
                        codeFixResult = codeFix.Fix(context);
                    }
                    catch (NotImplementedException)
                    {
                        message = $"CodeFix '{codeFix.GetType().Name}' is currently not implemented yet.";
                    }
                    catch (Exception ex)
                    {
                        message = $"Unexpected exception in CodeFix '{codeFix.GetType().Name}': {ex}";
                    }
                }
                else
                {
                    message = $"Error ({validationResult.ErrorId}) has no CodeFix.";
                }
            }
            else
            {
                message = "This should never happen!! Result isn't a ValidationResult.";
            }

            return codeFixResult ?? new CodeFixResult
            {
                Message = message
            };
        }

        internal static IEnumerable<IValidationResult> GroupResults(IEnumerable<IValidationResult> results)
        {
            var groups = results.GroupBy(r => r.GroupDescription);
            foreach (var group in groups)
            {
                var groupResults = group.ToList();
                if (String.IsNullOrWhiteSpace(group.Key) || groupResults.Count < 10)
                {
                    foreach (var result in groupResults)
                    {
                        yield return result;
                    }
                }
                else
                {
                    var firstResult = groupResults.First();

                    ValidationResult parentResult = new ValidationResult
                    {
                        Test = ((ValidationResult)firstResult).Test,
                        CheckId = firstResult.CheckId,
                        ErrorId = firstResult.ErrorId,
                        FullId = firstResult.FullId,
                        Category = firstResult.Category,
                        Severity = firstResult.Severity,
                        Certainty = firstResult.Certainty,
                        Source = firstResult.Source,
                        FixImpact = firstResult.FixImpact,
                        Description = firstResult.GroupDescription,
                        HowToFix = firstResult.HowToFix,
                        HasCodeFix = false,

                        //Position = firstResult.Position,
                        //Node = firstResult.Node,

                        SubResults = groupResults,
                        //ExtraData = firstResult.ExtraData,
                    };

                    yield return parentResult;
                }
            }
        }

        private static void RunValidateTest(IValidate test, ValidatorContext context, List<IValidationResult> results)
        {
            try
            {
                IEnumerable<IValidationResult> groupedResults = GroupResults(test.Validate(context));
                results.AddRange(groupedResults);
            }
            catch (NotImplementedException)
            {
                results.Add(new ValidationResult
                {
                    Severity = Severity.Critical,
                    Description = $"Test '{test.GetType().Name}' is currently not implemented yet.",
                    FullId = "0.0.0"
                });
            }
            catch (ReflectionTypeLoadException rtlex)
            {
                Type testType = test.GetType();

                string description = $"Unexpected exception in Test '{testType.Name}': {rtlex.Message}";
                description = rtlex.LoaderExceptions.Aggregate(description, (current, le) => current + $"{Environment.NewLine}LoaderException:{Environment.NewLine}{le.Message}");

                results.Add(new ValidationResult
                {
                    Severity = Severity.Critical,
                    Description = description,
                    FullId = "0.0.0"
                });
            }
            catch (Exception ex)
            {
                Type testType = test.GetType();

                string description = $"Unexpected exception in Test '{testType.Name}': {ex}";
                results.Add(new ValidationResult
                {
                    Severity = Severity.Critical,
                    Description = description,
                    FullId = "0.0.0"
                });
            }
        }

        private static void RunCompareTest(ICompare test, MajorChangeCheckContext context, List<IValidationResult> results)
        {
            try
            {
                results.AddRange(test.Compare(context));
            }
            catch (NotImplementedException)
            {
                results.Add(new ValidationResult
                {
                    Severity = Severity.Critical,
                    Description = $"Compare '{test.GetType().Name}' is currently not implemented yet.",
                    FullId = "0.0.0"
                });
            }
            catch (Exception ex)
            {
                results.Add(new ValidationResult
                {
                    Severity = Severity.Critical,
                    Description = $"Unexpected exception in Compare '{test.GetType().Name}': {ex}",
                    FullId = "0.0.0"
                });
            }
        }
    }
}
namespace Skyline.DataMiner.CICD.Tools.Validator.Commands.Validate
{
    using System.Diagnostics;
    using System.Xml.Linq;

    using Microsoft.Build.Locator;

    using Skyline.DataMiner.CICD.Parsers.Common.VisualStudio;
    using Skyline.DataMiner.CICD.Parsers.Common.VisualStudio.Projects;
    using Skyline.DataMiner.CICD.Tools.Validator.Helpers;
    using Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.Results;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Common.Suppressions;
    using Skyline.DataMiner.XmlSchemas.Protocol;

    internal class ValidateProtocolSolutionCommand : BaseCommand
    {
        public ValidateProtocolSolutionCommand() :
            base(name: "protocol-solution", description: "Validates a protocol solution.")
        {
            AddOption(new Option<bool>(
                aliases: ["--perform-restore", "-pr"],
                description: "Specifies whether to perform a dotnet restore operation.",
                getDefaultValue: () => true));

            AddOption(new Option<int>(
                aliases: ["--restore-timeout", "-rt"],
                description: "Specifies the timeout for the restore operation (in ms).",
                getDefaultValue: () => 300_000));
        }
    }

    internal class ValidateProtocolSolutionCommandHandler(ILogger<ValidateProtocolSolutionCommandHandler> logger) : BaseCommandHandler(logger)
    {
        /*
         * Automatic binding with System.CommandLine.NamingConventionBinder
         * The property names need to match with the command line argument names.
         * Example: --example-package-file will bind to ExamplePackageFile
         */

        public required bool PerformRestore { get; set; }

        public required int RestoreTimeout { get; set; }
        
        public override async Task<int> InvokeAsync(InvocationContext context)
        {
            logger.LogDebug("Starting {Method}...", nameof(ValidateProtocolSolutionCommand));

            try
            {
                IFileInfoIO solutionFile = GetSolutionFile();

                // Required for both building solution and for loading the MSBuildWorkspace to perform validation.
                if (!MSBuildLocator.IsRegistered)
                {
                    MSBuildLocator.RegisterDefaults();
                }

                // Check SDK style solution
                Solution solution = Solution.Load(solutionFile.FullName);
                if (solution.Projects.Select(solution.LoadProject).Any(project => project.ProjectStyle == ProjectStyle.Legacy))
                {
                    logger.LogError("No validation performed. This tool can only validate solutions that use the SDK-style project format. This solution uses the legacy-style project format. Consider migrating to the SDK-style project format.");
                    return (int)ExitCodes.Fail;
                }

                // Restore if needed
                if (PerformRestore)
                {
                    logger.LogInformation("Performing 'dotnet restore' on '{SolutionFile}'...", solutionFile);
                    RestoreSolution(solutionFile);
                }

                (IProtocolInputData inputData, ILineInfoProvider lineInfoProvider) =
                    await InputDataAndLineInfoHelper.GetDataBasedOnSolutionAsync(solutionFile.FullName, cancellationToken: context.GetCancellationToken());

                ValidatorSettings settings = new ValidatorSettings(Globals.MinSupportedDataMinerVersionWithBuildNumber, new UnitList(XDocument.Parse(Resources.uom)));

                Stopwatch sw = Stopwatch.StartNew();
                Task<IList<IValidationResult>>[] tasks =
                [
                    Task.Factory.StartNew(() =>
                    {
                        // Legacy validator.
                        var validator = new Validators.Protocol.Legacy.Validator();

                        return validator.RunValidate(inputData, settings, context.GetCancellationToken());
                    }, context.GetCancellationToken()),
                    Task.Factory.StartNew(() =>
                    {
                        // New validator.
                        var validator = new Validators.Protocol.Validator();

                        return validator.RunValidate(inputData, settings, context.GetCancellationToken());
                    }, context.GetCancellationToken())
                ];

                IList<IValidationResult> validatorResults = Task.WhenAll(tasks).Result.SelectMany(x => x).ToList();
                sw.Stop();

                // Validate only has suppression in the comments
                ISuppressionManager suppressionManager = new CommentSuppressionManager(inputData.Document, lineInfoProvider);

                ValidatorResults results = new ValidatorResults(inputData);

                ResultsConverter.ConvertResults(results, validatorResults, suppressionManager, lineInfoProvider);
                ResultsConverter.ProcessSubResults(results, results.Issues, IncludeSuppressed ?? false);
 
                results.ValidationTimeStamp = DateTime.Now;
                results.SuppressedIssuesIncluded = IncludeSuppressed ?? false;

                logger.LogInformation("Validation completed.");

                logger.LogInformation("  Detected {ResultsCriticalIssueCount} critical issue(s).", results.CriticalIssueCount);
                logger.LogInformation("  Detected {ResultsMajorIssueCount} major issue(s).", results.MajorIssueCount);
                logger.LogInformation("  Detected {ResultsMinorIssueCount} minor issue(s).", results.MinorIssueCount);
                logger.LogInformation("  Detected {ResultsWarningIssueCount} warning issue(s).", results.WarningIssueCount);

                logger.LogInformation("  Time elapsed: {ElapsedTime}", sw.Elapsed);

                if (String.IsNullOrWhiteSpace(OutputFileName))
                {
                    OutputFileName = $"ValidatorResults_{results.Protocol}_{results.Version}";
                }
                
                logger.LogInformation("Writing results...");

                OutputDirectory.Create();
                foreach (var writer in GetResultWriters(OutputFileName))
                {
                    writer.WriteResults(results);
                }

                logger.LogInformation("Writing results completed");

                logger.LogInformation("Finished");
                return (int)ExitCodes.Ok;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed the validation of the protocol solution.");
                return (int)ExitCodes.UnexpectedException;
            }
            finally
            {
                logger.LogDebug("Finished {Method}.", nameof(ValidateProtocolSolutionCommand));
            }
        }

        private void RestoreSolution(IFileInfoIO solutionFile)
        {
            using Process process = new Process();
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"restore \"{solutionFile.FullName}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

#pragma warning disable CA2254
            process.OutputDataReceived += (_, args) =>
            {
                if (args.Data != null)
                {
                    logger.LogInformation("Restore output: {Output}", args.Data);
                }
            };
            process.ErrorDataReceived += (_, args) =>
            {
                if (args.Data != null)
                {
                    logger.LogError("Restore error: {Error}", args.Data);
                }
            };
#pragma warning restore CA2254

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            bool exited = process.WaitForExit(RestoreTimeout);

            if (exited)
            {
                // See https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.exitcode?view=net-8.0#remarks
                process.WaitForExit();
            }

            if (!exited)
            {
                throw new TimeoutException("The restore of the solution timed out.");
            }

            var exitCode = process.ExitCode;

            if (exitCode != 0)
            {
                throw new InvalidOperationException("Could not restore the solution.");
            }
        }
    }
}
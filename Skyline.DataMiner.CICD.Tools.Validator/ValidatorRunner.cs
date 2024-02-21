namespace Skyline.DataMiner.CICD.Tools.Validator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Build.Locator;
    using Microsoft.Extensions.Logging;

    using Skyline.DataMiner.CICD.Parsers.Common.VisualStudio;
    using Skyline.DataMiner.CICD.Parsers.Common.VisualStudio.Projects;
    using Skyline.DataMiner.CICD.Tools.Reporter;
    using Skyline.DataMiner.CICD.Tools.Validator.OutputWriters;

    internal class ValidatorRunner
    {
        private readonly ILogger logger;

        public ValidatorRunner()
        {
            using (ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddSimpleConsole(options => { options.IncludeScopes = true; options.SingleLine = true; })))
            {
                logger = factory.CreateLogger("Validator");
            }
        }

        /// <summary>
        /// Validates the specified solution.
        /// </summary>
        /// <param name="solutionPath">The solution (.sln) file path.</param>
        /// <param name="validatorResultsOutputDirectory">The output directory.</param>
        /// <param name="validatorResultsFileName">The file name of the results.</param>
        /// <param name="outputFormats">The output formats.</param>
        /// <param name="includeSuppressed"><c>true</c> to include suppressed results; otherwise, <c>false</c>.</param>
        /// <param name="performRestore"><c>true</c> to perform a restore of the NuGet packages; otherwise, <c>false</c>.</param>
        /// <param name="restoreTimeout">The restore timeout.</param>
        /// <returns>Result value 0 indicates success.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="solutionPath"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid solution file path.</exception>
        public async Task<int> ValidateProtocolSolution(string solutionPath, string validatorResultsOutputDirectory, string validatorResultsFileName, string[] outputFormats, bool includeSuppressed, bool performRestore, int restoreTimeout)
        {
            if (solutionPath == null) throw new ArgumentNullException(nameof(solutionPath));

            if (String.IsNullOrEmpty(solutionPath)) throw new ArgumentException("Invalid solution path.", nameof(solutionPath));

            solutionPath = Path.GetFullPath(solutionPath);

            string solutionFilePath;

            if (File.Exists(solutionPath))
            {
                solutionFilePath = solutionPath;
            }
            else if (Directory.Exists(solutionPath))
            {
                var slnFiles = Directory.GetFiles(solutionPath, "*.sln", SearchOption.TopDirectoryOnly);

                if (slnFiles.Length == 0)
                {
                    throw new ArgumentException($"The specified solution path '{solutionPath}' does not contain a .sln file.");
                }

                if (slnFiles.Length > 1)
                {
                    throw new ArgumentException($"The specified solution path '{solutionPath}' contains multiple .sln files. Specify the full path of the solution you want to validate.");
                }

                solutionFilePath = slnFiles[0];
            }
            else
            {
                throw new ArgumentException($"The specified solution path '{solutionPath}' does not exist.", nameof(solutionPath));
            }

            // Required for both building solution and for loading the MSBuildWorkspace to perform validation.
            if (!MSBuildLocator.IsRegistered)
            {
                MSBuildLocator.RegisterDefaults();
            }

            var solution = Solution.Load(solutionFilePath);
            bool isLegacyStyleSolution = IsLegacyStyleSolution(solution);

            if (isLegacyStyleSolution)
            {
                logger.LogError("No validation performed. This tool can only validate solutions that use the SDK-style project format. This solution uses the legacy-style project format. Consider migrating to the SDK-style project format.");
                return 1;
            }

            if (performRestore)
            {
                logger.LogInformation($"Performing 'dotnet restore' on '{solutionFilePath}'...");
                RestoreSolution(solutionFilePath, restoreTimeout);
            }

            Validator validatorRunner = new Validator();
            logger.LogInformation($"Validating protocol solution '{solutionFilePath}'...");
            Stopwatch sw = Stopwatch.StartNew();
            var validatorResults = await validatorRunner.ValidateProtocolSolution(solutionFilePath, includeSuppressed);

            if (String.IsNullOrWhiteSpace(validatorResultsFileName))
            {
                validatorResultsFileName = $"ValidatorResults_{validatorResults.Protocol}_{validatorResults.Version}";
            }

            sw.Stop();

            logger.LogInformation("Validation completed.");

            logger.LogInformation($"  Detected {validatorResults.CriticalIssueCount} critical issue(s).");
            logger.LogInformation($"  Detected {validatorResults.MajorIssueCount} major issue(s).");
            logger.LogInformation($"  Detected {validatorResults.MinorIssueCount} minor issue(s).");
            logger.LogInformation($"  Detected {validatorResults.WarningIssueCount} warning issue(s).");

            logger.LogInformation("  Time elapsed: " + sw.Elapsed);

            List<IResultWriter> resultWriters = new List<IResultWriter>();

            if (outputFormats.Any(f => String.Equals(f, "XML", StringComparison.OrdinalIgnoreCase)))
            {
                resultWriters.Add(new ResultWriterXml(Path.Combine(validatorResultsOutputDirectory, $"{validatorResultsFileName}.xml"), logger));
            }

            if (outputFormats.Any(f => String.Equals(f, "JSON", StringComparison.OrdinalIgnoreCase)))
            {
                resultWriters.Add(new ResultWriterJson(Path.Combine(validatorResultsOutputDirectory, $"{validatorResultsFileName}.json"), logger));
            }

            if (outputFormats.Any(f => String.Equals(f, "HTML", StringComparison.OrdinalIgnoreCase)))
            {
                resultWriters.Add(new ResultWriterHtml(Path.Combine(validatorResultsOutputDirectory, $"{validatorResultsFileName}.html"), logger, includeSuppressed));
            }

            await SendMetricAsync("protocol", "solution");

            logger.LogInformation("Writing results...");

            if (!Directory.Exists(validatorResultsOutputDirectory))
            {
                Directory.CreateDirectory(validatorResultsOutputDirectory);
            }

            foreach (var writer in resultWriters)
            {
                writer.WriteResults(validatorResults);
            }

            logger.LogInformation("Writing results completed");

            logger.LogInformation("Finished");
            return 0;
        }

        private static bool IsLegacyStyleSolution(Solution solution)
        {
            bool isLegacyStyleSolution = false;

            foreach (var p in solution.Projects)
            {
                var project = solution.LoadProject(p);

                if (project.ProjectStyle == ProjectStyle.Legacy)
                {
                    isLegacyStyleSolution = true;
                    break;
                }
            }

            return isLegacyStyleSolution;
        }

        private void RestoreSolution(string solutionFilePath, int buildTimeout)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"restore \"{solutionFilePath}\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.OutputDataReceived += (sender, args) => { if (args.Data != null) logger.LogInformation(args.Data); };
                process.ErrorDataReceived += (sender, args) => { if (args.Data != null) logger.LogError(args.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                bool exited = process.WaitForExit(buildTimeout);

                if (exited)
                {
                    // See https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.exitcode?view=net-8.0#remarks
                    process.WaitForExit();
                }

                if (!exited) throw new TimeoutException("The restore of the solution timed out.");

                var exitCode = process.ExitCode;

                if (exitCode != 0)
                {
                    throw new InvalidOperationException("Could not restore the solution.");
                }
            }
        }

        private static async Task SendMetricAsync(string artifactType, string type)
        {
            try
            {
                DevOpsMetrics metrics = new DevOpsMetrics();
                string message = $"Skyline.DataMiner.CICD.Tools.Validator|{artifactType}";
                if (type != null)
                {
                    message += $"|{type}";
                }

                await metrics.ReportAsync(message);
            }
            catch
            {
                // Silently catch as if the request fails due to network issues we don't want the tool to fail.
            }
        }
    }
}

﻿namespace Skyline.DataMiner.CICD.Tools.Validator
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
        /// <param name="solutionFilePath">The solution (.sln) file path.</param>
        /// <param name="validatorResultsOutputDirectory">The output directory.</param>
        /// <param name="validatorResultsFileName">The file name of the results.</param>
        /// <param name="outputFormats">The output formats.</param>
        /// <param name="includeSuppressed"><c>true</c> to include suppressed results; otherwise, <c>false</c>.</param>
        /// <param name="performBuild"><c>true</c> to perform a build; otherwise, <c>false</c>.</param>
        /// <param name="buildTimeout">The build timeout.</param>
        /// <returns>Result value 0 indicates success.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="solutionFilePath"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid solution file path.</exception>
        public async Task<int> ValidateProtocolSolution(string solutionFilePath, string validatorResultsOutputDirectory, string validatorResultsFileName, string[] outputFormats, bool includeSuppressed, bool performBuild, int buildTimeout)
        {
            if (solutionFilePath == null) throw new ArgumentNullException(nameof(solutionFilePath));

            if (String.IsNullOrEmpty(solutionFilePath)) throw new ArgumentException("Invalid solution file path.", nameof(solutionFilePath));

            solutionFilePath = Path.GetFullPath(solutionFilePath);

            if (!File.Exists(solutionFilePath)) throw new ArgumentException($"The specified solution file '{solutionFilePath}' does not exist.", nameof(solutionFilePath));

            // Required for both building solution and for loading the MSBuildWorkspace to perform validation.
            if (!MSBuildLocator.IsRegistered)
            {
                MSBuildLocator.RegisterDefaults();
            }

            var solution = Solution.Load(solutionFilePath);
            bool isLegacyStyleSolution = IsLegacyStyleSolution(solution);
            bool isTargetingNetFramework48 = IsTargetingNetFramework48(solution);

            if (isLegacyStyleSolution)
            {
                logger.LogError("No validation performed. This tool can only validate solutions that use the SDK-style project format. This solution uses the legacy-style project format. Consider migrating to the SDK-style project format.");
            }

            if (!isTargetingNetFramework48)
            {
                logger.LogError("No validation performed. This tool can only validate solutions that target .NET Framework. The targeted version of .NET Framework must be at least version 4.8. Consider updating your targeted version in the solution project.");
            }

            if (isLegacyStyleSolution || !isTargetingNetFramework48)
            {
                return 1;
            }

            if (performBuild)
            {
                logger.LogInformation("Performing 'dotnet build'...");
                BuildSolution(solutionFilePath, buildTimeout);
            }

            Validator validatorRunner = new Validator();
            logger.LogInformation("Validating protocol solution...");
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
                resultWriters.Add(new ResultWriterHtml(Path.Combine(validatorResultsOutputDirectory, $"{validatorResultsFileName}.html"), logger));
            }

            await SendMetricAsync("protocol", "solution");

            logger.LogInformation("Writing results...");
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

        private static bool IsTargetingNetFramework48(Solution solution)
        {
            bool isTargetingNetFramework48 = true;

            foreach (var p in solution.Projects)
            {
                var project = solution.LoadProject(p);
                if (!project.TargetFrameworkMoniker.StartsWith(".NETFramework,Version=v4.8"))
                {
                    isTargetingNetFramework48 = false;
                    break;
                }
            }

            return isTargetingNetFramework48;
        }

        private void BuildSolution(string solutionFilePath, int buildTimeout)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"build \"{solutionFilePath}\" /property:WarningLevel=0";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.OutputDataReceived += (sender, args) => { if (args.Data != null) logger.LogInformation(args.Data); };
                process.ErrorDataReceived += (sender, args) => { if (args.Data != null) logger.LogError(args?.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                bool exited = process.WaitForExit(buildTimeout);

                if (exited)
                {
                    process.WaitForExit();
                }

                if (!exited) throw new TimeoutException("The build of the solution timed out.");

                var exitCode = process.ExitCode;

                if (exitCode != 0)
                {
                    throw new InvalidOperationException("Could not build the solution.");
                }
            }
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            logger.LogInformation(outLine.Data);
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
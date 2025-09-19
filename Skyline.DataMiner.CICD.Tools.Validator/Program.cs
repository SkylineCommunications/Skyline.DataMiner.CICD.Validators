namespace Skyline.DataMiner.CICD.Tools.Validator
{
    using System.CommandLine;
    using System.IO;
    using System.Reflection;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Microsoft.Extensions.Logging;
    using Skyline.DataMiner.CICD.Tools.Validator.OutputWriters;

    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Validates a DataMiner artifact or solution.");

            #region validate-protocol-solution

            var solutionPathOption = new Option<string>(
                name: "--solution-path",
                description: "Path to a solution file (.sln) of a DataMiner protocol or a directory that contains a .sln file. Note: In case the specified directory contains multiple .sln files, you must specify the file path of a specific solution.")
            {
                IsRequired = true
            };
            solutionPathOption.LegalFilePathsOnly();

            var validatorResultsOutputDirectoryOption = new Option<string>(
                name: "--output-directory",
                description: "Path to the directory where the validator results should be stored.")
            {
                IsRequired = true
            };

            var validatorResultsFileNameOption = new Option<string>(
                name: "--output-file-name",
                description:
                "Name of the results file. Note: Do not provide an extension, the extension is automatically added based on the results-output-formats option. Default: 'ValidatorResults_<protocolName>_<protocolVersion>'")
            {
                IsRequired = false
            };

            var outputFormatsOption = new Option<string[]>(
                name: "--output-format",
                description:
                "Specifies the output format. Possible values: JSON, XML, HTML. Specify a space separated list to output multiple formats.",
                getDefaultValue: () => new[] { "JSON", "HTML" })
            {
                Arity = ArgumentArity.ZeroOrMore,
                IsRequired = false,
                AllowMultipleArgumentsPerToken = true,
            };
            outputFormatsOption.FromAmong("JSON", "XML", "HTML");

            var includeSuppressedOption = new Option<bool>(
                name: "--include-suppressed",
                description: "Specifies whether the suppressed results should also be included in the results.",
                getDefaultValue: () => false)
            {
                IsRequired = false
            };

            var performRestoreOption = new Option<bool>(
                name: "--perform-restore",
                description: "Specifies whether to perform a dotnet restore operation.",
                getDefaultValue: () => true)
            {
                IsRequired = false
            };

            var restoreTimeoutOption = new Option<int>(
                name: "--restore-timeout",
                description: "Specifies the timeout for the restore operation (in ms).",
                getDefaultValue: () => 300000)
            {
                IsRequired = false
            };

            // output format.
            var validateProtocolSolutionCommand = new Command("validate-protocol-solution", "Validates a protocol solution.")
            {
                solutionPathOption,
                validatorResultsOutputDirectoryOption,
                validatorResultsFileNameOption,
                outputFormatsOption,
                includeSuppressedOption,
                performRestoreOption,
                restoreTimeoutOption
            };

            rootCommand.AddCommand(validateProtocolSolutionCommand);

            var validatorRunner = new ValidatorRunner();
            validateProtocolSolutionCommand.SetHandler(validatorRunner.ValidateProtocolSolution, solutionPathOption, validatorResultsOutputDirectoryOption, validatorResultsFileNameOption, outputFormatsOption, includeSuppressedOption, performRestoreOption, restoreTimeoutOption);


            #endregion

            #region major-change-checker

            var majorChangeCheckerSolutionPathOption = new Option<string>(
                    name: "--mcc-solution-path",
                    description: "Path to the new solution file (.sln) of a DataMiner protocol.")
            {
                IsRequired = true
            };
            majorChangeCheckerSolutionPathOption.LegalFilePathsOnly();

            var oldProtocolPathOption = new Option<string>(
                name: "--mcc-old-protocol-path",
                description: "Path to the old protocol.xml file for comparison.")
            {
                IsRequired = false
            };
            oldProtocolPathOption.LegalFilePathsOnly();

            var majorChangeCheckerOutputDirectoryOption = new Option<string>(
                name: "--mcc-output-directory",
                description: "Path to the directory where the MCC results should be stored.")
            {
                IsRequired = true
            };

            var majorChangeCheckerOutputFileNameOption = new Option<string>(
                name: "--output-file-name",
                description: "Name of the MCC results file.")
            {
                IsRequired = false
            };

            var majorChangeCheckerOutputFormatsOption = new Option<string[]>(
                name: "--output-format",
                description: "Specifies the output format for MCC results. Possible values: JSON, XML, HTML.",
                getDefaultValue: () => new[] { "JSON", "HTML"}) 
            {
                Arity = ArgumentArity.ZeroOrMore,
                IsRequired = false,
                AllowMultipleArgumentsPerToken = true,
            };
            majorChangeCheckerOutputFormatsOption.FromAmong("JSON", "XML", "HTML");

            var majorChangeCheckerIncludeSuppressedOption = new Option<bool>(
                name: "--include-suppressed",
                description: "Specifies whether the suppressed results should also be included in the MCC results.",
                getDefaultValue: () => false)
            {
                IsRequired = false
            };

            var catalogIdOption = new Option<string>(
                name: "--catalog-id",
                description: "Catalog ID for fetching previous protocol version from Catalog API.")
            {
                IsRequired = false
            };

            var catalogApiKeyOption = new Option<string>(
                name: "--catalog-api-key",
                description: "Subscription key for Catalog API access.")
            {
                IsRequired = false
            };

            var mccCommand = new Command("major-change-checker", "Performs major change checking between protocol versions.")
            {
                majorChangeCheckerSolutionPathOption,
                oldProtocolPathOption,
                majorChangeCheckerOutputDirectoryOption,
                majorChangeCheckerOutputFileNameOption,
                majorChangeCheckerOutputFormatsOption,
                majorChangeCheckerIncludeSuppressedOption,
                catalogIdOption,                
                catalogApiKeyOption
            };

            mccCommand.SetHandler(async (context) =>
            {
                var solutionPath = context.ParseResult.GetValueForOption(majorChangeCheckerSolutionPathOption);
                var oldProtocolPath = context.ParseResult.GetValueForOption(oldProtocolPathOption);
                var outputDirectory = context.ParseResult.GetValueForOption(majorChangeCheckerOutputDirectoryOption);
                var outputFileName = context.ParseResult.GetValueForOption(majorChangeCheckerOutputFileNameOption);
                var outputFormats = context.ParseResult.GetValueForOption(majorChangeCheckerOutputFormatsOption);
                var includeSuppressed = context.ParseResult.GetValueForOption(majorChangeCheckerIncludeSuppressedOption);
                var catalogId = context.ParseResult.GetValueForOption(catalogIdOption);
                var apiKey = context.ParseResult.GetValueForOption(catalogApiKeyOption);

                var runner = new MajorChangeCheckerRunner();
                int result = await runner.RunMajorChangeChecker(
                    solutionPath, oldProtocolPath, outputDirectory, outputFileName,
                    outputFormats, includeSuppressed, catalogId, apiKey);
                
              
                context.ExitCode = result;
            });

            rootCommand.AddCommand(mccCommand);
            #endregion

            int value = await rootCommand.InvokeAsync(args);
            return value;
        }
    }     
}   

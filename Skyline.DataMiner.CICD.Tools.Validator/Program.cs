namespace Skyline.DataMiner.CICD.Tools.Validator
{
    using System.CommandLine;
    using System.Threading.Tasks;

    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Validates a DataMiner artifact or solution.");

            var solutionFilePathOption = new Option<string>(
                name: "--solution-file-path",
                description: "Path to the solution file (.sln) of the protocol solution.")
            {
                IsRequired = true
            };
            solutionFilePathOption.LegalFilePathsOnly();

            var validatorResultsOutputDirectoryOption = new Option<string>(
                name: "--results-output-directory-path",
                description: "Path to the directory where the validator results should be stored.")
            {
                IsRequired = true
            };

            var validatorResultsFileNameOption = new Option<string>(
                name: "--results-file-name",
                description:
                "Name of the results file. Note: Do not provide an extension, the extension is automatically added based on the results-output-formats option. Default: 'ValidatorResults_<protocolName>_<protocolVersion>'")
            {
                IsRequired = false
            };

            var outputFormatsOption = new Option<string[]>(
                name: "--results-output-format",
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

            var performBuildOption = new Option<bool>(
                name: "--perform-build",
                description: "Specifies whether to perform a dotnet build operation.",
                getDefaultValue: () => true)
            {
                IsRequired = false
            };

            var buildTimeoutOption = new Option<int>(
                name: "build-timeout",
                description: "Specifies the timeout for the build operation (in ms).",
                getDefaultValue: () => 300000)
            {
                IsRequired = false
            };

            // output format.
            var validateProtocolSolutionCommand = new Command("validate-protocol-solution", "Validates a protocol solution.")
            {
                solutionFilePathOption,
                validatorResultsOutputDirectoryOption,
                validatorResultsFileNameOption,
                outputFormatsOption,
                includeSuppressedOption,
                performBuildOption,
                buildTimeoutOption
            };

            rootCommand.Add(validateProtocolSolutionCommand);

            var validatorRunner = new ValidatorRunner();
            validateProtocolSolutionCommand.SetHandler(validatorRunner.ValidateProtocolSolution, solutionFilePathOption, validatorResultsOutputDirectoryOption, validatorResultsFileNameOption, outputFormatsOption, includeSuppressedOption, performBuildOption, buildTimeoutOption);

            int value = await rootCommand.InvokeAsync(args);
            return value;
        }
    }
}

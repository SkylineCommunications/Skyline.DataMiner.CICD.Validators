namespace Skyline.DataMiner.CICD.Tools.Validator.Commands
{
    using Skyline.DataMiner.CICD.Tools.Validator.OutputWriters;
    using Skyline.DataMiner.CICD.Tools.Validator.SystemCommandLine;

    internal class BaseCommand : Command
    {
        protected BaseCommand(string name, string? description = null) : base(name, description)
        {
            AddOption(new Option<IFileSystemInfoIO>(
                aliases: ["--solution-path", "-sp"],
                description: "Path to a solution file (.sln or .slnx) of a DataMiner protocol or a directory that contains a solution file. Note: In case the specified directory contains multiple solution files, you must specify the file path of a specific solution.",
                parseArgument: OptionHelper.ParseFileSystemInfo!)
            {
                IsRequired = true
            }.LegalFilePathsOnly()!.ExistingOnly());

            AddOption(new Option<IDirectoryInfoIO>(
                aliases: ["--output-directory", "-od"],
                description: "Path to the directory where the validator results should be stored.",
                parseArgument: OptionHelper.ParseDirectoryInfo!)
            {
                IsRequired = true
            }.LegalFilePathsOnly());

            AddOption(new Option<string>(
                    aliases: ["--output-file-name", "-ofn"],
                    description: "Name of the results file. Note: Do not provide an extension, the extension is automatically added based on the results-output-formats option.")
                .LegalFileNamesOnly());

            AddOption(new Option<string[]>(
                aliases: ["--output-format", "-of"],
                description: "Specifies the output format. Possible values: JSON, XML, HTML. Specify a space separated list to output multiple formats.",
                getDefaultValue: () => ["JSON", "HTML"])
            {
                Arity = ArgumentArity.OneOrMore,
                AllowMultipleArgumentsPerToken = true
            }.FromAmong("JSON", "XML", "HTML"));

            AddOption(new Option<bool?>(
                aliases: ["--include-suppressed", "-is"],
                description: "Specifies whether the suppressed results should also be included in the results."));
        }
    }

    internal abstract class BaseCommandHandler(ILogger logger) : ICommandHandler
    {
        public required IFileSystemInfoIO SolutionPath { get; set; }

        public required IDirectoryInfoIO OutputDirectory { get; set; }

        public string? OutputFileName { get; set; }

        public required string[] OutputFormat { get; set; }

        public bool? IncludeSuppressed { get; set; }

        public int Invoke(InvocationContext context)
        {
            return (int)ExitCodes.NotImplemented;
        }

        public abstract Task<int> InvokeAsync(InvocationContext context);

        protected IFileInfoIO GetSolutionFile()
        {
            IFileInfoIO solutionFile;
            switch (SolutionPath)
            {
                case IFileInfoIO file:
                    solutionFile = file;
                    break;
                case IDirectoryInfoIO directory:
                {
                    var files = directory.GetFiles("*.sln", SearchOption.TopDirectoryOnly)
                                         .Concat(directory.GetFiles("*.slnx", SearchOption.TopDirectoryOnly))
                                         .ToArray();

                    if (files.Length == 0)
                    {
                        throw new ArgumentException($"The specified directory '{directory.FullName}' does not contain a solution file.");
                    }

                    if (files.Length > 1)
                    {
                        throw new ArgumentException(
                            $"The specified directory '{directory.FullName}' contains multiple solution files. Specify the full path of the solution.");
                    }

                    solutionFile = files[0];
                    break;
                }
                default:
                    throw new ArgumentException("The specified solution path is neither a file nor a directory.");
            }

            if (!solutionFile.Exists)
            {
                throw new FileNotFoundException($"The specified solution file '{solutionFile.FullName}' does not exist.", solutionFile.FullName);
            }

            return solutionFile;
        }

        protected IList<IResultWriter> GetResultWriters(string fileName)
        {
            List<IResultWriter> resultWriters = [];
            
            if (OutputFormat.Any(f => String.Equals(f, "XML", StringComparison.OrdinalIgnoreCase)))
            {
                resultWriters.Add(new ResultWriterXml(Path.Combine(OutputDirectory.FullName, $"{fileName}.xml"), logger));
            }

            if (OutputFormat.Any(f => String.Equals(f, "JSON", StringComparison.OrdinalIgnoreCase)))
            {
                resultWriters.Add(new ResultWriterJson(Path.Combine(OutputDirectory.FullName, $"{fileName}.json"), logger));
            }

            if (OutputFormat.Any(f => String.Equals(f, "HTML", StringComparison.OrdinalIgnoreCase)))
            {
                resultWriters.Add(new ResultWriterHtml(Path.Combine(OutputDirectory.FullName, $"{fileName}.html"), logger, IncludeSuppressed ?? false));
            }

            return resultWriters;
        }
    }
}
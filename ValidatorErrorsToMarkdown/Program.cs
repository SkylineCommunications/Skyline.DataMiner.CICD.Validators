namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
    using System.CommandLine;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// This tool converts an XML file of error messages from the validator of DIS to MarkDown files.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Creates the functionality of the tool by getting the input (inputFilePath) and storing the output (outputDirectoryPath).
        /// </summary>
        /// <param name="args"></param>
        public static async Task<int> Main(string[] args)
        {
            var inputFilePath = new Option<string>(
                name: "--inputFilePath",
                description: "File containing the ErrorMessages.")
            {
                IsRequired = true
            };

            var outputDirectoryPath = new Option<string>(
                name: "--outputDirectoryPath",
                description: "Directory where the MarkDown files are saved to.")
            {
                IsRequired = true
            };

            var rootCommand = new RootCommand("Returns MarkDown from XML file.")
            {
                inputFilePath,
                outputDirectoryPath,
            };

            rootCommand.SetHandler(Process, inputFilePath, outputDirectoryPath);

            await rootCommand.InvokeAsync(args);

            return 0;
        }

        private static void Process(string inputFilePath, string outputDirectoryPath)
        {
            XDocument xml = XDocument.Load(inputFilePath);
            var parser = new ParseXmlToMarkDown(xml, outputDirectoryPath);
            parser.ConvertToMarkDown();
        }
    }
}

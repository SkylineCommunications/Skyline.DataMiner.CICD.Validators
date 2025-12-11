namespace Skyline.DataMiner.CICD.Tools.Validator.Helpers
{
    using Skyline.DataMiner.CICD.FileSystem;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Parsers.Common.Xml;
    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Tools;

    internal static class InputDataAndLineInfoHelper
    {
        public static async Task<(IProtocolInputData, ILineInfoProvider)> GetDataBasedOnSolutionAsync(string solutionFilePath, bool includeQActionCompilationModel = true, CancellationToken cancellationToken = default)
        {
            if (!FileSystem.Instance.File.Exists(solutionFilePath))
            {
                throw new FileNotFoundException($"The protocol solution file '{solutionFilePath}' does not exist.", solutionFilePath);
            }

            string protocolCode = GetProtocolCodeFromSolution(solutionFilePath);

            var parser = new Parser(protocolCode);
            var document = parser.Document;
            var model = new ProtocolModel(document);

            ProtocolInputData data;
            if (includeQActionCompilationModel)
            {
                var workspace = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create();
                var solution = await workspace.OpenSolutionAsync(solutionFilePath, cancellationToken: cancellationToken);

                data = new ProtocolInputData(model, document, new QActionCompilationModel(model, solution));
            }
            else
            {
                data = new ProtocolInputData(model, document);
            }

            return (data, new SimpleLineInfoProvider(protocolCode));
        }

        public static (IProtocolInputData, ILineInfoProvider) GetBasedOnXml(string protocolXmlPath)
        {
            if (!FileSystem.Instance.File.Exists(protocolXmlPath))
            {
                throw new FileNotFoundException($"The protocol XML file '{protocolXmlPath}' does not exist.", protocolXmlPath);
            }

            string protocolCode = FileSystem.Instance.File.ReadAllText(protocolXmlPath);

            var parser = new Parser(protocolCode);
            var document = parser.Document;
            var model = new ProtocolModel(document);
            var data = new ProtocolInputData(model, document);
            return (data, new SimpleLineInfoProvider(protocolCode));
        }

        private static string GetProtocolCodeFromSolution(string solutionFilePath)
        {
            var solutionDirectoryPath = FileSystem.Instance.Path.GetDirectoryName(solutionFilePath);

            string protocolFilePath = FileSystem.Instance.Path.GetFullPath(FileSystem.Instance.Path.Combine(solutionDirectoryPath!, "protocol.xml"));

            if (!FileSystem.Instance.File.Exists(protocolFilePath))
            {
                throw new InvalidOperationException($"protocol.xml not found. Expected location '{protocolFilePath}'.");
            }

            return FileSystem.Instance.File.ReadAllText(protocolFilePath);
        }
    }
}
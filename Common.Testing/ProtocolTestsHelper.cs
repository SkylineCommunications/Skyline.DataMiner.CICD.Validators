namespace Common.Testing
{
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    using Skyline.DataMiner.CICD.Models.Common;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Parsers.Common.Xml;
    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    using EditModel = Skyline.DataMiner.CICD.Models.Protocol.Edit;
    using EditXml = Skyline.DataMiner.CICD.Parsers.Common.XmlEdit;
    using XmlDocument = Skyline.DataMiner.CICD.Parsers.Common.Xml.XmlDocument;

    public static class ProtocolTestsHelper
    {
        private static (IProtocolModel model, XmlDocument document, string protocolCode) ReadProtocol(string fileName, [CallerFilePath] string pathToClassFile = "")
        {
            string filePath = Path.Combine(Path.GetDirectoryName(pathToClassFile), fileName);

            string code;
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using (var textReader = new StreamReader(fileStream))
            {
                code = textReader.ReadToEnd();
            }

            return ParseProtocol(code);
        }

        private static (IProtocolModel model, XmlDocument document, string protocolCode) ParseProtocol(string protocolCode)
        {
            Parser parser = new Parser(new StringBuilder(protocolCode));

            return (new ProtocolModel(parser.Document), parser.Document, protocolCode);
        }

        public static QActionCompilationModel GetQActionCompilationModel(string xmlCode)
        {
            var document = new Parser(xmlCode).Document;
            var model = new ProtocolModel(document);

            return GetQActionCompilationModel(model, xmlCode);
        }

        public static QActionCompilationModel GetQActionCompilationModel(IProtocolModel model, string xmlCode)
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            baseDir = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\..\DLLs"));

            var dllImportResolver = new InternalFilesAssemblyResolver(baseDir);
            var qactionHelperProvider = new ProtocolQActionHelperProvider();

            string qactionHelperSourceCode = qactionHelperProvider.GetProtocolQActionHelper(xmlCode, ignoreErrors: true);
            return new QActionCompilationModel(qactionHelperSourceCode, model, dllImportResolver);
        }

        public static IProtocolInputData GetProtocolInputData(string fileName, [CallerFilePath] string pathToClassFile = "")
        {
            (IProtocolModel model, XmlDocument document, string protocolCode) = ReadProtocol(fileName, pathToClassFile);

            var qactionCompilationModel = GetQActionCompilationModel(model, protocolCode);

            return new ProtocolInputData(model, document, qactionCompilationModel);
        }

        public static EditModel.Protocol GetEditProtocol(IProtocolModel model, EditXml.XmlDocument xmlDocument)
        {
            return new EditModel.ProtocolDocumentEdit(model, xmlDocument).Protocol;
        }
    }
}
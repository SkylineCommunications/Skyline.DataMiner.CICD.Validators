namespace Common.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Tracing;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Locator;
    using Microsoft.Build.Logging;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.MSBuild;
    using Nito.AsyncEx.Synchronous;
    using Skyline.DataMiner.CICD.Models.Common;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Parsers.Common.Xml;
    using Skyline.DataMiner.CICD.Parsers.Protocol.VisualStudio;
    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    using EditModel = Skyline.DataMiner.CICD.Models.Protocol.Edit;
    using EditXml = Skyline.DataMiner.CICD.Parsers.Common.XmlEdit;
    using XmlDocument = Skyline.DataMiner.CICD.Parsers.Common.Xml.XmlDocument;

    public static class ProtocolTestsHelper
    {
        public static ProtocolInputData GetProtocolInputDataFromSolution(string solutionPath)
        {
            // Creating a build workspace.
            var workspace = MSBuildWorkspace.Create();
            
            // Opening the solution.
            Solution solution = workspace.OpenSolutionAsync(solutionPath).Result;

            ProtocolSolution protocolSolution = ProtocolSolution.Load(solutionPath);
            ProtocolModel protocolModel = new ProtocolModel(protocolSolution.ProtocolDocument);

            QActionCompilationModel qActionCompilationModel = new QActionCompilationModel(protocolModel, solution);

            return new ProtocolInputData(protocolModel, protocolSolution.ProtocolDocument, qActionCompilationModel);
        }

        public static async Task<ProtocolInputData> GetProtocolInputDataFromSolutionAsync(string solutionPath)
        {
            // Creating a build workspace.
            var workspace = MSBuildWorkspace.Create();
            
            // Opening the solution.
            Solution solution = await workspace.OpenSolutionAsync(solutionPath);

            ProtocolSolution protocolSolution = ProtocolSolution.Load(solutionPath);
            ProtocolModel protocolModel = new ProtocolModel(protocolSolution.ProtocolDocument);

            QActionCompilationModel qActionCompilationModel = new QActionCompilationModel(protocolModel, solution);

            return new ProtocolInputData(protocolModel, protocolSolution.ProtocolDocument, qActionCompilationModel);
        }
        
        public static ProtocolInputData GetProtocolInputDataFromXml(string xmlCode)
        {
            return new ProtocolInputData(xmlCode, GetQActionCompilationModel(xmlCode));
        }

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
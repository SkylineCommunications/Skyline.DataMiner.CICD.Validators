namespace ProtocolTests.Helpers.Software_Parameters
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Parsers.Common.Xml;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests;

    [TestClass]
    public class SoftwareParameters
    {
        [TestMethod]
        public void IsCorrectEnhancedServiceParam()
        {
            // Get Protocol
            string path = Path.Combine(Helper.GetProjectPath(), "Helpers\\Software Parameters\\Skyline Service Definition Basic\\Protocol.xml");
            var model = DocumentParsing.GetProtocol(path);

            StringBuilder sb = new StringBuilder();
            foreach (var item in model.Protocol.Params)
            {
                if (!ParamHelper.IsCorrectEnhancedServiceParam(item))
                {
                    sb.AppendLine($"[{item.Id.Value.Value}]{Environment.NewLine}\t{item.Name?.Value}{Environment.NewLine}\t{item.Description?.Value}");
                }
            }

            string message = sb.ToString();

            if (!String.IsNullOrWhiteSpace(message))
            {
                Assert.Fail($"{Environment.NewLine}{message}");
            }
        }

        [TestMethod]
        public void IsRestrictedParamName()
        {
            // Get Protocol
            string path = Path.Combine(Helper.GetProjectPath(), "Helpers\\Software Parameters\\DataMiner Element Control Protocol\\Protocol.xml");
            var model = DocumentParsing.GetProtocol(path);

            StringBuilder sb = new StringBuilder();
            foreach (var item in model.Protocol.Params)
            {
                if (!ParamHelper.IsRestrictedParamName(item))
                {
                    sb.AppendLine($"[{item.Id.Value.Value}]{Environment.NewLine}\t{item.Name?.Value}{Environment.NewLine}\t{item.Description?.Value}");
                }
            }

            string message = sb.ToString();

            if (!String.IsNullOrWhiteSpace(message))
            {
                Assert.Fail($"{Environment.NewLine}{message}");
            }
        }

        [TestMethod]
        [Ignore("Need to be revised (tables aren't listed apparently)")]
        public void IsGeneralParam()
        {
            // Get Protocol
            string path = Path.Combine(Helper.GetProjectPath(), "Helpers\\Software Parameters\\DataMiner Element Control Protocol\\Protocol.xml");
            var model = DocumentParsing.GetProtocol(path);

            StringBuilder sb = new StringBuilder();
            foreach (var item in model.Protocol.Params)
            {
                if (item.Id.Value.Value < 65000 || item.Type.Value == Skyline.DataMiner.CICD.Models.Protocol.Enums.EnumParamType.Write)
                {
                    continue;
                }

                if (!ParamHelper.IsGeneralParam(item))
                {
                    sb.AppendLine($"[{item.Id.Value.Value}]{Environment.NewLine}\t{item.Name?.Value}{Environment.NewLine}\t{item.Description?.Value}");
                }
            }

            string message = sb.ToString();

            if (!String.IsNullOrWhiteSpace(message))
            {
                Assert.Fail($"{Environment.NewLine}{message}");
            }
        }

        [DataTestMethod]
        [DataRow("Helpers\\Software Parameters\\Skyline SLA Definition Basic\\2.0.0.x\\protocol.xml")]
        [DataRow("Helpers\\Software Parameters\\Skyline SLA Definition Basic\\3.0.0.x\\Protocol.xml")]
        public void IsCorrectSlaParamName(string path)
        {
            // Get Protocol
            path = Path.Combine(Helper.GetProjectPath(), path);
            var model = DocumentParsing.GetProtocol(path);

            StringBuilder sb = new StringBuilder();
            foreach (var item in model.Protocol.Params)
            {
                if (!ParamHelper.IsCorrectSlaParam(item))
                {
                    sb.AppendLine($"[{item.Id.Value.Value}]{Environment.NewLine}\t{item.Name?.Value}{Environment.NewLine}\t{item.Description?.Value}");
                }
            }

            string message = sb.ToString();

            if (!String.IsNullOrWhiteSpace(message))
            {
                Assert.Fail($"{Environment.NewLine}{message}");
            }
        }
    }

    internal static class Helper
    {
        public static string GetProjectPath()
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\..\ProtocolTests"));
        }
    }

    internal static class DocumentParsing
    {
        internal static Skyline.DataMiner.CICD.Models.Protocol.Read.ProtocolModel GetProtocol(string path)
        {
            (string code, bool success) = ReadTextFromFile(path);
            if (!success)
            {
                Assert.Fail(code);
            }

            (var model, var document) = ParseProtocol(code);

            return model;
        }

        private static (string code, bool success) ReadTextFromFile(string pathToFile)
        {
            try
            {
                string code;
                var fileStream = new FileStream(pathToFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var textReader = new StreamReader(fileStream))
                {
                    code = textReader.ReadToEnd();
                }

                return (code, true);
            }
            catch (FileNotFoundException)
            {
                return (String.Format("Missing file:{0}{1}", Environment.NewLine, pathToFile), false);
            }
        }

        private static (Skyline.DataMiner.CICD.Models.Protocol.Read.ProtocolModel, Skyline.DataMiner.CICD.Parsers.Common.Xml.XmlDocument) ParseProtocol(string protocolCode)
        {
            Parser parser = new Parser(new StringBuilder(protocolCode));

            return (new Skyline.DataMiner.CICD.Models.Protocol.Read.ProtocolModel(parser.Document), parser.Document);
        }
    }
}
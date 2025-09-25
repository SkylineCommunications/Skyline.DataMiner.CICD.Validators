using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Build.Locator;
using Microsoft.Extensions.Logging;
using Skyline.DataMiner.CICD.Models.Protocol.Read;
using Skyline.DataMiner.CICD.Parsers.Common.VisualStudio.Projects;
using Skyline.DataMiner.CICD.Parsers.Common.Xml;
using Skyline.DataMiner.CICD.Tools.Reporter;
using Skyline.DataMiner.CICD.Tools.Validator.OutputWriters;
using Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.HtmlWriter;

namespace Skyline.DataMiner.CICD.Tools.Validator
{
    internal class MajorChangeCheckerRunner
    {
        private readonly ILogger logger;

        public MajorChangeCheckerRunner()
        {
            using (ILoggerFactory factory = LoggerFactory.Create(builder =>
                builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                })))
            {
                logger = factory.CreateLogger("MCC");
            }
        }

        public async Task<int> RunMajorChangeChecker(string solutionPath, string oldProtocolPath,
    string outputDirectory, string outputFileName, string[] outputFormats,
    bool includeSuppressed, string catalogId, string apiKey)
        {
            try
            {
                string oldProtocolCode;
                string oldProtocolName;
                string oldProtocolVersion;

                if (string.IsNullOrEmpty(oldProtocolPath))
                {

                    string newProtocolCode = GetProtocolCode(solutionPath);
                    var newParser = new Parser(newProtocolCode);
                    var newDocument = newParser.Document;
                    var newModel = new ProtocolModel(newDocument);

                    var catalogService = new CatalogService(logger, apiKey, newProtocolCode);
                    string effectiveCatalogId = catalogId;


                    if (string.IsNullOrEmpty(effectiveCatalogId))
                    {
                        logger.LogInformation("Catalog ID not provided. Searching public catalog...");
                        effectiveCatalogId = await catalogService.FindCatalogIdFromPublicCatalog(newModel.Protocol?.Name?.Value);

                        if (string.IsNullOrEmpty(effectiveCatalogId))
                        {
                            logger.LogError("Could not find catalog ID from public catalog for protocol: {ProtocolName}", newModel.Protocol?.Name?.Value);
                            return 1;
                        }

                        logger.LogInformation($"Found catalog ID from public catalog: {effectiveCatalogId}");
                    }

                    string downloadedProtocolPath = await catalogService.DownloadPreviousProtocolVersion(effectiveCatalogId, newModel);

                    if (string.IsNullOrEmpty(downloadedProtocolPath) || !File.Exists(downloadedProtocolPath))
                    {

                        logger.LogWarning("Download failed. Searching public catalog for correct catalog ID...");
                        effectiveCatalogId = await catalogService.FindCatalogIdFromPublicCatalog(newModel.Protocol?.Name?.Value);

                        if (!string.IsNullOrEmpty(effectiveCatalogId))
                        {
                            logger.LogInformation($"Retrying download with catalog ID: {effectiveCatalogId}");
                            downloadedProtocolPath = await catalogService.DownloadPreviousProtocolVersion(effectiveCatalogId, newModel);
                        }

                        if (string.IsNullOrEmpty(downloadedProtocolPath) || !File.Exists(downloadedProtocolPath))
                        {
                            logger.LogError("Failed to download protocol from catalog after retry");
                            return 1;
                        }
                    }

                    oldProtocolCode = File.ReadAllText(downloadedProtocolPath);
                    var protocolInfo = GetProtocolInfo(oldProtocolCode);
                    oldProtocolName = protocolInfo.Name;
                    oldProtocolVersion = protocolInfo.Version;

                    File.Delete(downloadedProtocolPath);
                }
                else
                {
                    oldProtocolCode = File.ReadAllText(oldProtocolPath);
                    var protocolInfo = GetProtocolInfo(oldProtocolCode);
                    oldProtocolName = protocolInfo.Name;
                    oldProtocolVersion = protocolInfo.Version;

                    logger.LogInformation($"Local protocol: {oldProtocolName} version {oldProtocolVersion}");
                }


                var checker = new MajorChangeChecker();
                var results = await checker.CheckMajorChanges(solutionPath, oldProtocolCode, includeSuppressed);

                results.OldProtocol = oldProtocolName;
                results.OldVersion = oldProtocolVersion;

                if (string.IsNullOrEmpty(outputFileName))
                {
                    outputFileName = $"MCCResults_{results.NewProtocol}_{results.NewVersion}";
                }

                Directory.CreateDirectory(outputDirectory);

                foreach (var format in outputFormats)
                {
                    string filePath = Path.Combine(outputDirectory, $"{outputFileName}.{format.ToLower()}");

                    switch (format.ToUpper())
                    {
                        case "JSON":
                            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                            string json = JsonSerializer.Serialize(results, jsonOptions);
                            File.WriteAllText(filePath, json);
                            break;

                        case "XML":
                            var xmlSerializer = new XmlSerializer(typeof(MajorChangeCheckerResults));
                            using (var writer = new StreamWriter(filePath))
                            {
                                xmlSerializer.Serialize(writer, results);
                            }
                            break;

                        case "HTML":
                            var htmlWriter = new MajorChangeCheckerResultWriterHtml(filePath, logger, includeSuppressed);
                            htmlWriter.WriteResults(results);
                            break;
                    }

                    logger.LogInformation($"Saved {format} results to {filePath}");
                }

                logger.LogInformation($"Major change check completed. Results saved to {outputDirectory}");
                return 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during major change check");
                return 1;
            }
        }
        private ProtocolInfo GetProtocolInfo(string protocolCode)
        {
            try
            {
                var parser = new Parser(protocolCode);
                var document = parser.Document;
                if (document == null)
                {
                    logger.LogWarning("Parsed document is null.");
                    return new ProtocolInfo("unknown", "unknown");
                }

                var model = new ProtocolModel(document);
                string name = model.Protocol?.Name?.Value;
                string version = model.Protocol?.Version?.Value;

                return new ProtocolInfo(name, version);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error parsing protocol information");
                return new ProtocolInfo("unknown", "unknown");
            }
        }


        private static string GetProtocolCode(string solutionPath)
        {
            var solutionDirectory = Path.GetDirectoryName(solutionPath);
            if (string.IsNullOrEmpty(solutionDirectory))
            {
                solutionDirectory = solutionPath;
            }

            string protocolFilePath = Path.Combine(solutionDirectory, "protocol.xml");
            if (!File.Exists(protocolFilePath))
            {
                throw new FileNotFoundException($"protocol.xml not found in solution directory: {solutionDirectory}");
            }

            return File.ReadAllText(protocolFilePath);
        }

        public class ProtocolInfo
        {
            public string Name { get; set; }
            public string Version { get; set; }

            public ProtocolInfo(string name, string version)
            {
                Name = name;
                Version = version;
            }
        }
    }
}
namespace Skyline.DataMiner.CICD.Tools.Validator.Commands.Compare
{
    using System.Diagnostics;
    using System.IO.Compression;
    using System.Xml.Linq;

    using Skyline.ArtifactDownloader.Identifiers;
    using Skyline.ArtifactDownloader.Services;
    using Skyline.DataMiner.CICD.FileSystem;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Tools.Validator;
    using Skyline.DataMiner.CICD.Tools.Validator.Helpers;
    using Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.Results;
    using Skyline.DataMiner.CICD.Tools.Validator.SystemCommandLine;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Common.Suppressions;
    using Skyline.DataMiner.XmlSchemas.Protocol;

    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    internal class CompareProtocolSolutionCommand : BaseCommand
    {
        public CompareProtocolSolutionCommand() :
            base(name: "protocol-solution", description: "Compare a protocol solution with the previous version.")
        {
            AddOption(new Option<IFileInfoIO?>(
                aliases: ["--previous-protocol-xml-path", "-ppxp"],
                description: "Path to the previous protocol.xml file.",
                parseArgument: OptionHelper.ParseFileInfo).LegalFilePathsOnly());

            AddOption(new Option<string?>(
                aliases: ["--catalog-id", "-ci"],
                description: "Catalog ID for fetching previous protocol version from the Catalog."));

            AddOption(new Option<string?>(
                aliases: ["--catalog-api-key", "-cak"],
                description: "Catalog API key for fetching previous protocol version from the Catalog. Can also be set with the environment variable DATAMINER_TOKEN. Required when not providing a previous protocol XML file."));
        }
    }

    internal class CompareProtocolSolutionCommandHandler(ILogger<CompareProtocolSolutionCommandHandler> logger, IConfiguration configuration, PublicCatalogService publicCatalogService) : BaseCommandHandler(logger)
    {
        /*
         * Automatic binding with System.CommandLine.NamingConventionBinder
         * The property names need to match with the command line argument names.
         * Example: --example-package-file will bind to ExamplePackageFile
         */

        public IFileInfoIO? PreviousProtocolXmlPath { get; set; }

        public string? CatalogId { get; set; }

        public string? CatalogApiKey { get; set; }

        public override async Task<int> InvokeAsync(InvocationContext context)
        {
            logger.LogDebug("Starting {Method}...", nameof(CompareProtocolSolutionCommand));

            string temporaryDirectory = FileSystem.Instance.Directory.CreateTemporaryDirectory();

            try
            {
                /* Prepare */
                IFileInfoIO solutionFile = GetSolutionFile();

                (IProtocolInputData inputData, ILineInfoProvider lineInfoProvider) =
                    // QActionCompilationModel is not needed as C# checks won't work anyway as the previous version will always be the XML format
                    await InputDataAndLineInfoHelper.GetDataBasedOnSolutionAsync(solutionFile.FullName, includeQActionCompilationModel: false, context.GetCancellationToken());

                // Get previous protocol input data
                string? previousProtocolXmlFile;
                if (PreviousProtocolXmlPath?.Exists == true)
                {
                    previousProtocolXmlFile = PreviousProtocolXmlPath.FullName;
                }
                else
                {
                    previousProtocolXmlFile = await GetPreviousProtocolVersionAsync(inputData.Model, temporaryDirectory, solutionFile.DirectoryName, context.GetCancellationToken());
                    if (previousProtocolXmlFile == null)
                    {
                        return (int)ExitCodes.Fail;
                    }

                    if (String.Equals("Skipped", previousProtocolXmlFile))
                    {
                        return (int)ExitCodes.Ok;
                    }

                    logger.LogInformation("Found previous protocol version.");
                }

                (IProtocolInputData previousInputData, _) = InputDataAndLineInfoHelper.GetBasedOnXml(previousProtocolXmlFile);

                ValidatorSettings settings = new ValidatorSettings(Globals.MinSupportedDataMinerVersionWithBuildNumber, new UnitList(XDocument.Parse(Resources.uom)));

                /* Compare */
                Stopwatch sw = Stopwatch.StartNew();
                IList<IValidationResult> compareResults = new Validators.Protocol.Validator().RunCompare(inputData, previousInputData, settings, context.GetCancellationToken());
                sw.Stop();

                /* Handle suppression and converting to format fit for files */
                // Compare only has suppression in the version history
                ISuppressionManager suppressionManager = new VersionHistorySuppressionManager(inputData.Model.Protocol);

                MajorChangeCheckerResults results = new MajorChangeCheckerResults(inputData, previousInputData);

                ResultsConverter.ConvertResults(results, compareResults, suppressionManager, lineInfoProvider);
                ResultsConverter.ProcessSubResults(results, results.Issues, IncludeSuppressed ?? false);

                results.ValidationTimeStamp = DateTime.Now;
                results.SuppressedIssuesIncluded = IncludeSuppressed ?? false;

                logger.LogInformation("Comparison completed.");

                logger.LogInformation("  Detected {ResultsCriticalIssueCount} critical issue(s).", results.CriticalIssueCount);
                logger.LogInformation("  Detected {ResultsMajorIssueCount} major issue(s).", results.MajorIssueCount);
                logger.LogInformation("  Detected {ResultsMinorIssueCount} minor issue(s).", results.MinorIssueCount);
                logger.LogInformation("  Detected {ResultsWarningIssueCount} warning issue(s).", results.WarningIssueCount);

                logger.LogInformation("  Time elapsed: {SwElapsed}", sw.Elapsed);

                if (String.IsNullOrWhiteSpace(OutputFileName))
                {
                    OutputFileName = $"MajorChangeCheckerResults_{results.Protocol}_{results.Version}";
                }

                logger.LogInformation("Writing results...");

                OutputDirectory.Create();
                foreach (var writer in GetResultWriters(OutputFileName))
                {
                    writer.WriteResults(results);
                }

                logger.LogInformation("Writing results completed");

                logger.LogInformation("Finished");
                return (int)ExitCodes.Ok;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to compare the protocol solution with the previous version.");
                return (int)ExitCodes.UnexpectedException;
            }
            finally
            {
                FileSystem.Instance.Directory.Delete(temporaryDirectory, true);
                logger.LogDebug("Finished {Method}.", nameof(CompareProtocolSolutionCommand));
            }
        }

        private async Task<string?> GetPreviousProtocolVersionAsync(IProtocolModel currentProtocol, string temporaryDirectory, string solutionFileDirectoryName, CancellationToken cancellationToken = default)
        {
            /* Catalog API checks */
            if (String.IsNullOrWhiteSpace(CatalogApiKey))
            {
                CatalogApiKey = configuration["DATAMINER_TOKEN"];
                if (String.IsNullOrWhiteSpace(CatalogApiKey))
                {
                    logger.LogError("Unable to fetch previous protocol version from Catalog due to missing Catalog API key.");
                    return null;
                }
            }

            ICatalogService catalogService = ArtifactDownloader.Downloader.FromCatalog(new HttpClient(), CatalogApiKey);

            string previousVersion = RetrievePreviousVersion(currentProtocol.Protocol);

            switch (previousVersion)
            {
                case "Missing":
                    logger.LogError("Unable to resolve the protocol version based on the provided protocol solution.");
                    return null;
                case "None":
                    // Current version is the first version, no previous version exists.
                    logger.LogInformation("Protocol solution is version 1.0.0.1, comparison will be skipped.");
                    return "Skipped";
            }

            Guid? catalogId = await TryGetCatalogIdAsync(solutionFileDirectoryName, currentProtocol, cancellationToken);
            if (catalogId != null)
            {
                return (await TryDownloadPreviousVersion(catalogId.Value, previousVersion, catalogService, temporaryDirectory))?.FullName;
            }

            logger.LogError("Unable to locate previous protocol version.");
            return null;
        }

        private async Task<Guid?> TryGetCatalogIdAsync(string solutionFileDirectoryName, IProtocolModel currentProtocol, CancellationToken cancellationToken = default)
        {
            /*
             * Priority order:
             *  - Catalog ID
             *  - Search Catalog ID in catalog.yml or manifest.yml (in case it would be overwritten by user)
             *  - Search Catalog ID in .githubtocatalog/auto-generated-catalog.yml (in case user did not overwrite it)
             *  - Search on public Catalog based on protocol name
             */

            /* Catalog ID */
            if (!String.IsNullOrWhiteSpace(CatalogId) && Guid.TryParse(CatalogId, out Guid catalogId))
            {
                return catalogId;
            }

            logger.LogInformation("No valid Catalog ID provided.");

            /* YAML files */
            if (TryGetCatalogIdFromYamlFiles(solutionFileDirectoryName, out Guid? catalogIdAsync))
            {
                return catalogIdAsync;
            }

            /* Public Catalog Search */
            logger.LogInformation("Falling back to searching on the public Catalog.");
            var publicId = await publicCatalogService.SearchConnectorOnNameAsync(currentProtocol.Protocol.Name.Value, cancellationToken);
            if (publicId != null)
            {
                logger.LogInformation("Found protocol with following Catalog ID: {PublicCatalogId}", publicId);
                return publicId.Value;
            }

            // TODO: Check if there could be a search call on the key Catalog API that could be used instead of the public Catalog.
            // As we already have a Catalog API key, it would be better to use that instead of relying on the public Catalog.
            // Check with Cloud team to see if this could be implemented.

            return null;
        }

        private bool TryGetCatalogIdFromYamlFiles(string solutionFileDirectoryName, out Guid? catalogId)
        {
            var deserializer = new DeserializerBuilder()
                               .WithNamingConvention(UnderscoredNamingConvention.Instance)
                               .IgnoreUnmatchedProperties()
                               .Build();

            /* catalog.yml / manifest.yml */
            logger.LogInformation("Falling back to searching the Catalog ID in the 'catalog.yml' or 'manifest.yml' file.");
            var catalogFilePath = FileSystem.Instance.Path.Combine(solutionFileDirectoryName, "catalog.yml");
            if (!FileSystem.Instance.File.Exists(catalogFilePath))
            {
                catalogFilePath = FileSystem.Instance.Path.Combine(solutionFileDirectoryName, "manifest.yml");
            }

            if (FileSystem.Instance.File.Exists(catalogFilePath))
            {
                string? id = deserializer.Deserialize<CatalogYaml>(FileSystem.Instance.File.ReadAllText(catalogFilePath)).Id;
                if (!String.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out Guid temp))
                {
                    logger.LogInformation("Found protocol with following Catalog ID: {CatalogId}", temp);
                    catalogId = temp;
                    return true;
                }

                logger.LogInformation("No valid Catalog ID found in YAML file.");
            }
            else
            {
                logger.LogInformation("No catalog.yml or manifest.yml file found.");
            }

            /* .githubtocatalog/auto-generated-catalog.yml */
            logger.LogInformation("Falling back to searching the Catalog ID in the '.githubtocatalog/auto-generated-catalog.yml' file.");
            catalogFilePath = FileSystem.Instance.Path.Combine(solutionFileDirectoryName, ".githubtocatalog", "auto-generated-catalog.yml");
            if (FileSystem.Instance.File.Exists(catalogFilePath))
            {
                string? id = deserializer.Deserialize<CatalogYaml>(FileSystem.Instance.File.ReadAllText(catalogFilePath)).Id;

                if (!String.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out Guid temp))
                {
                    logger.LogInformation("Found protocol with following Catalog ID: {CatalogId}", temp);
                    catalogId = temp;
                    return true;
                }

                logger.LogInformation("No valid Catalog ID found in YAML file.");
            }
            else
            {
                logger.LogInformation("No YAML file found.");
            }

            catalogId = null;
            return false;
        }

        private async Task<IFileInfoIO?> TryDownloadPreviousVersion(Guid id, string previousVersion, ICatalogService catalogService, string temporaryDirectory)
        {
            try
            {
                CatalogDownloadResult downloadResult = await catalogService.DownloadCatalogItemAsync(CatalogIdentifier.WithVersion(id, previousVersion));

                if (downloadResult.Type != PackageType.Dmprotocol)
                {
                    throw new InvalidDataException("Downloaded package is not a protocol.");
                }

                using MemoryStream ms = new MemoryStream(downloadResult.Content);
                using ZipArchive archive = new ZipArchive(ms);
                archive.ExtractToDirectory(temporaryDirectory);

                var fileInfo = new FileInfo(FileSystem.Instance.Path.Combine(temporaryDirectory, "Protocol", "Protocol.xml"));
                if (!fileInfo.Exists)
                {
                    throw new FileNotFoundException("Downloaded protocol from Catalog did not contain a Protocol.xml file");
                }

                return fileInfo;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to download protocol with ID '{CatalogId}' from Catalog.", id);
                return null;
            }
        }

        private static string RetrievePreviousVersion(IProtocol protocol)
        {
            string previousVersion = "Missing";

            if (protocol.Version?.Value == null || !Version.TryParse(protocol.Version.Value, out Version? protocolVersion))
            {
                return previousVersion;
            }

            string currentVersion = protocol.Version.Value;

            if (currentVersion == "1.0.0.1")
            {
                // This is the initial version, it has no previous version.
                return "None";
            }

            if (protocolVersion.Revision == 1)
            {
                if (protocolVersion.Build > 0 || protocolVersion.Minor > 0)
                {
                    previousVersion = "Missing";
                }
                else
                {
                    // This means the current version is an X.0.0.1 where X is > 1. 
                    // If no basedOn attribute is used, it indicates that this is a brand-new development.
                    // If a basedOn attribute is used, it will be detected later in the program.
                    previousVersion = "None";
                }
            }

            // Check if VersionHistory contains a basedOn attribute for this version.
            if (protocol.TryGetBasedOnVersion(out Version version))
            {
                previousVersion = version.ToString();
            }
            else
            {
                // If basedOn attribute is not explicitly specified, we assume it is based on the previous minor version.
                if (protocolVersion.Revision > 1)
                {
                    previousVersion = $"{protocolVersion.Major}.{protocolVersion.Minor}.{protocolVersion.Build}.{protocolVersion.Revision - 1}";
                }
            }

            return previousVersion;
        }

        [YamlSerializable(typeof(CatalogYaml))]
        private sealed class CatalogYaml
        {
            /// <summary>
            /// Gets or sets the unique identifier for the catalog entry.
            /// </summary>
            /// <value>A string representing the unique ID of the catalog entry.</value>
            [YamlMember]
            public string? Id { get; set; }
        }
    }
}
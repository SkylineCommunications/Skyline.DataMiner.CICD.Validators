using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Extensions.Logging;
using Skyline.ArtifactDownloader;
using Skyline.ArtifactDownloader.Exceptions;
using Skyline.ArtifactDownloader.Identifiers;
using Skyline.ArtifactDownloader.Services;
using Skyline.DataMiner.CICD.Models.Protocol.Read;
using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
using Skyline.DataMiner.Net.Messages;

namespace Skyline.DataMiner.CICD.Tools.Validator
{
    internal class CatalogService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly XDocument doc;

        private readonly XmlNamespaceManager mgr;

        private readonly XNamespace ns;


        public CatalogService(ILogger logger, string apiKey, string protocolXml)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _apiKey = apiKey;
            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
            }

            if (!string.IsNullOrEmpty(protocolXml))
            {
                doc = XDocument.Parse(protocolXml);
                ns = doc.Root.GetDefaultNamespace();
                mgr = new XmlNamespaceManager(new NameTable());
                mgr.AddNamespace("ns", ns.NamespaceName);
            }
        }
        public async Task<string> DownloadPreviousProtocolVersion(string catalogId, IProtocolModel currentProtocol)
        {
            try
            {
                if (doc == null)
                {
                    throw new InvalidOperationException("Protocol XML document is not initialized.");
                }

                string protocolName = RetrieveName();
                string currentVersion = currentProtocol.Protocol?.Version?.Value;

                if (string.IsNullOrEmpty(currentVersion))
                {
                    throw new Exception("Could not determine protocol version from protocol.xml");
                }

                string previousVersion = RetrievePreviousVersion();

                if (previousVersion == "None")
                {
                    _logger.LogInformation($"No previous version available for protocol {protocolName} (current version: {currentVersion}). Comparison cannot be performed.");
                    return null; 
                }

                if (previousVersion == "Missing")
                {
                    throw new Exception("Previous version information is missing. Cannot determine which version to download.");
                }

                if (string.IsNullOrEmpty(previousVersion))
                {
                    throw new Exception("Could not determine previous protocol version.");
                }

                if (!Guid.TryParse(catalogId, out Guid catalogGuid))
                {
                    throw new ArgumentException($"Catalog ID '{catalogId}' is not a valid GUID.");
                }

                var downloader = Downloader.FromCatalog(new HttpClient(), _apiKey);

                byte[] content = null;
                CatalogDownloadResult downloadResult = null;

                try
                {
                    downloadResult = await downloader.DownloadCatalogItemAsync(
                        CatalogIdentifier.WithVersion(catalogGuid, previousVersion));
                    content = downloadResult?.Content;
                }
                catch (ArtifactDownloadException ex)
                {
                    _logger.LogWarning($"Download failed with catalog ID {catalogId}: {ex.Message}. Searching public catalog...");

                    string publicCatalogId = await FindCatalogIdFromPublicCatalog(protocolName);

                    if (!string.IsNullOrEmpty(publicCatalogId) && Guid.TryParse(publicCatalogId, out Guid publicGuid))
                    {
                        _logger.LogInformation($"Found public catalog ID {publicGuid}. Retrying download...");

                        try
                        {
                            downloadResult = await downloader.DownloadCatalogItemAsync(
                                CatalogIdentifier.WithVersion(publicGuid, previousVersion));
                            content = downloadResult?.Content;
                        }
                        catch (Exception retryEx)
                        {
                            _logger.LogError(retryEx, $"Failed to download with public catalog ID {publicCatalogId}");
                            throw new Exception($"Failed to download protocol version {previousVersion} even with public catalog ID", retryEx);
                        }
                    }
                    else
                    {
                        _logger.LogError("Could not find valid catalog ID from public catalog");
                        throw new Exception($"Failed to find valid catalog ID for protocol: {protocolName}");
                    }
                }

                if (content == null || content.Length == 0)
                {
                    throw new Exception($"Failed to download protocol version {previousVersion}");
                }

                string tempFilePath = Path.Combine(Path.GetTempPath(), $"{protocolName}_{previousVersion}.xml");
                await File.WriteAllBytesAsync(tempFilePath, content);

                _logger.LogInformation($"Downloaded previous version {previousVersion} to: {tempFilePath}");
                return tempFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download previous protocol version from catalog");
                throw;
            }
        }

        public async Task<string> FindCatalogIdFromPublicCatalog(string protocolName)
        {

            using (var publicCatalogClient = new HttpClient())
            {
                try
                {
                    if (string.IsNullOrEmpty(protocolName))
                    {
                        _logger.LogError("Protocol name is required to search public catalog");
                        return null;
                    }

                    string searchUrl = $"https://api.dataminer.services/api/public-catalog/v2-0/catalogs/search?query={Uri.EscapeDataString(protocolName)}&pageLimit=50";

                    HttpResponseMessage response = await publicCatalogClient.GetAsync(searchUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError($"Public catalog API returned {response.StatusCode}: {errorContent}");
                        return null;
                    }

                    string responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogDebug($"Public catalog response: {responseContent}");

                    using JsonDocument doc = JsonDocument.Parse(responseContent);
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("catalogSearchPage", out JsonElement searchPage) && searchPage.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement catalog in searchPage.EnumerateArray())
                        {
                            if (catalog.TryGetProperty("name", out JsonElement nameElement)&& catalog.TryGetProperty("type", out JsonElement typeElement))
                            {
                                string catalogName = nameElement.GetString();
                                string catalogType = typeElement.GetString();

                                if (catalogName != null && catalogName.Equals(protocolName, StringComparison.OrdinalIgnoreCase) && catalogType != null && catalogType.Equals("Connector", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (catalog.TryGetProperty("id", out JsonElement idElement))
                                    {
                                        return idElement.GetString();
                                    }
                                }
                            }
                        }

                        foreach (JsonElement catalog in searchPage.EnumerateArray())
                        {
                            if (catalog.TryGetProperty("id", out JsonElement idElement))
                            {
                                return idElement.GetString();
                            }
                        }
                    }

                    _logger.LogWarning($"No catalog ID found in response for protocol: {protocolName}");
                    _logger.LogDebug($"Response content: {responseContent}");
                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to search public catalog for protocol: {ProtocolName}", protocolName);
                    return null;
                }
            }
        }

        public string RetrievePreviousVersion()
        {
            var previousVersion = "Missing";

            XElement versionElement = this.doc.XPathSelectElement("ns:Protocol/ns:Version", this.mgr);

            if (versionElement == null)
            {
                return previousVersion;
            }

            string currentVersion = versionElement.Value;

            if (currentVersion == "1.0.0.1")
            {
                // This is the initial version, it has no previous version.
                return "None";
            }

            var protocolVersion = new ProtocolVersion(currentVersion);

            if (protocolVersion.MinorVersion == 1)
            {
                if (protocolVersion.MajorVersion > 0 || protocolVersion.SystemVersion > 0)
                {
                    previousVersion = "Missing";
                }
                else
                {
                    // This means the current version is a X.0.0.1 where X is > 1. 
                    // If no basedOn attribute is used, it indicates that this is a brand new development.
                    // If a basedOn attribute is used, it will be detected later in the program.
                    previousVersion = "None";
                }
            }

            // Check if VersionHistory contains a basedOn attribute for this version.
            string basedOnVersion = this.RetrieveBasedOnVersion(protocolVersion);

            if (basedOnVersion != null)
            {
                previousVersion = basedOnVersion;
            }
            else
            {
                // If basedOn attribute is not explicitly specified, we assume it is based on the previous minor version.
                if (protocolVersion.MinorVersion > 1)
                {
                    previousVersion = protocolVersion.BranchVersion
                                    + "."
                                    + protocolVersion.SystemVersion
                                    + "."
                                    + protocolVersion.MajorVersion
                                    + "."
                                    + (protocolVersion.MinorVersion - 1);
                }
            }

            return previousVersion;
        }
        public string RetrieveBasedOnVersion(ProtocolVersion protocolVersion)
        {
            string basedOnVersion = null;

            XElement minorVersionElement = this.doc.XPathSelectElement(
                "ns:Protocol/ns:VersionHistory/ns:Branches/ns:Branch[@id='"
                + protocolVersion.BranchVersion
                + "']/ns:SystemVersions/ns:SystemVersion[@id='"
                + protocolVersion.SystemVersion
                + "']/ns:MajorVersions/ns:MajorVersion[@id='"
                + protocolVersion.MajorVersion
                + "']/ns:MinorVersions/ns:MinorVersion[@id='"
                + protocolVersion.MinorVersion
                + "']",
                this.mgr);

            string value = minorVersionElement?.Attribute("basedOn")?.Value;

            if (!string.IsNullOrWhiteSpace(value))
            {
                basedOnVersion = value;
            }

            return basedOnVersion;
        }
        public string RetrieveName()
        {
            XElement nameElement = this.doc.XPathSelectElement("ns:Protocol/ns:Name", this.mgr);

            if (nameElement != null && !string.IsNullOrWhiteSpace(nameElement.Value))
            {
                return nameElement.Value;
            }

            throw new InvalidOperationException(
                "The protocol.xml file does not contain a Name element or it is empty.");
        }
    }
}
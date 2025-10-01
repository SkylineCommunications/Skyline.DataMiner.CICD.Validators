namespace Skyline.DataMiner.CICD.Tools.Validator
{
    using System.Text.Json;

    internal class PublicCatalogService(ILogger<PublicCatalogService> logger)
    {
        private const string publicCatalogApi = "https://api.dataminer.services/api/public-catalog/v2-0/";

        public async Task<Guid?> SearchConnectorOnNameAsync(string protocolName)
        {
            using var publicCatalogClient = new HttpClient();

            try
            {
                if (String.IsNullOrWhiteSpace(protocolName))
                {
                    logger.LogError("Protocol name is required to search public Catalog");
                    return null;
                }

                string searchUrl = $"{publicCatalogApi}catalogs/search?query={Uri.EscapeDataString(protocolName)}&pageLimit=50&typeFilters=Connector";

                HttpResponseMessage response = await publicCatalogClient.GetAsync(searchUrl);

                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Public Catalog API returned {ResponseStatusCode}: {ErrorContent}", response.StatusCode, content);
                    return null;
                }

                logger.LogDebug("Public catalog response: {ResponseContent}", content);

                if (String.IsNullOrWhiteSpace(content))
                {
                    logger.LogError("Public Catalog API returned an empty response on the search call.");
                    return null;
                }

                using JsonDocument doc = JsonDocument.Parse(content);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("catalogSearchPage", out JsonElement searchPage) && searchPage.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement catalog in searchPage.EnumerateArray())
                    {
                        if (catalog.TryGetProperty("name", out JsonElement nameElement) && String.Equals(protocolName, nameElement.GetString(), StringComparison.OrdinalIgnoreCase) &&
                            catalog.TryGetProperty("id", out JsonElement idElement) && Guid.TryParse(idElement.GetString(), out Guid id))
                        {
                            return id;
                        }
                    }
                }

                logger.LogWarning("No valid Catalog ID found in response for protocol: {ProtocolName}", protocolName);
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to search public Catalog for protocol: {ProtocolName}", protocolName);
                return null;
            }
        }
    }
}
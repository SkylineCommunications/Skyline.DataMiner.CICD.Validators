namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters
{
    using System.IO;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.Results;

    internal class ResultWriterJson(string resultsFilePath, ILogger logger) : IResultWriter
    {
        public void WriteResults(ValidatorResults results)
        {
            logger.LogInformation("  Writing results to {ResultsFilePath}...", resultsFilePath);

            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            serializer.Converters.Add(new StringEnumConverter());

            using StreamWriter sw = new StreamWriter(resultsFilePath);
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, results);
        }

        public void WriteResults(MajorChangeCheckerResults results)
        {
            logger.LogInformation("  Writing results to {ResultsFilePath}...", resultsFilePath);

            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            serializer.Converters.Add(new StringEnumConverter());

            using StreamWriter sw = new StreamWriter(resultsFilePath);
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, results);
        }
    }
}

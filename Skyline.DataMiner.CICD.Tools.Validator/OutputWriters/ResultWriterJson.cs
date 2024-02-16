namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters
{
    using System;
    using System.IO;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    internal class ResultWriterJson : IResultWriter
    {
        private readonly string resultsFilePath;
        private readonly ILogger logger;

        public ResultWriterJson(string resultsFilePath, ILogger logger)
        {
            this.resultsFilePath = resultsFilePath;
            this.logger = logger;
        }

        public void WriteResults(ValidatorResults validatorResults)
        {
            logger.LogInformation("  Writing results to " + resultsFilePath + "...");

            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;
            serializer.Converters.Add(new StringEnumConverter());

            using (StreamWriter sw = new StreamWriter(resultsFilePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, validatorResults);
            }
        }
    }
}

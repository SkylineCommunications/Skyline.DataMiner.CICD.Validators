﻿namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters
{
    using System;
    using System.IO;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    internal class ResultWriterJson : IResultWriter
    {
        private readonly string resultsFilePath;

        public ResultWriterJson(string resultsFilePath)
        {
            this.resultsFilePath = resultsFilePath;
        }

        public void WriteResults(ValidatorResults validatorResults)
        {
            Console.WriteLine("  Writing results to " + resultsFilePath + "...");

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
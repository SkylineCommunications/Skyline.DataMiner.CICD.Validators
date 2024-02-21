namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters
{
    using System.IO;
    using System.Xml.Serialization;

    using Microsoft.Extensions.Logging;

    internal class ResultWriterXml : IResultWriter
    {
        private readonly string resultsFilePath;
        private readonly ILogger logger;

        public ResultWriterXml(string resultsFilePath, ILogger logger)
        {
            this.resultsFilePath = resultsFilePath;
            this.logger = logger;
        }

        public void WriteResults(ValidatorResults validatorResults)
        {
            logger.LogInformation("  Writing results to " + resultsFilePath + "...");

            FileStream fs = new FileStream(resultsFilePath, FileMode.Create);
            XmlSerializer s = new XmlSerializer(typeof(ValidatorResults));
            s.Serialize(fs, validatorResults);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ValidatorResults));
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, validatorResults);
            }
        }
    }
}

namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters
{
    using System.IO;
    using System.Xml.Serialization;

    using Microsoft.Extensions.Logging;

    using Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.Results;

    internal class ResultWriterXml(string resultsFilePath, ILogger logger) : IResultWriter
    {
        public void WriteResults(ValidatorResults results)
        {
            logger.LogInformation("  Writing results to {ResultsFilePath}...", resultsFilePath);

            FileStream fs = new FileStream(resultsFilePath, FileMode.Create);
            XmlSerializer s = new XmlSerializer(typeof(ValidatorResults));
            s.Serialize(fs, results);
        }

        public void WriteResults(MajorChangeCheckerResults results)
        {
            logger.LogInformation("  Writing results to {ResultsFilePath}...", resultsFilePath);

            FileStream fs = new FileStream(resultsFilePath, FileMode.Create);
            XmlSerializer s = new XmlSerializer(typeof(MajorChangeCheckerResults));
            s.Serialize(fs, results);
        }
    }
}

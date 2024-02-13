namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    internal class ResultWriterXml : IResultWriter
    {
        private readonly string resultsFilePath;

        public ResultWriterXml(string resultsFilePath)
        {
            this.resultsFilePath = resultsFilePath;
        }

        public void WriteResults(ValidatorResults validatorResults)
        {
            Console.WriteLine("  Writing results to " + resultsFilePath + "...");

            FileStream fs = new FileStream(resultsFilePath, FileMode.OpenOrCreate);
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

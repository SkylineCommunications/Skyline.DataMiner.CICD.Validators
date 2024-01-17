namespace ExportErrorMessages.Serialization
{
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Static class that contains all the methods to read and write to and from the XML file.
    /// Also the methods to convert the XML class hierarchy to a list of check objects, is located in here.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Reads the XML file and converts the content in a <see cref="Validator"/> object.
        /// </summary>
        /// <param name="filePath">The relative or absolute path to the XML file.</param>
        /// <returns>All the data from the XML file in the corresponding class hierarchy.</returns>
        public static Validator ReadXml(string filePath)
        {
            Validator validator;
            XmlSerializer serializer = new XmlSerializer(typeof(Validator));

            using (StreamReader reader = new StreamReader(filePath))
            {
                validator = (Validator)serializer.Deserialize(reader);
                reader.Close();
            }
            
            return validator;
        }
    }
}
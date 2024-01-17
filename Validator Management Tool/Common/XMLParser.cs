namespace Validator_Management_Tool.Common
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Model;

    /// <summary>
    /// The old implementation to convert the XML file into check objects.
    /// This class could only read them and not write them back to the XML file.
    /// It is not used in the current application.
    /// </summary>
    public static class XMLParser
    {
        /// <summary>
        /// Gets the different data types that are used in a dictionary, to switch between them.
        /// </summary>
        public static Dictionary<Type, int> Types { get; } = new Dictionary<Type, int>
        {
            { typeof(string), 0 },
            { typeof(uint), 1 },
            { typeof(bool), 2 },
            { typeof(Severity), 3 },
            { typeof(Certainty), 4 },
            { typeof(Category), 5 },
            { typeof(int), 6 },
            { typeof(Source), 7 },
            { typeof(FixImpact), 8 },
        };
    }
}
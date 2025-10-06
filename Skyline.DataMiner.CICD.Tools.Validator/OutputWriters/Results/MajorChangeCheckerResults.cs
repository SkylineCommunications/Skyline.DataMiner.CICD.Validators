namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.Results
{
    using System.Xml.Serialization;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    /// <summary>
    /// Represents the major change checker results.
    /// </summary>
    /// <param name="inputData"></param>
    /// <param name="previousInputData"></param>
    [Serializable]
    [XmlRoot("MajorChangeCheckerResults")]
    public class MajorChangeCheckerResults(IProtocolInputData inputData, IProtocolInputData? previousInputData) : ValidatorResults(inputData)
    {
        /// <summary>
        /// Gets or sets the name of the previous protocol.
        /// </summary>
        public string PreviousProtocol { get; } = previousInputData?.Model?.Protocol?.Name?.Value ?? Unknown;

        /// <summary>
        /// Gets or sets the version of the previous protocol.
        /// </summary>
        public string PreviousVersion { get; }  = previousInputData?.Model?.Protocol?.Version?.Value ?? Unknown;
    }
}

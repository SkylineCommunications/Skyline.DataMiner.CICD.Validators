namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters
{
    using Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.Results;

    /// <summary>
    /// Results writer interface.
    /// </summary>
    internal interface IResultWriter
    {
        /// <summary>
        /// Writes the specified validator results to the specified output file.
        /// </summary>
        /// <param name="results">The validator results.</param>
        void WriteResults(ValidatorResults results);

        /// <summary>
        /// Writes the specified comparison results to the specified output file.
        /// </summary>
        /// <param name="results">The comparison results.</param>
        void WriteResults(MajorChangeCheckerResults results);
    }
}

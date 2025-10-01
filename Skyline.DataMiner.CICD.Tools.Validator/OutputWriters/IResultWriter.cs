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
        /// <param name="validatorResults">The validator results.</param>
        void WriteResults(ValidatorResults validatorResults);

        /// <summary>
        /// Writes the specified comparison results to the specified output file.
        /// </summary>
        /// <param name="majorChangeCheckerResults">The comparison results.</param>
        void WriteResults(MajorChangeCheckerResults majorChangeCheckerResults);
    }
}

namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters
{
	/// <summary>
	/// Results writer interface.
	/// </summary>
	public interface IResultWriter
	{
		/// <summary>
		/// Writes the specified validator results to the specified output file.
		/// </summary>
		/// <param name="validatorResults">The validator results.</param>
		void WriteResults(ValidatorResults validatorResults);
	}
}

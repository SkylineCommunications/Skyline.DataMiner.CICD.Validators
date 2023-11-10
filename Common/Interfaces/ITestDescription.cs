namespace Skyline.DataMiner.CICD.Validators.Common.Interfaces
{
	using Skyline.DataMiner.CICD.Validators.Common.Model;

	/// <summary>
	/// Represents a test description.
	/// </summary>
	public interface ITestDescription
    {
        /// <summary>
        /// Gets the test ID.
        /// </summary>
        /// <value>The test ID.</value>
        uint Id { get; }

		/// <summary>
		/// Gets the test name.
		/// </summary>
		/// <value>The test name.</value>
		string Name { get; }

		/// <summary>
		/// Gets the test category.
		/// </summary>
		/// <value>The test category.</value>
		ITestCategory Category { get; }

		/// <summary>
		/// Gets the result severity.
		/// </summary>
		/// <value>The result severity.</value>
		Severity ResultSeverity { get; }
    }
}

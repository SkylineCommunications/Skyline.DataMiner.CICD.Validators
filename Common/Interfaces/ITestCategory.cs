namespace Skyline.DataMiner.CICD.Validators.Common.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a test category.
    /// </summary>
    public interface ITestCategory
    {
		/// <summary>
		/// Gets the ID of the test category.
		/// </summary>
		/// <value>The ID of the test category.</value>
		int Id { get; }

		/// <summary>
		/// Gets the name of the test category.
		/// </summary>
		/// <value>The name of the test category.</value>
		string Name { get; }

		/// <summary>
		/// Gets the tests that belong to this test category.
		/// </summary>
		/// <value>The tests that belong to this test category.</value>
		IList<ITestDescription> Tests { get; }
    }
}
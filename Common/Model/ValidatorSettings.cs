namespace Skyline.DataMiner.CICD.Validators.Common.Model
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Common;
    using Skyline.DataMiner.XmlSchemas.Protocol;

    /// <summary>
    /// Represents the validator settings.
    /// </summary>
    public class ValidatorSettings
    {
        private readonly List<(Category catergory, uint checkId)> testsToExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorSettings"/> class.
        /// </summary>
        public ValidatorSettings(DataMinerVersion minimumSupportedDataMinerVersion)
        {
	        testsToExecute = new List<(Category catergory, uint checkId)>();
	        UnitList = new UnitList();

            MinimumSupportedDataMinerVersion = minimumSupportedDataMinerVersion;
        }

        /// <summary>
        /// Gets or sets the expected provider.
        /// </summary>
        /// <value>The expected provider.</value>
        public string ExpectedProvider { get; set; }

        /// <summary>
        /// Gets or sets the minimum supported DataMiner version.
        /// </summary>
        public DataMinerVersion MinimumSupportedDataMinerVersion { get; }

        public IUnitList UnitList { get; set; }

        /// <summary>
        /// Gets or sets the tests to execute. If this list is empty, all test will be executed.
        /// </summary>
        public IReadOnlyList<(Category catergory, uint checkId)> TestsToExecute => testsToExecute.AsReadOnly();

        /// <summary>
        /// Add a test to be executed.
        /// </summary>
        /// <param name="tests">Tests to execute.</param>
        public ValidatorSettings WithTestToExecute(params (Category category, uint checkId)[] tests)
        {
            testsToExecute.AddRange(tests);
            return this;
        }

        /// <summary>
        /// Add a test to be executed.
        /// </summary>
        /// <param name="tests">Tests to execute.</param>
        public ValidatorSettings WithTestToExecute(IEnumerable<(Category category, uint checkId)> tests)
        {
            testsToExecute.AddRange(tests);
            return this;
        }
    }
}

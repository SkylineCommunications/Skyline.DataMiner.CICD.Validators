namespace Skyline.DataMiner.CICD.Validators.Common.Model
{
    using System;
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
        /// Used for unit tests.
        /// </summary>
        internal ValidatorSettings()
        {
            testsToExecute = new List<(Category catergory, uint checkId)>();
            UnitList = new UnitList();
            MinimumSupportedDataMinerVersion = new DataMinerVersion(new Version(10, 1, 0, 0), 9966);
            ExpectedProvider = "Skyline Communications";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorSettings"/> class.
        /// </summary>
        /// <param name="minimumSupportedDataMinerVersion">The minimum supported DataMiner version.</param>
        /// <param name="unitList">The unit list.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="minimumSupportedDataMinerVersion"/> or <paramref name="unitList"/> is <see langword="null"/>.
        /// </exception>
        public ValidatorSettings(DataMinerVersion minimumSupportedDataMinerVersion, IUnitList unitList)
        {
            testsToExecute = new List<(Category catergory, uint checkId)>();
            UnitList = unitList ?? throw new ArgumentNullException(nameof(unitList));
            MinimumSupportedDataMinerVersion = minimumSupportedDataMinerVersion ?? throw new ArgumentNullException(nameof(minimumSupportedDataMinerVersion));
        }

        /// <summary>
        /// Gets or sets the expected provider.
        /// </summary>
        /// <value>The expected provider.</value>
        public string ExpectedProvider { get; set; }

        /// <summary>
        /// Gets the minimum supported DataMiner version.
        /// </summary>
        public DataMinerVersion MinimumSupportedDataMinerVersion { get; }

        /// <summary>
        /// Gets the unit list.
        /// </summary>
        public IUnitList UnitList { get; }

        /// <summary>
        /// Gets or sets the tests to execute. If this list is empty, all test will be executed.
        /// </summary>
        public IReadOnlyList<(Category category, uint checkId)> TestsToExecute => testsToExecute.AsReadOnly();

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

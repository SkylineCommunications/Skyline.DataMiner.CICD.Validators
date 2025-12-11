namespace Skyline.DataMiner.CICD.Tools.Validator.Helpers
{
    using Skyline.DataMiner.CICD.Common;

    internal static class Globals
    {
        /// <summary>
        /// Gets the minimum DataMiner version with its build number for which support is still given.
        /// </summary>
        /// <value>The minimum DataMiner version with its build number for which support is still given.</value>
        public static DataMinerVersion MinSupportedDataMinerVersionWithBuildNumber { get; } = DataMinerVersion.Parse("10.3.0.0 - 12752");
    }
}
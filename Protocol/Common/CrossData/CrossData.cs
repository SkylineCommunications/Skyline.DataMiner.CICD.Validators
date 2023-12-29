namespace Skyline.DataMiner.CICD.Validators.Protocol.Common.CrossData
{
    /// <summary>
    /// Data that is used across multiple checks.
    /// </summary>
    internal class CrossData
    {
        /// <summary>
        /// Gets the data for the <see cref="Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.RTDisplay.CheckRTDisplayTag.CheckRTDisplayTag"/> check.
        /// </summary>
        public RtDisplayData RtDisplay { get; } = new RtDisplayData();
    }
}
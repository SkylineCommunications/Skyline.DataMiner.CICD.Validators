namespace SLDisValidator2.Common.CrossData
{
    /// <summary>
    /// Data that is used across multiple checks.
    /// </summary>
    public class CrossData
    {
        /// <summary>
        /// Gets the data for the <see cref="SLDisValidator2.Tests.Protocol.Params.Param.Display.RTDisplay.CheckRTDisplayTag.CheckRTDisplayTag"/> check.
        /// </summary>
        public RtDisplayData RtDisplay { get; } = new RtDisplayData();
    }
}
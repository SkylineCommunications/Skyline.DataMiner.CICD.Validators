namespace Skyline.DataMiner.CICD.Validators.Common.Interfaces
{
    /// <summary>
    /// Protocol QAction helper code provider interface;
    /// </summary>
    public interface IProtocolQActionHelperProvider
    {
        /// <summary>
        /// Retrieves the protocol QAction helper code provider.
        /// </summary>
        /// <param name="protocolCode">The protocol XML code.</param>
        /// <param name="ignoreErrors">Value indicating whether errors during helper code generation should be ignored.</param>
        /// <returns>The protocol QAction helper code.</returns>
        string GetProtocolQActionHelper(string protocolCode, bool ignoreErrors = false);
    }
}

namespace Skyline.DataMiner.CICD.Validators.Common.Interfaces
{
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Parsers.Common.Xml;

    /// <summary>
    /// Protocol input data interface.
    /// </summary>
    public interface IProtocolInputData
    {
        /// <summary>
        /// Gets the XML document.
        /// </summary>
        /// <value>The XML document.</value>
        XmlDocument Document { get; }

        /// <summary>
        /// Gets the protocol model.
        /// </summary>
        /// <value>The protocol model.</value>
        IProtocolModel Model { get; }

        /// <summary>
        /// Gets the QAction compilation model.
        /// </summary>
        /// <value>The QAction compilation model.</value>
        QActionCompilationModel QActionCompilationModel { get; }
    }
}

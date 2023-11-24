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
        /// Gets the protocol code.
        /// </summary>
        /// <value>The protocol code.</value>
        string Code { get; }

        /// <summary>
        /// Gets the QAction compilation model.
        /// </summary>
        /// <value>The QAction compilation model.</value>
        QActionCompilationModel QActionCompilationModel { get; }

		/// <summary>
		/// If this is the model of an exported protocol, this property contains the model of the main protocol that was used to create the export.
		/// </summary>
		/// <value>The model of the main protocol</value>
		IProtocolModel MainProtocolModel { get; }

        /// <summary>
        /// Gets a value indicating whether this is the model of an exported protocol.
        /// </summary>
        /// <value><c>true</c> if this is the model of an exported protocol; otherwise, <c>false</c>.</value>
        bool IsExportedProtocol { get; }
    }
}

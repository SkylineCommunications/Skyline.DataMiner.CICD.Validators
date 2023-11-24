namespace Skyline.DataMiner.CICD.Validators.Common.Data
{
    using System;

    using Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Parsers.Common.Xml;

    /// <summary>
    /// Protocol input data.
    /// </summary>
    public class ProtocolInputData : IProtocolInputData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProtocolInputData"/> class.
		/// </summary>
		/// <param name="model">The protocol model.</param>
		/// <param name="document">The document.</param>
		/// <param name="protocolCode">The protocol code.</param>
		/// <exception cref="ArgumentNullException"><paramref name="model"/>, <paramref name="document"/>, <paramref name="protocolCode"/> or <paramref name="lineInfoProvider"/> are <see langword="null"/>.</exception>
		public ProtocolInputData(IProtocolModel model, XmlDocument document, string protocolCode)
		{
			Model = model ?? throw new ArgumentNullException(nameof(model));
			Document = document ?? throw new ArgumentNullException(nameof(document));
			Code = protocolCode ?? throw new ArgumentNullException(nameof(protocolCode));
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolInputData"/> class.
        /// </summary>
        /// <param name="model">The protocol model.</param>
        /// <param name="document">The document.</param>
        /// <param name="protocolCode">The protocol code.</param>
        /// <param name="qactionCompilationModel">The QAction compilation model.</param>
        /// <exception cref="ArgumentNullException"><paramref name="model"/>, <paramref name="document"/>, <paramref name="protocolCode"/> or <paramref name="lineInfoProvider"/> are <see langword="null"/>.</exception>
        public ProtocolInputData(IProtocolModel model, XmlDocument document, string protocolCode, QActionCompilationModel qactionCompilationModel)
			: this(model, document, protocolCode)
		{
			QActionCompilationModel = qactionCompilationModel;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProtocolInputData"/> class.
		/// </summary>
		/// <param name="protocolCode">The protocol code.</param>
		/// <param name="qactionCompilationModel">The QAction compilation model.</param>
		/// <exception cref="ArgumentNullException"><paramref name="protocolCode"/> is <see langword="null"/>.</exception>
		public ProtocolInputData(string protocolCode, QActionCompilationModel qactionCompilationModel)
		{
			Code = protocolCode ?? throw new ArgumentNullException(nameof(protocolCode));

			var parser = new Parser(protocolCode);
			Document = parser.Document;
			Model = new ProtocolModel(Document);

			QActionCompilationModel = qactionCompilationModel;
		}

		/// <inheritdoc/>
		public XmlDocument Document { get; }

		/// <inheritdoc/>
		public IProtocolModel Model { get; }

		/// <inheritdoc/>
		public string Code { get; }
		
		/// <inheritdoc/>
		public QActionCompilationModel QActionCompilationModel { get; }

		/// <inheritdoc/>
		public IProtocolModel MainProtocolModel => Model?.MainProtocolModel;

		/// <inheritdoc/>
		public bool IsExportedProtocol => MainProtocolModel != null;
	}
}

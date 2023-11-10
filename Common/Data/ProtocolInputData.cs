namespace Skyline.DataMiner.CICD.Validators.Common.Data
{
	using System;

	using Interfaces;

	using Skyline.DataMiner.CICD.Models.Protocol;
	using Skyline.DataMiner.CICD.Models.Protocol.Read;
	using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
	using Skyline.DataMiner.CICD.Parsers.Common.Xml;
	using Skyline.DataMiner.CICD.Validators.Common.Tools;

	/// <summary>
	/// Protocol input data.
	/// </summary>
	public class ProtocolInputData : IProtocolInputData
	{
		private readonly Lazy<QActionCompilationModel> qactionCompilationModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProtocolInputData"/> class.
		/// </summary>
		/// <param name="model">The protocol model.</param>
		/// <param name="document">The document.</param>
		/// <param name="protocolCode">The protocol code.</param>
		/// <param name="lineInfoProvider">The line info provider.</param>
		/// <exception cref="ArgumentNullException"><paramref name="model"/>, <paramref name="document"/>, <paramref name="protocolCode"/> or <paramref name="lineInfoProvider"/> are <see langword="null"/>.</exception>
		public ProtocolInputData(IProtocolModel model, XmlDocument document, string protocolCode, ILineInfoProvider lineInfoProvider)
		{
			Model = model ?? throw new ArgumentNullException(nameof(model));
			Document = document ?? throw new ArgumentNullException(nameof(document));
			Code = protocolCode ?? throw new ArgumentNullException(nameof(protocolCode));
			LineInfoProvider = lineInfoProvider ?? throw new ArgumentNullException(nameof(lineInfoProvider));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProtocolInputData"/> class.
		/// </summary>
		/// <param name="model">The protocol model.</param>
		/// <param name="document">The document.</param>
		/// <param name="protocolCode">The protocol code.</param>
		/// <param name="lineInfo">The line info provider.</param>
		/// <param name="qactionCompilationModelProvider">The QAction compilation model provider.</param>
		/// <exception cref="ArgumentNullException"><paramref name="model"/>, <paramref name="document"/>, <paramref name="protocolCode"/> or <paramref name="lineInfoProvider"/> are <see langword="null"/>.</exception>
		public ProtocolInputData(IProtocolModel model, XmlDocument document, string protocolCode, ILineInfoProvider lineInfo, IQActionCompilationModelProvider qactionCompilationModelProvider)
			: this(model, document, protocolCode, lineInfo)
		{
			QActionCompilationModelProvider = qactionCompilationModelProvider;
			qactionCompilationModel = new Lazy<QActionCompilationModel>(() => CreateQActionCompilationModel(qactionCompilationModelProvider, model, document, protocolCode));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProtocolInputData"/> class.
		/// </summary>
		/// <param name="protocolCode">The protocol code.</param>
		/// <param name="qactionCompilationModelProvider">The QAction compilation model provider.</param>
		/// <exception cref="ArgumentNullException"><paramref name="protocolCode"/> is <see langword="null"/>.</exception>
		public ProtocolInputData(string protocolCode, IQActionCompilationModelProvider qactionCompilationModelProvider)
		{
			Code = protocolCode ?? throw new ArgumentNullException(nameof(protocolCode));

			var parser = new Parser(protocolCode);
			Document = parser.Document;
			Model = new ProtocolModel(Document);
			LineInfoProvider = new SimpleLineInfoProvider(protocolCode);

			QActionCompilationModelProvider = qactionCompilationModelProvider;
			qactionCompilationModel = new Lazy<QActionCompilationModel>(() => CreateQActionCompilationModel(qactionCompilationModelProvider, Model, Document, protocolCode));
		}

		/// <inheritdoc/>
		public XmlDocument Document { get; }

		/// <inheritdoc/>
		public IProtocolModel Model { get; }

		/// <inheritdoc/>
		public string Code { get; }

		/// <inheritdoc/>
		public ILineInfoProvider LineInfoProvider { get; }

		/// <inheritdoc/>
		public IQActionCompilationModelProvider QActionCompilationModelProvider { get; }

		/// <inheritdoc/>
		public IProtocolModel MainProtocolModel => Model?.MainProtocolModel;

		/// <inheritdoc/>
		public bool IsExportedProtocol => MainProtocolModel != null;

		private static QActionCompilationModel CreateQActionCompilationModel(IQActionCompilationModelProvider provider, IProtocolModel model, XmlDocument document, string code)
		{
			return provider?.GetQActionCompilationModel(document, model, code);
		}
	}
}

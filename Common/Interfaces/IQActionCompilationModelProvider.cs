namespace Skyline.DataMiner.CICD.Validators.Common.Interfaces
{
	using Skyline.DataMiner.CICD.Models.Protocol;
	using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
	using Skyline.DataMiner.CICD.Parsers.Common.Xml;

	/// <summary>
	/// Represents a the QAction compilation model provider for a connector.
	/// </summary>
	public interface IQActionCompilationModelProvider
    {
		/// <summary>
		/// Retrieves the QAction compilation model.
		/// </summary>
		/// <param name="document">The connector XML document.</param>
		/// <param name="model">The protocol model.</param>
		/// <param name="code">The code.</param>
		/// <returns>The QAction compilation model.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="document"/> or <paramref name="model"/> is <see langword="null"/>.</exception>
		QActionCompilationModel GetQActionCompilationModel(XmlDocument document, IProtocolModel model, string code);
    }
}

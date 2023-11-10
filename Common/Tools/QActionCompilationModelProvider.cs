namespace Skyline.DataMiner.CICD.Validators.Common.Tools
{
    using System;

    using Skyline.DataMiner.CICD.Parsers.Common.Xml;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;

    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Common;

    /// <summary>
    /// Represents a the QAction compilation model provider for a connector.
    /// </summary>
    public class QActionCompilationModelProvider : IQActionCompilationModelProvider
    {
        private readonly IAssemblyResolver _dllImportResolver;
        private readonly IProtocolQActionHelperProvider _qactionHelperProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="QActionCompilationModelProvider"/> class.
		/// </summary>
		/// <param name="assemblyResolver">The assembly resolver.</param>
		/// <param name="qactionHelperProvider">The QAction helper code provider.</param>
		/// <exception cref="ArgumentNullException"><paramref name="assemblyResolver"/> or <paramref name="qactionHelperProvider"/> is <see langword="null"/>.</exception>
		public QActionCompilationModelProvider(IAssemblyResolver assemblyResolver, IProtocolQActionHelperProvider qactionHelperProvider)
        {
            _dllImportResolver = assemblyResolver ?? throw new ArgumentNullException(nameof(assemblyResolver));
            _qactionHelperProvider = qactionHelperProvider ?? throw new ArgumentNullException(nameof(qactionHelperProvider));
        }

        /// <summary>
        /// Retrieves the QAction compilation model.
        /// </summary>
        /// <param name="document">The connector XML document.</param>
        /// <param name="model">The protocol model.</param>
        /// <param name="code">The code.</param>
        /// <returns>The QAction compilation model.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="document"/> or <paramref name="model"/> is <see langword="null"/>.</exception>
        public QActionCompilationModel GetQActionCompilationModel(XmlDocument document, IProtocolModel model, string code)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            string qactionHelperSourceCode = _qactionHelperProvider?.GetProtocolQActionHelper(code, ignoreErrors: true);

            var solution = new QActionCompilationModel(qactionHelperSourceCode, model, _dllImportResolver);

            return solution;
        }
    }
}

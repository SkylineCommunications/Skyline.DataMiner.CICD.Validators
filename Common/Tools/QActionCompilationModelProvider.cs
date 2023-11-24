namespace Skyline.DataMiner.CICD.Validators.Common.Tools
{
    using System;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.Models.Common;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    /// <summary>
    /// Represents a the QAction compilation model provider for a connector.
    /// </summary>
    internal class QActionCompilationModelProvider
    {
	    private readonly IProtocolModel model;
	    private readonly string xmlCode;
	    private readonly IAssemblyResolver assemblyResolver;
	    private readonly IProtocolQActionHelperProvider qactionHelperProvider;
	    private readonly Solution solution;

        /// <summary>
        /// Initializes a new instance of the <see cref="QActionCompilationModelProvider"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="model"/> or
        /// <paramref name="solution"/>
        /// is <see langword="null"/>.
        /// </exception>
        public QActionCompilationModelProvider(IProtocolModel model, Solution solution)
	    {
		    this.model = model ?? throw new ArgumentNullException(nameof(model));
		    this.solution = solution ?? throw new ArgumentNullException(nameof(solution));
	    }

        /// <summary>
        /// Initializes a new instance of the <see cref="QActionCompilationModelProvider"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="model"/> or
        /// <paramref name="xmlCode"/> or
        /// <paramref name="assemblyResolver"/> or
        /// <paramref name="qactionHelperProvider"/>
        /// is <see langword="null"/>.
        /// </exception>
        public QActionCompilationModelProvider(IProtocolModel model, string xmlCode, IAssemblyResolver assemblyResolver, IProtocolQActionHelperProvider qactionHelperProvider)
	    {
		    this.model = model ?? throw new ArgumentNullException(nameof(model));
		    this.xmlCode = xmlCode ?? throw new ArgumentNullException(nameof(xmlCode));
		    this.assemblyResolver = assemblyResolver ?? throw new ArgumentNullException(nameof(assemblyResolver));
		    this.qactionHelperProvider = qactionHelperProvider ?? throw new ArgumentNullException(nameof(qactionHelperProvider));
	    }
        
        /// <inheritdoc />
        public QActionCompilationModel GetQActionCompilationModel()
        {
	        if (solution == null)
	        {
		        string qactionHelperSourceCode = qactionHelperProvider.GetProtocolQActionHelper(xmlCode, ignoreErrors: true);
		        return new QActionCompilationModel(qactionHelperSourceCode, model, assemblyResolver);
            }

	        return new QActionCompilationModel(model, solution);
        }
    }
}

namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results
{
    using System;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    /// <summary>
    /// Represent a item that uses the feature.
    /// </summary>
    public class CSharpFeatureCheckResultItem : FeatureCheckResultItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFeatureCheckResultItem"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="csharp">The csharp.</param>
        /// <exception cref="System.ArgumentNullException">csharp</exception>
        public CSharpFeatureCheckResultItem(IReadable node, ICSharpObject<SyntaxNode> csharp) : base(node)
        {
            CSharp = csharp ?? throw new ArgumentNullException(nameof(csharp));
        }

        /// <summary>
        /// Gets the C# object.
        /// </summary>
        /// <value>
        /// The c sharp.
        /// </value>
        public ICSharpObject<SyntaxNode> CSharp { get; }
    }
}
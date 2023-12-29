namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results
{
    using System;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal class CSharpFeatureCheckResultItem : FeatureCheckResultItem
    {
        public ICSharpObject<SyntaxNode> CSharp { get; }

        public CSharpFeatureCheckResultItem(IReadable node, ICSharpObject<SyntaxNode> csharp) : base(node)
        {
            CSharp = csharp ?? throw new ArgumentNullException(nameof(csharp));
        }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System;
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("10.1.0.0-9966", "10.0.5.0-9164")]
    internal class ExecuteScript : IFeatureCheck
    {
        public string Title => "ExecuteScript";

        public string Description => "Execute Script From QAction.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 24475 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            if (context?.Model?.Protocol?.QActions == null || context.CompiledQActions == null)
            {
                return new FeatureCheckResult();
            }

            var items = new List<CSharpFeatureCheckResultItem>();

            foreach ((IQActionsQAction qaction, SyntaxTree syntaxTree, SemanticModel semanticModel) in context.EachQActionProjectsAndSyntaxTreesAndModels())
            {
                QActionAnalyzer analyzer = new QActionAnalyzer(items, qaction, semanticModel);
                RoslynVisitor parser = new RoslynVisitor(analyzer);

                parser.Visit(syntaxTree.GetRoot());
            }

            return new FeatureCheckResult(items);
        }

        private sealed class QActionAnalyzer : CSharpAnalyzerBase
        {
            private readonly List<CSharpFeatureCheckResultItem> items;
            private readonly IQActionsQAction qAction;
            private readonly SemanticModel semanticModel;

            public QActionAnalyzer(List<CSharpFeatureCheckResultItem> items, IQActionsQAction qAction, SemanticModel semanticModel)
            {
                this.items = items;
                this.qAction = qAction;
                this.semanticModel = semanticModel;
            }

            public override void CheckCallingMethod(CallingMethodClass callingMethod)
            {
                if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "ExecuteScript"))
                {
                    return;
                }

                items.Add(new CSharpFeatureCheckResultItem(qAction, callingMethod));
                Cancel();
            }
        }
    }
}
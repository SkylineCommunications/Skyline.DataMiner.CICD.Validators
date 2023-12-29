namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("10.0.0.0-9118", "9.6.13.0-8820")]
    internal class GetPkIdByTableId : IFeatureCheck
    {
        public string Title => "Get PK ID by Table ID";

        public string Description => "Get PK ID by Table ID.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 23579 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            if (context?.Model?.Protocol?.QActions == null || context.CompiledQActions == null)
            {
                return new FeatureCheckResult();
            }

            var items = new List<CSharpFeatureCheckResultItem>();

            foreach ((IQActionsQAction qaction, SyntaxTree syntaxTree, SemanticModel semanticModel, CompiledQActionProject projectData) in context.EachQActionProjectsAndSyntaxTreesAndModelsAndProjectDatas())
            {
                var solution = projectData.Project.Solution;
                QActionAnalyzer analyzer = new QActionAnalyzer(items, qaction, semanticModel, solution);
                RoslynVisitor parser = new RoslynVisitor(analyzer);

                parser.Visit(syntaxTree.GetRoot());
            }

            return new FeatureCheckResult(items);
        }

        private sealed class QActionAnalyzer : CSharpAnalyzerBase
        {
            private readonly IList<CSharpFeatureCheckResultItem> items;
            private readonly IQActionsQAction qAction;
            private readonly SemanticModel semanticModel;
            private readonly Solution solution;

            public QActionAnalyzer(IList<CSharpFeatureCheckResultItem> items, IQActionsQAction qAction, SemanticModel semanticModel, Solution solution)
            {
                this.items = items;
                this.qAction = qAction;
                this.semanticModel = semanticModel;
                this.solution = solution;
            }

            public override void CheckCallingMethod(CallingMethodClass callingMethod)
            {
                if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 394))
                {
                    return;
                }

                items.Add(new CSharpFeatureCheckResultItem(qAction, callingMethod));
                Cancel();
            }
        }
    }
}
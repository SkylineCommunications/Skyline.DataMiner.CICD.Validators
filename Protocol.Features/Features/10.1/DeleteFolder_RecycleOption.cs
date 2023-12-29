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

    //[MinDataMinerVersions("10.1.0.0-9966", "10.0.3.0-8964")]
    internal class DeleteFolderRecycleOption : IFeatureCheck
    {
        public string Title => "Delete Folder - Recycle Option";

        public string Description => "Option to not create the zipped file in Skyline DataMiner\\Recycle bin when calling NT_DELETE_FOLDER.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 24639 };

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
                if (!callingMethod.IsNotifyDataMiner(semanticModel, solution, 182))
                {
                    return;
                }

                if (callingMethod.Arguments.Count < 3)
                {
                    return;
                }

                if (!callingMethod.Arguments[2].TryParseToValue(semanticModel, solution, out Value value) || value.Type != Value.ValueType.Boolean)
                {
                    return;
                }

                items.Add(new CSharpFeatureCheckResultItem(qAction, callingMethod));
                Cancel();
            }
        }
    }
}
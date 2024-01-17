namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("10.0.0.0-9118", "9.6.6.0-8270")]
    internal class HistorySetsFillArrayWithColumnBulk : IFeatureCheck
    {
        public string Title => "History Sets - FillArrayWithColumn - Bulk";

        public string Description => "History Sets - FillArrayWithColumn - Bulk.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 21482 };

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
            private readonly List<CSharpFeatureCheckResultItem> items;
            private readonly IQActionsQAction qAction;
            private readonly SemanticModel semanticModel;
            private readonly Solution solution;

            public QActionAnalyzer(List<CSharpFeatureCheckResultItem> items, IQActionsQAction qAction, SemanticModel semanticModel, Solution solution)
            {
                this.items = items;
                this.qAction = qAction;
                this.semanticModel = semanticModel;
                this.solution = solution;
            }

            public override void CheckCallingMethod(CallingMethodClass callingMethod)
            {
                if (CheckNotifyProtocol(callingMethod))
                {
                    return;
                }

                if (CheckFillArrayWrapper(callingMethod))
                {
                    return;
                }

                CheckQActionTable(callingMethod);
            }

            private void CheckQActionTable(CallingMethodClass callingMethod)
            {
                if (!callingMethod.IsQActionTable(semanticModel) || !String.Equals(callingMethod.Name, "SetColumn"))
                {
                    return;
                }

                if (callingMethod.Arguments.Count < 4)
                {
                    return;
                }

                string fqn = callingMethod.Arguments[3].GetFullyQualifiedName(semanticModel);
                if (!String.Equals(fqn, "System.DateTime"))
                {
                    return;
                }

                items.Add(new CSharpFeatureCheckResultItem(qAction, callingMethod));
                Cancel();
            }

            private bool CheckFillArrayWrapper(CallingMethodClass callingMethod)
            {
                if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "FillArrayWithColumn"))
                {
                    return false;
                }

                // DateTime is always on position 4
                if (callingMethod.Arguments.Count < 5)
                {
                    return false;
                }

                string fqn = callingMethod.Arguments[4].GetFullyQualifiedName(semanticModel);
                if (!String.Equals(fqn, "System.DateTime"))
                {
                    return false;
                }

                items.Add(new CSharpFeatureCheckResultItem(qAction, callingMethod));
                Cancel();
                return true;
            }

            private bool CheckNotifyProtocol(CallingMethodClass callingMethod)
            {
                if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 220))
                {
                    return false;
                }

                if (callingMethod.Arguments.Count < 2)
                {
                    return false;
                }

                var argument = callingMethod.Arguments[1];
                if (!argument.TryParseToValue(semanticModel, solution, out Value columnInfo) || columnInfo.Type != Value.ValueType.Array)
                {
                    return false;
                }

                if (columnInfo.IsMethodArgument)
                {
                    // No array that could be parsed.
                    return false;
                }

                // Get last item as it the call can be for multiple columns
                var extra = columnInfo.Array.LastOrDefault();
                if (extra == null || extra.Type != Value.ValueType.Array || extra.Array == null || extra.Array.Count != 2)
                {
                    return false;
                }

                var dateTime = extra.Array[1];
                if (dateTime == null || dateTime.Type != Value.ValueType.Unknown || !String.Equals(dateTime.Object, "System.DateTime"))
                {
                    return false;
                }

                items.Add(new CSharpFeatureCheckResultItem(qAction, callingMethod));
                Cancel();
                return true;
            }
        }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System;
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

    //[MinDataMinerVersions("10.0.0.0-9118", "9.6.6.0-8270")]
    internal class HistorySetsFillArrayBulk : IFeatureCheck
    {
        public string Title => "History Sets - FillArray - Bulk";

        public string Description => "History Sets - FillArray - Bulk.";

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
                if (!callingMethod.IsQActionTable(semanticModel) || !String.Equals(callingMethod.Name, "FillArray"))
                {
                    return;
                }

                if (callingMethod.Arguments.Count < 2)
                {
                    return;
                }

                string fqn = callingMethod.Arguments[1].GetFullyQualifiedName(semanticModel);
                if (!String.Equals(fqn, "System.DateTime"))
                {
                    return;
                }

                items.Add(new CSharpFeatureCheckResultItem(qAction, callingMethod));
                Cancel();
            }

            private bool CheckFillArrayWrapper(CallingMethodClass callingMethod)
            {
                if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "FillArray"))
                {
                    return false;
                }

                return CheckWhenRows() || CheckWhenColumns();

                bool CheckWhenRows()
                {
                    // DateTime is always on position 3
                    if (callingMethod.Arguments.Count < 4)
                    {
                        return false;
                    }

                    string fqn = callingMethod.Arguments[3].GetFullyQualifiedName(semanticModel);
                    if (!String.Equals(fqn, "System.DateTime"))
                    {
                        return false;
                    }

                    items.Add(new CSharpFeatureCheckResultItem(qAction, callingMethod));
                    Cancel();
                    return true;
                }

                bool CheckWhenColumns()
                {
                    // DateTime is always on position 2
                    if (callingMethod.Arguments.Count < 3)
                    {
                        return false;
                    }

                    string fqn = callingMethod.Arguments[2].GetFullyQualifiedName(semanticModel);
                    if (!String.Equals(fqn, "System.DateTime"))
                    {
                        return false;
                    }

                    items.Add(new CSharpFeatureCheckResultItem(qAction, callingMethod));
                    Cancel();
                    return true;
                }
            }

            private bool CheckNotifyProtocol(CallingMethodClass callingMethod)
            {
                if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 193))
                {
                    return false;
                }

                if (callingMethod.Arguments.Count < 2)
                {
                    return false;
                }

                var argument = callingMethod.Arguments[1];
                if (!argument.TryParseToValue(semanticModel, solution, out Value value) || value.Type != Value.ValueType.Array)
                {
                    return false;
                }

                if (value.IsMethodArgument)
                {
                    // No array that could be parsed.
                    return false;
                }

                if (value.Array.Count != 3)
                {
                    return false;
                }

                if (value.Array[2] == null)
                {
                    // Unable to parse the value.
                    return false;
                }

                if (value.Array[2].Type != Value.ValueType.Unknown || !String.Equals(value.Array[2].Object, "System.DateTime"))
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
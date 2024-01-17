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

    // [MinDataMinerVersions("10.0.0.0-9118", "9.6.13.0-8820")]
    internal class HistorySetsFillArraySingle/* : IFeatureCheck*/
    {
        public string Title => "History Sets - FillArray - Single";

        public string Description => "History Sets - FillArray - Single.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 23815 };

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

                // Not sure if this one is properly supported (would need to go inside the wrapper method to check)
                //// CheckFillArrayWrapper(callingMethod);
            }

            ////private void CheckFillArrayWrapper(CallingMethodClass callingMethod)
            ////{
            ////    if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "FillArray"))
            ////    {
            ////        return;
            ////    }

            ////    if (callingMethod.Arguments.Count < 2)
            ////    {
            ////        return;
            ////    }

            ////    var tableContent = callingMethod.Arguments[1];
            ////    if (!tableContent.TryParseToValue(semanticModel, solution, out Value content) ||
            ////        content.Type != Value.ValueType.Array)
            ////    {
            ////        return;
            ////    }

            ////    if (content.IsMethodArgument)
            ////    {
            ////        // No array that could be parsed.
            ////        return;
            ////    }

            ////    if (content.Array.Count < 2)
            ////    {
            ////        return;
            ////    }

            ////    var column2 = content.Array[1];
            ////    if (column2 == null || column2.Type != Value.ValueType.Array || column2.IsMethodArgument)
            ////    {
            ////        return;
            ////    }

            ////    foreach (Value value in column2.Array)
            ////    {
            ////        if (value.Type != Value.ValueType.Array || value.Array.Count < 2)
            ////        {
            ////            continue;
            ////        }

            ////        if (String.Equals(value.Array[1].Object, "System.DateTime"))
            ////        {
            ////            items.Add(new CSharpFeatureCheckResultItem(qAction, callingMethod));
            ////            Cancel();
            ////            return;
            ////        }
            ////    }
            ////}

            private bool CheckNotifyProtocol(CallingMethodClass callingMethod)
            {
                if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 193))
                {
                    return false;
                }

                if (callingMethod.Arguments.Count < 3)
                {
                    return false;
                }

                var tableContent = callingMethod.Arguments[2];
                if (!tableContent.TryParseToValue(semanticModel, solution, out Value content) ||
                    content.Type != Value.ValueType.Array || content.Array.Count < 2)
                {
                    return false;
                }

                var column2 = content.Array[1];
                if (column2 == null || column2.Type != Value.ValueType.Array)
                {
                    return false;
                }

                foreach (Value value in column2.Array)
                {
                    if (value.Type != Value.ValueType.Array || value.Array.Count < 2)
                    {
                        continue;
                    }

                    if (String.Equals(value.Array[1]?.Object, "System.DateTime"))
                    {
                        items.Add(new CSharpFeatureCheckResultItem(qAction, callingMethod));
                        Cancel();
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
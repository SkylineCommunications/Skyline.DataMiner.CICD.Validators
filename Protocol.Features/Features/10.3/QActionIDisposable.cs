namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System;
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    [MinDataMinerVersions("10.3.0.0-12752", "10.2.9.0-12190")]
    internal class QActionIDisposable : IFeatureCheck
    {
        public string Title => "IDisposable interface";

        public string Description => "DataMiner will call the Dispose method of the IDisposable interface when when disposing of the QAction.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 33965 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = new List<IReadable>();

            foreach ((CompiledQActionProject projectData, IQActionsQAction qaction) in context.EachQActionProject())
            {
                // If no triggers are defined then there is no entry point.
                if (qaction.Triggers == null)
                {
                    continue;
                }

                var entryPoints = qaction.GetEntryPoints().EntryPoints;

                QActionAnalyzer analyzer = new QActionAnalyzer(qaction, items, entryPoints);
                RoslynVisitor parser = new RoslynVisitor(analyzer);

                foreach ((SyntaxTree syntaxTree, _) in projectData.EachQActionSyntaxTreesAndModels())
                {
                    parser.Visit(syntaxTree.GetRoot());
                }
            }

            return new FeatureCheckResult(items);
        }
    }

    internal sealed class QActionAnalyzer : CSharpAnalyzerBase
    {
        private readonly IList<IReadable> results;

        private readonly IQActionsQAction qAction;
        private readonly IReadOnlyList<QActionEntryPoints.EntryPoint> entryPoints;

        public QActionAnalyzer(IQActionsQAction qAction, IList<IReadable> results, IReadOnlyList<QActionEntryPoints.EntryPoint> entryPoints)
        {
            this.qAction = qAction;
            this.results = results;
            this.entryPoints = entryPoints;
        }

        public override void CheckClass(ClassClass classClass)
        {
            foreach (var entryPoint in entryPoints)
            {
                string entryPointClassName = entryPoint.GetClassNameOrDefault();
                if (!String.Equals(classClass.Name, entryPointClassName))
                {
                    continue;
                }

                foreach (var inheritanceItem in classClass.InheritanceItems)
                {
                    if (String.Equals("IDisposable", inheritanceItem) && !results.Contains(qAction))
                    {
                        results.Add(qAction);
                    }
                }
            }
        }
    }
}

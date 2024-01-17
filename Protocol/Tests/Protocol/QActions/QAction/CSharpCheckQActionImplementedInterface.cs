namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpCheckQActionImplementedInterface
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    //[Test(CheckId.CSharpCheckQActionImplementedInterface, Category.QAction)]
    internal class CSharpCheckQActionImplementedInterface : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();
            /*
            foreach ((CompiledQActionProject projectData, IQActionsQAction qaction) in context.EachQActionProject())
            {
                // If no triggers are defined then there is no entry point.
                if (qaction.Triggers == null)
                {
                    continue;
                }

                var entryPoints = qaction.GetEntryPoints().EntryPoints;

                QActionAnalyzer analyzer = new QActionAnalyzer(this, qaction, results, entryPoints);
                RoslynVisitor parser = new RoslynVisitor(analyzer);

                foreach ((SyntaxTree syntaxTree, _) in projectData.EachQActionSyntaxTreesAndModels())
                {
                    parser.Visit(syntaxTree.GetRoot());
                }
            }
            */
            return results;
        }
    }
    /*
    internal class QActionAnalyzer : CSharpAnalyzerBase
    {
        private readonly List<IValidationResult> results;
        private readonly IValidate test;
        private readonly IQActionsQAction qAction;
        private readonly IReadOnlyList<QActionEntryPoints.EntryPoint> entryPoints;

        public QActionAnalyzer(IValidate test, IQActionsQAction qAction, List<IValidationResult> results, IReadOnlyList<QActionEntryPoints.EntryPoint> entryPoints)
        {
            this.test = test;
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
                    // Check for presence of interface name.
                    //if (String.Equals("IInterfaceName", inheritanceItem))
                    //{
                    //    results.Add(Error.UnsupportedIDisposable(test, qAction, qAction, entryPointClassName, qAction.Id.RawValue).WithCSharp(classClass));
                    //}

                }
            }
        }
    }*/
}
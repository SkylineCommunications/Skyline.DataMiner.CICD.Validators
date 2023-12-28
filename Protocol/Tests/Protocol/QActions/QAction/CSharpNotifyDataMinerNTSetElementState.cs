namespace SLDisValidator2.Tests.Protocol.QActions.QAction.CSharpNotifyDataMinerNTSetElementState
{
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CSharpNotifyDataMinerNTSetElementState, Category.QAction)]
    public class CSharpNotifyDataMinerNTSetElementState : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IQActionsQAction qaction, SyntaxTree syntaxTree, SemanticModel semanticModel, CompiledQActionProject projectData) in context.EachQActionProjectsAndSyntaxTreesAndModelsAndProjectDatas())
            {
                Solution solution = projectData.Project.Solution;
                QActionAnalyzer analyzer = new QActionAnalyzer(this, qaction, results, context.ProtocolModel, semanticModel, solution);
                RoslynVisitor parser = new RoslynVisitor(analyzer);

                parser.Visit(syntaxTree.GetRoot());
            }

            return results;
        }
    }

    internal class QActionAnalyzer : CSharpAnalyzerBase
    {
        private readonly List<IValidationResult> results;
        private readonly IValidate test;
        private readonly IQActionsQAction qAction;
        private readonly IProtocolModel protocolModel;
        private readonly SemanticModel semanticModel;
        private readonly Solution solution;

        public QActionAnalyzer(IValidate test, IQActionsQAction qAction, List<IValidationResult> results, IProtocolModel protocolModel, SemanticModel semanticModel, Solution solution)
        {
            this.test = test;
            this.qAction = qAction;
            this.results = results;
            this.protocolModel = protocolModel;
            this.semanticModel = semanticModel;
            this.solution = solution;
        }

        public override void CheckCallingMethod(CallingMethodClass callingMethod)
        {
            // protocol.NotifyDataMiner(115, [Array], null)

            if (!callingMethod.IsNotifyDataMiner(semanticModel, solution, 115))
            {
                return;
            }

            if (!callingMethod.Arguments[1].TryParseToValue(semanticModel, solution, out Value value))
            {
                // Couldn't parse the argument
                return;
            }

            if (!value.HasStaticValue)
            {
                return;
            }

            if (value.Type != Value.ValueType.Array)
            {
                // Invalid input argument
                results.Add(Error.DeltIncompatible(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(value));
                return;
            }

            if (value.Array.Count < 4)
            {
                // Invalid amount of arguments
                results.Add(Error.DeltIncompatible(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(value));
                return;
            }

            if (!value.Array[3]?.IsNumeric() == true)
            {
                // Not numeric => Can't be an id.
                results.Add(Error.DeltIncompatible(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(value));
            }
        }
    }
}
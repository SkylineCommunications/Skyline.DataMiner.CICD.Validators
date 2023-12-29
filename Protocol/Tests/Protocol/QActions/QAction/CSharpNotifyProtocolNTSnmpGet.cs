namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpNotifyProtocolNTSnmpGet
{
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CSharpNotifyProtocolNTSnmpGet, Category.QAction)]
    internal class CSharpNotifyProtocolNTSnmpGet : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IQActionsQAction qaction, SyntaxTree syntaxTree, SemanticModel semanticModel, CompiledQActionProject projectData) in context.EachQActionProjectsAndSyntaxTreesAndModelsAndProjectDatas())
            {
                Solution solution = projectData.Project.Solution;
                QActionAnalyzer analyzer = new QActionAnalyzer(this, qaction, results, semanticModel, solution);
                RoslynVisitor parser = new RoslynVisitor(analyzer);

                parser.Visit(syntaxTree.GetRoot());
            }

            return results;
        }
    }

    internal class QActionAnalyzer : QActionAnalyzerBase
    {
        public QActionAnalyzer(IValidate test, IQActionsQAction qAction, List<IValidationResult> results, SemanticModel semanticModel, Solution solution)
            : base(test, results, qAction, semanticModel, solution)
        {
        }

        public override void CheckCallingMethod(CallingMethodClass callingMethod)
        {
            // protocol.NotifyProtocol(295, [Array], null)

            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 295))
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

            if (value.Array.Count < 8)
            {
                // Invalid amount of arguments
                results.Add(Error.DeltIncompatible(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(value));
                return;
            }

            if (!value.Array[7]?.IsNumeric() == true)
            {
                // Not numeric => Can't be an id.
                results.Add(Error.DeltIncompatible(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(value));
            }
        }
    }
}
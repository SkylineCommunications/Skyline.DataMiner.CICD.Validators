namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolCheckTrigger
{
    using System;
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CSharpSLProtocolCheckTrigger, Category.QAction)]
    internal class CSharpSLProtocolCheckTrigger : IValidate
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

    internal class QActionAnalyzer : QActionAnalyzerBase
    {
        private readonly IProtocolModel protocolModel;

        public QActionAnalyzer(IValidate test, IQActionsQAction qAction, List<IValidationResult> results, IProtocolModel protocolModel, SemanticModel semanticModel, Solution solution)
            : base(test, results, qAction, semanticModel, solution)
        {
            this.protocolModel = protocolModel;
        }

        public override void CheckCallingMethod(CallingMethodClass callingMethod)
        {
            // protocol.CheckTrigger([ID])

            if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "CheckTrigger"))
            {
                return;
            }

            if (callingMethod.Arguments.Count != 1)
            {
                // Invalid amount of arguments
                return;
            }

            if (!callingMethod.Arguments[0].TryParseToValue(semanticModel, solution, out Value value) || !value.IsNumeric())
            {
                // Couldn't parse argument OR argument isn't numeric.
                return;
            }

            if (!value.HasStaticValue)
            {
                return;
            }

            string id = Convert.ToString(value.Object);
            if (!protocolModel.TryGetObjectByKey<ITriggersTrigger>(Mappings.TriggersById, id, out _))
            {
                results.Add(Error.NonExistingTrigger(test, qAction, qAction, id, qAction.Id.RawValue).WithCSharp(value));
            }
        }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolTriggerAction
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

    [Test(CheckId.CSharpSLProtocolTriggerAction, Category.QAction)]
    internal class CSharpSLProtocolTriggerAction : IValidate/*, ICodeFix, ICompare*/
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
            // protocol.NotifyProtocol(221, [ID], null)

            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 221))
            {
                return;
            }

            if (!callingMethod.Arguments[1].TryParseToValue(semanticModel, solution, out Value value) || !value.IsNumeric())
            {
                // Argument couldn't be parsed OR argument isn't a numeric value.
                return;
            }

            if (!value.HasStaticValue)
            {
                // Value could have changed => Uncertain
                return;
            }

            string id = Convert.ToString(value.Object);
            if (!protocolModel.TryGetObjectByKey<IActionsAction>(Mappings.ActionsById, id, out _))
            {
                results.Add(Error.NonExistingActionId(test, qAction, qAction, id, qAction.Id.RawValue).WithCSharp(value));
            }
        }
    }
}
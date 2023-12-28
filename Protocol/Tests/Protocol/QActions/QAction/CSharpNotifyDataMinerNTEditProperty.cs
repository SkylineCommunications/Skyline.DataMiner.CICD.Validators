namespace SLDisValidator2.Tests.Protocol.QActions.QAction.CSharpNotifyDataMinerNTEditProperty
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

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Interfaces;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    [Test(CheckId.CSharpNotifyDataMinerNTEditProperty, Category.QAction)]
    internal class CSharpNotifyDataMinerNTEditProperty : IValidate
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
            // protocol.NotifyDataMiner(62, [Array], null)

            if (!callingMethod.IsNotifyDataMiner(semanticModel, solution, 62))
            {
                return;
            }

            if (!callingMethod.Arguments[1].TryParseToValue(semanticModel, solution, out Value value))
            {
                // Couldn't parse the argument OR argument isn't a string.
                return;
            }

            if (!value.HasStaticValue)
            {
                return;
            }

            if (value.Type == Value.ValueType.String)
            {
                CheckSingleEdit(value);
            }
            else if (value.Type == Value.ValueType.Array)
            {
                foreach (var edit in value.Array)
                {
                    CheckSingleEdit(edit);
                }
            }
            else
            {
                results.Add(Error.DeltIncompatible(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(callingMethod));
            }
        }

        private void CheckSingleEdit(Value value)
        {
            string[] parts = Convert.ToString(value?.Object).Split(':');

            switch (parts[0].ToUpperInvariant())
            {
                case "ELEMENT":
                    if (parts.Length != 3)
                    {
                        results.Add(Error.DeltIncompatible(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(value));
                    }

                    return;

                case "SERVICE":
                    if (parts.Length != 3)
                    {
                        results.Add(Error.DeltIncompatible(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(value));
                    }

                    return;

                case "ALARM":
                    if (parts.Length != 4)
                    {
                        results.Add(Error.DeltIncompatible(test, qAction, qAction, qAction.Id.RawValue).WithCSharp(value));
                    }

                    return;

                case "VIEW": // No agent id needed here
                default: // Unknown
                    return;
            }
        }
    }
}

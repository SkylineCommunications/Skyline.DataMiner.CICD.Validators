namespace SLDisValidator2.Tests.Protocol.QActions.QAction.CSharpSLProtocolGetParameter
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

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CSharpSLProtocolGetParameter, Category.QAction)]
    internal class CSharpSLProtocolGetParameter : IValidate
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
            if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "GetParameter"))
            {
                return;
            }

            if (!callingMethod.Arguments[0].TryParseToValue(semanticModel, solution, out Value value) || !value.IsNumeric())
            {
                // Argument couldn't be parsed OR argument isn't a numeric value.
                return;
            }

            if (!value.HasStaticValue)
            {
                return;
            }

            int pid = value.AsInt32;

            // Internal parameters don't have to be explicitly in driver and are allowed to be hard-coded
            if (ParamHelper.IsInternalPid(pid))
            {
                return;
            }

            // Check if the parameter exists.
            if (!protocolModel.TryGetObjectByKey(Mappings.ParamsById, pid.ToString(), out IParamsParam param))
            {
                results.Add(Error.NonExistingParam(test, qAction, qAction, pid.ToString(), qAction.Id.RawValue).WithCSharp(value));
            }

            if (!value.IsParameterClass(semanticModel, solution))
            {
                if (param != null && !param.IsRead() && !param.IsWrite())
                {
                    // Example: polling ip
                    return;
                }

                results.Add(Error.HardCodedPid(test, qAction, qAction, pid.ToString(), qAction.Id.RawValue).WithCSharp(value));
            }
        }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolSetParameter
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

    [Test(CheckId.CSharpSLProtocolSetParameter, Category.QAction)]
    internal class CSharpSLProtocolSetParameter : IValidate, ICodeFix/*, ICompare*/
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

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();
            switch (context.Result.ErrorId)
            {
                case ErrorIds.ParamMissingHistorySet:
                    if (!(context.Result.ReferenceNode is IParamsParam param))
                    {
                        result.Message = "Invalid Node";
                        break;
                    }

                    var editParam = context.Protocol.Params.Get(param);
                    editParam.HistorySet = true;
                    result.Success = true;
                    break;

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
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
            if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "SetParameter"))
            {
                return;
            }

            if (!callingMethod.Arguments[0].TryParseToValue(semanticModel, solution, out Value value) || !value.IsNumeric())
            {
                // Argument couldn't be parsed OR argument isn't numeric.
                return;
            }

            if (!value.HasStaticValue)
            {
                // Uncertain about the value
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
                results.Add(Error.HardCodedPid(test, qAction, qAction, pid.ToString(), qAction.Id.RawValue).WithCSharp(value));
            }

            if (param == null)
            {
                return;
            }

            if (false)
            {
                // TODO: Check if it's only a standalone param (no table, column, matrix, title, dummy, pagebutton, ...)

                // TODO: Errormessage that it isn't a standalone param
            }

            // DateTime is always on position 2
            if (callingMethod.Arguments.Count < 3)
            {
                return;
            }

            string fqn = callingMethod.Arguments[2].GetFullyQualifiedName(semanticModel);

            if (!String.Equals(fqn, "System.DateTime"))
            {
                return;
            }

            if (param.HistorySet?.Value != true)
            {
                // Reference node needs to be param as that's where the Fix needs to happen.
                results.Add(Error.ParamMissingHistorySet(test, param, qAction, param.Id.RawValue).WithCSharp(callingMethod));
            }
        }
    }
}
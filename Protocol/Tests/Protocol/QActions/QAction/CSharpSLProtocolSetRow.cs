namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolSetRow
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

    [Test(CheckId.CSharpSLProtocolSetRow, Category.QAction)]
    internal class CSharpSLProtocolSetRow : IValidate, ICodeFix
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
            if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "SetRow"))
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
                // Unsure about the value
                return;
            }

            int tablePid = value.AsInt32;

            // Internal parameters don't have to be explicitly in driver and are allowed to be hard-coded
            if (ParamHelper.IsInternalPid(tablePid))
            {
                return;
            }

            // Check if the parameter exists.
            if (!protocolModel.TryGetObjectByKey(Mappings.ParamsById, tablePid.ToString(), out IParamsParam tableParam))
            {
                results.Add(Error.NonExistingParam(test, qAction, qAction, tablePid.ToString(), qAction.Id.RawValue).WithCSharp(value));
            }

            if (!value.IsParameterClass(semanticModel, solution))
            {
                results.Add(Error.HardCodedPid(test, qAction, qAction, tablePid.ToString(), qAction.Id.RawValue).WithCSharp(value));
            }

            if (tableParam == null)
            {
                // Invalid parameter
                return;
            }

            if (!tableParam.IsTable())
            {
                // TODO: Error message that it isn't a table
                // Maybe combine it with the nonexistingparam?
                return;
            }

            // DateTime is always on position 3
            if (callingMethod.Arguments.Count < 4)
            {
                return;
            }

            string fqn = callingMethod.Arguments[3].GetFullyQualifiedName(semanticModel);

            if (!String.Equals(fqn, "System.DateTime"))
            {
                return;
            }

            uint? pkColumn = tableParam.ArrayOptions?.Index?.Value;
            foreach ((uint? idx, string pid, IParamsParam columnParam) in tableParam.GetColumns(protocolModel.RelationManager, true))
            {
                if (pkColumn == idx || idx == null || String.IsNullOrWhiteSpace(pid))
                {
                    continue;
                }

                if (columnParam.HistorySet?.Value == true)
                {
                    continue;
                }

                // Reference node needs to be param as that's where the Fix needs to happen.
                results.Add(Error.ParamMissingHistorySet(test, columnParam, qAction, columnParam.Id.RawValue).WithCSharp(value));
            }
        }
    }
}
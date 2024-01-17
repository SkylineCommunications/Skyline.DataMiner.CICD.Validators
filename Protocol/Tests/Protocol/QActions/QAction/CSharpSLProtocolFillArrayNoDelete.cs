namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolFillArrayNoDelete
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

    [Test(CheckId.CSharpSLProtocolFillArrayNoDelete, Category.QAction)]
    internal class CSharpSLProtocolFillArrayNoDelete : IValidate, ICodeFix
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
            if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "FillArrayNoDelete"))
            {
                return;
            }

            if (!callingMethod.Arguments[0].TryParseToValue(semanticModel, solution, out Value value) || !value.IsNumeric())
            {
                // Was unable to parse the argument OR value wasn't numeric
                return;
            }

            if (!value.HasStaticValue)
            {
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
                // TODO: Errormessage that it isn't a table
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

            uint? primaryKeyIdx = tableParam.ArrayOptions?.Index?.Value;
            foreach ((uint? columnIdx, string columnPid, IParamsParam columnParam) in tableParam.GetColumns(protocolModel.RelationManager, true))
            {
                if (primaryKeyIdx == columnIdx || columnIdx == null || String.IsNullOrWhiteSpace(columnPid))
                {
                    continue;
                }

                if (columnParam.HistorySet?.Value == true)
                {
                    continue;
                }

                // Reference node needs to be param as that's where the Fix needs to happen.
                results.Add(Error.ParamMissingHistorySet(test, columnParam, qAction, columnParam.Id.RawValue).WithCSharp(callingMethod));
            }
        }
    }
}
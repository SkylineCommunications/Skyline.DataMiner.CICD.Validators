namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolFillArrayWithColumn
{
    using System;
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Classes;
    using Skyline.DataMiner.CICD.CSharpAnalysis.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CSharpSLProtocolFillArrayWithColumn, Category.QAction)]
    internal class CSharpSLProtocolFillArrayWithColumn : IValidate, ICodeFix
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
            if (!callingMethod.IsSLProtocol(semanticModel) || !String.Equals(callingMethod.Name, "FillArrayWithColumn"))
            {
                return;
            }

            if (callingMethod.Arguments.Count < 2)
            {
                return;
            }

            bool wasAbleToRetrieveTable = CheckTableArgument(callingMethod.Arguments[0], out IParamsParam table);
            bool wasAbleToRetrieveColumn = CheckColumnArgument(callingMethod.Arguments[1], out IParamsParam column);

            if (wasAbleToRetrieveTable && !QActionHelper.TableCanBeSet(table))
            {
                // TODO: Error that the table can't be set
                return;
            }

            if (!wasAbleToRetrieveColumn || !CheckIfColumnCanBeSet(column, table, callingMethod))
            {
                // Column couldn't be retrieved OR Column should not be set, so no point in checking the column further.
                return;
            }

            if (wasAbleToRetrieveTable)
            {
                CheckHistorySetOnColumn(callingMethod, table, column);
            }
        }

        private bool CheckTableArgument(Argument argument, out IParamsParam table)
        {
            table = null;

            if (!argument.TryParseToValue(semanticModel, solution, out Value value) || !value.IsNumeric())
            {
                // Argument couldn't be parsed OR argument isn't a numeric value.
                return false;
            }

            if (!value.HasStaticValue)
            {
                return false;
            }

            int tablePid = value.AsInt32;

            // Internal parameters don't have to be explicitly in driver and are allowed to be hard-coded
            if (ParamHelper.IsInternalPid(tablePid))
            {
                return false;
            }

            // Check if the parameter exists.
            if (!QActionHelper.TryToGetParamFromValueLite(value, protocolModel, out table))
            {
                results.Add(Error.NonExistingTable(test, qAction, qAction, tablePid.ToString(), qAction.Id.RawValue).WithCSharp(value));
            }

            if (!value.IsParameterClass(semanticModel, solution))
            {
                results.Add(Error.HardCodedTablePid(test, qAction, qAction, tablePid.ToString(), qAction.Id.RawValue).WithCSharp(value));
            }

            return table != null;
        }

        private bool CheckColumnArgument(Argument argument, out IParamsParam column)
        {
            column = null;

            if (!argument.TryParseToValue(semanticModel, solution, out Value value) || !value.IsNumeric())
            {
                // Argument couldn't be parsed OR argument isn't a numeric value.
                return false;
            }

            if (!value.HasStaticValue)
            {
                return false;
            }

            int columnPid = value.AsInt32;

            // Internal parameters don't have to be explicitly in driver and are allowed to be hard-coded
            if (ParamHelper.IsInternalPid(columnPid))
            {
                return false;
            }

            // Check if the parameter exists.
            if (!QActionHelper.TryToGetParamFromValueLite(value, protocolModel, out column))
            {
                results.Add(Error.NonExistingColumn(test, qAction, qAction, columnPid.ToString(), qAction.Id.RawValue).WithCSharp(value));
            }

            if (!value.IsParameterClass(semanticModel, solution))
            {
                results.Add(Error.HardCodedColumnPid(test, qAction, qAction, columnPid.ToString(), qAction.Id.RawValue).WithCSharp(value));
            }

            return column != null;
        }

        private bool CheckIfColumnCanBeSet(IParamsParam column, IParamsParam table, CallingMethodClass callingMethod)
        {
            int originalResultCount = results.Count;

            // ColumnOption@type
            if (!QActionHelper.ColumnCanBeSetBasedOnType(column, protocolModel.RelationManager, out EnumColumnOptionType? unsettableType, table))
            {
                string type = unsettableType == null ? String.Empty : EnumColumnOptionTypeConverter.ConvertBack(unsettableType.Value);
                results.Add(Error.ColumnManagedByDataMiner(test, qAction, qAction, column.Id.RawValue, "ColumnOption@type", type).WithCSharp(callingMethod));
            }

            if (ParamHelper.IsColumnOfTypeSnmp(column, protocolModel.RelationManager, table))
            {
                results.Add(Error.UnrecommendedSetOnSnmpParam(test, qAction, qAction, column.Id.RawValue).WithCSharp(callingMethod));
            }

            // ColumnOption@option
            if (!QActionHelper.ColumnCanBeSetBasedOnOptions(column, protocolModel.RelationManager, out IReadOnlyList<string> failingColumnOptions, table))
            {
                foreach (string failingOption in failingColumnOptions)
                {
                    results.Add(Error.ColumnManagedByDataMiner(test, qAction, qAction, column.Id.RawValue, "ColumnOption@option", failingOption).WithCSharp(callingMethod));
                }
            }

            // Other protocol features
            if (!QActionHelper.ColumnCanBeSetBasedOnTimerOptions(column, protocolModel, out IReadOnlyDictionary<string, IReadOnlyList<string>> failingTimerOptionsByTimerId, table))
            {
                foreach (var failingTimerOptions in failingTimerOptionsByTimerId)
                {
                    string timerId = failingTimerOptions.Key;
                    foreach (string failingOption in failingTimerOptions.Value)
                    {
                        results.Add(Error.ColumnManagedByProtocolItem(test, qAction, qAction, column.Id.RawValue, "Timer", timerId, "Timer@options", failingOption).WithCSharp(callingMethod));
                    }
                }
            }

            return originalResultCount == results.Count;
        }

        private void CheckHistorySetOnColumn(CallingMethodClass callingMethod, IParamsParam table, IParamsParam column)
        {
            // DateTime is always on position 4
            if (callingMethod.Arguments.Count < 5)
            {
                return;
            }

            string fqn = callingMethod.Arguments[4].GetFullyQualifiedName(semanticModel);

            if (!String.Equals(fqn, "System.DateTime"))
            {
                return;
            }

            if (column.HistorySet?.Value == true)
            {
                return;
            }

            if (table.TryGetPrimaryKeyColumn(protocolModel.RelationManager, out IParamsParam indexColumn) && indexColumn == column)
            {
                // Ignore index column
                return;
            }

            // Reference node needs to be param as that's where the Fix needs to happen.
            results.Add(Error.ParamMissingHistorySet(test, column, qAction, column.Id.RawValue).WithCSharp(callingMethod));
        }
    }
}
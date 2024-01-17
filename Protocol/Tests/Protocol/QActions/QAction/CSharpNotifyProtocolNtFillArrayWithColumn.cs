namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpNotifyProtocolNtFillArrayWithColumn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

    [Test(CheckId.CSharpNotifyProtocolNtFillArrayWithColumn, Category.QAction)]
    internal class CSharpNotifyProtocolNtFillArrayWithColumn : IValidate, ICodeFix/*, ICompare*/
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
                case ErrorIds.ColumnMissingHistorySet:
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

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }

    internal class Column
    {
        /// <summary>
        /// Value (column id) from the 2nd argument
        /// </summary>
        public Value ColumnId { get; set; }

        /// <summary>
        /// Value from the 3rd argument
        /// </summary>
        public Value MatchingValue { get; set; }

        public IParamsParam MatchingParam { get; set; }

        public IParamsParam MatchingTable { get; set; }

        public bool NeedsHistorySet { get; set; }
    }

    internal class Table
    {
        /// <summary>
        /// Value (table id) from the 2nd argument
        /// </summary>
        public Value TableId { get; set; }

        /// <summary>
        /// Value from the 3rd argument which should be the keys.
        /// </summary>
        public Value MatchingValue { get; set; }

        public IParamsParam MatchingParam { get; set; }
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
            //// protocol.NotifyProtocol(220 /*NT_FILL_ARRAY_WITH_COLUMN*/, columnInfo, values);

            if (!callingMethod.IsNotifyProtocol(semanticModel, solution, 220))
            {
                return;
            }

            if (callingMethod.Arguments.Count != 3)
            {
                // Invalid amount of arguments (NT_FILL_ARRAY_WITH_COLUMN always has 3 arguments)
                return;
            }

            if (!TryRetrieveInformation(callingMethod, out Table table, out IReadOnlyDictionary<int, Column> columnsByPosition, out bool? allowClearAndLeave, out bool hasGlobalHistorySet))
            {
                // Invalid syntax of the arguments
                // TODO: Add InvalidSyntax error
                return;
            }

            List<IValidationResult> subResults = new List<IValidationResult>();
            bool wasAbleToRetrieveTable = CheckTable(table, subResults, callingMethod);

            Dictionary<int, bool> wasAbleToRetrieveColumns = new Dictionary<int, bool>();
            foreach (KeyValuePair<int, Column> column in columnsByPosition)
            {
                bool wasAbleToRetrieveColumn = CheckColumn(column.Value, subResults, callingMethod);
                wasAbleToRetrieveColumns[column.Key] = wasAbleToRetrieveColumn;
            }

            if (wasAbleToRetrieveTable && !QActionHelper.TableCanBeSet(table.MatchingParam))
            {
                // TODO: Error that the table can't be set
                UpdateResults();
                return;
            }

            List<Column> columnsToCheckHistorySet = new List<Column>();
            foreach (KeyValuePair<int, Column> column in columnsByPosition)
            {
                if (!wasAbleToRetrieveColumns[column.Key] || !CheckIfColumnCanBeSet(column.Value.MatchingParam, table.MatchingParam, callingMethod))
                {
                    // Column couldn't be retrieved OR Column should not be set, so no point in checking the column further.
                    continue;
                }

                columnsToCheckHistorySet.Add(column.Value);
            }

            if (wasAbleToRetrieveTable)
            {
                foreach (Column column in columnsToCheckHistorySet)
                {
                    CheckHistorySetOnColumn(table.MatchingParam, column, hasGlobalHistorySet, subResults);
                }
            }

            UpdateResults();

            void UpdateResults()
            {
                if (subResults.Count > 1 || subResults.Any(r => r.ErrorId == ErrorIds.ColumnMissingHistorySet))
                {
                    string arguments = $"({callingMethod.Arguments[0].RawValue}, {callingMethod.Arguments[1].RawValue}, {callingMethod.Arguments[2].RawValue})";

                    ICSharpValidationResult cSharpValidationResult = Error.UnexpectedImplementation(test, qAction, qAction, arguments, qAction.Id.RawValue)
                                                                          .WithSubResults(subResults.ToArray())
                                                                          .WithCSharp(callingMethod);
                    results.Add(cSharpValidationResult);
                }
                else
                {
                    results.AddRange(subResults);
                }
            }
        }

        private bool TryRetrieveInformation(CallingMethodClass callingMethod, out Table table, out IReadOnlyDictionary<int, Column> columsByPosition, out bool? allowClearOrLeave, out bool hasGlobalHistorySet)
        {
            if (!TryRetrieveColumnInfoInformation(callingMethod, out table, out columsByPosition, out allowClearOrLeave, out hasGlobalHistorySet))
            {
                // Invalid syntax. Couldn't parse the columnInfo.
                return false;
            }

            if (!TryRetrieveValuesInformation(callingMethod, table, columsByPosition))
            {
                // Invalid syntax. Couldn't parse the values.
                return false;
            }

            return true;
        }

        private bool TryRetrieveColumnInfoInformation(
            CallingMethodClass callingMethod,
            out Table table,
            out IReadOnlyDictionary<int, Column> columnsByPosition,
            out bool? allowClearOrLeave,
            out bool hasGlobalHistorySet)
        {
            table = null;
            columnsByPosition = null;
            allowClearOrLeave = null;
            hasGlobalHistorySet = false;

            if (!callingMethod.Arguments[1].TryParseToValue(semanticModel, solution, out Value columnInfo) || columnInfo.Type != Value.ValueType.Array ||
                columnInfo.IsMethodArgument || columnInfo.Array == null || columnInfo.Array.Count == 0)
            {
                return false;
            }

            #region Table

            QActionHelper.TryToGetParamFromValue(columnInfo.Array[0], protocolModel, out IParamsParam tableParam);
            table = new Table
            {
                TableId = columnInfo.Array[0],
                MatchingParam = tableParam
            };

            #endregion

            #region Optional stuff

            var lastItem = columnInfo.Array.Last();

            if (lastItem != null)
            {
                switch (lastItem.Type)
                {
                    case Value.ValueType.Array:
                        if (lastItem.HasStaticValue)
                        {
                            allowClearOrLeave = false;
                            if (lastItem.Array.Count > 0)
                            {
                                allowClearOrLeave = CheckAllowClearOrLeave(lastItem.Array[0]);

                                if (lastItem.Array.Count == 2 && IsDateTime(lastItem.Array[1]))
                                {
                                    hasGlobalHistorySet = true;
                                }
                            }
                        }

                        break;

                    case Value.ValueType.Boolean:
                        allowClearOrLeave = CheckAllowClearOrLeave(lastItem);
                        break;
                }

                bool? CheckAllowClearOrLeave(Value optional)
                {
                    if (optional.Type == Value.ValueType.Boolean && optional.HasStaticValue)
                    {
                        return (bool)optional.Object;
                    }

                    return null;
                }
            }

            #endregion

            #region Columns

            int numberOfColumns = columnInfo.Array.Count - 1; // Minus Table

            // Check if the optional items have been found.
            if (allowClearOrLeave != null)
            {
                numberOfColumns--; // Minus optional
            }

            Dictionary<int, Column> columns = new Dictionary<int, Column>();
            for (int i = 0; i < numberOfColumns; i++)
            {
                Value value = columnInfo.Array[i + 1];

                QActionHelper.TryToGetParamFromValue(value, protocolModel, out IParamsParam column);
                IParamsParam matchingTable = null;
                column?.TryGetTable(protocolModel.RelationManager, out matchingTable);
                columns.Add(i, new Column
                {
                    ColumnId = value,
                    MatchingParam = column,
                    MatchingTable = matchingTable
                });
            }

            columnsByPosition = columns;

            #endregion

            return true;
        }

        private bool TryRetrieveValuesInformation(CallingMethodClass callingMethod, Table table, IReadOnlyDictionary<int, Column> columnsByPosition)
        {
            int totalNumberOfValuesNeeded = columnsByPosition.Count + 1;
            if (!callingMethod.Arguments[2].TryParseToValue(semanticModel, solution, out Value values) || values.Type != Value.ValueType.Array ||
                values.IsMethodArgument || values.Array == null || values.Array.Count != totalNumberOfValuesNeeded)
            {
                return false;
            }

            table.MatchingValue = values.Array[0];
            for (int i = 0; i < values.Array.Count - 1; i++)
            {
                var value = values.Array[i + 1];
                columnsByPosition[i].MatchingValue = value;

                if (value?.Array?.Any(x => x?.Array != null && x.Array.Count == 2 && IsDateTime(x.Array[1])) == true)
                {
                    columnsByPosition[i].NeedsHistorySet = true;
                }
            }

            return true;
        }

        private bool CheckIfColumnCanBeSet(IParamsParam column, IParamsParam table, CallingMethodClass callingMethod)
        {
            int originalResultCount = results.Count;

            // ColumnOption@type
            if (!QActionHelper.ColumnCanBeSetBasedOnType(column, protocolModel.RelationManager, out EnumColumnOptionType? type, table))
            {
                string stringType = type == null ? String.Empty : EnumColumnOptionTypeConverter.ConvertBack(type.Value);
                results.Add(Error.ColumnManagedByDataMiner(test, qAction, qAction, column.Id.RawValue, "ColumnOption@type", stringType).WithCSharp(callingMethod));
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

        private void CheckHistorySetOnColumn(IParamsParam table, Column column, bool globalHistorySet, List<IValidationResult> subResults)
        {
            if (!globalHistorySet && !column.NeedsHistorySet)
            {
                return;
            }

            if (column.MatchingParam.HistorySet?.Value == true)
            {
                return;
            }

            if (table.TryGetPrimaryKeyColumn(protocolModel.RelationManager, out IParamsParam indexColumn) && indexColumn == column.MatchingParam)
            {
                // Ignore index column
                return;
            }

            // Reference node needs to be param as that's where the Fix needs to happen.
            subResults.Add(Error.ColumnMissingHistorySet(test, column.MatchingParam, qAction, column.MatchingParam.Id.RawValue));
        }

        /// <summary>
        /// returns true if object is of type DateTime
        /// </summary>
        private static bool IsDateTime(Value value)
        {
            if (value == null)
            {
                // Can't resolve for now
                return false;
            }

            switch (value.Type)
            {
                case Value.ValueType.DateTime: return true;

                // datetime in Value is parsing as type unknown
                case Value.ValueType.Unknown: return String.Equals(value.Object, "System.DateTime");
            }

            return false;
        }

        private bool CheckTable(Table table, List<IValidationResult> subResults, CallingMethodClass callingMethod)
        {
            if (table.TableId == null || !table.TableId.HasStaticValue)
            {
                return false;
            }

            int tablePid = table.TableId.AsInt32;

            // Check if the parameter exists.
            if (table.MatchingParam == null)
            {
                subResults.Add(Error.NonExistingTable(test, qAction, qAction, tablePid.ToString(), qAction.Id.RawValue).WithCSharp(callingMethod));
            }

            if (!table.TableId.IsParameterClass(semanticModel, solution))
            {
                subResults.Add(Error.HardCodedTablePid(test, qAction, qAction, tablePid.ToString(), qAction.Id.RawValue).WithCSharp(callingMethod));
            }

            return table.MatchingParam != null;
        }

        private bool CheckColumn(Column column, List<IValidationResult> subResults, CallingMethodClass callingMethod)
        {
            if (column.ColumnId == null || !column.ColumnId.HasStaticValue)
            {
                return false;
            }

            int columnPid = column.ColumnId.AsInt32;

            // Check if the parameter exists.
            if (column.MatchingParam == null)
            {
                subResults.Add(Error.NonExistingColumn(test, qAction, qAction, columnPid.ToString(), qAction.Id.RawValue).WithCSharp(callingMethod));
            }

            if (!column.ColumnId.IsParameterClass(semanticModel, solution))
            {
                subResults.Add(Error.HardCodedColumnPid(test, qAction, qAction, columnPid.ToString(), qAction.Id.RawValue).WithCSharp(callingMethod));
            }

            return column.MatchingParam != null;
        }
    }
}
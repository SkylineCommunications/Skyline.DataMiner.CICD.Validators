namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckVolatileTables
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
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

    [Test(CheckId.CheckVolatileTables, Category.Param)]
    internal class CheckVolatileTables : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            var results = new List<IValidationResult>();

            // Prepare.
            var tablesInfo = context.EachParamWithValidId()
                .Where(p => p.IsTable())
                .Select(t => new
                {
                    TableParam = t,
                    IsVolatile = t.ArrayOptions?.GetOptions()?.HasVolatile == true,
                    SubResults = new List<IValidationResult>()
                }).ToList();

            var subResultsPerTablePid = tablesInfo
                .ToDictionary(x => x.TableParam.Id.RawValue, x => x.SubResults);

            // Validate.
            ValidateHelper helper = new ValidateHelper(this, context, results, subResultsPerTablePid);

            foreach (var tableInfo in tablesInfo)
            {
                var tableParam = tableInfo.TableParam;
                var subResults = tableInfo.SubResults;

                helper.Validate(tableParam, subResults);
            }

            // Results: Incompatible volatile.
            results.AddRange(
                tablesInfo
                    .Where(t => t.IsVolatile && t.SubResults.Any())
                    .Select(t => Error.IncompatibleVolatileTable(
                        this,
                        referenceNode: t.TableParam,
                        positionNode: t.TableParam,
                        tablePID: t.TableParam.Id?.RawValue
                    ).WithSubResults(t.SubResults.ToArray()))
            );

            // Results: Suggested volatile.
            results.AddRange(
                tablesInfo
                    .Where(t => !t.IsVolatile && !t.SubResults.Any())
                    .Select(t => Error.SuggestedVolatileOption(
                        this,
                        referenceNode: t.TableParam,
                        positionNode: t.TableParam,
                        tablePID: t.TableParam.Id?.RawValue
                    ))
            );

            return results;
        }

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {
        ////        default:
        ////            result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
        ////            break;
        ////    }

        ////    return result;
        ////}

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        private readonly IProtocolModel model;
        private readonly Dictionary<string, List<IValidationResult>> subResultsPerTablePid;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, Dictionary<string, List<IValidationResult>> subResultsPerTablePid)
            : base(test, context, results)
        {
            model = context.ProtocolModel;
            this.subResultsPerTablePid = subResultsPerTablePid;
        }

        public void Validate(IParamsParam tableParam, List<IValidationResult> subResults)
        {
            // This table.
            ValidateColumns(tableParam, subResults);
            ValidateColumnOptions(tableParam, subResults);

            ValidateDcfUsage(tableParam, subResults);

            // Other linked tables.
            ValidateForeignKeyDestinationTable(tableParam);
        }

        private void ValidateColumns(IParamsParam tableParam, List<IValidationResult> subResults)
        {
            var relationManager = model.RelationManager;

            foreach (var columnInfo in tableParam.GetColumns(relationManager, returnBaseColumnsIfDuplicateAs: true))
            {
                var columnParam = columnInfo.columnParam;

                // Alarming.
                if (columnParam.Alarm?.Monitored?.Value == true)
                {
                    subResults.Add(Error.IncompatibleVolatileTable_Alarming(
                        test,
                        referenceNode: columnParam,
                        positionNode: columnParam.Alarm.Monitored,
                        monitoredValue: columnParam.Alarm.Monitored.RawValue,
                        columnPID: columnInfo.pid));
                }
            }
        }

        private void ValidateColumnOptions(IParamsParam tableParam, List<IValidationResult> subResults)
        {
            foreach (var columnOption in tableParam.ArrayOptions)
            {
                var options = columnOption.GetOptions();

                // Save.
                if (options?.IsSaved == true)
                {
                    subResults.Add(Error.IncompatibleVolatileTable_ColumnOption(
                        test,
                        referenceNode: tableParam,
                        positionNode: columnOption,
                        optionName: "save",
                        columnIdx: columnOption.Idx?.RawValue));
                }

                // DVE.
                if (options?.DVE?.IsElement == true)
                {
                    subResults.Add(Error.IncompatibleVolatileTable_ColumnOption(
                        test,
                        referenceNode: tableParam,
                        positionNode: columnOption,
                        optionName: "element",
                        columnIdx: columnOption.Idx?.RawValue));
                }

                // ForeignKey (source side - child table).
                if (options?.ForeignKey?.Pid != null)
                {
                    subResults.Add(Error.IncompatibleVolatileTable_ColumnOption(
                        test,
                        referenceNode: tableParam,
                        positionNode: columnOption,
                        optionName: "foreignKey",
                        columnIdx: columnOption.Idx?.RawValue));
                }
            }
        }

        private void ValidateDcfUsage(IParamsParam tableParam, List<IValidationResult> subResults)
        {
            if (model.Protocol?.ParameterGroups == null)
            {
                return;
            }

            foreach (var parameterGroup in model.Protocol.ParameterGroups)
            {
                if (parameterGroup.DynamicId?.Value == tableParam.Id?.Value)
                {
                    subResults.Add(Error.IncompatibleVolatileTable_DCF(
                        test,
                        referenceNode: parameterGroup,
                        positionNode: parameterGroup.DynamicId,
                        dynamicID: parameterGroup.DynamicId.RawValue,
                        parameterGroupID: parameterGroup.Id?.RawValue));
                }
            }
        }

        private void ValidateForeignKeyDestinationTable(IParamsParam tableParam)
        {
            foreach (var columnOption in tableParam.ArrayOptions)
            {
                var options = columnOption.GetOptions();

                var fkPid = options?.ForeignKey?.Pid?.ToString();
                if (string.IsNullOrEmpty(fkPid))
                {
                    continue;
                }

                // FK destination side (parent table).
                if (model.TryGetObjectByKey(Mappings.ParamsById, fkPid, out IParamsParam destTable) && destTable.IsTable())
                {
                    var destTablePid = destTable.Id?.RawValue;
                    if (!string.IsNullOrEmpty(destTablePid) && subResultsPerTablePid.TryGetValue(destTablePid, out var destSubs))
                    {
                        destSubs.Add(Error.IncompatibleVolatileTable_ForeignKeyDestination(
                            test,
                            referenceNode: tableParam,
                            positionNode: columnOption,
                            fkValue: fkPid,
                            columnIdx: columnOption.Idx?.RawValue));
                    }
                }
            }
        }
    }
}
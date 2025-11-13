namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckVolatileTables
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckVolatileTables, Category.Param)]
    internal class CheckVolatileTables : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            var results = new List<IValidationResult>();

            // Collect all array params.
            var allTables = context.EachParamWithValidId()
                                   .Where(p => p.Type?.Value == EnumParamType.Array)
                                   .ToList();

            // Pre-create an entry for every table so FK destination sub results can be added immediately (order independent).
            var tableInfo = allTables
                .ConvertAll(t => new
                {
                    Table = t,
                    IsVolatile = t.ArrayOptions?.GetOptions()?.HasVolatile == true,
                    Subs = new List<IValidationResult>()
                })
;

            // Map PID -> info for quick FK destination lookups.
            var subsMap = tableInfo
                .Where(x => !string.IsNullOrEmpty(x.Table.Id?.RawValue))
                .ToDictionary(x => x.Table.Id.RawValue, x => x.Subs);

            // Run validations per table.
            foreach (var info in tableInfo)
            {
                var table = info.Table;
                var subs = info.Subs;

                ValidateColumns(this, context, table, subs);
                ValidateColumnOptions(this, table, subs);
                ValidateDcfUsage(this, context, table, subs);
                ValidateForeignKeys(this, context, table, subs, subsMap);
            }

            // Incompatible volatile tables.
            results.AddRange(
                tableInfo
                    .Where(t => t.IsVolatile && t.Subs.Any())
                    .Select(t => Error.IncompatibleVolatileTable(
                        this,
                        referenceNode: t.Table,
                        positionNode: t.Table,
                        tablePID: t.Table.Id?.RawValue
                    ).WithSubResults(t.Subs.ToArray()))
            );

            // Suggested volatile tables
            results.AddRange(
                tableInfo
                    .Where(t => !t.IsVolatile && t.Subs.Count == 0)
                    .Select(t => Error.SuggestedVolatileOption(
                        this,
                        referenceNode: t.Table,
                        positionNode: t.Table,
                        tablePID: t.Table.Id?.RawValue
                    ))
            );

            return results;
        }

        private static void ValidateColumns(IValidate test, ValidatorContext context, IParamsParam tableParam, List<IValidationResult> results)
        {
            var relationManager = context.ProtocolModel?.RelationManager;
            var tableColumns = tableParam.GetColumns(relationManager, returnBaseColumnsIfDuplicateAs: true)
                .Select(c => c.pid)
                .Where(pid => !string.IsNullOrEmpty(pid));

            foreach (string columnPid in tableColumns)
            {
                if (!context.ProtocolModel.TryGetObjectByKey(Mappings.ParamsById, columnPid, out IParamsParam columnParam))
                    continue;

                if (columnParam.Alarm?.Monitored?.Value == true)
                {
                    results.Add(Error.IncompatibleVolatileTable_Alarming(
                        test,
                        referenceNode: tableParam,
                        positionNode: columnParam,
                        monitoredValue: columnParam.Alarm?.Monitored?.RawValue,
                        columnPID: columnPid));
                }
            }
        }

        private static void ValidateDcfUsage(IValidate test, ValidatorContext context, IParamsParam tableParam, List<IValidationResult> results)
        {
            if (context.ProtocolModel?.Protocol?.ParameterGroups == null || tableParam.Id?.Value == null)
            {
                return;
            }

            foreach (var ParameterGroup in context.ProtocolModel.Protocol.ParameterGroups)
            {
                if (ParameterGroup.DynamicId?.Value != tableParam.Id?.Value)
                {
                    continue;
                }

                results.Add(Error.IncompatibleVolatileTable_DCF(
                    test,
                    referenceNode: tableParam,
                    positionNode: tableParam,
                    dynamicID: ParameterGroup.DynamicId.RawValue,
                    parameterGroupID: ParameterGroup.Id?.RawValue));
            }
        }

        private static void ValidateColumnOptions(IValidate test, IParamsParam tableParam, List<IValidationResult> results)
        {
            foreach (var columnOption in tableParam.ArrayOptions)
            {
                var options = columnOption.GetOptions();
                if (options == null)
                    continue;

                if (options.IsSaved)
                {
                    results.Add(Error.IncompatibleVolatileTable_ColumnOption(
                        test,
                        referenceNode: tableParam,
                        positionNode: columnOption,
                        optionName: "save",
                        columnIdx: columnOption.Idx.RawValue));
                }

                if (options.DVE?.IsElement == true)
                {
                    results.Add(Error.IncompatibleVolatileTable_ColumnOption(
                        test,
                        referenceNode: tableParam,
                        positionNode: columnOption,
                        optionName: "element",
                        columnIdx: columnOption.Idx.RawValue));
                }
            }
        }

        private static void ValidateForeignKeys(
            IValidate test,
            ValidatorContext context,
            IParamsParam tableParam,
            List<IValidationResult> subResults,
            Dictionary<string, List<IValidationResult>> subsMap)
        {
            foreach (var columnOption in tableParam.ArrayOptions)
            {
                var options = columnOption.GetOptions();
                var fkPid = options?.ForeignKey?.Pid.ToString();
                if (string.IsNullOrEmpty(fkPid))
                    continue;

                string columnIdx = columnOption.Idx?.RawValue;

                // Source side.
                subResults.Add(Error.IncompatibleVolatileTable_ColumnOption(
                    test,
                    referenceNode: tableParam,
                    positionNode: columnOption,
                    optionName: "foreignKey",
                    columnIdx: columnIdx));

                // Destination side (add sub result to destination table if volatile).
                if (context.ProtocolModel.TryGetObjectByKey(Mappings.ParamsById, fkPid, out IParamsParam destTable) &&
                    destTable.Type?.Value == EnumParamType.Array)
                {
                    var destId = destTable.Id?.RawValue;
                    if (!string.IsNullOrEmpty(destId) && subsMap.TryGetValue(destId, out var destSubs))
                    {
                        destSubs.Add(
                            Error.IncompatibleVolatileTable_ForeignKeyTable(
                                test,
                                referenceNode: destTable,
                                positionNode: columnOption,
                                relationPath: $"{tableParam.Id?.RawValue};{fkPid}"));
                    }
                }
            }
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
}
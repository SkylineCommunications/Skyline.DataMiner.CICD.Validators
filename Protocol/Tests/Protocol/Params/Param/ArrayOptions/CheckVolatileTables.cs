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

            // Pre-create an entry for every table so FK destination sub results can be added immediately (order independent).
            var tablesInfo = context.EachParamWithValidId()
                .Where(p => p.IsTable())
                .Select(t => new
                {
                    Table = t,
                    IsVolatile = t.ArrayOptions?.GetOptions()?.HasVolatile == true,
                    Subs = new List<IValidationResult>()
                }).ToList();

            // Map PID -> info for quick FK destination look-ups.
            var subsMap = tablesInfo
                .ToDictionary(x => x.Table.Id.RawValue, x => x.Subs);

            // Validate (by preparing sub-results).
            ValidateHelper helper = new ValidateHelper(this, context, results, subsMap);

            foreach (var tableInfo in tablesInfo)
            {
                var table = tableInfo.Table;
                var subs = tableInfo.Subs;

                helper.Validate(table, subs);
            }

            // Incompatible volatile.
            results.AddRange(
                tablesInfo
                    .Where(t => t.IsVolatile && t.Subs.Any())
                    .Select(t => Error.IncompatibleVolatileTable(
                        this,
                        referenceNode: t.Table,
                        positionNode: t.Table,
                        tablePID: t.Table.Id?.RawValue
                    ).WithSubResults(t.Subs.ToArray()))
            );

            // Suggested volatile.
            results.AddRange(
                tablesInfo
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
            ValidateColumns(tableParam, subResults);
            ValidateColumnOptions(tableParam, subResults);
            ValidateDcfUsage(tableParam, subResults);
            ValidateForeignKeys(tableParam, subResults);
        }

        private void ValidateColumns(IParamsParam tableParam, List<IValidationResult> subResults)
        {
            var relationManager = context.ProtocolModel?.RelationManager;

            foreach (var columnInfo in tableParam.GetColumns(relationManager, returnBaseColumnsIfDuplicateAs: true))
            {
                var columnParam = columnInfo.columnParam;

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

                if (options?.IsSaved == true)
                {
                    subResults.Add(Error.IncompatibleVolatileTable_ColumnOption(
                        test,
                        referenceNode: tableParam,
                        positionNode: columnOption,
                        optionName: "save",
                        columnIdx: columnOption.Idx.RawValue));
                }

                if (options?.DVE?.IsElement == true)
                {
                    subResults.Add(Error.IncompatibleVolatileTable_ColumnOption(
                        test,
                        referenceNode: tableParam,
                        positionNode: columnOption,
                        optionName: "element",
                        columnIdx: columnOption.Idx.RawValue));
                }
            }
        }

        private void ValidateDcfUsage(IParamsParam tableParam, List<IValidationResult> subResults)
        {
            if (context.ProtocolModel?.Protocol?.ParameterGroups == null)
            {
                return;
            }

            foreach (var ParameterGroup in context.ProtocolModel.Protocol.ParameterGroups)
            {
                if (ParameterGroup.DynamicId?.Value == tableParam.Id?.Value)
                {
                    subResults.Add(Error.IncompatibleVolatileTable_DCF(
                        test,
                        referenceNode: ParameterGroup,
                        positionNode: ParameterGroup.DynamicId,
                        dynamicID: ParameterGroup.DynamicId.RawValue,
                        parameterGroupID: ParameterGroup.Id?.RawValue));
                }
            }
        }

        private void ValidateForeignKeys(IParamsParam tableParam, List<IValidationResult> subResults)
        {
            foreach (var columnOption in tableParam.ArrayOptions)
            {
                var options = columnOption.GetOptions();

                var fkPid = options?.ForeignKey?.Pid?.ToString();
                if (string.IsNullOrEmpty(fkPid))
                {
                    continue;
                }

                // Source side (child table).
                subResults.Add(Error.IncompatibleVolatileTable_ColumnOption(
                    test,
                    referenceNode: tableParam,
                    positionNode: columnOption,
                    optionName: "foreignKey",
                    columnIdx: columnOption.Idx?.RawValue));

                // Destination side (parent table).
                if (context.ProtocolModel.TryGetObjectByKey(Mappings.ParamsById, fkPid, out IParamsParam destTable)
                    && destTable.IsTable())
                {
                    var destTablePid = destTable.Id?.RawValue;
                    if (!string.IsNullOrEmpty(destTablePid) && subResultsPerTablePid.TryGetValue(destTablePid, out var destSubs))
                    {
                        destSubs.Add(Error.IncompatibleVolatileTable_ForeignKeyTable(
                            test,
                            referenceNode: tableParam,
                            positionNode: columnOption,
                            relationPath: $"{tableParam.Id?.RawValue};{fkPid}"));
                    }
                }
            }
        }

    }
}
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
            List<IValidationResult> results = new List<IValidationResult>();

            // IncompatibleVolatileTable: tables already marked as volatile but using incompatible features
            var volatileTables = context.EachParamWithValidId().Where(IsVolatileArray);

            foreach (IParamsParam tableParam in volatileTables)
            {
                string tablePid = tableParam.Id?.RawValue;

                // Column Param
                var columnResults = new List<IValidationResult>();
                ValidateColumns(this, context, tableParam, columnResults);

                // ColumnOptions
                var columnOptionResults = new List<IValidationResult>();
                ValidateColumnOptions(this, tableParam, columnOptionResults);

                // DCF
                var dcfUsageResults = new List<IValidationResult>();
                ValidateDcfUsage(this, context, tableParam, dcfUsageResults);

                // Summary
                if (columnResults.Count > 0 || dcfUsageResults.Count > 0 || columnOptionResults.Count > 0)
                {
                    var parentResult = Error.IncompatibleVolatileTable(
                        this,
                        referenceNode: tableParam,
                        positionNode: tableParam,
                        tablePID: tablePid
                    ).WithSubResults(
                        columnResults
                            .Concat(dcfUsageResults)
                            .Concat(columnOptionResults)
                            .ToArray()
                    );

                    results.Add(parentResult);
                }
            }

            // SuggestedVolatileOption: tables that are NOT volatile, and not using any incompatible features
            var relationManager = context.ProtocolModel?.RelationManager;

            var nonVolatileTables = context.EachParamWithValidId()
                                           .Where(p => p.Type?.Value == EnumParamType.Array &&
                                                       p.ArrayOptions?.GetOptions()?.HasVolatile != true);

            foreach (var tableParam in nonVolatileTables.Where(tp => IsCleanForVolatile(context, tp, relationManager)))
            {
                string tablePid = tableParam.Id?.RawValue;
                results.Add(Error.SuggestedVolatileOption(
                    this,
                    referenceNode: tableParam,
                    positionNode: tableParam,
                    tablePID: tablePid));
            }


            return results;
        }

        private static bool IsCleanForVolatile(ValidatorContext context, IParamsParam tableParam, RelationManager relationManager)
        {
            // No DCF usage
            if (context.ProtocolModel?.Protocol?.ParameterGroups?.Any(pg => pg.DynamicId?.Value == tableParam.Id?.Value) == true)
            {
                return false;
            }

            // Not used for DVEs
            if (context.ProtocolModel?.Protocol?.ExportRules?.Any(er => er.Table?.Value == tableParam.Id?.RawValue) == true)
            {
                return false;
            }

            // No relations including this table
            var tableIdRaw = tableParam.Id?.RawValue;
            var relations = context.ProtocolModel?.Protocol?.Relations;
            if (relations != null && relations.Any(r => (r.Path?.Value ?? "").Split(';').Contains(tableIdRaw)))
            {
                return false;
            }

            // Inspect columns
            var columnPids = tableParam
                .GetColumns(relationManager, returnBaseColumnsIfDuplicateAs: true)
                .Select(c => c.pid)
                .Where(pid => !string.IsNullOrEmpty(pid));

            foreach (string pid in columnPids)
            {
                if (!context.ProtocolModel.TryGetObjectByKey(Mappings.ParamsById, pid, out IParamsParam col))
                {
                    continue;
                }

                if (col.Alarm?.Monitored?.Value == true)
                {
                    return false;
                }
            }

            // Column options
            foreach (var column in tableParam.ArrayOptions)
            {
                var opts = column.GetOptions();
                if (opts == null)
                {
                    continue;
                }

                if (opts.IsSaved)
                {
                    return false;
                }

                if (opts.ForeignKey?.Pid != null)
                {
                    return false;
                }

                if (opts.DVE?.IsElement == true)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsVolatileArray(IParamsParam param) =>
            param.Type?.Value == EnumParamType.Array &&
            param.ArrayOptions?.GetOptions()?.HasVolatile == true;

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

                if (options.ForeignKey?.Pid != null)
                {
                    results.Add(Error.IncompatibleVolatileTable_ColumnOption(
                        test,
                        referenceNode: tableParam,
                        positionNode: columnOption,
                        optionName: "foreignKey",
                        columnIdx: columnOption.Idx.RawValue));
                }

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
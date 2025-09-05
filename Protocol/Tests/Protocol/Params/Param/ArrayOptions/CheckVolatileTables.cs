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

            // 1) INCOMPATIBLE: tables already marked as volatile but using incompatible features
            var volatileTables = context.EachParamWithValidId().Where(IsVolatileArray);

            foreach (IParamsParam tableParam in volatileTables)
            {
                string tableId = tableParam.Id?.RawValue;

                var columnResults = new List<IValidationResult>();
                ValidateColumns(this, context, tableParam, columnResults);

                var exportRuleResults = new List<IValidationResult>();
                ValidateExportRule(this, context, tableParam, exportRuleResults, tableId);

                var dcfUsageResults = new List<IValidationResult>();
                ValidateDcfUsage(this, context, tableParam, dcfUsageResults, tableId);

                var columnOptionResults = new List<IValidationResult>();
                ValidateColumnOptions(this, tableParam, columnOptionResults, tableId);

                if (columnResults.Count > 0 || exportRuleResults.Count > 0 || dcfUsageResults.Count > 0 || columnOptionResults.Count > 0)
                {
                    var parentResult = Error.IncompatibleVolatileTable(
                        this,
                        referenceNode: tableParam,
                        positionNode: tableParam,
                        item2Value: tableId
                    ).WithSubResults(
                        columnResults.Concat(exportRuleResults)
                                     .Concat(dcfUsageResults)
                                     .Concat(columnOptionResults)
                                     .ToArray()
                    );

                    results.Add(parentResult);
                }
            }

            // 2) SUGGESTION: tables that are arrays, NOT volatile, and "clean" suggest adding volatile
            var relationManager = context.ProtocolModel?.RelationManager;

            var nonVolatileTables = context.EachParamWithValidId()
                                           .Where(p => p.Type?.Value == EnumParamType.Array &&
                                                       p.ArrayOptions?.GetOptions()?.HasVolatile != true);

            foreach (var tableParam in nonVolatileTables.Where(tp => IsCleanForVolatile(context, tp, relationManager)))
            {
                string tableId = tableParam.Id?.RawValue;
                results.Add(Error.SuggestedVolatileOption(
                    this,
                    referenceNode: tableParam,
                    positionNode: tableParam,
                    itemId: tableId));
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

                if (col.Trending?.Value == true)
                {
                    return false;
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

                if (columnParam.Trending?.Value == true)
                {
                    results.Add(Error.IncompatibleVolatileOption(
                        test,
                        referenceNode: tableParam,
                        positionNode: columnParam,
                        item2Title: "Param@trending",
                        item2Value: "true",
                        itemKind: "Column",
                        idOrPid: "PID",
                        itemId: columnPid));
                }

                if (columnParam.Alarm?.Monitored?.Value == true)
                {
                    results.Add(Error.IncompatibleVolatileOption(
                        test,
                        referenceNode: tableParam,
                        positionNode: columnParam,
                        item2Title: "Alarm/Monitored",
                        item2Value: "true",
                        itemKind: "Column",
                        idOrPid: "PID",
                        itemId: columnPid));
                }
            }
        }

        private static void ValidateExportRule(IValidate test, ValidatorContext context, IParamsParam tableParam, List<IValidationResult> results, string tableId)
        {
            if (context.ProtocolModel?.Protocol?.ExportRules?.Any(er => er.Table?.Value == tableParam.Id?.RawValue) == true)
            {
                results.Add(Error.IncompatibleVolatileOption(
                    test,
                    referenceNode: tableParam,
                    positionNode: tableParam,
                    item2Title: "ExportRule@table",
                    item2Value: "ExportRule",
                    itemKind: "Table",
                    idOrPid: "PID",
                    itemId: tableId));
            }
        }

        private static void ValidateDcfUsage(IValidate test, ValidatorContext context, IParamsParam tableParam, List<IValidationResult> results, string tableId)
        {
            if (context.ProtocolModel?.Protocol?.ParameterGroups?.Any(pg => pg.DynamicId?.Value == tableParam.Id?.Value) == true)
            {
                results.Add(Error.IncompatibleVolatileOption(
                    test,
                    referenceNode: tableParam,
                    positionNode: tableParam,
                    item2Title: "ParameterGroups/Group@dynamicId",
                    item2Value: "dynamicId",
                    itemKind: "Table",
                    idOrPid: "PID",
                    itemId: tableId));
            }
        }

        private static void ValidateColumnOptions(IValidate test, IParamsParam tableParam, List<IValidationResult> results, string tableId)
        {
            foreach (var column in tableParam.ArrayOptions)
            {
                var options = column.GetOptions();
                if (options == null)
                    continue;

                if (options.ForeignKey?.Pid != null)
                {
                    results.Add(Error.IncompatibleVolatileOption(
                        test,
                        referenceNode: tableParam,
                        positionNode: column,
                        item2Title: "ColumnOption/options",
                        item2Value: "foreignKey",
                        itemKind: "Table",
                        idOrPid: "PID",
                        itemId: tableId));
                }

                if (options.IsSaved)
                {
                    results.Add(Error.IncompatibleVolatileOption(
                        test,
                        referenceNode: tableParam,
                        positionNode: column,
                        item2Title: "ColumnOption/options",
                        item2Value: "save",
                        itemKind: "Table",
                        idOrPid: "PID",
                        itemId: tableId));
                }

                if (options.DVE?.IsElement == true)
                {
                    results.Add(Error.IncompatibleVolatileOption(
                        test,
                        referenceNode: tableParam,
                        positionNode: column,
                        item2Title: "ColumnOption/options",
                        item2Value: "element",
                        itemKind: "Table",
                        idOrPid: "PID",
                        itemId: tableId));
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
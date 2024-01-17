namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckOptionsAttribute
{
    using System;
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    using static Skyline.DataMiner.CICD.Models.Protocol.Read.ColumnOptionOptions;

    [Test(CheckId.CheckOptionsAttribute, Category.Param)]
    internal class CheckOptionsAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam tableParam in context.EachParamWithValidId())
            {
                var arrayOptions = tableParam.ArrayOptions;
                if (arrayOptions == null)
                {
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, tableParam);

                bool hasFilterChange = arrayOptions.GetOptions()?.FilterChange != null;
                foreach (var column in arrayOptions)
                {
                    ColumnOptionOptions options = column.GetOptions();
                    if (options == null)
                    {
                        continue;
                    }

                    helper.CheckViewTable(hasFilterChange, column, options);
                    helper.CheckForeignKey(column, options);
                    helper.CheckOptionsRequiringRTDisplay(column, options);
                    ////helper.CheckSave(column, options);
                }
            }

            return results;
        }

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {

        ////        default:
        ////            result.Message = $"This error ({context.Result.ErrorId.ToString()}) isn't implemented.";
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

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly IProtocolModel model;
        private readonly RelationManager relationManager;
        private readonly List<IValidationResult> results;

        private readonly IParamsParam tableParam;
        private readonly IEnumerable<(uint? idx, string pid, IParamsParam columnParam)> tableColumns;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam tableParam)
        {
            this.test = test;
            this.context = context;
            this.model = context.ProtocolModel;
            this.relationManager = model.RelationManager;
            this.results = results;

            this.tableParam = tableParam;
            this.tableColumns = tableParam.GetColumns(relationManager, returnBaseColumnsIfDuplicateAs: true);
        }

        public void CheckViewTable(bool hasFilterChange, ITypeColumnOption columnOption, ColumnOptionOptions options)
        {
            ViewTableClass viewTable = options.ViewTable;

            if (viewTable == null)
            {
                // No ViewTable options
                return;
            }

            // 3 valid syntaxes are possible
            // * view=1201
            // * view=1204:2004
            // * view=:2802:1000:1 -> first ':' is not a typo!

            uint? idx = columnOption.Idx?.Value;
            uint? pid = columnOption.Pid?.Value;

            if (viewTable.Id == null)
            {
                // Unrecognized syntax
                results.Add(Error.ViewInvalidSyntax(test, tableParam, columnOption, Convert.ToString(idx), tableParam.Id?.RawValue));
                return;
            }

            if (hasFilterChange)
            {
                // Option cannot be used in combination with filterChange=
                results.Add(Error.ViewInvalidCombinationFilterChange(test, tableParam, columnOption, tableParam.Id?.RawValue));
            }
            else
            {
                string optionValue = viewTable.OptionValue;

                bool option2 = viewTable.Id2 != null;

                if (viewTable.Id == pid || (option2 && viewTable.Id2 == pid))
                {
                    results.Add(Error.ViewInvalidColumnReference(test, tableParam, columnOption, Severity.Critical, optionValue, tableParam.Id?.RawValue));
                }
                else if (!ParamHelper.TryFindTableParamForColumnPid(model, Convert.ToString(viewTable.Id), out var otherTableParam1) || tableParam == otherTableParam1 ||
                        (!viewTable.IsRemote && option2 && (!ParamHelper.TryFindTableParamForColumnPid(model, Convert.ToString(viewTable.Id2), out var otherTableParam2) || tableParam == otherTableParam2)))
                {
                    results.Add(Error.ViewInvalidColumnReference(test, tableParam, columnOption, Severity.Major, optionValue, tableParam.Id?.RawValue));
                }
            }
        }

        public void CheckForeignKey(ITypeColumnOption columnOption, ColumnOptionOptions options)
        {
            // No ForeignKey
            if (options?.ForeignKey == null)
            {
                return;
            }

            // Wrong ForeignKey Format
            if (options.ForeignKey.Pid == null)
            {
                // TODO: error message about invalid foreignKey ?
                return;
            }

            // Source column
            if (columnOption.Pid.Value.HasValue && columnOption.Idx.Value.HasValue)
            {
                CheckForeignKeyColumn(columnOption.Idx.Value.Value);
            }

            // Target table
            string fkToTablePid = Convert.ToString(options.ForeignKey.Pid);
            CheckForeignKeyTargetTable(columnOption, options, fkToTablePid);

            // Relations
            if (!context.ProtocolModel.IsExportedProtocolModel && // Relations don't exist in exported protocols
                fkToTablePid != tableParam.Id.RawValue)           // Recursive FKs don't require a relation
            {
                var relations = model?.Protocol?.Relations;
                bool foundRelation = CheckForeignKeyInRelations(fkToTablePid, relations);
                if (!foundRelation)
                {
                    results.Add(Error.ForeignKeyMissingRelation(test, tableParam, columnOption, fkToTablePid, tableParam.Id.RawValue, columnOption.Pid.RawValue));
                }
            }
        }

        public void CheckOptionsRequiringRTDisplay(ITypeColumnOption columnOptionTag, ColumnOptionOptions options)
        {
            // Non Existing Param
            if (columnOptionTag.Pid?.Value == null ||
                !TryGetColumn(Convert.ToString(columnOptionTag.Pid.Value), out var column) ||
                column.param == null)
            {
                return;
            }

            foreach (string option in options.OriginalValue.Split(new char[] { options.Separator }, StringSplitOptions.RemoveEmptyEntries))
            {
                // TODO: the below won't 100% do as if one uses option "severityBlabla",
                // it will match "severity" and end up in a false positive.
                // We'll see in practice if we need to fine-tune this.
                // To have this 100% correct, we probably should split optionsRequiringRTDisplay in different arrays.
                // - 1 for exact match
                // - 1 for startWith followed by separator x
                // - 1 for startWith followed by separator y
                // - ...
                if (!OptionsRequiringRtDisplay.Any(s => option.StartsWith(s, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                // RTDisplay Expected
                IValidationResult rtDisplayError = Error.ColumnOptionExpectingRTDisplay(test, tableParam, columnOptionTag, column.pid, option, tableParam.Id.RawValue);
                context.CrossData.RtDisplay.AddParam(column.param, rtDisplayError);

                // Write column
                if (column.param.TryGetWrite(relationManager, out var writeColumn))
                {
                    // RTDisplay Expected
                    IValidationResult rtDisplayErrorOnWriteColumn = Error.ColumnOptionExpectingRTDisplay(test, tableParam, columnOptionTag, writeColumn.Id?.RawValue, option, tableParam.Id.RawValue);
                    context.CrossData.RtDisplay.AddParam(writeColumn, rtDisplayErrorOnWriteColumn);
                }

                // ForeignKey part more deeply handled by CheckForeignKey method
            }
        }

        private void CheckForeignKeyColumn(uint fkColumnIdx)
        {
            // Lookup based on IDX rather than PID to make sure it works also for view tables
            var fkColumnParam = tableParam.GetColumns(relationManager, returnBaseColumnsIfDuplicateAs: true).Where(t => t.idx == fkColumnIdx).Select(t => t.columnParam).FirstOrDefault();
            if (fkColumnParam == null)
            {
                // NonExistingColumn handled by ColumnOption.Pid check
                return;
            }

            // FK Param/Type
            var paramType = fkColumnParam.Type;
            if (paramType == null)
            {
                // Covered by Param/Type check
                ////results.Add(Error.ForeignKeyColumnInvalidType(test, fkColumnParam, fkColumnParam, "UNDEFINED", fkColumnParam.Id.RawValue));
            }
            else if (paramType.Value == EnumParamType.Write
                || paramType.Value == EnumParamType.Group
                || paramType.Value == EnumParamType.ReadBit
                || paramType.Value == EnumParamType.WriteBit)
            {
                // FK Param/Type Invalid (Only check for types that are valid for normal columns but not for FKs, other ones are covered by Param.CheckColumns)
                results.Add(Error.ForeignKeyColumnInvalidType(test, fkColumnParam, paramType, paramType.RawValue, fkColumnParam.Id.RawValue));
            }

            // FK Interprete/Type
            var interpretType = fkColumnParam.Interprete?.Type;
            if (interpretType == null)
            {
                results.Add(Error.ForeignKeyColumnInvalidInterpreteType(test, fkColumnParam, fkColumnParam, "UNDEFINED", fkColumnParam.Id.RawValue));
            }
            else if (interpretType.Value.HasValue
                && interpretType.Value != EnumParamInterpretType.String)
            {
                results.Add(Error.ForeignKeyColumnInvalidInterpreteType(test, fkColumnParam, interpretType, interpretType.RawValue, fkColumnParam.Id.RawValue));
            }

            // FK Measurement/Type
            if (tableParam.GetRTDisplay() || tableParam.WillBeExported())
            {
                var measurementType = fkColumnParam.Measurement?.Type;
                if (measurementType == null)
                {
                    results.Add(Error.ForeignKeyColumnInvalidMeasurementType(test, fkColumnParam, fkColumnParam, "UNDEFINED", fkColumnParam.Id.RawValue));
                }
                else if (measurementType.Value.HasValue
                    && measurementType.Value != EnumParamMeasurementType.String
                    && measurementType.Value != EnumParamMeasurementType.Number)
                {
                    results.Add(Error.ForeignKeyColumnInvalidMeasurementType(test, fkColumnParam, measurementType, measurementType.RawValue, fkColumnParam.Id.RawValue));
                }
            }
        }

        private void CheckForeignKeyTargetTable(ITypeColumnOption columnOption, ColumnOptionOptions options, string fkToTablePid)
        {
            // Ref to NonExistingTable
            if (!model.TryGetObjectByKey(Mappings.ParamsById, fkToTablePid, out IParamsParam fkToTableParam))
            {
                // Covered by Relation.CheckPathAttribute check
                return;
            }

            if (fkToTableParam.TryGetPrimaryKeyColumn(relationManager, out var pkColumn))
            {
                // RTDisplay Expected
                IValidationResult rtDisplayError = Error.ForeignKeyTargetExpectingRTDisplayOnPK(test, tableParam, columnOption, pkColumn.Id?.RawValue, options.ForeignKey.OriginalValue, tableParam.Id.RawValue);
                context.CrossData.RtDisplay.AddParam(pkColumn, rtDisplayError);
            }
        }

        private bool CheckForeignKeyInRelations(string fkToTablePid, IRelations relations)
        {
            if (relations == null)
            {
                return false;
            }

            // Going through relations
            bool foundRelation = false;
            foreach (IRelationsRelation relation in relations)
            {
                (GenericStatus status, string _, string value) = GenericTests.CheckBasics(relation.Path, isRequired: true);

                // Sanity checks
                if (status.HasFlag(GenericStatus.Missing))
                {
                    continue;
                }

                if (status.HasFlag(GenericStatus.Empty))
                {
                    continue;
                }

                // Going through relation
                string[] pathTableIds = value.Split(';');
                for (int index = 0; index < pathTableIds.Length - 1; index++)
                {
                    string table1Pid = pathTableIds[index];
                    string table2Pid = pathTableIds[index + 1];

                    // Sanity checks
                    (GenericStatus valueStatus, uint _) = GenericTests.CheckBasics<uint>(table1Pid);
                    if (valueStatus.HasFlag(GenericStatus.Invalid))
                    {
                        continue;
                    }

                    (GenericStatus valueStatus2, uint _) = GenericTests.CheckBasics<uint>(table2Pid);
                    if (valueStatus2.HasFlag(GenericStatus.Invalid))
                    {
                        continue;
                    }

                    // Relation part
                    string fkFromTablePid = Convert.ToString(tableParam.Id.Value);
                    bool oneToTwo = table1Pid == fkFromTablePid && table2Pid == fkToTablePid;
                    bool twoToOne = table2Pid == fkFromTablePid && table1Pid == fkToTablePid;
                    if (oneToTwo || twoToOne)
                    {
                        foundRelation = true;
                        break;
                    }
                }

                if (foundRelation)
                {
                    break;
                }
            }

            return foundRelation;
        }

        private bool TryGetColumn(string columnPid, out (uint? idx, string pid, IParamsParam param) column)
        {
            foreach ((uint? idx, string pid, IParamsParam param) tableColumn in tableColumns)
            {
                if (tableColumn.pid != columnPid)
                {
                    continue;
                }

                column = tableColumn;
                return true;
            }

            column = (null, null, null);
            return false;
        }

        private static readonly string[] OptionsRequiringRtDisplay =
        {
            // Column Header options
            "disableHeaderAvg", "disableHeaderMax", "disableHeaderMin", "disableHeaderSum",
            "disableHeatmap", "disableHistogram",
            "enableHeaderAvg", "enableHeaderMax", "enableHeaderMin", "enableHeaderSum",
            "enableHeatmap", "enableHistogram", 

            // DVE options
            "element", "view", "severity", "hidden",

            // EPM options
            "CpeDummyColumn", "hideKpi", "kpiHideWrite", "viewImpact",

            // SOM options (Service Overview Manager)
            "hideSummaryColumn", "severityColumn", "severityColumnIndex",

            // VISIO options
            "xPos", "yPos", "linkElement", "dynamicData",
            "selectionSetVar", "selectionSetCardVar", "SelectionSetPageVar", "SelectionSetWorkspaceVar",

            // KPI Window
            "space", "subtitle",

            // Other
            "displayIcon", "displayElementAlarm", "displayServiceAlarm", "displayViewAlarm",
            "foreignKey", "rowTextColoring", "showReadAsKpi",

            // Assumed to be fully handled by SLProtocol so shouldn't be taken into account for the RTDisplay check
            ////"indexColumn", "groupBy"
        };
    }
}
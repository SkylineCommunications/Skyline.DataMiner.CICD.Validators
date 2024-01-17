namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Type.CheckOptionsAttribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Skyline.DataMiner.CICD.Models.Protocol.Edit;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    using static Skyline.DataMiner.CICD.Models.Protocol.Read.MeasurementTypeOptions.TableClass;

    [Test(CheckId.CheckOptionsAttribute, Category.Param)]
    internal class CheckOptionsAttribute : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.IsTable())
                {
                    var tableOptions = param.Measurement?.Type?.GetOptions()?.Table;
                    if (tableOptions == null)
                    {
                        continue;
                    }

                    TableValidator tableValidator = new TableValidator(this, context, results, param, tableOptions);

                    // RTDisplay
                    tableValidator.CheckColumnParams();

                    // Sorting
                    if (tableValidator.CheckColumnsSorting())
                    {
                        tableValidator.CheckDateTimeSorting();
                    }
                }
                else if (param.IsMatrix())
                {
                    if (param.Type?.Value == EnumParamType.Write)
                    {
                        continue;
                    }

                    MatrixValidator matrixValidator = new MatrixValidator(this, param, param.Id.RawValue);
                    if (results.AddIfNotNull(matrixValidator.CheckMissingAttribute()))
                    {
                        continue;
                    }

                    if (results.AddIfNotNull(matrixValidator.CheckMissingMatrixOption()))
                    {
                        continue;
                    }

                    if (results.AddIfNotNull(matrixValidator.CheckInvalidMatrixOption()))
                    {
                        continue;
                    }

                    bool invalidInputCount = results.AddIfNotNull(matrixValidator.CheckInvalidMatrixDimensionsToInputCount());
                    bool invalidOutputCount = results.AddIfNotNull(matrixValidator.CheckInvalidColumnDimensionsToOutputCount());
                    if (invalidInputCount || invalidOutputCount)
                    {
                        continue;
                    }

                    if (results.AddIfNotNull(matrixValidator.CheckInvalidConnectedMatrixPoints()))
                    {
                        continue;
                    }
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.MissingAttribute:
                    MatrixValidator.FixMissingAttribute(context);
                    result.Success = true;
                    break;

                case ErrorIds.InvalidMatrixDimensionsToInputCount:
                    MatrixValidator.FixInvalidMatrixDimensionsToInputCount(context);
                    result.Success = true;
                    break;

                case ErrorIds.InvalidColumnDimensionsToOutputCount:
                    MatrixValidator.FixInvalidColumnDimensionsToOutputCount(context);
                    result.Success = true;
                    break;

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                if (!newParam.GetRTDisplay())
                {
                    continue;
                }

                // Old order
                MeasurementTypeOptions oldOptions = oldParam.Measurement?.Type?.GetOptions();
                IEnumerable<uint?> oldOrderedPIDs = oldOptions?.Table?.ColumnsFull?.OrderBy(p => p.DisplayedIndex).Select(p => p.Pid);
                if (oldOrderedPIDs == null)
                {
                    continue;
                }

                string oldColumnOrder = String.Join("-", oldOrderedPIDs);

                // New order
                MeasurementTypeOptions newOptions = newParam.Measurement?.Type?.GetOptions();
                IEnumerable<uint?> newOrderedPIDs = newOptions?.Table?.ColumnsFull?.OrderBy(p => p.DisplayedIndex).Select(p => p.Pid);
                string newColumnOrder = String.Empty;
                if (newOrderedPIDs != null)
                {
                    newColumnOrder = String.Join("-", newOrderedPIDs);
                }

                // Compare
                if (!newColumnOrder.StartsWith(oldColumnOrder))
                {
                    results.Add(ErrorCompare.ColumnOrderChanged(newParam, newParam, oldColumnOrder, newParam.Id?.RawValue, newColumnOrder));
                }
            }

            return results;
        }
    }

    internal class MatrixValidator
    {
        public MatrixValidator(CheckOptionsAttribute linkToTest, IParamsParam param, string pid)
        {
            LinkToTest = linkToTest;
            Pid = pid;
            Param = param;
            ParamType = param.Measurement?.Type;
            Options = param.Measurement?.Type?.GetOptions()?.Matrix;
        }

        public CheckOptionsAttribute LinkToTest { get; }

        public MeasurementTypeOptions.MatrixClass Options { get; }

        public IParamsParam Param { get; }

        public IParamsParamMeasurementType ParamType { get; }

        public string Pid { get; }

        public static void FixInvalidColumnDimensionsToOutputCount(CodeFixContext context)
        {
            IParamsParam readParam = (IParamsParam)context.Result.ReferenceNode;
            ParamsParam writeParam = context.Protocol.Params.Get(readParam);

            if (writeParam?.Measurement?.Type == null)
            {
                // Handled by FixMissingAttribute
                return;
            }

            var extraData = context.Result.ExtraData;
            if (extraData == null || !extraData.TryGetValue(ExtraData.TypeColumns, out object typeColumnCount))
            {
                return;
            }

            var options = readParam.Measurement.Type.GetOptions().Matrix;
            if (!options.IsValid)
            {
                return;
            }

            var currentOptions = writeParam.Measurement.Type.Options;

            // Regex pattern:
            // matrix=16,32,0,1,0,31
            // (group1)(group2)(group3)(group4)
            // (matrix=16,)(32)(,)(0,1,0,31)
            // We are replacing (group2)
            string newOptions = Regex.Replace(currentOptions.Value, @"(matrix=\d*,)(\d*)(,)(.*)", m => m.Groups[1].Value + typeColumnCount + m.Groups[3].Value + m.Groups[4].Value, RegexOptions.IgnorePatternWhitespace);

            writeParam.Measurement.Type.Options = new AttributeValue<string>(newOptions);
        }

        public static void FixInvalidMatrixDimensionsToInputCount(CodeFixContext context)
        {
            IParamsParam readParam = (IParamsParam)context.Result.ReferenceNode;
            ParamsParam writeParam = context.Protocol.Params.Get(readParam);

            if (writeParam?.Measurement?.Type == null)
            {
                // Handled by FixMissingAttribute
                return;
            }

            var extraData = context.Result.ExtraData;
            if (extraData == null || !extraData.TryGetValue(ExtraData.TypeRows, out object typeRowCount))
            {
                return;
            }

            var options = readParam.Measurement.Type.GetOptions().Matrix;
            if (!options.IsValid)
            {
                return;
            }

            var currentOptions = writeParam.Measurement.Type.Options;

            // Regex pattern:
            // matrix=16,32,0,1,0,31
            // (group1)(group2)(group3)(group4)
            // (matrix=)(16)(,)(32,0,1,0,31)
            // We are replacing (group2)
            string newOptions = Regex.Replace(currentOptions.Value, @"(matrix=)(\d*)(,)(.*)", m => m.Groups[1].Value + typeRowCount + m.Groups[3].Value + m.Groups[4].Value, RegexOptions.IgnorePatternWhitespace);

            writeParam.Measurement.Type.Options = new AttributeValue<string>(newOptions);
        }

        public static void FixMissingAttribute(CodeFixContext context)
        {
            IParamsParam readParam = (IParamsParam)context.Result.ReferenceNode;
            ParamsParam writeParam = context.Protocol.Params.Get(readParam);

            if (writeParam.Measurement == null)
            {
                writeParam.Measurement = new ParamsParamMeasurement();
            }

            if (writeParam.Measurement.Type == null)
            {
                writeParam.Measurement.Type = new ParamsParamMeasurementType(EnumParamMeasurementType.Matrix);
            }

            writeParam.Measurement.Type.Options = new AttributeValue<string>("matrix=InputCount,OutputCount,MinConnectedInputs,MaxConnectedInputs,MinConnectedOutputs,MaxConnectedOutputs");
        }

        public IValidationResult CheckInvalidColumnDimensionsToOutputCount()
        {
            if (String.IsNullOrWhiteSpace(ParamType.Options?.Value))
            {
                return null;
            }

            uint? typeColumns = Param.Type?.GetOptions()?.Dimensions?.Columns;
            if (typeColumns == null)
            {
                return null;
            }

            uint? measurementOutputs = Options?.Outputs;
            if (measurementOutputs == null)
            {
                return null;
            }

            if (typeColumns.Value != measurementOutputs.Value)
            {
                IValidationResult error = Error.InvalidColumnDimensionsToOutputCount(LinkToTest, Param, ParamType, Pid, Convert.ToString(measurementOutputs.Value), Convert.ToString(typeColumns.Value));
                error.WithExtraData(ExtraData.TypeColumns, typeColumns.Value);
                return error;
            }

            return null;
        }

        public IValidationResult CheckInvalidConnectedMatrixPoints()
        {
            if (String.IsNullOrWhiteSpace(ParamType.Options?.Value) || !Options.IsValid)
            {
                return null;
            }

            var typeOptions = Param.Type?.GetOptions();
            if (typeOptions == null)
            {
                return null;
            }

            uint? maxAllowedInputs = typeOptions.Dimensions?.Rows;
            uint? maxAllowedOutputs = typeOptions.Dimensions?.Columns;

            if (maxAllowedInputs == null || maxAllowedOutputs == null)
            {
                return null;
            }

            if (Options.COMin < 0 || Options.COMin > maxAllowedInputs)
            {
                return Error.InvalidConnectedMatrixPoints(LinkToTest, Param, ParamType, $"COMin {Options.COMin}", "minimum", "output", Pid);
            }

            if (Options.COMax < 1 || Options.COMax > maxAllowedInputs)
            {
                return Error.InvalidConnectedMatrixPoints(LinkToTest, Param, ParamType, $"COMax {Options.COMax}", "maximum", "output", Pid);
            }

            if (Options.COMin > Options.COMax)
            {
                return Error.InvalidConnectedMatrixPoints(LinkToTest, Param, ParamType, $"COMin {Options.COMin} and COMax {Options.COMax}", "max smaller than min", "output", Pid);
            }

            if (Options.CIMin < 0 || Options.CIMin > maxAllowedOutputs)
            {
                return Error.InvalidConnectedMatrixPoints(LinkToTest, Param, ParamType, $"CIMin {Options.CIMin}", "minimum", "input", Pid);
            }

            if (Options.CIMax < 1 || Options.CIMax > maxAllowedOutputs)
            {
                return Error.InvalidConnectedMatrixPoints(LinkToTest, Param, ParamType, $"CIMax {Options.CIMax}", "maximum", "input", Pid);
            }

            if (Options.CIMin > Options.CIMax)
            {
                return Error.InvalidConnectedMatrixPoints(LinkToTest, Param, ParamType, $"CIMin {Options.CIMin} and CIMax {Options.CIMax}", "max smaller than min", "input", Pid);
            }

            return null;
        }

        public IValidationResult CheckInvalidMatrixDimensionsToInputCount()
        {
            if (String.IsNullOrWhiteSpace(ParamType.Options?.Value))
            {
                return null;
            }

            uint? typeRows = Param.Type?.GetOptions()?.Dimensions?.Rows;
            if (typeRows == null)
            {
                return null;
            }

            uint? measurementInputs = Options?.Inputs;
            if (measurementInputs == null)
            {
                return null;
            }

            if (typeRows.Value != measurementInputs.Value)
            {
                IValidationResult error = Error.InvalidMatrixDimensionsToInputCount(LinkToTest, Param, ParamType, Pid, Convert.ToString(measurementInputs.Value), Convert.ToString(typeRows.Value));
                error.WithExtraData(ExtraData.TypeRows, typeRows.Value);
                return error;
            }

            return null;
        }

        public IValidationResult CheckInvalidMatrixOption()
        {
            if (String.IsNullOrWhiteSpace(ParamType.Options?.Value))
            {
                return null;
            }

            if (!Options.IsValid)
            {
                return Error.InvalidMatrixOption(LinkToTest, Param, ParamType, "matrix", Pid);
            }

            return null;
        }

        public IValidationResult CheckMissingAttribute()
        {
            if (ParamType.Options == null)
            {
                return Error.MissingAttribute(LinkToTest, Param, ParamType, Pid);
            }

            return null;
        }

        public IValidationResult CheckMissingMatrixOption()
        {
            if (ParamType.Options == null)
            {
                return null;
            }

            if (!ParamType.Options.Value.ToLower().Contains("matrix="))
            {
                return Error.MissingMatrixOption(LinkToTest, Param, ParamType, "matrix", Pid);
            }

            return null;
        }
    }

    internal class TableValidator : ValidateHelperBase
    {
        private readonly RelationManager relationManager;

        private readonly IParamsParam tableParam;
        private readonly MeasurementTypeOptions.TableClass tableOptions;
        private readonly IEnumerable<(uint? idx, string pid, IParamsParam columnParam)> tableColumns;

        public TableValidator(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam tableParam, MeasurementTypeOptions.TableClass tableOptions)
            : base(test, context, results)
        {
            relationManager = context.ProtocolModel.RelationManager;

            this.tableParam = tableParam;
            this.tableOptions = tableOptions;
            tableColumns = tableParam.GetColumns(relationManager, returnBaseColumnsIfDuplicateAs: true);
        }

        public void CheckColumnParams()
        {
            bool isTableDisplayed = tableParam.GetRTDisplay() || tableParam.WillBeExported();

            if (tableOptions?.Columns?.Columns == null)
            {
                return;
            }

            foreach ((uint _, uint? columnPid, uint? _) in tableOptions.Columns.Columns)
            {
                // Non Existing Param
                if (!TryGetColumn(Convert.ToString(columnPid), out var column) ||
                    column.param == null)
                {
                    // Below error message should rather be done via check on ColumnOptions (same thing regarding column param type)
                    // TODO: Instead, this check could make sure column referred by measurement are also part of ColumnOptions and that the IDX matches
                    //results.Add(Error.NonExistingId(test, tableParam, tableParam.Measurement.Type.Options, tableParam.Id.RawValue));
                    continue;
                }

                // RTDisplay Expected
                if (isTableDisplayed)
                {
                    IValidationResult rtDisplayError = Error.ReferencedParamRTDisplayExpected(test, tableParam, tableParam.Measurement.Type.Options, column.pid, tableParam.Id.RawValue);
                    context.CrossData.RtDisplay.AddParam(column.param, rtDisplayError);
                }

                // Write column
                if (!column.param.TryGetWrite(relationManager, out var writeColumnParam))
                {
                    continue;
                }

                // RTDisplay Expected for Write column
                if (isTableDisplayed)
                {
                    IValidationResult rtDisplayError = Error.ReferencedParamRTDisplayExpected(test, tableParam, tableParam.Measurement.Type.Options, writeColumnParam.Id?.RawValue, tableParam.Id.RawValue);
                    context.CrossData.RtDisplay.AddParam(writeColumnParam, rtDisplayError);
                }
            }
        }

        public bool CheckColumnsSorting()
        {
            bool isSortingOk = true;
            if (tableOptions.Sort == null)
            {
                return isSortingOk;
            }

            int columnsWithSortingCount = 0;
            int columnsWithNonZeroPriorityCount = 0;

            foreach ((uint _, SortType? _, SortDirection? direction, uint? priority) in tableOptions.Sort.Columns)
            {
                if (direction != null)
                {
                    columnsWithSortingCount++;
                }

                if (priority != null)
                {
                    columnsWithNonZeroPriorityCount++;
                }
            }

            if (columnsWithSortingCount > 1 && columnsWithSortingCount > columnsWithNonZeroPriorityCount)
            {
                results.Add(Error.MissingPriorityForSortedColumns(test, tableParam, tableParam.Measurement.Type.Options, tableParam.Id.RawValue));
                isSortingOk = false;
            }

            return isSortingOk;
        }

        public void CheckDateTimeSorting()
        {
            // Get the main sorted column if present.
            uint? mainSortedColumnPid = null;

            // Obtain date(time) columns that are shown and have width > 0.
            List<uint> displayedDateTimeColumns = new List<uint>();

            var displayedColumnsInfo = tableOptions.ColumnsFull.ToList();
            foreach (var displayedColumnInfo in displayedColumnsInfo)
            {
                // Width
                uint? width = displayedColumnInfo.Width;
                if (width > 0 &&
                    displayedColumnInfo.Pid != null &&
                    TryGetColumn(Convert.ToString(displayedColumnInfo.Pid.Value), out var column) &&
                    column.param != null && column.param.IsDateTime())
                {
                    // Get corresponding column pid.
                    displayedDateTimeColumns.Add(displayedColumnInfo.Pid.Value);
                }

                // Sort
                if (displayedColumnInfo.SortPriority == 0)
                {
                    mainSortedColumnPid = displayedColumnInfo.Pid;
                    //break;
                }
                else
                {
                    if (mainSortedColumnPid == null && displayedColumnInfo.SortDirection != null)
                    {
                        mainSortedColumnPid = displayedColumnInfo.Pid;
                    }
                }
            }

            if (displayedDateTimeColumns.Count > 0 && (mainSortedColumnPid == null || !displayedDateTimeColumns.Contains(mainSortedColumnPid.Value)))
            {
                List<IValidationResult> subResults = new List<IValidationResult>(displayedDateTimeColumns.Count);
                for (int i = 0; i < displayedDateTimeColumns.Count; i++)
                {
                    subResults.Add(Error.MissingSortingOnDateTimeColumn(test, tableParam.Measurement.Type, tableParam.Measurement.Type, tableParam.Id.RawValue, displayedDateTimeColumns[i].ToString()));
                }

                if (subResults.Count > 1)
                {
                    var missingSortingOnDateTimeColumn = Error.MissingSortingOnDateTimeColumn(test, tableParam.Measurement.Type, tableParam.Measurement.Type, tableParam.Id.RawValue, String.Join(", ", displayedDateTimeColumns))
                                                              .WithSubResults(subResults.ToArray());
                    results.Add(missingSortingOnDateTimeColumn);
                }
                else
                {
                    results.Add(subResults[0]);
                }
            }
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
    }

    internal enum ExtraData
    {
        TypeRows,
        TypeColumns
    }
}
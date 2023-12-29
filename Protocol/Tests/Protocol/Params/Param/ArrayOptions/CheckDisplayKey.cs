namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckDisplayKey
{
    using System;
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    using static Skyline.DataMiner.CICD.Models.Protocol.Read.ArrayOptionsOptions;

    [Test(CheckId.CheckDisplayKey, Category.Param)]
    internal class CheckDisplayKey : IValidate, ICodeFix, ICompare
    {
        internal const string SyntaxTitleForNaming = "ArrayOptions@options:naming";
        internal const string SyntaxTitleForNamingFormat = "ArrayOptions/NamingFormat";

        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                var arrayOptions = param.ArrayOptions;
                if (arrayOptions == null)
                {
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, param, arrayOptions);
                helper.Validate();
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.DuplicateDisplayKeyDefinition:
                    {
                        var readNode = (IParamsParam)context.Result.ReferenceNode;
                        var editNode = context.Protocol.Params.Get(readNode);

                        var arrayOptions = editNode.ArrayOptions;

                        var displayColumn = arrayOptions.DisplayColumn;
                        var namingOption = FixHelper.GetNamingOption(arrayOptions);
                        var namingFormat = arrayOptions.NamingFormat;

                        if (displayColumn != null && (namingOption != null || namingFormat != null))
                        {
                            arrayOptions.DisplayColumn = null;
                        }

                        if (namingOption != null && namingFormat != null)
                        {
                            string optionStringWithoutNamingOption = FixHelper.RemoveNamingOption(arrayOptions.Options.Value);

                            if (optionStringWithoutNamingOption.Length > 1)
                            {
                                arrayOptions.Options.Value = optionStringWithoutNamingOption;
                            }
                            else
                            {
                                arrayOptions.Options = null;
                            }
                        }

                        result.Success = true;
                        break;
                    }

                case ErrorIds.DisplayColumnSameAsPK:
                    {
                        var readNode = (IParamsParam)context.Result.ReferenceNode;
                        var editNode = context.Protocol.Params.Get(readNode);

                        editNode.ArrayOptions.DisplayColumn = null;

                        result.Success = true;
                        break;
                    }

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
                var oldArrayOptions = oldParam?.ArrayOptions;
                var newArrayOptions = newParam?.ArrayOptions;
                if (oldArrayOptions == null || newArrayOptions == null)
                {
                    continue;
                }

                string tablePid = newParam?.Id?.RawValue;

                var oldNamingOption = oldArrayOptions.GetOptions()?.Naming;
                var newNamingOption = newArrayOptions.GetOptions()?.Naming;

                string oldNamingFormat = oldArrayOptions.NamingFormat?.Value;
                string newNamingFormat = newArrayOptions.NamingFormat?.Value;

                bool oldHasNamingFormat = !String.IsNullOrWhiteSpace(oldNamingFormat);
                bool newHasNamingFormat = !String.IsNullOrWhiteSpace(newNamingFormat);

                // Removed naming & NamingFormat
                if ((oldHasNamingFormat || oldNamingOption != null) && (!newHasNamingFormat && newNamingOption == null))
                {
                    string oldNamingSyntax;
                    if (oldHasNamingFormat)
                    {
                        oldNamingSyntax = SyntaxTitleForNamingFormat;
                    }
                    else
                    {
                        oldNamingSyntax = SyntaxTitleForNaming;
                    }

                    results.Add(ErrorCompare.FormatRemoved(newArrayOptions, newArrayOptions, oldNamingSyntax, tablePid));
                    continue;
                }

                // Updated naming & NamingFormat
                if (oldHasNamingFormat || (oldNamingOption != null) && (newHasNamingFormat || newNamingOption != null))
                {
                    DisplayKey oldDisplayKey = CompareHelper.ExtractFormatSegments(oldHasNamingFormat, oldNamingFormat, oldNamingOption);
                    DisplayKey newDisplayKey = CompareHelper.ExtractFormatSegments(newHasNamingFormat, newNamingFormat, newNamingOption);

                    if (!oldDisplayKey.FormatSegments.SequenceEqual(newDisplayKey.FormatSegments))
                    {
                        results.Add(ErrorCompare.FormatChanged(newArrayOptions, newArrayOptions, oldDisplayKey.Syntax, oldDisplayKey.Format, newDisplayKey.Syntax, newDisplayKey.Format, tablePid));
                    }
                }
            }

            return results;
        }
    }

    internal class DisplayKey
    {
        public string[] FormatSegments { get; set; }

        public string Syntax { get; set; }

        public string Format { get; set; }
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        private readonly RelationManager relationManager;

        private readonly IParamsParam tableParam;
        private readonly IParamsParamArrayOptions arrayOptions;

        private readonly IValueTag<uint?> displayColumn;
        private readonly NamingClass namingOption;
        private readonly IParamsParamArrayOptionsNamingFormat namingFormat;
        private readonly IEnumerable<(uint? idx, string pid, IParamsParam columnParam)> columns;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam tableParam, IParamsParamArrayOptions arrayOptions)
            : base(test, context, results)
        {
            relationManager = context.ProtocolModel.RelationManager;

            this.tableParam = tableParam;
            this.arrayOptions = arrayOptions;

            displayColumn = arrayOptions.DisplayColumn;
            namingOption = arrayOptions.GetOptions()?.Naming;
            namingFormat = arrayOptions.NamingFormat;
            columns = tableParam.GetColumns(context.ProtocolModel.RelationManager, returnBaseColumnsIfDuplicateAs: true);
        }

        public void Validate()
        {
            CheckDisplayKeyDefinitions();
            CheckDisplayKeyColumn();
        }

        private void CheckDisplayKeyColumn()
        {
            IParamsParam displayKeyParam = null;

            var displayKeyColumnOptions = arrayOptions.Where(c => c.Type?.Value == EnumColumnOptionType.Displaykey).ToList();
            if (displayKeyColumnOptions.Any())
            {
                // ColumnOption@type="displaykey"
                if (displayKeyColumnOptions.Count > 1)
                {
                    results.Add(Error.DuplicateDisplayKeyColumn(test, tableParam, arrayOptions, tableParam.Id.RawValue));
                    return;
                }

                string displayKeyColumnPid = Convert.ToString(displayKeyColumnOptions.First().Pid?.Value);
                if (tableParam.TryGetColumn(columns, displayKeyColumnPid, out displayKeyParam))
                {
                    CheckDisplayKeyColumnParam(displayKeyParam, isAlsoPrimaryKey: false, isDisplayColumn: false);
                }
            }
            else if (namingFormat != null)
            {
                // NamingFormat
                if (String.IsNullOrWhiteSpace(namingFormat.Value))
                {
                    // Covered by check on NamingFormat tag.
                    return;
                }

                string[] namingFormatParts = namingFormat.Value.Split(new char[] { namingFormat.Value[0] }, StringSplitOptions.RemoveEmptyEntries);
                if (namingFormatParts.Length < 1)
                {
                    // Covered by check on NamingFormat tag.
                    return;
                }

                if (namingFormatParts.Length == 1)
                {
                    string displayKeyColumnPid = namingFormatParts[0];
                    if (tableParam.TryGetColumn(columns, displayKeyColumnPid, out displayKeyParam))
                    {
                        CheckDisplayKeyColumnParam(displayKeyParam, isAlsoPrimaryKey: false, isDisplayColumn: false);
                    }
                }
                else
                {
                    results.Add(Error.DisplayKeyColumnMissing(test, tableParam, arrayOptions, tableParam.Id.RawValue));
                }
            }
            else if (namingOption != null)
            {
                // Naming option
                if (namingOption.Columns == null || namingOption.Columns.Count == 0)
                {
                    // Covered by check on ArrayOptions@options attribute
                    return;
                }

                if (namingOption.Columns.Count == 1)
                {
                    string displayKeyColumnPid = Convert.ToString(namingOption.Columns.First());
                    if (tableParam.TryGetColumn(columns, displayKeyColumnPid, out displayKeyParam))
                    {
                        CheckDisplayKeyColumnParam(displayKeyParam, isAlsoPrimaryKey: false, isDisplayColumn: false);
                    }
                }
                else
                {
                    results.Add(Error.DisplayKeyColumnMissing(test, tableParam, arrayOptions, tableParam.Id.RawValue));
                }
            }
            else if (displayColumn?.Value != null)
            {
                // DisplayColumn
                if (tableParam.TryGetColumn(columns, displayColumn.Value.Value, out displayKeyParam))
                {
                    CheckDisplayKeyColumnParam(displayKeyParam, isAlsoPrimaryKey: false, isDisplayColumn: true);
                }
            }
            else
            {
                // When no displayKey is defined, the PK will be used as DK
                if (tableParam.TryGetPrimaryKeyColumn(relationManager, out displayKeyParam))
                {
                    CheckDisplayKeyColumnParam(displayKeyParam, isAlsoPrimaryKey: true, isDisplayColumn: false);
                }
            }

            CheckForExcessiveIdxSuffixes(displayKeyParam);
        }

        private void CheckDisplayKeyColumnParam(IParamsParam displayKeyParam, bool isAlsoPrimaryKey, bool isDisplayColumn)
        {
            // Sanity Checks
            if (displayKeyParam == null)
            {
                return;
            }

            // Checks that might already be covered by the PK checks
            if (!isAlsoPrimaryKey)
            {
                // PK Param/Type
                var paramType = displayKeyParam.Type;
                if (paramType == null)
                {
                    // Covered by Param/Type check
                    ////results.Add(Error.DisplayKeyColumnInvalidType(test, displayKeyParam, displayKeyParam, "UNDEFINED", displayKeyParam.Id.RawValue));
                }
                else if (paramType.Value == EnumParamType.Write
                    || paramType.Value == EnumParamType.Group
                    || paramType.Value == EnumParamType.ReadBit
                    || paramType.Value == EnumParamType.WriteBit)
                {
                    // PK Param/Type Invalid (Only check for types that are valid for normal columns but not for PKs, other ones are covered by Param.CheckColumns)
                    results.Add(Error.DisplayKeyColumnInvalidType(test, displayKeyParam, paramType, paramType.RawValue, displayKeyParam.Id.RawValue));
                }

                // Below checks are only needed when using the old ArrayOptions@displayColumn attribute.
                if (isDisplayColumn)
                {
                    // PK Interprete/Type
                    var interpretType = displayKeyParam.Interprete?.Type;
                    if (interpretType == null)
                    {
                        results.Add(Error.DisplayKeyColumnInvalidInterpreteType(test, displayKeyParam, displayKeyParam, "UNDEFINED", displayKeyParam.Id.RawValue));
                    }
                    else if (interpretType.Value.HasValue
                        && interpretType.Value != EnumParamInterpretType.String)
                    {
                        results.Add(Error.DisplayKeyColumnInvalidInterpreteType(test, displayKeyParam, interpretType, interpretType.RawValue, displayKeyParam.Id.RawValue));
                    }

                    // PK Measurement/Type
                    if (tableParam.GetRTDisplay() || tableParam.WillBeExported())
                    {
                        var measurementType = displayKeyParam.Measurement?.Type;
                        if (measurementType == null)
                        {
                            results.Add(Error.DisplayKeyColumnInvalidMeasurementType(test, displayKeyParam, displayKeyParam, "UNDEFINED", displayKeyParam.Id.RawValue));
                        }
                        else if (measurementType.Value.HasValue
                            && measurementType.Value != EnumParamMeasurementType.String
                            && measurementType.Value != EnumParamMeasurementType.Number)
                        {
                            results.Add(Error.DisplayKeyColumnInvalidMeasurementType(test, displayKeyParam, measurementType, measurementType.RawValue, displayKeyParam.Id.RawValue));
                        }
                    }
                }
            }

            // Checks not covered by the PK checks
        }

        private void CheckDisplayKeyDefinitions()
        {
            // Multiple Definitions
            if (!CheckDuplicateDisplayKeyDefinitions())
            {
                return;
            }

            // Redundant displayColumn
            var index = arrayOptions.Index;
            if (!CheckDisplayColumnIsSameAsPK(index))
            {
                return;
            }

            // Unrecommended displayColumn
            if (namingOption == null && namingFormat == null)
            {
                CheckUnrecommendedUseOfDisplayColumn();
            }
        }

        private bool CheckDisplayColumnIsSameAsPK(IValueTag<uint?> index)
        {
            if (displayColumn == null || index == null || displayColumn.Value != index.Value)
            {
                return true;
            }

            results.Add(Error.DisplayColumnSameAsPK(test, tableParam, tableParam.ArrayOptions, tableParam.Id?.RawValue));
            return false;
        }

        private bool CheckDuplicateDisplayKeyDefinitions()
        {
            if ((displayColumn == null || namingOption == null) && (displayColumn == null || namingFormat == null) &&
                (namingOption == null || namingFormat == null))
            {
                return true;
            }

            results.Add(Error.DuplicateDisplayKeyDefinition(test, tableParam, tableParam.ArrayOptions, tableParam.Id?.RawValue));
            return false;
        }

        private void CheckForExcessiveIdxSuffixes(IParamsParam displayKeyParam)
        {
            foreach (var columnParam in columns.Select(c => c.columnParam))
            {
                if (columnParam == null)
                {
                    // NonExistingColumn handled by checks on Param/Type@id and ColumnOption@pid
                    continue;
                }

                if (columnParam == displayKeyParam)
                {
                    // [IDX] suffix allowed on the displayKeyParam
                    continue;
                }

                var columnDescription = columnParam.Description?.Value;
                if (!String.IsNullOrEmpty(columnDescription) && columnDescription.IndexOf("[IDX]", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    results.Add(Error.UnexpectedIdxSuffix(test, columnParam, columnParam.Description, columnParam.Id?.RawValue));
                }
            }
        }

        private void CheckUnrecommendedUseOfDisplayColumn()
        {
            if (displayColumn == null)
            {
                return;
            }

            results.Add(Error.DisplayColumnUnrecommended(test, tableParam, tableParam.ArrayOptions, tableParam.Id?.RawValue, false));
        }
    }

    internal static class CompareHelper
    {
        public static DisplayKey ExtractFormatSegments(bool hasNamingFormat, string namingFormat, NamingClass namingOption)
        {
            DisplayKey displayKey;
            if (hasNamingFormat)
            {
                displayKey = ExtractNamingFormatSegments(namingFormat);
            }
            else
            {
                displayKey = ExtractNamingFormatSegments(namingOption);
            }

            return displayKey;
        }

        private static DisplayKey ExtractNamingFormatSegments(string namingFormat)
        {
            char separator = namingFormat[0];
            string[] formatSegments = namingFormat.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            DisplayKey displayKey = new DisplayKey
            {
                FormatSegments = formatSegments,
                Syntax = CheckDisplayKey.SyntaxTitleForNamingFormat,
                Format = namingFormat,
            };

            return displayKey;
        }

        private static DisplayKey ExtractNamingFormatSegments(NamingClass namingOption)
        {
            DisplayKey displayKey = new DisplayKey
            {
                Syntax = CheckDisplayKey.SyntaxTitleForNaming,
                Format = namingOption.OriginalValue.Replace("naming=", null),
            };

            if (namingOption.Columns == null || namingOption.Columns.Count <= 0)
            {
                displayKey.FormatSegments = Array.Empty<string>();
                return displayKey;
            }

            string[] formatSegments = new string[namingOption.Columns.Count * 2 - 1];
            for (int i = 0; i < namingOption.Columns.Count - 1; i++)
            {
                formatSegments[i * 2] = Convert.ToString(namingOption.Columns.ElementAt(i));
                formatSegments[i * 2 + 1] = Convert.ToString(namingOption.Separator);
            }

            formatSegments[formatSegments.Length - 1] = Convert.ToString(namingOption.Columns.ElementAt(namingOption.Columns.Count - 1));

            displayKey.FormatSegments = formatSegments;
            return displayKey;
        }
    }

    internal static class FixHelper
    {
        public static string GetNamingOption(Skyline.DataMiner.CICD.Models.Protocol.Edit.ParamsParamArrayOptions arrayOptions)
        {
            string options = arrayOptions?.Options?.Value;

            if (String.IsNullOrEmpty(options))
            {
                return null;
            }

            string naming = null;
            char firstChar = options[0];

            // Defaulting to ';' if first character is not a letter.
            // Did a search and found 191 drivers (20/12/2018) that have options="naming=... 
            if (Char.IsLetter(firstChar))
            {
                firstChar = ';';
            }

            string[] splitOptions = options.Split(firstChar);
            for (int i = 0; i < splitOptions.Length; i++)
            {
                string thisOption = splitOptions[i];
                string[] namingSplit = thisOption.Split('=');
                if (String.Equals(namingSplit[0], "naming", StringComparison.OrdinalIgnoreCase))
                {
                    naming = thisOption;
                    break;
                }
            }

            return naming;
        }

        public static string RemoveNamingOption(string options)
        {
            char firstChar = ';';

            if (String.IsNullOrEmpty(options))
            {
                return String.Empty;
            }

            // Defaulting to ';' if first character is not a letter.
            if (!Char.IsLetter(options[0]))
            {
                firstChar = options[0];
            }

            string[] splitOptions = options.Split(firstChar);

            List<string> remainingOptions = new List<string>(splitOptions.Length - 1);

            for (int i = 0; i < splitOptions.Length; i++)
            {
                string thisOption = splitOptions[i];
                string[] namingSplit = thisOption.Split('=');
                if (!String.Equals(namingSplit[0], "naming", StringComparison.OrdinalIgnoreCase))
                {
                    remainingOptions.Add(thisOption);
                }
            }

            return String.Join(firstChar.ToString(), remainingOptions);
        }
    }
}
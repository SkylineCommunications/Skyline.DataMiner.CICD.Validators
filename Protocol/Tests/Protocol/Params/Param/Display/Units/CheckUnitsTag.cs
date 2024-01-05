namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Units.CheckUnitsTag
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckUnitsTag, Category.Param)]
    internal class CheckUnitsTag : IValidate, ICodeFix
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var model = context.ProtocolModel;
            ValidateHelper helper = new ValidateHelper(this, context.ValidatorSettings, results);
            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                var display = param.Display;
                if (display == null)
                {
                    continue;
                }

                var units = display.Units;
                (GenericStatus status, string _, string _) = GenericTests.CheckBasics(units, isRequired: true);

                // Missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    if (param.IsPositioned(model.RelationManager))
                    {
                        if (param.TryGetTable(model.RelationManager, out var tableParam))
                        {
                            // Index Column
                            if (tableParam.TryGetPrimaryKeyColumn(model.RelationManager, out var indexColumn) &&
                                indexColumn == param)
                            {
                                continue;
                            }

                            // Normal column, so don't skip it.
                        }
                    }
                    else if (param.IsWrite() &&
                        param.TryGetRead(model.RelationManager, out IParamsParam readParameter) &&
                        readParameter.IsPositioned(model.RelationManager))
                    {
                        // In case of a write parameter of a column
                    }
                    else
                    {
                        continue;
                    }

                    // Check the Measurement Type
                    if (param.IsNumber())
                    {
                        if (!param.IsDateTime() && !param.IsTime())
                        {
                            // Normal Number Parameter
                            //results.Add(Error.MissingTag(this, param, display, 
                            results.Add(Error.MissingTag(this, param, display, param.Measurement?.Type?.ReadNode.InnerText, param.Id.RawValue));
                        }
                    }
                    else if (param.IsAnalog() || param.IsProgress())
                    {
                        results.Add(Error.MissingTag(this, param, display, param.Measurement?.Type?.ReadNode.InnerText, param.Id.RawValue));
                    }

                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyTag(this, param, units, param.Id.RawValue));
                    continue;
                }

                // Unsupported with Measurement Type
                if (helper.HasUnsupportedMeasurementCombo(param))
                {
                    continue;
                }

                // Invalid
                if (!helper.IsValidUnit(param, units))
                {
                    continue;
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedTag(this, param, units, param.Id.RawValue, units.RawValue));
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.OutdatedValue:
                    {
                        var readParam = (IParamsParam)context.Result.ReferenceNode;
                        if (readParam == null)
                        {
                            result.Message = "'readParam' is null.";
                            break;
                        }

                        var editParam = context.Protocol?.Params?.Get(readParam);
                        if (editParam == null)
                        {
                            result.Message = "'editParam' is null.";
                            break;
                        }

                        editParam.Display.Units = Convert.ToString(context.Result.ExtraData[ExtraData.ExpectedUnit]);
                        result.Success = true;

                        break;
                    }

                case ErrorIds.UntrimmedTag:
                    {
                        var readParam = (IParamsParam)context.Result.ReferenceNode;
                        if (readParam == null)
                        {
                            result.Message = "'readParam' is null.";
                            break;
                        }

                        var editParam = context.Protocol?.Params?.Get(readParam);
                        if (editParam == null)
                        {
                            result.Message = "'editParam' is null.";
                            break;
                        }

                        editParam.Display.Units = readParam.Display.Units.Value.Trim();
                        result.Success = true;

                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }
    }

    internal enum ExtraData
    {
        ExpectedUnit
    }

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorSettings contextValidatorSettings;
        private readonly List<IValidationResult> results;

        public ValidateHelper(IValidate test, ValidatorSettings contextValidatorSettings, List<IValidationResult> results)
        {
            this.test = test;
            this.contextValidatorSettings = contextValidatorSettings;
            this.results = results;
        }

        public bool IsValidUnit(IParamsParam param, IValueTag<string> displayUnit)
        {
            foreach (var uom in contextValidatorSettings.UnitList.Units)
            {
                if (uom.Value == displayUnit.Value)
                {
                    return true;
                }

                if (uom.LegacyNotations.Contains(displayUnit.Value))
                {
                    IValidationResult outdatedValue = Error.OutdatedValue(test, param, displayUnit, displayUnit.Value, uom.Value, param.Id?.RawValue);
                    outdatedValue.WithExtraData(ExtraData.ExpectedUnit, uom.Value);
                    results.Add(outdatedValue);
                    return false;
                }
            }

            results.Add(Error.InvalidTag(test, param, displayUnit, displayUnit.Value, param.Id?.RawValue));
            return false;
        }

        public bool HasUnsupportedMeasurementCombo(IParamsParam param)
        {
            if (param == null)
            {
                return false;
            }

            var measurementType = param.Measurement?.Type;
            if (measurementType?.Value == null)
            {
                results.Add(Error.ExcessiveTag(test, param, param.Display.Units, "Units", "missing Measurement tag", param.Id?.RawValue));
                return true;
            }

            if (param.IsNumber() || param.IsAnalog() || param.IsProgress())
            {
                return false;
            }

            results.Add(Error.UnsupportedTag(test, param, param.Display.Units, measurementType.RawValue, param.Id?.RawValue));
            return true;
        }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.Discreet.Display.CheckDisplayTag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckDisplayTag, Category.Param)]
    internal class CheckDisplayTag : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();
            List<IValidationResult> casingResults = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.Measurement?.Discreets == null)
                {
                    continue;
                }

                foreach (var discreet in param.Measurement.Discreets)
                {
                    var display = discreet.Display;
                    (GenericStatus status, string rawDisplayValue, string displayValue) = GenericTests.CheckBasics(display, isRequired: true);

                    // Missing
                    if (status.HasFlag(GenericStatus.Missing))
                    {
                        results.Add(Error.MissingTag(this, param, discreet, param.Id.RawValue));
                        continue;
                    }

                    // Empty
                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyTag(this, param, display, param.Id.RawValue));
                        continue;
                    }

                    // Ellipsis on PageButtons
                    if (param.IsPageButton())
                    {
                        string convertedCaption = FixHelper.FixPagebuttonCaption(displayValue);

                        if (!String.Equals(convertedCaption, displayValue))
                        {
                            IValidationResult invalidPagebuttonCaption = Error.InvalidPagebuttonCaption(this, param, display, rawDisplayValue, convertedCaption, param.Id.RawValue);
                            invalidPagebuttonCaption.WithExtraData(ExtraData.DiscreetToFix, discreet);
                            results.Add(invalidPagebuttonCaption);
                            continue;
                        }
                    }

                    // Title Casing
                    if (!param.IsContextMenu() && !context.Helpers.TitleCasing.IsTitleCase(displayValue, out string expectedDisplayValue))
                    {
                        IValidationResult wrongCasingSub = Error.WrongCasing_Sub(this, param, display, rawDisplayValue, expectedDisplayValue, param.Id.RawValue);
                        wrongCasingSub.WithExtraData(ExtraData.DiscreetToFix, discreet)
                                      .WithExtraData(ExtraData.ExpectedValue, expectedDisplayValue);
                        casingResults.Add(wrongCasingSub);
                    }

                    // Untrimmed
                    if (status.HasFlag(GenericStatus.Untrimmed))
                    {
                        IValidationResult untrimmedTag = Error.UntrimmedTag(this, param, display, param.Id.RawValue, rawDisplayValue);
                        untrimmedTag.WithExtraData(ExtraData.DiscreetToFix, discreet);
                        results.Add(untrimmedTag);
                    }
                }

                var resultsForDuplicates = GenericTests.CheckDuplicates(
                    items: param.Measurement.Discreets.Where(d => !String.IsNullOrEmpty(d.Display?.RawValue)).Select(d => d.Display),
                    getDuplicationIdentifier: d => d.Value,
                    generateSubResult: x => Error.DuplicatedValue(this, x.item, x.item, x.item.RawValue, param.Id?.RawValue),
                    generateSummaryResult: x => Error.DuplicatedValue(this, param.Measurement.Discreets, param.Measurement.Discreets, x.duplicateValue, param.Id?.RawValue).WithSubResults(x.subResults)
                    );

                results.AddRange(resultsForDuplicates);
            }

            if (casingResults.Count > 0)
            {
                IValidationResult wrongCasing = Error.WrongCasing(this, null, null).WithSubResults(casingResults.ToArray());
                results.Add(wrongCasing);
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (!(context.Result.ReferenceNode is IParamsParam readParam))
            {
                result.Message = "context.Result.Node is not of type IParamsParam";
                return result;
            }

            var editParam = context.Protocol?.Params?.Get(readParam);
            if (editParam == null)
            {
                result.Message = "editParam is null.";
                return result;
            }

            if (!context.Result.ExtraData.TryGetValue(ExtraData.DiscreetToFix, out object o)
                || !(o is IParamsParamMeasurementDiscreetsDiscreet readDiscreet))
            {
                result.Message = "ExtraData[discreetToFix] is not of type IParamsParamMeasurementDiscreetsDiscreet";
                return result;
            }

            var editDiscreet = editParam.Measurement?.Discreets?.Get(readDiscreet);
            if (editDiscreet == null)
            {
                result.Message = "editDiscreet is null.";
                return result;
            }

            if (editDiscreet.Display == null)
            {
                result.Message = "editDiscreet.Display is null.";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.InvalidPagebuttonCaption:
                    {
                        editDiscreet.Display.Value = FixHelper.FixPagebuttonCaption(readDiscreet.Display.Value);
                        result.Success = true;
                        break;
                    }

                case ErrorIds.UntrimmedTag:
                    {
                        editDiscreet.Display.Value = readDiscreet.Display.Value.Trim();
                        result.Success = true;
                        break;
                    }

                case ErrorIds.WrongCasing_Sub:
                    {
                        editDiscreet.Display.Value = Convert.ToString(context.Result.ExtraData[ExtraData.ExpectedValue]);
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
                var discreets = oldParam?.Measurement?.Discreets;
                if (discreets == null)
                {
                    continue;
                }

                foreach (var previousDiscreet in oldParam.Measurement.Discreets)
                {
                    IValidationResult result = CompareHelper.CompareDiscreetDisplayChange(newParam, previousDiscreet);
                    if (result == null)
                    {
                        continue;
                    }

                    results.Add(result);
                }
            }

            return results;
        }
    }

    internal static class CompareHelper
    {
        public static IValidationResult CompareDiscreetDisplayChange(IParamsParam newParam, IParamsParamMeasurementDiscreetsDiscreet oldDiscreet)
        {
            var newDiscreetsByValue = newParam.Measurement?.Discreets?.Where(p => p.ValueElement?.Value == oldDiscreet.ValueElement?.Value);
            if (newDiscreetsByValue == null)
            {
                return null;
            }

            IValidationResult invalidResult = null;
            foreach (var newDiscreet in newDiscreetsByValue)
            {
                string simplifiedOldDisplay = oldDiscreet.Display?.Value?.Replace(" ", null).Trim('.');
                string simplifiedNewDisplay = newDiscreet.Display?.Value?.Replace(" ", null).Trim('.');

                if (String.Equals(simplifiedNewDisplay, simplifiedOldDisplay, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string oldDisplay = oldDiscreet.Display?.Value;
                string paramId = newParam.Id?.RawValue;
                string oldValue = oldDiscreet.ValueElement?.Value;
                string newDisplay = newDiscreet.Display?.Value;
                invalidResult = ErrorCompare.UpdatedValue(newDiscreet, newDiscreet, oldValue, paramId, oldDisplay, newDisplay);
            }

            return invalidResult;
        }
    }

    internal static class FixHelper
    {
        public static string FixPagebuttonCaption(string name)
        {
            return $"{name.Replace("...", null).Trim()}...";
        }
    }

    internal enum ExtraData
    {
        DiscreetToFix,
        ExpectedValue
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.Discreet.CheckOptionsAttribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Edit;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckOptionsAttribute, Category.Param)]
    internal class CheckOptionsAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            ValidateHelper validateHelper = new ValidateHelper(this, results);

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                var discreets = param?.Measurement?.Discreets;
                if (discreets == null || !discreets.Any())
                {
                    continue;
                }

                validateHelper.CheckContextMenuOptions(param, discreets);
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (!FixHelper.FindEditDiscreet(context, result, out ParamsParamMeasurementDiscreetsDiscreet editDiscreet))
            {
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedConfirmOption:
                    {
                        string[] options = editDiscreet.Options.Value.Split(';');
                        for (int i = 0; i < options.Length; i++)
                        {
                            string optionLowerCase = options[i].ToLower();
                            if (optionLowerCase.StartsWith("confirm:"))
                            {
                                string confirmCaption = options[i].Substring(options[i].IndexOf(':') + 1);
                                options[i] = $"confirm:{confirmCaption.Trim()}";
                            }
                        }

                        editDiscreet.Options.Value = String.Join(";", options);
                        result.Success = true;
                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }

    internal enum ExtraData
    {
        DiscreetToFix,
    }

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly List<IValidationResult> results;

        internal ValidateHelper(IValidate test, List<IValidationResult> results)
        {
            this.test = test;
            this.results = results;
        }

        public void CheckContextMenuOptions(IParamsParam param, IParamsParamMeasurementDiscreets discreets)
        {
            if (!param.IsContextMenu())
            {
                return;
            }

            CheckContextMenuConfirm(param, discreets);
        }

        private void CheckContextMenuConfirm(IParamsParam param, IParamsParamMeasurementDiscreets discreets)
        {
            List<IValidationResult> subResults = new List<IValidationResult>();
            foreach (IParamsParamMeasurementDiscreetsDiscreet discreet in discreets)
            {
                if (discreet?.Display?.Value == null)
                {
                    continue;
                }

                CheckContextMenuConfirmOnDiscreet(param, discreet, subResults);
            }

            if (subResults.Any())
            {
                results.Add(Error.MisconfiguredConfirmOptions(test, param, param, param.Id.RawValue).WithSubResults(subResults.ToArray()));
            }
        }

        private void CheckContextMenuConfirmOnDiscreet(IParamsParam param, IParamsParamMeasurementDiscreetsDiscreet discreet, List<IValidationResult> subResults)
        {
            string displayValue = discreet.Display.Value;
            var confirmOption = discreet.GetOptions()?.Confirm;

            if (confirmOption == null)
            {
                if (Helper.IsCriticalActionCaption(displayValue))
                {
                    subResults.Add(Error.MissingConfirmOption(test, param, discreet.Display, displayValue, param.Id.RawValue));
                }

                return;
            }

            if (String.IsNullOrWhiteSpace(confirmOption.Message))
            {
                subResults.Add(Error.EmptyConfirmOption(test, param, discreet, displayValue, param.Id.RawValue));
            }

            if (Helper.IsUntrimmed(confirmOption.Message))
            {
                IValidationResult untrimmedConfirmOption = Error.UntrimmedConfirmOption(test, param, discreet, displayValue, param.Id.RawValue, confirmOption.Message);
                untrimmedConfirmOption.WithExtraData(ExtraData.DiscreetToFix, discreet);
                subResults.Add(untrimmedConfirmOption);
            }
        }
    }

    internal static class FixHelper
    {
        public static bool FindEditDiscreet(CodeFixContext context, CodeFixResult result, out ParamsParamMeasurementDiscreetsDiscreet editDiscreet)
        {
            editDiscreet = null;

            if (!(context.Result.ReferenceNode is IParamsParam readParam))
            {
                result.Message = "context.Result.Node is not of type IParamsParam";
                return false;
            }

            var editParam = context.Protocol?.Params?.Get(readParam);
            if (editParam == null)
            {
                result.Message = "editParam is null.";
                return false;
            }

            if (!context.Result.ExtraData.TryGetValue(ExtraData.DiscreetToFix, out object o) || !(o is IParamsParamMeasurementDiscreetsDiscreet readDiscreet))
            {
                result.Message = "ExtraData[discreetToFix] is not of type IParamsParamMeasurementDiscreetsDiscreet";
                return false;
            }

            editDiscreet = editParam.Measurement?.Discreets?.Get(readDiscreet);
            if (editDiscreet == null)
            {
                result.Message = "editDiscreet is null.";
                return false;
            }

            return true;
        }
    }
}
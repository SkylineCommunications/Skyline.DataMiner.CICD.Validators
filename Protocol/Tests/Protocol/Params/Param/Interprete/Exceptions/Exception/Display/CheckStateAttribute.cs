namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Exceptions.Exception.Display.CheckStateAttribute
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.CICD.Models.Protocol.Edit;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckStateAttribute, Category.Param)]
    internal class CheckStateAttribute : IValidate, ICodeFix // , ICompare
    {
        // Please comment out the interfaces that aren't used together with the respective methods.

        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.Interprete?.Exceptions == null)
                {
                    continue;
                }

                foreach (var exception in param.Interprete.Exceptions)
                {
                    if (exception.Display == null)
                    {
                        continue;
                    }

                    var state = exception.Display.State;
                    (GenericStatus status, string rawDisplayValue, var stateValue) = GenericTests.CheckBasics(state, isRequired: true);

                    // Missing
                    if (status.HasFlag(GenericStatus.Missing))
                    {
                        results.Add(Error.MissingAttribute(this, param, exception.Display, param.Id?.RawValue).WithExtraData(ExtraData.ExceptionToFix, exception));
                        continue;
                    }

                    // Empty
                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyAttribute(this, param, exception.Display.State, param.Id?.RawValue).WithExtraData(ExtraData.ExceptionToFix, exception));
                        continue;
                    }

                    // Invalid
                    if (status.HasFlag(GenericStatus.Invalid))
                    {
                        results.Add(Error.InvalidAttributeValue(this, param, exception.Display.State, rawDisplayValue, param.Id?.RawValue).WithExtraData(ExtraData.ExceptionToFix, exception));
                        continue;
                    }

                    if (stateValue == EnumDisplayState.Enabled)
                    {
                        results.Add(Error.UnrecommendedEnabledValue(this, param, exception.Display.State, param.Id?.RawValue).WithExtraData(ExtraData.ExceptionToFix, exception));
                        continue;
                    }

                    // Untrimmed
                    if (status.HasFlag(GenericStatus.Untrimmed))
                    {
                        results.Add(Error.UntrimmedAttributeValue(this, param, exception.Display.State, param.Id?.RawValue, rawDisplayValue).WithExtraData(ExtraData.ExceptionToFix, exception));
                        continue;
                    }
                }
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

            if (!context.Result.ExtraData.TryGetValue(ExtraData.ExceptionToFix, out object o)
                || !(o is IParamsParamInterpreteExceptionsException readException))
            {
                result.Message = "ExtraData[exceptionToFix] is not of type IParamsParamInterpreteExceptionsException";
                return result;
            }

            var editException = editParam.Interprete?.Exceptions?.Get(readException);
            if (editException == null)
            {
                result.Message = "editException is null.";
                return result;
            }

            if (editException.Display == null)
            {
                result.Message = "editException.Display is null.";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.EmptyAttribute:
                case ErrorIds.UnrecommendedEnabledValue:
                case ErrorIds.InvalidAttributeValue:
                    editException.Display.State.Value = EnumDisplayState.Disabled;
                    result.Success = true;
                    break;

                case ErrorIds.MissingAttribute:
                    editException.Display.State = new ParamsParamInterpreteExceptionsExceptionDisplayState(EnumDisplayState.Disabled);
                    result.Success = true;
                    break;

                case ErrorIds.UntrimmedAttributeValue:
                    Enum.TryParse<EnumDisplayState>(editException.Display.State.RawValue.Trim() == "disabled" ? "Disabled" : "Enabled", out EnumDisplayState parsedValue);
                    editException.Display.State.Value = parsedValue;
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

            return results;
        }
    }

    internal enum ExtraData
    {
        ExceptionToFix,
    }
}
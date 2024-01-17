namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Range.Low.CheckLowTag
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckLowTag, Category.Param)]
    internal class CheckLowTag : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.Display?.Range == null)
                {
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, param);
                helper.Validate();
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedValue:
                    {
                        var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
                        var paramEditNode = context.Protocol.Params.Get(paramReadNode);

                        paramEditNode.Display.Range.Low.Value = paramReadNode.Display.Range.Low.Value;
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
                results.AddIfNotNull(CompareHelper.CheckAddedOrIncreased(oldParam, newParam));
            }

            return results;
        }
    }

    internal static class CompareHelper
    {
        public static IValidationResult CheckAddedOrIncreased(IParamsParam oldParam, IParamsParam newParam)
        {
            string pid = newParam.Id?.RawValue;

            bool? oldParamVisible = oldParam?.Display?.RTDisplay?.Value;

            if (oldParamVisible == null || !oldParamVisible.Value)
            {
                return null;
            }

            var newLow = newParam.Display?.Range?.Low;

            if (newLow?.Value == null)
            {
                return null;
            }

            var oldLow = oldParam.Display?.Range?.Low;

            if (oldLow?.Value == null)
            {
                return ErrorCompare.AddedLowRange(newLow, newLow, newLow.RawValue, pid);
            }

            if (oldLow.Value < newLow.Value)
            {
                return ErrorCompare.UpdatedLowRange(newLow, newLow, oldLow.RawValue, pid, newLow.RawValue);
            }

            return null;
        }
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        private readonly RelationManager relationManager;
        private readonly IParamsParam param;

        private readonly IParamsParamDisplay display;
        private readonly IValueTag<decimal?> low;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param)
            : base(test, context, results)
        {
            relationManager = context.ProtocolModel.RelationManager;
            this.param = param;

            display = param.Display;
            low = display.Range.Low;
        }

        public void Validate()
        {
            (GenericStatus status, _, _) = GenericTests.CheckBasics(low, isRequired: false);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyTag(test, param, low, param.Id.RawValue));
                return;
            }

            // Invalid
            if (status.HasFlag(GenericStatus.Invalid))
            {
                results.Add(Error.InvalidValue(test, param, low, low.RawValue, param.Id.RawValue));
                return;
            }

            // LowerOrEqualToZero
            if (display.Trending?.Logarithmic?.Value == true && (low == null || low.Value <= 0))
            {
                results.Add(Error.LogarithmicLowerOrEqualToZero(test, param, low, low?.RawValue, param.Id.RawValue));
                return;
            }

            if (low == null)
            {
                return;
            }

            // Write vs Read
            if (!CheckWriteVsRead())
            {
                return;
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedValue(test, param, low, param.Id.RawValue, low.RawValue));
                return;
            }
        }

        private bool CheckWriteVsRead()
        {
            if (!param.IsWrite())
            {
                // Only checking on write param, otherwise we would end up with duplicate results.
                return true;
            }

            if (!param.TryGetRead(relationManager, out var readParam))
            {
                // No read param found.
                return true;
            }

            var readParamRange = readParam.Display?.Range;
            if (low.Value == readParamRange?.Low?.Value)
            {
                // Matching values
                return true;
            }

            results.Add(Error.WriteDifferentThanRead(test, param, low, low.RawValue, readParamRange?.Low?.RawValue, param.Id.RawValue));
            return false;
        }
    }
}
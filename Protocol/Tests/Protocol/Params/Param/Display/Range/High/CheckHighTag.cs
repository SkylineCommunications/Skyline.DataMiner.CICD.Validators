namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Range.High.CheckHighTag
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

    [Test(CheckId.CheckHighTag, Category.Param)]
    internal class CheckHighTag : IValidate, ICodeFix, ICompare
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

                        paramEditNode.Display.Range.High.Value = paramReadNode.Display.Range.High.Value;
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
                results.AddIfNotNull(CompareHelper.CheckAddedOrDecreased(oldParam, newParam));
            }

            return results;
        }
    }

    internal static class CompareHelper
    {
        public static IValidationResult CheckAddedOrDecreased(IParamsParam oldParam, IParamsParam newParam)
        {
            string newPid = newParam.Id?.RawValue;

            bool? oldParamVisible = oldParam?.Display?.RTDisplay?.Value;

            if (oldParamVisible == null || !oldParamVisible.Value)
            {
                return null;
            }

            var newHigh = newParam.Display?.Range?.High;

            if (newHigh?.Value == null)
            {
                return null;
            }

            var oldHigh = oldParam.Display?.Range?.High;

            if (oldHigh?.Value == null)
            {
                return ErrorCompare.AddedHighRange(newHigh, newHigh, newHigh.RawValue, newPid);
            }

            if (newHigh.Value < oldHigh.Value)
            {
                return ErrorCompare.UpdatedHighRange(newHigh, newHigh, oldHigh.RawValue, newPid, newHigh.RawValue);
            }

            return null;
        }
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        private readonly RelationManager relationManager;
        private readonly IParamsParam param;

        private readonly IParamsParamDisplay display;
        private readonly IValueTag<decimal?> high;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param)
            : base(test, context, results)
        {
            relationManager = context.ProtocolModel.RelationManager;
            this.param = param;

            display = param.Display;
            high = display.Range.High;
        }

        public void Validate()
        {
            (GenericStatus status, _, _) = GenericTests.CheckBasics(high, isRequired: false);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyTag(test, param, high, param.Id.RawValue));
                return;
            }

            // Invalid
            if (status.HasFlag(GenericStatus.Invalid))
            {
                results.Add(Error.InvalidValue(test, param, high, high.RawValue, param.Id.RawValue));
                return;
            }

            // LowerOrEqualToZero
            if (display.Trending?.Logarithmic?.Value == true && (high == null || high.Value <= 0))
            {
                results.Add(Error.LogarithmicLowerOrEqualToZero(test, param, high, high?.RawValue, param.Id.RawValue));
                return;
            }

            if (high == null)
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
                results.Add(Error.UntrimmedValue(test, param, high, param.Id.RawValue, high.RawValue));
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
            if (high.Value == readParamRange?.High?.Value)
            {
                // Matching values
                return true;
            }

            results.Add(Error.WriteDifferentThanRead(test, param, high, high.RawValue, readParamRange?.High?.RawValue, param.Id.RawValue));
            return false;
        }
    }
}
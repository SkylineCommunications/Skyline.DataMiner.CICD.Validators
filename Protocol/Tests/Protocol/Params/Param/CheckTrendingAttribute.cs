namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.CheckTrendingAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckTrendingAttribute, Category.Param)]
    internal class CheckTrendingAttribute : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var param in context.EachParamWithValidId())
            {
                var trending = param.Trending;
                if (trending == null)
                {
                    continue;
                }

                (GenericStatus status, string trendingRawValue, bool? trendingValue) = GenericTests.CheckBasics(trending, isRequired: false);

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, param, trending, param.Id.RawValue));
                    continue;
                }

                if (trendingValue == true)
                {
                    // RTDisplay Expected
                    IValidationResult rtDisplayError = Error.RTDisplayExpected(this, param, trending, param.Id.RawValue);
                    context.CrossData.RtDisplay.AddParam(param, rtDisplayError);
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, param, trending, param.Id.RawValue, trendingRawValue));
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol?.Params == null)
            {
                result.Message = "No Param found";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    {
                        var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
                        var paramEditNode = context.Protocol.Params.Get(paramReadNode);

                        paramEditNode.Trending.Value = paramReadNode.Trending.Value;
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
                string newPid = newParam.Id?.RawValue;

                if (!newParam.GetRTDisplay())
                {
                    continue;
                }

                bool oldTrending = oldParam.Trending?.Value ?? true;
                bool newTrending = newParam.Trending?.Value ?? true;
                if (oldTrending && !newTrending)
                {
                    results.Add(ErrorCompare.DisabledTrending(newParam, newParam, newPid));
                }
            }

            return results;
        }
    }
}
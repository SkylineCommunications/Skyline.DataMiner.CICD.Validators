namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Trending.Type.CheckTypeTag
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckTypeTag, Category.Param)]
    internal class CheckTypeTag : /*IValidate, ICodeFix, */ICompare
    {
        ////public List<IValidationResult> Validate(ValidatorContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {
        ////        default:
        ////            result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
        ////            break;
        ////    }

        ////    return result;
        ////}

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                string newPid = newParam?.Id?.RawValue;

                EnumTrendingType oldTrendingType = oldParam?.Display?.Trending?.Type?.Value ?? Skyline.DataMiner.CICD.Models.Protocol.Enums.EnumTrendingType.Average;
                EnumTrendingType newTrendingType = newParam?.Display?.Trending?.Type?.Value ?? Skyline.DataMiner.CICD.Models.Protocol.Enums.EnumTrendingType.Average;

                if (oldTrendingType != newTrendingType)
                {
                    results.Add(ErrorCompare.UpdatedTrendType(newParam, newParam, Convert.ToString(oldTrendingType), newPid, Convert.ToString(newTrendingType)));
                }
            }

            return results;
        }
    }
}
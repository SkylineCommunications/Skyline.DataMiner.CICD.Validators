namespace SLDisValidator2.Tests.Protocol.Params.Param.ArrayOptions.CheckPartialAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckPartialAttribute, Category.Param)]
    public class CheckPartialAttribute : /*IValidate, ICodeFix, */ICompare
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
                if (!oldParam.GetRTDisplay())
                {
                    continue;
                }

                string newPid = newParam?.Id?.RawValue;

                string oldPartial = oldParam?.ArrayOptions?.Partial?.Value;
                string newPartial = newParam?.ArrayOptions?.Partial?.Value;

                bool hasOldPartial = !(oldPartial == null || oldPartial.Contains("false"));
                bool hasNewPartial = !(newPartial == null || newPartial.Contains("false"));

                if (!hasOldPartial && hasNewPartial)
                {
                    results.Add(ErrorCompare.EnabledPartial(newParam, newParam, newPid));
                }
            }

            return results;
        }
    }
}
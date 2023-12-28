namespace SLDisValidator2.Tests.Protocol.ParameterGroups.Group.CheckDynamicIndexAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckDynamicIndexAttribute, Category.ParameterGroup)]
    internal class CheckDynamicIndexAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParameterGroupsGroup parameterGroup in context.EachParameterGroupWithValidId())
            {
                if (parameterGroup.DynamicIndex == null)
                {
                    continue;
                }

                if (parameterGroup.DynamicId?.Value == null)
                {
                    results.Add(Error.MissingDynamicIdAttribute(this, parameterGroup, parameterGroup, parameterGroup.Id.RawValue));
                }
            }

            return results;
        }

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

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }
}
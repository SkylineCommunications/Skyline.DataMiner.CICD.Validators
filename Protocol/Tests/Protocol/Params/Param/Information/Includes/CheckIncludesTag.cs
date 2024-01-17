namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Information.Includes.CheckIncludesTag
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckIncludesTag, Category.Param)]
    internal class CheckIncludesTag : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var param in context.EachParamWithValidId())
            {
                var includes = param.Information?.Includes;
                if (includes == null)
                {
                    continue;
                }

                results.Add(Error.ObsoleteTag(this, param, includes, param.Id.RawValue));
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (!(context.Result.ReferenceNode is IParamsParam readParam))
            {
                result.Message = "readParam is null.";
                return result;
            }

            var editParam = context.Protocol?.Params?.Get(readParam);
            if (editParam == null)
            {
                result.Message = "editParam is null.";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.ObsoleteTag:
                    editParam.Information.Includes = null;
                    result.Success = true;
                    break;
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
}
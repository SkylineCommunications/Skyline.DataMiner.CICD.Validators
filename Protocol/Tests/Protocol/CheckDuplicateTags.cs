namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckDuplicateTags
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckDuplicateTags, Category.Protocol)]
    internal class CheckDuplicateTags : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context.ProtocolModel.Protocol == null)
            {
                return results;
            }

            // Check duplicate tags within a parameter
            foreach (IParamsParam paramsParam in context.EachParamWithValidId())
            {
                if (paramsParam.Interprete != null)
                {
                    var interpreteXmlNode = paramsParam.Interprete.ReadNode;
                    if (interpreteXmlNode.Elements["RawType"].Count() > 1)
                    {
                        results.Add(Error.DuplicateRawTypeTag(this, paramsParam, paramsParam.Interprete, paramsParam.Id.RawValue));
                    }
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
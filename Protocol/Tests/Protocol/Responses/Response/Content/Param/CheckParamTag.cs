namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Responses.Response.Content.Param.CheckParamTag
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckParamTag, Category.Response)]
    internal class CheckParamTag : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IResponsesResponse response in context.EachResponseWithValidId())
            {
                if (response.Content == null)
                {
                    continue;
                }

                foreach (var paramInResponse in response.Content)
                {
                    (GenericStatus status, string rawValue, uint? _) = GenericTests.CheckBasics(paramInResponse, false);

                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyParamTag(this, paramInResponse, paramInResponse, response.Id.RawValue));
                        continue;
                    }

                    if (status.HasFlag(GenericStatus.Invalid))
                    {
                        results.Add(Error.InvalidParamTag(this, paramInResponse, paramInResponse, rawValue, response.Id.RawValue));
                        continue;
                    }

                    if (!context.ProtocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, rawValue, out _))
                    {
                        results.Add(Error.NonExistingId(this, paramInResponse, paramInResponse, rawValue, response.Id.RawValue));
                        continue;
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
        ////            result.Message = String.Format("This error ({0}) isn't implemented.", context.Result.ErrorId.ToString());
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
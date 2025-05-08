namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.CheckSaveAttribute
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckSaveAttribute, Category.Param)]
    internal class CheckSaveAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context?.ProtocolModel?.Protocol?.Params == null)
            {
                return results;
            }

            var protocol = context.ProtocolModel.Protocol;
            var relationManager = context.ProtocolModel.RelationManager;

            foreach (IParamsParam param in protocol.Params)
            {
                IEnumerable<Link> links = relationManager.GetReverseLinks(param);
                foreach (Link link in links)
                {
                    if (link.Source is IResponsesResponse && param.Save?.Value == true)
                    {
                        results.Add(Error.UndesiredSavedReadParam(this, null, null, Convert.ToString(param.Id.RawValue)));
                    }
                }
            }

            return results;
        }

        //public ICodeFixResult Fix(CodeFixContext context)
        //{
        //    CodeFixResult result = new CodeFixResult();

        //    switch (context.Result.ErrorId)
        //    {
        //        default:
        //            result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
        //            break;
        //    }

        //    return result;
        //}

        //public List<IValidationResult> Compare(MajorChangeCheckContext context)
        //{
        //    List<IValidationResult> results = new List<IValidationResult>();

        //    return results;
        //}
    }
}
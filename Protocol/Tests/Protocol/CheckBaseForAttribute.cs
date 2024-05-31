namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckBaseForAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckBaseForAttribute, Category.Protocol)]
    internal class CheckBaseForAttribute : IValidate //, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var protocol = context?.ProtocolModel?.Protocol;
            if (protocol == null)
            {
                return results;
            }

            string baseFor = protocol.BaseFor?.Value;
            string elementType = protocol.ElementType?.Value;

            if (!String.IsNullOrEmpty(baseFor) && !String.IsNullOrEmpty(elementType) &&
                String.Equals(baseFor, elementType, StringComparison.OrdinalIgnoreCase))
            {
                results.Add(Error.InvalidAttribute(this, protocol, protocol, baseFor));
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
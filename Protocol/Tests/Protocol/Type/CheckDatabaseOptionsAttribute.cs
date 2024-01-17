namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Type.CheckDatabaseOptionsAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckDatabaseOptionsAttribute, Category.Protocol)]
    internal class CheckDatabaseOptionsAttribute : /*IValidate, ICodeFix, */ICompare
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

            var oldDatabaseOptions = context.PreviousProtocolModel?.Protocol?.Type?.DatabaseOptions?.Value;
            var newProtocolType = context.NewProtocolModel?.Protocol?.Type;
            var newDatabaseOptions = newProtocolType?.DatabaseOptions?.Value;

            if ((oldDatabaseOptions == null || !oldDatabaseOptions.ToLower().Contains("partitionedtrending")) &&
                newDatabaseOptions != null && newDatabaseOptions.ToLower().Contains("partitionedtrending"))
            {
                results.Add(ErrorCompare.EnabledPartitionedTrending(newProtocolType, newProtocolType));
            }

            return results;
        }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Chains.CheckChildNameAttributes
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

    [Test(CheckId.CheckChildNameAttributes, Category.Chain)]
    internal class CheckChildNameAttributes : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var chains = context.ProtocolModel?.Protocol?.Chains;
            if (chains == null)
            {
                return results;
            }

            var resultsForDuplicateNames = GenericTests.CheckDuplicates(
                items: chains,
                getDuplicationIdentifier: x =>
                    {
                        switch (x)
                        {
                            case IChainsChain c: return c.Name?.Value;
                            case IChainsSearchChain c: return c.Name?.Value;
                            default: return "";
                        }
                    },
                generateSubResult: x => Error.DuplicatedValue(this, x.item, x.item, x.duplicateValue),
                generateSummaryResult: x => Error.DuplicatedValue(this, chains, null, x.duplicateValue).WithSubResults(x.subResults)
            );

            results.AddRange(resultsForDuplicateNames);

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
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Topologies.Topology.CheckNameAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckNameAttribute, Category.Topology)]
    internal class CheckNameAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var topologies = context.ProtocolModel?.Protocol?.Topologies;
            if (topologies == null)
            {
                return results;
            }

            var resultsForDuplicateNames = GenericTests.CheckDuplicates(
                items: topologies,
                getDuplicationIdentifier: topology => topology.Name?.Value,
                generateSubResult: x => Error.DuplicatedValue(this, x.item, x.item, x.duplicateValue),
                generateSummaryResult: x => Error.DuplicatedValue(this, topologies, null, x.duplicateValue).WithSubResults(x.subResults)
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
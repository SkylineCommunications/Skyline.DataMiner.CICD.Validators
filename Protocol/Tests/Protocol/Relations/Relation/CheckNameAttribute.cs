namespace SLDisValidator2.Tests.Protocol.Relations.Relation.CheckNameAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Generic;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckNameAttribute, Category.Relation)]
    public class CheckNameAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var relations = context.ProtocolModel?.Protocol?.Relations;
            if (relations == null)
            {
                return results;
            }

            var resultsForDuplicateNames = GenericTests.CheckDuplicates(
                items: relations,
                getDuplicationIdentifier: relation => relation.Name?.Value,
                generateSubResult: x => Error.DuplicatedValue(this, x.item, x.item, x.duplicateValue),
                generateSummaryResult: x => Error.DuplicatedValue(this, relations, null, x.duplicateValue).WithSubResults(x.subResults)
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
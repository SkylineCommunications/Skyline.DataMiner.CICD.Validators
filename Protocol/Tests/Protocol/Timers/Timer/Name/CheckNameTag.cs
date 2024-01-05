namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Timers.Timer.Name.CheckNameTag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckNameTag, Category.Timer)]
    internal class CheckNameTag : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var timers = context.EachTimerWithValidId().ToList();

            foreach (var timer in timers)
            {
                // TODO: missing, empty, invalid, untrimmed, etc...
            }

            var resultsForDuplicateNames = GenericTests.CheckDuplicates(
                items: timers,
                getDuplicationIdentifier: timer => timer.Name?.Value,
                getId: timer => timer.Id.RawValue,
                generateSubResult: x => Error.DuplicatedValue(this, x.item, x.item, x.duplicateValue, x.id),
                generateSummaryResult: x => Error.DuplicatedValue(this, null, null, x.duplicateValue, String.Join(", ", x.ids)).WithSubResults(x.subResults)
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
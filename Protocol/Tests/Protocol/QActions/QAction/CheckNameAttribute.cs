namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckNameAttribute
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

    [Test(CheckId.CheckNameAttribute, Category.QAction)]
    internal class CheckNameAttribute : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var qactions = context.EachQActionWithValidId().ToList();

            foreach (var qaction in qactions)
            {
                // TODO: missing, empty, invalid, untrimmed, etc...
            }

            var resultsForDuplicateNames = GenericTests.CheckDuplicates(
                items: qactions,
                getDuplicationIdentifier: x => x.Name?.Value,
                getId: x => x.Id?.RawValue,
                generateSubResult: x => Error.DuplicatedValue(this, x.item, x.item, x.duplicateValue, x.id),
                generateSummaryResult: x => Error.DuplicatedValue(this, null, null, x.duplicateValue, String.Join(", ", x.ids)).WithSubResults(x.subResults)
                );

            results.AddRange(resultsForDuplicateNames);

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {

                default:
                    result.Message = String.Format("This error ({0}) isn't implemented.", context.Result.ErrorId.ToString());
                    break;
            }

            return result;
        }

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            return results;
        }
    }
}
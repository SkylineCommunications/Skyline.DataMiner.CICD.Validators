namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Pairs.Pair.Condition.CheckConditionTag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers.Conditions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckConditionTag, Category.Pair)]
    internal class CheckConditionTag : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IPairsPair pair in context.EachPairWithValidId())
            {
                if (pair.Condition == null)
                {
                    continue;
                }

                if (String.IsNullOrWhiteSpace(pair.Condition.Value))
                {
                    results.Add(Error.InvalidCondition(this, pair, pair, pair.Condition.RawValue, "Condition is empty.", pair.Id.RawValue));
                    continue;
                }

                var conditionValidationResults = new List<IValidationResult>();

                var addInvalidConditionError = new Action<string>(message => conditionValidationResults.Add(Error.InvalidCondition(this, pair, pair.Condition, pair.Condition.Value, message, pair.Id.RawValue)));
                var addInvalidParamIdError = new Action<string>(paramId => conditionValidationResults.Add(Error.NonExistingId(this, pair, pair.Condition, paramId, pair.Id.RawValue)));
                var conditionCanBeSimplifiedWarning = new Action(() => { if (conditionValidationResults.All(r => r.FullId != "9.7.3")) { conditionValidationResults.Add(Error.ConditionCanBeSimplified(this, pair, pair.Condition, pair.Condition.Value, pair.Id.RawValue)); } });

                Conditional conditional = new Conditional(addInvalidConditionError, addInvalidParamIdError, conditionCanBeSimplifiedWarning);

                conditional.ParseConditional(pair.Condition.Value);

                if (!conditionValidationResults.Any(result => result.Severity == Severity.Major || result.Severity == Severity.Critical))
                {
                    conditional.CheckConditional(context.ProtocolModel);
                }

                results.AddRange(conditionValidationResults);
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
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Timers.Timer.Condition.CheckConditionTag
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

    [Test(CheckId.CheckConditionTag, Category.Timer)]
    internal class CheckConditionTag : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (ITimersTimer timer in context.EachTimerWithValidId())
            {
                if (timer.Condition == null)
                {
                    continue;
                }

                results.Add(Error.UnrecommendedCondition(this, timer, timer.Condition, timer.Id.RawValue));

                if (String.IsNullOrWhiteSpace(timer.Condition.Value))
                {
                    results.Add(Error.InvalidCondition(this, timer, timer, timer.Condition.RawValue, "Condition is empty.", timer.Id.RawValue));
                    continue;
                }

                var conditionValidationResults = new List<IValidationResult>();

                var addInvalidConditionError = new Action<string>(message => conditionValidationResults.Add(Error.InvalidCondition(this, timer, timer.Condition, timer.Condition.Value, message, timer.Id.RawValue)));
                var addInvalidParamIdError = new Action<string>(paramId => conditionValidationResults.Add(Error.NonExistingId(this, timer, timer.Condition, paramId, timer.Id.RawValue)));
                var conditionCanBeSimplifiedWarning = new Action(() => { if (conditionValidationResults.All(r => r.FullId != "7.4.4")) { conditionValidationResults.Add(Error.ConditionCanBeSimplified(this, timer, timer.Condition, timer.Condition.Value, timer.Id.RawValue)); } });

                Conditional conditional = new Conditional(addInvalidConditionError, addInvalidParamIdError, conditionCanBeSimplifiedWarning);

                conditional.ParseConditional(timer.Condition.Value);

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
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Triggers.Trigger.Condition.CheckConditionTag
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

    [Test(CheckId.CheckConditionTag, Category.Trigger)]
    internal class CheckConditionTag : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (ITriggersTrigger trigger in context.EachTriggerWithValidId())
            {
                if (trigger.Condition == null)
                {
                    continue;
                }

                if (String.IsNullOrWhiteSpace(trigger.Condition.Value))
                {
                    results.Add(Error.InvalidCondition(this, trigger, trigger, trigger.Condition.RawValue, "Condition is empty.", trigger.Id.RawValue));
                    continue;
                }

                var conditionValidationResults = new List<IValidationResult>();

                var addInvalidConditionError = new Action<string>(message => conditionValidationResults.Add(Error.InvalidCondition(this, trigger, trigger.Condition, trigger.Condition.Value, message, trigger.Id.RawValue)));
                var addInvalidParamIdError = new Action<string>(paramId => conditionValidationResults.Add(Error.NonExistingId(this, trigger, trigger.Condition, paramId, trigger.Id.RawValue)));
                var conditionCanBeSimplifiedWarning = new Action(() => { if (conditionValidationResults.All(r => r.FullId != "5.5.3")) { conditionValidationResults.Add(Error.ConditionCanBeSimplified(this, trigger, trigger.Condition, trigger.Condition.Value, trigger.Id.RawValue)); } });

                Conditional conditional = new Conditional(addInvalidConditionError, addInvalidParamIdError, conditionCanBeSimplifiedWarning);

                conditional.ParseConditional(trigger.Condition.Value);

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
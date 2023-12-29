namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Actions.Action.Condition.CheckConditionTag
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

    [Test(CheckId.CheckConditionTag, Category.Action)]
    internal class CheckConditionTag : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IActionsAction action in context.EachActionWithValidId())
            {
                if (action.Condition == null)
                {
                    continue;
                }

                if (String.IsNullOrWhiteSpace(action.Condition.Value))
                {
                    results.Add(Error.InvalidCondition(this, action, action, action.Condition.RawValue, "Condition is empty.", action.Id.RawValue));
                    continue;
                }

                var conditionValidationResults = new List<IValidationResult>();

                var addInvalidConditionError = new Action<string>(message => conditionValidationResults.Add(Error.InvalidCondition(this, action, action.Condition, action.Condition.Value, message, action.Id.RawValue)));
                var addInvalidParamIdError = new Action<string>(paramId => conditionValidationResults.Add(Error.NonExistingId(this, action, action.Condition, paramId, action.Id.RawValue)));
                var conditionCanBeSimplifiedWarning = new Action(() => { if (conditionValidationResults.All(r => r.FullId != "6.4.3")) { conditionValidationResults.Add(Error.ConditionCanBeSimplified(this, action, action.Condition, action.Condition.Value, action.Id.RawValue)); } });

                Conditional conditional = new Conditional(addInvalidConditionError, addInvalidParamIdError, conditionCanBeSimplifiedWarning);

                conditional.ParseConditional(action.Condition.Value);

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
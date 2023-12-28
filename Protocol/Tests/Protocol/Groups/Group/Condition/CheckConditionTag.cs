namespace SLDisValidator2.Tests.Protocol.Groups.Group.Condition.CheckConditionTag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Helpers.Conditions;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckConditionTag, Category.Group)]
    public class CheckConditionTag : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IGroupsGroup group in context.EachGroupWithValidId())
            {
                if (group.Condition == null)
                {
                    continue;
                }

                if (String.IsNullOrWhiteSpace(group.Condition.Value))
                {
                    results.Add(Error.InvalidCondition(this, group, group, group.Condition.RawValue, "Condition is empty.", group.Id.RawValue));
                    continue;
                }

                var conditionValidationResults = new List<IValidationResult>();

                var addInvalidConditionError = new Action<string>(message => conditionValidationResults.Add(Error.InvalidCondition(this, group, group.Condition, group.Condition.Value, message, group.Id.RawValue)));
                var addInvalidParamIdError = new Action<string>(paramId => conditionValidationResults.Add(Error.NonExistingId(this, group, group.Condition, paramId, group.Id.RawValue)));
                var addConditionCanBeSimpliefiedWarning = new Action(() => { if (!conditionValidationResults.Any(r => r.FullId == "4.9.3")) { conditionValidationResults.Add(Error.ConditionCanBeSimplified(this, group, group.Condition, group.Condition.Value, group.Id.RawValue)); }});

                Conditional conditional = new Conditional(addInvalidConditionError, addInvalidParamIdError, addConditionCanBeSimpliefiedWarning);

                conditional.ParseConditional(group.Condition.Value);

                if (conditionValidationResults.Count == 0 || !conditionValidationResults.Any(result => (result.Severity == Skyline.DataMiner.CICD.Validators.Common.Model.Severity.Major || result.Severity == Skyline.DataMiner.CICD.Validators.Common.Model.Severity.Critical)))
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
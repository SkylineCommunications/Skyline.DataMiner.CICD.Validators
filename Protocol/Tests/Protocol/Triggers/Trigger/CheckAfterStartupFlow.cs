namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Triggers.Trigger.CheckAfterStartupFlow
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckAfterStartupFlow, Category.Trigger)]
    internal class CheckAfterStartupFlow : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            var results = new List<IValidationResult>();

            var model = context.ProtocolModel;

            foreach (ITriggersTrigger trigger in context.EachTriggerWithValidId())
            {
                EnumTriggerTime? triggerTime = EnumTriggerTimeConverter.Convert(trigger.Time?.Value);

                if (trigger.On?.Value != EnumTriggerOn.Protocol
                    || !triggerTime.HasValue || triggerTime.Value != EnumTriggerTime.AfterStartup)
                {
                    continue;
                }

                if (!String.IsNullOrWhiteSpace(trigger.Condition?.Value))
                {
                    results.Add(Error.InvalidAfterStartupTriggerCondition(this, trigger, trigger.Condition, trigger.Id.RawValue));
                    continue;
                }

                if (trigger.Type?.Value != EnumTriggerType.Action)
                {
                    var triggerPositionNode = trigger.Type != null ? (IReadable)trigger.Type : trigger;
                    results.Add(Error.InvalidAfterStartupTriggerType(this, trigger, triggerPositionNode, trigger.Id.RawValue));
                    continue;
                }

                foreach (IActionsAction action in trigger.GetActions(model.RelationManager))
                {
                    if (!String.IsNullOrWhiteSpace(action.Condition?.Value))
                    {
                        results.Add(Error.InvalidAfterStartupActionCondition(this, action, action.Condition, action.Id?.RawValue));
                        continue;
                    }

                    if (action.On?.Value != EnumActionOn.Group)
                    {
                        var actionPositionNode = action.On != null ? (IReadable)action.On : action;
                        results.Add(Error.InvalidAfterStartupActionOn(this, action, actionPositionNode, action.Id?.RawValue));
                        continue;
                    }

                    if (action.Type?.Value != EnumActionType.Execute &&
                        action.Type?.Value != EnumActionType.ExecuteNext &&
                        action.Type?.Value != EnumActionType.ExecuteOneNow &&
                        action.Type?.Value != EnumActionType.ExecuteOneTop)
                    {
                        var actionPositionNode = action.Type != null ? (IReadable)action.Type : action;
                        results.Add(Error.InvalidAfterStartupActionType(this, action, actionPositionNode, action.Id?.RawValue));
                        continue;
                    }

                    foreach (IGroupsGroup group in action.GetGroups(model.RelationManager))
                    {
                        if (group.Type == null ||
                            group.Type.Value == EnumGroupType.Poll ||
                            group.Type.Value == EnumGroupType.PollAction ||
                            group.Type.Value == EnumGroupType.PollTrigger)
                        {
                            continue;
                        }

                        results.Add(Error.InvalidAfterStartupGroupType(this, group, group.Type, group.Id?.RawValue));
                    }
                }
            }

            return results;
        }
    }
}
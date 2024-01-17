namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Triggers.Trigger.CheckOnTagTimeTagCombination
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckOnTagTimeTagCombination, Category.Trigger)]
    internal class CheckOnTagTimeTagCombination : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            var results = new List<IValidationResult>();

            var model = context.ProtocolModel;
            if (model.Protocol?.Triggers == null)
            {
                return results;
            }

            var groupedByOnTimeCombination = new Dictionary<(EnumTriggerOn, string, string), List<ITriggersTrigger>>();

            var triggers = model.Protocol.Triggers;
            foreach (ITriggersTrigger trigger in context.EachTriggerWithValidId())
            {
                var triggerOn = trigger.On?.Value;
                var triggerOnId = trigger.On?.Id?.Value;
                var triggerTimeAsString = trigger.Time?.Value;

                if (triggerOn == null || triggerTimeAsString == null)
                {
                    // This will get checked by a separate check for the On and Time tag
                    continue;
                }

                EnumTriggerTime? triggerTime = EnumTriggerTimeConverter.Convert(triggerTimeAsString);
                if (!triggerTime.HasValue)
                {
                    // Not yet supported (see task 141135 for details)
                    continue;
                }

                // Invalid combinations
                if (triggerTime.HasValue && !TriggerHelper.IsValidOnTimeTriggerCombination(triggerOn.Value, triggerTime.Value))
                {
                    results.Add(Error.InvalidOnTagTimeTagCombination(
                        this,
                        trigger,
                        trigger,
                        trigger.On?.RawValue,
                        trigger.Time?.RawValue,
                        trigger.Id.RawValue));

                    continue;
                }

                // Filling in dictionary for duplicate trigger check
                var combination = (triggerOn.Value, triggerOnId, triggerTimeAsString);
                if (!groupedByOnTimeCombination.TryGetValue(combination, out var group))
                {
                    group = new List<ITriggersTrigger>();
                    groupedByOnTimeCombination.Add(combination, group);
                }

                group.Add(trigger);
            }

            // Check duplicates
            foreach (var group in groupedByOnTimeCombination.Values.Where(g => g.Count > 1))
            {
                List<uint> triggerIds = new List<uint>();
                List<IValidationResult> subResults = new List<IValidationResult>();

                /*
                 * If all triggers have a condition defined, we are uncertain because it could be that only one trigger becomes active at the same time
                 * the content of the condition itself is verified in another check.
                 */
                Certainty certainty = group.All(t => t.Condition != null) ? Certainty.Uncertain : Certainty.Certain;

                foreach (ITriggersTrigger trigger in group)
                {
                    uint? id = trigger.Id?.Value;
                    if (id != null)
                    {
                        triggerIds.Add(id.Value);
                    }

                    subResults.Add(Error.DuplicateTrigger(this, triggers, trigger, certainty, Convert.ToString(id)));
                }

                IValidationResult error = Error.DuplicateTrigger(this, triggers, @group.First(), certainty, String.Join(", ", triggerIds));
                error.WithSubResults(subResults.ToArray());
                results.Add(error);
            }

            return results;
        }
    }
}
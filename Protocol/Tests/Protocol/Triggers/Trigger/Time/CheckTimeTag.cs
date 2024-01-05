namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Triggers.Trigger.Time.CheckTimeTag
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

    [Test(CheckId.CheckTimeTag, Category.Trigger)]
    internal class CheckTimeTag : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context.ProtocolModel?.Protocol?.Triggers == null)
            {
                return results;
            }

            Dictionary<string, ITriggersTrigger> afterStartupPairs = new Dictionary<string, ITriggersTrigger>();

            foreach (ITriggersTrigger trigger in context.EachTriggerWithValidId())
            {
                EnumTriggerTime? triggerTime = EnumTriggerTimeConverter.Convert(trigger.Time?.Value);

                if (!triggerTime.HasValue || triggerTime.Value != EnumTriggerTime.AfterStartup)
                {
                    continue;
                }

                if (!afterStartupPairs.ContainsKey(trigger.Id.RawValue))
                {
                    afterStartupPairs.Add(trigger.Id.RawValue, trigger);
                }
            }

            if (afterStartupPairs.Count <= 1)
            {
                return results;
            }

            var firstPairValue = afterStartupPairs.First();
            IValidationResult error = Error.MultipleAfterStartup(this, firstPairValue.Value, firstPairValue.Value.Time, String.Join(", ", afterStartupPairs.Keys.ToArray()));

            foreach (KeyValuePair<string, ITriggersTrigger> triggerKvp in afterStartupPairs)
            {
                IValidationResult subResult = Error.MultipleAfterStartup(this, triggerKvp.Value, triggerKvp.Value.Time, triggerKvp.Key);
                error.WithSubResults(subResult);
            }

            results.Add(error);

            return results;
        }
    }
}
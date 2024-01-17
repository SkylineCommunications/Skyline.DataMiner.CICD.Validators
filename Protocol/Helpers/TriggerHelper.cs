namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;

    internal static class TriggerHelper
    {
        private static readonly Dictionary<EnumTriggerOn, EnumTriggerTime[]> TriggerValidOnTimeTagCombinations = new Dictionary<EnumTriggerOn, EnumTriggerTime[]>
        {
            { EnumTriggerOn.Parameter, new []{ EnumTriggerTime.Change, EnumTriggerTime.ChangeAfterResponse, EnumTriggerTime.Timeout, EnumTriggerTime.TimeoutAfterRetries } },
            { EnumTriggerOn.Command, new []{ EnumTriggerTime.Before, EnumTriggerTime.After } },
            { EnumTriggerOn.Response, new []{ EnumTriggerTime.Before, EnumTriggerTime.After } },
            { EnumTriggerOn.Pair, new []{ EnumTriggerTime.Succeeded, EnumTriggerTime.Timeout, EnumTriggerTime.TimeoutAfterRetries } },
            { EnumTriggerOn.Group, new []{ EnumTriggerTime.Before, EnumTriggerTime.After } },
            { EnumTriggerOn.Session, new []{ EnumTriggerTime.Timeout } },
            { EnumTriggerOn.Timer, new []{ EnumTriggerTime.Before } },
            { EnumTriggerOn.Protocol, new []{ EnumTriggerTime.AfterStartup, EnumTriggerTime.LinkFileChange } },
            //{ EnumTriggerOn.Communication, new EnumTriggerTime[0] },
        };

        public static bool IsValidOnTimeTriggerCombination(EnumTriggerOn onValue, EnumTriggerTime timeValue)
        {
            if (!TriggerValidOnTimeTagCombinations.TryGetValue(onValue, out EnumTriggerTime[] timeValues))
            {
                // For onValues that are not included in triggerValidOnTimeTagCombinations, any string within timeValue is allowed.
                // Ex: trigger on communication.
                return true;
            }

            return timeValues.Contains(timeValue);
        }
    }
}
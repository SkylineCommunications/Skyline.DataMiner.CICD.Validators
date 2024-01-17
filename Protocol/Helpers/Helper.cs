namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Skyline.DataMiner.CICD.Validators.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    internal static class Helper
    {
        public static readonly string[] CriticalActions =
        {
            "reboot", "restart",
            "remove", "delete",
            "update", "upgrade",
            "shutdown", "shut-down", "shut down",
            "stop", "start", "activate"
        };

        public static bool IsUntrimmed(string value)
        {
            return !String.Equals(value?.Trim(), value);
        }

        public static bool HasLeadingChar(string value, char c)
        {
            return !String.Equals(value?.TrimStart(c), value);
        }

        public static bool TryMatchRegex(string input, string pattern, out Match match)
        {
            match = Regex.Match(input, pattern);
            return match.Success;
        }

        public static IEnumerable<char> CheckInvalidChars(string name, char[] invalidChars)
        {
            return name.Where(invalidChars.Contains).Distinct();
        }

        public static void AddResultOrGroupedResults(ICollection<IValidationResult> results,
            List<IValidationResult> potentialNewResults,
            Func<IValidationResult[], IValidationResult> generateSummaryResult)
        {
            if (potentialNewResults == null)
            {
                return;
            }

            switch (potentialNewResults.Count)
            {
                case 0: return;
                case 1:
                    results.Add(potentialNewResults.FirstOrDefault());
                    break;
                default:
                    results.Add(generateSummaryResult(potentialNewResults.ToArray()));
                    break;
            }
        }

        public static bool IsCriticalActionCaption(string actionCaption)
        {
            if (actionCaption == null)
            {
                return false;
            }

            return CriticalActions.Any(criticalAction => actionCaption.Contains(criticalAction, StringComparison.OrdinalIgnoreCase));
        }
    }
}
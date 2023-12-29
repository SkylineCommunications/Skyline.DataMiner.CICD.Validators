namespace Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    internal static class ValidationResultListExtensions
    {
        public static bool AddIfNotNull(this List<IValidationResult> results, IEnumerable<IValidationResult> newResults)
        {
            if (results != null && newResults != null && newResults.Any())
            {
                results.AddRange(newResults);
                return true;
            }

            return false;
        }

        public static bool AddIfNotNull(this List<IValidationResult> results, IValidationResult newResult)
        {
            if (results != null && newResult != null)
            {
                results.Add(newResult);
                return true;
            }

            return false;
        }
    }
}
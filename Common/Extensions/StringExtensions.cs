namespace Skyline.DataMiner.CICD.Validators.Common.Extensions
{
    using System;

    internal static class StringExtensions
    {
        /// <summary>
        /// Allows case insensitive checks
        /// </summary>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            if (source == null)
            {
                return false;
            }

            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}
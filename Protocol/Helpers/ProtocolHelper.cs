namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests
{
    using System;
    using System.Linq;

    internal static class ProtocolHelper
    {
        public static char[] RestrictedProtocolNameChars { get; } = { '<', '>', ':', '"', '/', '\\', '|', '?', '*', ';', '°' };

        public static string[] RestrictedProtocolNamePrefixes { get; } = { "Production" };

        public static string ReplaceProtocolNameInvalidChars(string protocolName, string replacementChar = "_")
        {
            string[] temp = protocolName.Split(RestrictedProtocolNameChars, StringSplitOptions.RemoveEmptyEntries);
            return String.Join(replacementChar, temp);
        }

        public static string GetProtocolNameInvalidPrefix(string protocolName)
        {
            return RestrictedProtocolNamePrefixes.FirstOrDefault(prefix => protocolName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        public static string RemoveProtocolNameInvalidPrefix(string protocolName)
        {
            string invalidPrefix = GetProtocolNameInvalidPrefix(protocolName);
            return protocolName.Substring(invalidPrefix.Length).Trim();
        }
    }
}
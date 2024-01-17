namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests
{
    using System;
    using System.Collections.Generic;

    internal static class HttpHelper
    {
        private static readonly HashSet<string> RedundantHttpRequestHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "User-Agent" };

        public static bool IsRedundantHttpRequestHeader(string header)
        {
            return RedundantHttpRequestHeaders.Contains(header);
        }
    }
}
namespace SLDisValidator2.Tests
{
    using System;
    using System.Collections.Generic;

    public static class HttpHelper
    {
        private static readonly HashSet<string> RedundantHttpRequestHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "User-Agent" };

        public static bool IsRedundantHttpRequestHeader(string header)
        {
            return RedundantHttpRequestHeaders.Contains(header);
        }
    }
}
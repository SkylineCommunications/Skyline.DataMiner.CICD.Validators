namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("8.5-2859", "8.0.8.4-2556")]
    internal class ParameterDateTime : IFeatureCheck
    {
        public string Title => "Parameter DateTime";

        public string Description => "The parameter will be displayed as a datetime. The value represents a decimal number indicating the total number of days that have passed since midnight 1899-12-30. The Interprete/Decimals and Display/Decimals tags of date and datetime parameters need to be set to 8 to avoid rounding errors.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 6046 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var columnOptionsByPid = context?.Model?.Protocol?.Params?
                .Where(x => x?.ArrayOptions != null)
                .SelectMany(x => x.ArrayOptions)
                .Where(x => x?.Pid?.Value != null)
                .GroupBy(x => x.Pid.Value.Value)
                .ToDictionary(x => x.Key, x => x.ToList());

            var items = context?.Model?.Protocol?.Params
                            ?.Where(x => IsDateOrDateTimeWithRequiredDecimals(x) && IsHeaderSumDisabledIfInTable(x, columnOptionsByPid))
                            .Select(x => (IReadable)x)
                            .ToList();

            return new FeatureCheckResult(items);
        }

        private static bool IsDateOrDateTimeWithRequiredDecimals(IParamsParam param)
        {
            return IsDateOrDateTime(param) &&
                   param.Interprete?.Decimals?.Value == 8 &&
                   param.Display?.Decimals?.Value == 8;
        }

        private static bool IsDateOrDateTime(IParamsParam param)
        {
            var options = param?.Measurement?.Type?.Options?.Value;
            if (String.IsNullOrWhiteSpace(options))
            {
                return false;
            }

            return options.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(option => option.Trim())
                          .Any(option => option.Equals("date", StringComparison.OrdinalIgnoreCase) || option.StartsWith("datetime", StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsHeaderSumDisabledIfInTable(IParamsParam param, Dictionary<uint, List<ITypeColumnOption>> columnOptionsByPid)
        {
            if (param?.Id?.Value == null || columnOptionsByPid == null || !columnOptionsByPid.TryGetValue(param.Id.Value.Value, out var columnOptions))
            {
                return true;
            }

            return columnOptions.All(IsHeaderSumDisabled);
        }

        private static bool IsHeaderSumDisabled(ITypeColumnOption columnOption)
        {
            string options = columnOption?.Options?.Value;
            if (String.IsNullOrWhiteSpace(options))
            {
                return false;
            }

            var optionTokens = GetOptionTokens(options);
            bool hasDisable = optionTokens.Any(option => option.StartsWith("disableHeaderSum", StringComparison.OrdinalIgnoreCase));
            bool hasEnable = optionTokens.Any(option => option.StartsWith("enableHeaderSum", StringComparison.OrdinalIgnoreCase));

            return hasDisable && !hasEnable;
        }

        private static IEnumerable<string> GetOptionTokens(string options)
        {
            if (String.IsNullOrEmpty(options))
            {
                return Enumerable.Empty<string>();
            }

            var separator = options[0];
            if (options.Length <= 1 || Char.IsLetterOrDigit(separator))
            {
                return new[] { options.Trim() };
            }

            return options.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(option => option.Trim());
        }
    }

}
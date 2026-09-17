namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Type.CheckDateTimeAndDateParameters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckDateTimeAndDateParameters, Category.Param)]
    internal class CheckDateTimeAndDateParameters : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            var results = new List<IValidationResult>();

            var dateColumnsByPid = context.ProtocolModel.Protocol?.Params
                ?.Where(param => param?.ArrayOptions != null)
                .SelectMany(tableParam => tableParam.ArrayOptions.Where(column => column?.Pid?.Value != null).Select(column => (tableParam, column)))
                .GroupBy(item => item.column.Pid.Value.Value)
                .ToDictionary(group => group.Key, group => group.ToList());

            foreach (var param in context.EachParamWithValidId())
            {
                if (!IsDateOrDateTime(param))
                {
                    continue;
                }

                if (param.Interprete?.Decimals?.Value != 8)
                {
                    IReadable positionNode = param.Interprete?.Decimals as IReadable ?? param;
                    results.Add(Error.InvalidInterpreteDecimals(this, param, positionNode, param.Id.RawValue));
                }

                if (param.Display?.Decimals?.Value != 8)
                {
                    IReadable positionNode = param.Display?.Decimals as IReadable ?? param;
                    results.Add(Error.InvalidDisplayDecimals(this, param, positionNode, param.Id.RawValue));
                }

                if (param.Id?.Value == null || dateColumnsByPid == null || !dateColumnsByPid.TryGetValue(param.Id.Value.Value, out var tableColumns))
                {
                    continue;
                }

                foreach (var (tableParam, columnOption) in tableColumns)
                {
                    if (!HasDisableHeaderSumWithoutEnable(columnOption?.Options?.Value))
                    {
                        results.Add(Error.MissingDisableHeaderSum(this, param, columnOption, param.Id.RawValue, tableParam.Id?.RawValue));
                    }
                }
            }

            return results;
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
                          .Any(option => option.Equals("date", StringComparison.OrdinalIgnoreCase) ||
                                         option.Equals("datetime", StringComparison.OrdinalIgnoreCase) ||
                                         option.StartsWith("datetime:", StringComparison.OrdinalIgnoreCase));
        }

        private static bool HasDisableHeaderSumWithoutEnable(string optionsRawValue)
        {
            if (String.IsNullOrWhiteSpace(optionsRawValue))
            {
                return false;
            }

            IEnumerable<string> tokens;

            char separator = optionsRawValue[0];
            if (Char.IsLetterOrDigit(separator))
            {
                tokens = new[] { optionsRawValue };
            }
            else
            {
                tokens = optionsRawValue.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            }

            bool hasDisable = false;
            bool hasEnable = false;

            foreach (var rawToken in tokens)
            {
                string token = rawToken.Trim();
                if (token.StartsWith("disableHeaderSum", StringComparison.OrdinalIgnoreCase))
                {
                    hasDisable = true;
                }
                else if (token.StartsWith("enableHeaderSum", StringComparison.OrdinalIgnoreCase))
                {
                    hasEnable = true;
                }
            }

            return hasDisable && !hasEnable;
        }
    }
}

namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.Discreet.Value.CheckValueTag
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

    [Test(CheckId.CheckValueTag, Category.Param)]
    internal class CheckValueTag : ICompare
    {
        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                var discreets = oldParam.Measurement?.Discreets;
                if (discreets == null)
                {
                    continue;
                }

                foreach (IParamsParamMeasurementDiscreetsDiscreet oldDiscreet in discreets)
                {
                    IValidationResult result = CompareHelper.CompareToOldDiscreet(newParam, oldDiscreet);
                    if (result == null)
                    {
                        continue;
                    }

                    results.Add(result);
                }
            }

            return results;
        }
    }

    internal static class CompareHelper
    {
        public static IValidationResult CompareToOldDiscreet(IParamsParam newParam, IParamsParamMeasurementDiscreetsDiscreet previousDiscreet)
        {
            IValidationResult result = CompareDiscreetValueChange(newParam, previousDiscreet);
            if (result != null)
            {
                return result;
            }

            result = CompareDiscreetRemoved(newParam, previousDiscreet);
            return result;
        }

        private static IValidationResult CompareDiscreetRemoved(IParamsParam newParam, IParamsParamMeasurementDiscreetsDiscreet previousDiscreet)
        {
            var newDiscreetWithValue = newParam.Measurement?.Discreets?.FirstOrDefault(p => String.Equals(p.ValueElement?.Value, previousDiscreet.ValueElement?.Value, StringComparison.OrdinalIgnoreCase));
            if (newDiscreetWithValue == null)
            {
                return ErrorCompare.RemovedItem(previousDiscreet, previousDiscreet, previousDiscreet.ValueElement?.Value, newParam.Id?.RawValue);
            }

            return null;
        }

        private static IValidationResult CompareDiscreetValueChange(IParamsParam newParam, IParamsParamMeasurementDiscreetsDiscreet previousDiscreet)
        {
            var newDiscreetByDisplays = newParam.Measurement?.Discreets?.Where(p => String.Equals(p.Display?.Value, previousDiscreet.Display?.Value, StringComparison.OrdinalIgnoreCase));
            if (newDiscreetByDisplays == null)
            {
                return null;
            }

            IValidationResult invalidResult = null;
            foreach (var newDiscreet in newDiscreetByDisplays)
            {
                if (String.Equals(newDiscreet.ValueElement?.Value, previousDiscreet.ValueElement?.Value, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                string oldDisplay = previousDiscreet.Display?.Value;
                string paramId = newParam.Id?.RawValue;
                string oldValue = previousDiscreet.ValueElement?.Value;
                string newValue = newDiscreet.ValueElement?.Value;
                invalidResult = ErrorCompare.UpdatedValue(newDiscreet, newDiscreet, oldDisplay, paramId, oldValue, newValue);
            }

            return invalidResult;
        }
    }
}
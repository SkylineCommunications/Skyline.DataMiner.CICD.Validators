namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.CheckMatrixDiscreets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    using static Skyline.DataMiner.CICD.Models.Protocol.Read.ParamTypeOptions;

    [Test(CheckId.CheckMatrixDiscreets, Category.Param)]
    internal class CheckMatrixDiscreets : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            ValidateHelper helper = new ValidateHelper(this);
            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.Type?.Value == EnumParamType.Write)
                {
                    continue;
                }

                if (!param.IsMatrix())
                {
                    continue;
                }

                // Check if the Discreets aren't 1-based
                var discreets = param.Measurement?.Discreets;
                if (results.AddIfNotNull(helper.CheckDiscreetsNotOneBased(param.Id.RawValue, param, discreets)))
                {
                    continue;
                }

                // Check if there are missing Discreet values
                var dimensions = param.Type?.GetOptions()?.Dimensions;
                if (results.AddIfNotNull(helper.CheckMissingDiscreetValue(param.Id.RawValue, param, dimensions, discreets)))
                {
                    continue;
                }

                // Check if the amount of discreets is correct
                if (results.AddIfNotNull(helper.CheckInvalidDiscreetCount(param.Id.RawValue, param, dimensions, discreets)))
                {
                    continue;
                }
            }

            return results;
        }

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {

        ////        default:
        ////            result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
        ////            break;
        ////    }

        ////    return result;
        ////}

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }

    internal class ValidateHelper
    {
        private readonly IValidate test;

        public ValidateHelper(IValidate test)
        {
            this.test = test;
        }

        private static bool IsFirstValueOne(IParamsParamMeasurementDiscreets discreets)
        {
            if (discreets == null || discreets.Count == 0)
            {
                return false;
            }

            string discreet = discreets[0]?.ValueElement?.Value;
            return String.Equals(discreet, "1");
        }

        public IValidationResult CheckDiscreetsNotOneBased(string pid, IParamsParam param, IParamsParamMeasurementDiscreets discreets)
        {
            // Missing discreets will be checked by the InvalidDiscreetCount
            if (discreets == null || discreets.Count == 0)
            {
                return null;
            }

            if (!IsFirstValueOne(discreets))
            {
                return Error.DiscreetsNotOneBased(test, discreets, param, pid);
            }

            return null;
        }

        public IValidationResult CheckMissingDiscreetValue(string pid, IParamsParam param, DimensionsClass dimensions, IParamsParamMeasurementDiscreets discreets)
        {
            // Missing discreets will be checked by the InvalidDiscreetCount
            if (dimensions == null || discreets == null || discreets.Count == 0 || !IsFirstValueOne(discreets))
            {
                return null;
            }

            var allValues = new HashSet<string>(discreets.Select(p => p?.ValueElement?.Value).Distinct());
            List<string> missingValues = new List<string>();

            uint expectedDiscreetCount = (dimensions.Columns ?? 0) + (dimensions.Rows ?? 0);

            for (int expectedValue = 1; expectedValue <= expectedDiscreetCount; expectedValue++)
            {
                string expectedRowValueText = Convert.ToString(expectedValue);
                if (!allValues.Contains(expectedRowValueText))
                {
                    missingValues.Add(expectedRowValueText);
                }
            }

            if (missingValues.Count > 0)
            {
                return Error.MissingDiscreetValue(test, discreets, param, String.Join(";", missingValues), pid);
            }

            return null;
        }

        public IValidationResult CheckInvalidDiscreetCount(string pid, IParamsParam param, DimensionsClass dimensions, IParamsParamMeasurementDiscreets discreets)
        {
            if (dimensions == null)
            {
                return null;
            }

            uint expectedDiscreetCount = (dimensions.Columns ?? 0) + (dimensions.Rows ?? 0);
            if (discreets == null || discreets.Count == 0)
            {
                return Error.InvalidDiscreetCount(test, discreets, param, "0", Convert.ToString(expectedDiscreetCount), pid);
            }

            uint totalDiscreets = (uint)discreets.Count;
            if (totalDiscreets != expectedDiscreetCount)
            {
                return Error.InvalidDiscreetCount(test, discreets, param, Convert.ToString(totalDiscreets), Convert.ToString(expectedDiscreetCount), pid);
            }

            return null;
        }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Exceptions.CheckExceptionsTag
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

    [Test(CheckId.CheckExceptionsTag, Category.Param)]
    internal class CheckExceptionsTag : ICompare
    {
        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            var results = new List<IValidationResult>();

            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                string newPid = newParam.Id?.RawValue;

                if (oldParam.Alarm?.Monitored?.Value == false)
                {
                    continue;
                }

                var oldExceptions = oldParam.Interprete?.Exceptions;
                var newExceptions = newParam.Interprete?.Exceptions;
                if (oldExceptions == newExceptions)
                {
                    continue;
                }

                var newExceptionIds = new List<uint>();
                if (newExceptions != null)
                {
                    var oldRange = oldParam.Display?.Range;
                    var oldDiscreets = oldParam.Measurement?.Discreets;

                    foreach (var newException in newExceptions)
                    {
                        uint? exceptionId = newException.Id?.Value;
                        if (exceptionId == null)
                        {
                            continue;
                        }

                        newExceptionIds.Add(exceptionId.Value);

                        var oldException = oldExceptions?.FirstOrDefault(e => e.Id?.Value == exceptionId);
                        if (oldException == null)
                        {
                            decimal? valueAttribute = null;
                            if (Decimal.TryParse(newException.ValueAttribute?.Value, out decimal tempValueAttribute))
                            {
                                valueAttribute = tempValueAttribute;
                            }

                            decimal? valueElement = null;
                            if (Decimal.TryParse(newException.ValueAttribute?.Value, out decimal tempValueElement))
                            {
                                valueElement = tempValueElement;
                            }

                            bool wasWithinAllowedValues;
                            if (oldParam.Measurement?.Type?.Value == Skyline.DataMiner.CICD.Models.Protocol.Enums.EnumParamMeasurementType.Discreet)
                            {
                                wasWithinAllowedValues = CompareHelper.IsWithinDiscreets(oldDiscreets, newException.ValueAttribute?.Value) ||
                                CompareHelper.IsWithinDiscreets(oldDiscreets, newException.ValueElement?.Value);
                            }
                            else
                            {
                                wasWithinAllowedValues = CompareHelper.IsWithinRange(oldRange, valueAttribute) || CompareHelper.IsWithinRange(oldRange, valueElement);
                            }

                            if (wasWithinAllowedValues)
                            {
                                results.Add(ErrorCompare.AddedException(newException, newException, newException.Id?.RawValue, newPid));
                            }

                            continue;
                        }

                        if (newException == oldException)
                        {
                            continue;
                        }

                        string oldExceptionValue = oldException.ValueElement?.Value;
                        string newExceptionValue = newException.ValueElement?.Value;
                        if (oldExceptionValue == newExceptionValue)
                        {
                            continue;
                        }

                        results.Add(ErrorCompare.UpdatedExceptionValueTag(newException, newException, newException.Id?.RawValue, newPid, oldExceptionValue, newExceptionValue));
                    }
                }

                if (oldExceptions != null)
                {
                    foreach (var oldException in oldExceptions)
                    {
                        uint? exceptionId = oldException.Id?.Value;
                        if (exceptionId == null)
                        {
                            continue;
                        }

                        if (newExceptionIds.Contains(exceptionId.Value))
                        {
                            continue;
                        }

                        results.Add(ErrorCompare.RemovedException(oldException, oldException, oldException.Id?.RawValue, newPid));
                    }
                }
            }

            return results;
        }
    }

    internal static class CompareHelper
    {
        public static bool IsWithinRange(IParamsParamDisplayRange range, decimal? value)
        {
            if (range == null)
            {
                return true;
            }

            if (range.Low != null && value.HasValue && value.Value < range.Low.Value)
            {
                return false;
            }

            if (range.High != null && value.HasValue && value.Value > range.High.Value)
            {
                return false;
            }

            return true;
        }

        public static bool IsWithinDiscreets(IParamsParamMeasurementDiscreets discreets, string value)
        {
            if (discreets == null)
            {
                return false;
            }

            foreach (IParamsParamMeasurementDiscreetsDiscreet discreet in discreets)
            {
                if (discreet?.ValueElement?.Value == null)
                {
                    continue;
                }

                if (discreet.ValueElement.Value == value)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
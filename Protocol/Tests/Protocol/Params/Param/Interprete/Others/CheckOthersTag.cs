namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Others.CheckOthersTag
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

    [Test(CheckId.CheckOthersTag, Category.Param)]
    internal class CheckOthersTag : /*IValidate, ICodeFix, */ICompare
    {
        ////public List<IValidationResult> Validate(ValidatorContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}

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

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                bool oldParamWasAlarmed = oldParam.Alarm?.Monitored?.Value == true;

                var oldOthers = oldParam.Interprete?.Others;
                var newOthers = newParam.Interprete?.Others;
                if (oldOthers == newOthers)
                {
                    continue;
                }

                string newPid = newParam.Id?.RawValue;

                // Check if one or more values are removed
                if (oldOthers != null && oldParamWasAlarmed)
                {
                    var subResults = new List<IValidationResult>();
                    var removedValues = new List<decimal?>();

                    foreach (var oldOther in oldOthers)
                    {
                        decimal? value = oldOther.ValueElement?.Value;
                        if (value == null)
                        {
                            continue;
                        }

                        if (newOthers != null && newOthers.Any(o => o.ValueElement?.Value == value))
                        {
                            continue;
                        }

                        removedValues.Add(value);
                        subResults.Add(ErrorCompare.DeletedValue(oldOther, oldOther, oldOther.ValueElement?.RawValue, newPid));
                    }

                    if (removedValues.Any())
                    {
                        IValidationResult deletedValue = ErrorCompare.DeletedValue(oldOthers, oldOthers, String.Join(", ", removedValues), newPid);
                        if (subResults.Count > 1)
                        {
                            deletedValue.WithSubResults(subResults.ToArray());
                        }

                        results.Add(deletedValue);
                        //continue;
                    }
                }

                // Check if one ore more values are added
                if (newOthers != null && oldParamWasAlarmed)
                {
                    var subResults = new List<IValidationResult>();
                    var addedValues = new List<decimal?>();

                    foreach (var newOther in newOthers)
                    {
                        decimal? value = newOther.ValueElement?.Value;
                        if (value == null)
                        {
                            continue;
                        }

                        if (oldOthers != null && oldOthers.Any(o => o.ValueElement?.Value == value))
                        {
                            continue;
                        }

                        // Check if new value was within previous valid range (or no range defined)
                        var oldRange = oldParam.Interprete?.Range;
                        if (oldRange != null)
                        {
                            if (value < oldRange.Low?.Value || value > oldRange.High?.Value)
                            {
                                // Value is outside the previous valid range
                                continue;
                            }
                        }

                        addedValues.Add(value);
                        subResults.Add(ErrorCompare.AddedOthers(newOther, newOther, newOther.ValueElement?.RawValue, newPid));
                    }

                    if (addedValues.Any())
                    {
                        IValidationResult addedOthers = ErrorCompare.AddedOthers(newOthers, newOthers, String.Join(", ", addedValues), newPid);
                        if (subResults.Count > 1)
                        {
                            addedOthers.WithSubResults(subResults.ToArray());
                        }

                        results.Add(addedOthers);
                        //continue;
                    }
                }

                // Compare existing others
                if (oldOthers != null && newOthers != null)
                {
                    foreach (var oldOther in oldOthers)
                    {
                        decimal? value = oldOther.ValueElement?.Value;
                        if (value == null)
                        {
                            continue;
                        }

                        var newOther = newOthers.FirstOrDefault(o => o.ValueElement?.Value == value);
                        if (newOther == null)
                        {
                            continue;
                        }

                        uint? oldId = oldOther.Id?.Value;
                        uint? newId = newOther.Id?.Value;

                        if (oldId != newId)
                        {
                            results.Add(ErrorCompare.UpdateOtherId(newOther, newOther, Convert.ToString(oldId), Convert.ToString(newId), Convert.ToString(value), newPid));
                        }

                        string oldDisplay = oldOther.Display?.Value;
                        string newDisplay = newOther.Display?.Value;

                        if (!String.Equals(oldDisplay, newDisplay))
                        {
                            results.Add(ErrorCompare.UpdateOtherDisplay(newOther, newOther, oldDisplay, newDisplay, Convert.ToString(value), newPid));
                        }
                    }
                }
            }

            return results;
        }
    }
}
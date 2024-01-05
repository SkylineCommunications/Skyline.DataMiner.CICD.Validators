namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.CheckOptionsAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckOptionsAttribute, Category.Param)]
    internal class CheckOptionsAttribute : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var model = context.ProtocolModel;

            foreach (var param in context.EachParamWithValidId())
            {
                if (param.Alarm?.Options == null)
                {
                    continue;
                }

                var options = param.Alarm.Options;

                ValidateHelper helper = new ValidateHelper(this, context, model, results, param, options);
                helper.Validate();
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol?.Params == null)
            {
                result.Message = "No Params found!";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.EmptyAttribute:
                    {
                        var readNode = (IParamsParam)context.Result.ReferenceNode;
                        var editNode = context.Protocol.Params.Get(readNode);
                        if (editNode == null)
                        {
                            result.Message = "editNode could not be found!";
                            return result;
                        }

                        editNode.Alarm.Options = null;
                        result.Success = true;

                        break;
                    }
                case ErrorIds.UntrimmedAttribute:
                    {
                        var readNode = (IParamsParam)context.Result.ReferenceNode;
                        var editNode = context.Protocol.Params.Get(readNode);
                        if (editNode == null)
                        {
                            result.Message = "editNode could not be found!";
                            return result;
                        }

                        editNode.Alarm.Options.Value = readNode.Alarm.Options.Value.Trim();
                        result.Success = true;

                        break;
                    }
                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                string pid = newParam.Id?.RawValue;

                results.AddIfNotNull(CompareHelper.CompareThresholds(pid, oldParam, newParam));
            }

            return results;
        }
    }

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly IProtocolModel model;
        private readonly List<IValidationResult> results;

        private readonly IParamsParam param;
        private readonly IValueTag<string> alarmOptions;

        internal ValidateHelper(IValidate test, ValidatorContext context, IProtocolModel model, List<IValidationResult> results, IParamsParam param, IValueTag<string> alarmOptions)
        {
            this.test = test;
            this.context = context;
            this.model = model;
            this.results = results;

            this.param = param;
            this.alarmOptions = alarmOptions;
        }

        internal void Validate()
        {
            (GenericStatus status, string rawValue, string value) = GenericTests.CheckBasics(alarmOptions, isRequired: false);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, param, alarmOptions, param.Id.RawValue));
                return;
            }

            // Options
            string[] options = value.Split(';');
            foreach (string option in options)
            {
                string[] optionParts = option.Split(new char[] { ':' }, 2);
                if (optionParts.Length < 2)
                {
                    // TODO: Invalid Option ?
                    continue;
                }

                int propertyNamesCount = 0;
                int propertyValuesCount = 0;
                switch (optionParts[0])
                {
                    case "threshold":
                        ValidateThreshold(optionParts[1]);
                        break;
                    case "propertyNames":
                        propertyNamesCount = ValidatePropertyNames(optionParts[1]);
                        break;
                    case "properties":
                        propertyValuesCount = ValidateProperties(optionParts[1]);
                        break;
                    default:
                        break;
                }

                if (propertyNamesCount != propertyValuesCount)
                {
                    // TODO: property names & values mismatch
                }
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(test, param, alarmOptions, param.Id.RawValue, rawValue));
            }
        }

        internal void ValidateThreshold(string threshold)
        {
            string[] thresholdParts = threshold.Split(',');
            if (thresholdParts.Length != 2)
            {
                // TODO: Invalid threshold ?
                return;
            }

            foreach (string thresholdPart in thresholdParts)
            {
                if (!Int32.TryParse(thresholdPart, out int _))
                {
                    // TODO: Invalid Threshold part ?
                    return;
                }

                // Non Existing Param
                if (!model.TryGetObjectByKey(Mappings.ParamsById, thresholdPart, out IParamsParam referencedParam))
                {
                    results.Add(Error.NonExistingId(test, param, alarmOptions, thresholdPart, param.Id.RawValue));
                    continue;
                }

                // Reference Param Requiring RTDisplay
                IValidationResult rtDisplayError = Error.ReferencedParamRTDisplayExpected(test, param, alarmOptions, referencedParam.Id.RawValue, param.Id.RawValue);
                context.CrossData.RtDisplay.AddParam(referencedParam, rtDisplayError);

                // If column, the table is also expected on containing table
                if (!referencedParam.TryGetTable(context.ProtocolModel.RelationManager, out var table))
                {
                    continue;
                }

                // Containing Table Requiring RTDisplay
                IValidationResult rtDisplayErrorForTable = Error.ReferencedParamRTDisplayExpected(test, param, alarmOptions, table.Id.RawValue, param.Id.RawValue);
                context.CrossData.RtDisplay.AddParam(table, rtDisplayErrorForTable);
            }
        }

        internal int ValidatePropertyNames(string propertyNamesString)
        {
            string[] propertyNames = propertyNamesString.Split(',');

            // TODO: make list doesn't contain any empty or duplicates?

            return propertyNames.Length;
        }

        internal int ValidateProperties(string propertiesString)
        {
            if (String.IsNullOrWhiteSpace(propertiesString))
            {
                // TODO: invalid properties ?
                return 0;
            }

            char propertySeparator = propertiesString[0];
            string[] properties = propertiesString.Split(propertySeparator);
            foreach (string property in properties)
            {
                string[] propertyParts = property.Split('*');
                foreach (string propertyPart in propertyParts)
                {
                    if (!Int32.TryParse(propertyPart, out int _))
                    {
                        // Hard-coded part
                        continue;
                    }

                    // Non Existing Param
                    if (!model.TryGetObjectByKey(Mappings.ParamsById, propertyPart, out IParamsParam referencedParam))
                    {
                        results.Add(Error.NonExistingId(test, param, alarmOptions, propertyPart, param.Id.RawValue));
                        continue;
                    }

                    // Reference Param Requiring RTDisplay
                    IValidationResult rtDisplayError = Error.ReferencedParamRTDisplayExpected(test, param, alarmOptions, referencedParam.Id.RawValue, param.Id.RawValue);
                    context.CrossData.RtDisplay.AddParam(referencedParam, rtDisplayError);

                    // If column, the table is also expected on containing table
                    if (!referencedParam.TryGetTable(context.ProtocolModel.RelationManager, out var table))
                    {
                        continue;
                    }

                    // Containing Table Requiring RTDisplay
                    IValidationResult rtDisplayErrorForTable = Error.ReferencedParamRTDisplayExpected(test, param, alarmOptions, table.Id.RawValue, param.Id.RawValue);
                    context.CrossData.RtDisplay.AddParam(table, rtDisplayErrorForTable);
                }
            }

            return properties.Length;
        }
    }

    internal static class CompareHelper
    {
        public static IValidationResult CompareThresholds(string pid, IParamsParam oldParam, IParamsParam newParam)
        {
            var newOptions = newParam?.Alarm?.Options;
            var oldOptions = oldParam?.Alarm?.Options;

            string newThreshold = ParsingHelper.GetOptionValue(newOptions, "threshold:");
            string oldThreshold = ParsingHelper.GetOptionValue(oldOptions, "threshold:");

            if (newThreshold != null && oldThreshold != null && !newThreshold.Equals(oldThreshold))
            {
                return ErrorCompare.UpdatedThresholdAlarmType(newOptions, newOptions, oldThreshold, pid, newThreshold);
            }

            if (newThreshold != null && oldThreshold == null)
            {
                return ErrorCompare.AddedThresholdAlarmType(newOptions, newOptions, newThreshold, pid);
            }

            if (newThreshold == null && oldThreshold != null)
            {
                // Referenced param here as the option could have been removed or the alarm could have been removed.
                return ErrorCompare.RemovedThresholdAlarmType(newParam, newParam, oldThreshold, pid);
            }

            return null;
        }
    }
}
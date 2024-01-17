namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.Discreet.CheckDependencyValuesAttribute
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

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

    [Test(CheckId.CheckDependencyValuesAttribute, Category.Param)]
    internal class CheckDependencyValuesAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var param in context.EachParamWithValidId())
            {
                var discreets = param?.Measurement?.Discreets;
                if (discreets == null)
                {
                    continue;
                }

                foreach (var discreet in discreets)
                {
                    if (discreet.DependencyValues == null)
                    {
                        continue;
                    }

                    ValidateHelper helper = new ValidateHelper(this, context, results, param, discreet);
                    helper.Validate();
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    {
                        var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
                        var paramEditNode = context.Protocol.Params.Get(paramReadNode);

                        var discreetReadNode = (IParamsParamMeasurementDiscreetsDiscreet)context.Result.ExtraData[ExtraData.Discreet];
                        var discreetEditNode = paramEditNode.Measurement.Discreets.Get(discreetReadNode);

                        discreetEditNode.DependencyValues.Value = discreetReadNode.DependencyValues.Value;
                        result.Success = true;

                        break;
                    }
                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly IProtocolModel model;
        private readonly List<IValidationResult> results;

        private readonly IParamsParam param;
        private readonly IParamsParamMeasurementDiscreetsDiscreet discreet;
        private readonly IValueTag<string> dependencyValues;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results,
            IParamsParam param, IParamsParamMeasurementDiscreetsDiscreet discreet)
        {
            this.test = test;
            this.context = context;
            this.model = context.ProtocolModel;
            this.results = results;

            this.param = param;
            this.discreet = discreet;
            this.dependencyValues = discreet.DependencyValues;
        }

        public void Validate()
        {
            (GenericStatus status, _, _) = GenericTests.CheckBasics(dependencyValues, isRequired: false);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, param, dependencyValues, param.Id.RawValue));
                return;
            }

            if (param.IsContextMenu())
            {
                ValidateContextMenu();
            }
            else if (param.Measurement.Discreets.DependencyId != null)
            {
                ValidateDynamicDropdown();
            }
            else
            {
                // TODO: Excessive attribute ?
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(test, param, dependencyValues, param.Id.RawValue, dependencyValues.RawValue)
                    .WithExtraData(ExtraData.Discreet, discreet));
            }
        }

        private void ValidateContextMenu()
        {
            string[] dependencies = dependencyValues.Value.Split(';');
            foreach (string dependency in dependencies)
            {
                if (String.IsNullOrWhiteSpace(dependency))
                {
                    // TODO: Invalid/empty dependency
                    continue;
                }

                string[] dependencyParts = dependency.Split(new char[] { ':' }, 2);

                // Dependency Param
                ////bool optional = dependencyParts[0].EndsWith("?");
                string dependencyPid = dependencyParts[0].TrimEnd('?');
                ValidateContextMenuDependencyParam(dependencyPid);

                // Dependency Default Value
                if (dependencyParts.Length > 1)
                {
                    ValidateContextMenuDependencyDefaultValue(dependencyParts[1]);
                }
            }
        }

        private void ValidateContextMenuDependencyParam(string dependencyPid)
        {
            // Non Existing Param
            if (!model.TryGetObjectByKey(Mappings.ParamsById, dependencyPid, out IParamsParam dependencyParam))
            {
                results.Add(Error.NonExistingId(test, param, dependencyValues, dependencyPid, param.Id.RawValue));
                return;
            }

            // RTDisplay Expected
            IValidationResult rtDisplayError = Error.ReferencedParamExpectingRTDisplay(test, param, dependencyValues, dependencyPid, param.Id.RawValue);
            context.CrossData.RtDisplay.AddParam(dependencyParam, rtDisplayError);
        }

        private void ValidateContextMenuDependencyDefaultValue(string dependencyDefaultValue)
        {
            ////string pattern = @"\[value:(\d*)\]";                    // Only covering case where a number is provided
            ////string pattern = @"\[value:(.*)\]";                     // Not working as it then goes with greedy matching instead of lazy matching
            ////string pattern = @"\[value:(.*?)\]";                    // working with lazy matching

            ////string pattern = @"\[(.*?):(.*?)\]";                    // validating both the placeholder title and value (only for placeholders expecting 2 parts)
            ////string pattern = @"\[(?<title>.*?):(?<value>.*?)\]";    // with group names

            string pattern = @"\[(?<placeholder>.*?)\]";    // Validating ALL placeholders
            MatchCollection matches = Regex.Matches(dependencyDefaultValue, pattern);
            foreach (Match match in matches)
            {
                string[] placeholderParts = match.Groups["placeholder"].Value.Split(':');
                string placeholderTitle = placeholderParts[0];

                // Placeholder without separator
                if (placeholderTitle.Equals("this element", StringComparison.OrdinalIgnoreCase) ||
                    placeholderTitle.Equals("tableIndex", StringComparison.OrdinalIgnoreCase) ||
                    placeholderTitle.Equals("primaryKey", StringComparison.OrdinalIgnoreCase) ||
                    placeholderTitle.Equals("displayTableIndex", StringComparison.OrdinalIgnoreCase) ||
                    placeholderTitle.Equals("displayKey", StringComparison.OrdinalIgnoreCase))
                {
                    if (placeholderParts.Length != 1)
                    {
                        // TODO: Invalid placeholder format for placeholder with title '{placeholderTitle}'
                    }

                    continue;
                }

                // Placeholders with 1 separator
                if (placeholderTitle.Equals("var", StringComparison.OrdinalIgnoreCase) ||
                    placeholderTitle.Equals("cardVar", StringComparison.OrdinalIgnoreCase) ||
                    placeholderTitle.Equals("pageVar", StringComparison.OrdinalIgnoreCase) ||
                    placeholderTitle.Equals("property", StringComparison.OrdinalIgnoreCase) ||
                    placeholderTitle.Equals("value", StringComparison.OrdinalIgnoreCase) ||
                    placeholderTitle.Equals("tableIndex", StringComparison.OrdinalIgnoreCase))
                {
                    if (placeholderParts.Length != 2)
                    {
                        // TODO: Invalid placeholder format for placeholder with title '{placeholderTitle}'
                        continue;
                    }

                    if (placeholderTitle.Equals("value", StringComparison.OrdinalIgnoreCase))
                    {
                        ValidateContextMenuDependencyDefaultValuePlaceHolderValue(placeholderParts[1]);
                    }

                    continue;
                }

                // Placeholders with 2 separators
                if (placeholderTitle.Equals("param", StringComparison.OrdinalIgnoreCase))
                {
                    if (placeholderParts.Length != 3)
                    {
                        // TODO: Invalid placeholder format for placeholder with title '{placeholderTitle}'
                        continue;
                    }

                    continue;
                }

                // TODO: Invalid placeholder title
            }
        }

        private void ValidateContextMenuDependencyDefaultValuePlaceHolderValue(string pid)
        {
            // Non Existing Param
            if (!model.TryGetObjectByKey(Mappings.ParamsById, pid, out IParamsParam dependencyParam))
            {
                results.Add(Error.NonExistingId(test, param, dependencyValues, pid, param.Id.RawValue));
                return;
            }

            // RTDisplay Expected
            IValidationResult rtDisplayError = Error.ReferencedParamExpectingRTDisplay(test, param, dependencyValues, pid, param.Id.RawValue);
            context.CrossData.RtDisplay.AddParam(dependencyParam, rtDisplayError);
        }

        private void ValidateDynamicDropdown()
        {
            // TODO: Note that if Discreets@dependencyId is not null,
            // - either none of the discreet tag should have dependencyValues,
            // - either all of them should
            // There is probably not much more validation to make since other than that the attribute should contain a ';' separated list of strings.
        }
    }

    internal enum ExtraData { Discreet }
}
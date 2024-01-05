namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.ExtraTab.Tab.CheckParameterAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Edit;
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

    [Test(CheckId.CheckParameterAttribute, Category.TreeControl)]
    internal class CheckParameterAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var model = context.ProtocolModel;

            foreach (ITreeControlsTreeControl treeControl in context.EachTreeControlWithValidParameterId())
            {
                if (context.ProtocolModel.IsExportedProtocolModel && !treeControl.MainParameterExists(context.ProtocolModel))
                {
                    continue;
                }

                if (treeControl.ExtraTabs == null)
                {
                    continue;
                }

                foreach (var extraTab in treeControl.ExtraTabs)
                {
                    ValidateHelper helper = new ValidateHelper(this, context, model, results, treeControl, extraTab);
                    helper.Validate();
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol?.TreeControls == null)
            {
                result.Message = "No TreeControls found!";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    {
                        var readNode = (ITreeControlsTreeControlExtraTabsTab)context.Result.ReferenceNode;

                        TreeControlsTreeControlExtraTabsTab editNode = null;
                        foreach (var treeControl in context.Protocol.TreeControls)
                        {
                            editNode = treeControl.ExtraTabs?.Get(readNode);
                            if (editNode != null)
                            {
                                break;
                            }
                        }

                        if (editNode == null)
                        {
                            result.Message = "editNode could not be found!";
                            return result;
                        }

                        editNode.Parameter.Value = readNode.Parameter.Value.Trim();
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

        private readonly ITreeControlsTreeControl treeControl;
        private readonly ITreeControlsTreeControlExtraTabsTab extraTab;

        internal ValidateHelper(IValidate test, ValidatorContext context, IProtocolModel model, List<IValidationResult> results, ITreeControlsTreeControl treeControl, ITreeControlsTreeControlExtraTabsTab extraTab)
        {
            this.test = test;
            this.context = context;
            this.model = model;
            this.results = results;
            this.treeControl = treeControl;
            this.extraTab = extraTab;
        }

        internal void Validate()
        {
            var type = extraTab.Type?.Value;
            bool isRequired = String.Equals(type, "parameters", StringComparison.OrdinalIgnoreCase)
                || String.Equals(type, "relation", StringComparison.OrdinalIgnoreCase)
                || String.Equals(type, "summary", StringComparison.OrdinalIgnoreCase);

            var parameterAttribute = extraTab.Parameter;
            (GenericStatus status, string rawValue, string value) = GenericTests.CheckBasics(parameterAttribute, isRequired);

            // TODO: Excessive
            if (!isRequired)
            {
                ////if (parameter != null)
                ////{
                ////    results.Add(Error.ExcessiveAttribute(this, treeControl, parameter, treeControl.ParameterId.RawValue));
                ////    continue;
                ////}

                return;
            }

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingAttribute(test, treeControl, extraTab, treeControl.ParameterId.RawValue));
                return;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, treeControl, parameterAttribute, treeControl.ParameterId.RawValue));
                return;
            }

            // Invalid: Will actually never happen as long as the Model doesn't consider xsd types defined via regex.
            ////if (status.HasFlag(GenericStatus.Invalid))
            ////{
            ////    results.Add(Error.InvalidValue(this, treeControl, parameter, rawValue, treeControl.ParameterId.RawValue));
            ////    continue;
            ////}

            // Invalid
            string[] pids;
            if (String.Equals(type, "parameters", StringComparison.OrdinalIgnoreCase))
            {
                // If type="parameters", parameter attribute should contains a comma-separated list of PIDs.
                pids = value.Split(',');
            }
            else
            {
                // If type="relation", parameter attribute should contains a single columnPID.
                // If type="summary", parameter attribute should contains a single tablePID.
                pids = new[] { value };
            }

            List<IValidationResult> invalidSubResults = new List<IValidationResult>();
            foreach (string pid in pids)
            {
                if (!UInt32.TryParse(pid, out _))
                {
                    invalidSubResults.Add(Error.InvalidValue(test, treeControl, parameterAttribute, pid, treeControl.ParameterId.RawValue));
                    continue;
                }

                // Non Existing Param
                if (!model.TryGetObjectByKey(Mappings.ParamsById, pid, out IParamsParam referencedParam))
                {
                    invalidSubResults.Add(Error.NonExistingId(test, treeControl, parameterAttribute, pid));
                    continue;
                }

                // More checks depending on type
                switch (type)
                {
                    case "parameters":
                        break;
                    case "relation":
                        {
                            if (!referencedParam.TryGetTable(context.ProtocolModel.RelationManager, out var tableParam))
                            {
                                // TODO: parameter is expected to be a column
                                continue;
                            }

                            // Table Requiring RTDisplay
                            IValidationResult rtDisplayErrorForTable = Error.ReferencedParamExpectingRTDisplay(test, treeControl, parameterAttribute, tableParam.Id.RawValue);
                            context.CrossData.RtDisplay.AddParam(tableParam, rtDisplayErrorForTable);
                        }
                        break;
                    case "summary":
                        break;
                    default:
                        break;
                }

                // Param Requiring RTDisplay
                IValidationResult rtDisplayError = Error.ReferencedParamExpectingRTDisplay(test, treeControl, parameterAttribute, referencedParam.Id.RawValue);
                context.CrossData.RtDisplay.AddParam(referencedParam, rtDisplayError);
            }

            if (invalidSubResults.Count == 1)
            {
                results.Add(invalidSubResults[0]);
                return;
            }

            if (invalidSubResults.Count > 1)
            {
                IValidationResult invalidValue = Error.InvalidValue(test, treeControl, parameterAttribute, rawValue, treeControl.ParameterId.RawValue);
                invalidValue.WithSubResults(invalidSubResults.ToArray());
                results.Add(invalidValue);
                return;
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(test, extraTab, parameterAttribute, treeControl.ParameterId.RawValue, rawValue));
                return;
            }
        }
    }
}
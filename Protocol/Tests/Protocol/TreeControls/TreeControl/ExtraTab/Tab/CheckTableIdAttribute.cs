namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.ExtraTab.Tab.CheckTableIdAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Edit;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckTableIdAttribute, Category.TreeControl)]
    internal class CheckTableIdAttribute : IValidate, ICodeFix
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
                    var tableId = extraTab.TableId;
                    (GenericStatus status, string rawValue, uint? value) = GenericTests.CheckBasics(tableId, isRequired: true);

                    // Missing
                    if (status.HasFlag(GenericStatus.Missing))
                    {
                        results.Add(Error.MissingAttribute(this, treeControl, extraTab, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    // Empty
                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyAttribute(this, treeControl, tableId, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    // Invalid
                    if (status.HasFlag(GenericStatus.Invalid))
                    {
                        results.Add(Error.InvalidValue(this, treeControl, tableId, rawValue, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    // Non-Existing PID
                    var paramIdString = Convert.ToString(value);
                    if (!model.TryGetObjectByKey(Mappings.ParamsById, paramIdString, out IParamsParam _))
                    {
                        results.Add(Error.NonExistingId(this, treeControl, tableId, paramIdString));
                        continue;
                    }

                    // TODO: Check if the table is part of the treeControl Hierarchy
                    // No need to check for RTDisplay cause this will then be covered by the Hierarchy checks

                    // Untrimmed
                    if (status.HasFlag(GenericStatus.Untrimmed))
                    {
                        results.Add(Error.UntrimmedAttribute(this, extraTab, tableId, treeControl.ParameterId.RawValue, rawValue));
                    }
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol.TreeControls == null)
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

                        if (editNode != null)
                        {
                            editNode.TableId.Value = readNode.TableId.Value;
                            result.Success = true;
                        }

                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }
    }
}
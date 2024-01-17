namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.Hierarchy.Table.CheckParentAttribute
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

    [Test(CheckId.CheckParentAttribute, Category.TreeControl)]
    internal class CheckParentAttribute : IValidate, ICodeFix
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

                if (treeControl.Hierarchy == null)
                {
                    continue;
                }

                for (int i = 0; i < treeControl.Hierarchy.Count; i++)
                {
                    var table = treeControl.Hierarchy[i];
                    var parent = table.ParentAttribute;

                    if (i == 0)
                    {
                        if (parent != null)
                        {
                            results.Add(Error.ExcessiveAttribute(this, treeControl, table, treeControl.ParameterId.RawValue));
                        }

                        continue;
                    }

                    (GenericStatus status, string rawValue, uint? value) = GenericTests.CheckBasics(parent, isRequired: true);

                    if (status.HasFlag(GenericStatus.Missing))
                    {
                        results.Add(Error.MissingAttribute(this, treeControl, table, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyAttribute(this, treeControl, parent, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    if (status.HasFlag(GenericStatus.Invalid))
                    {
                        results.Add(Error.InvalidValue(this, treeControl, parent, rawValue, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    // Check if parameter links to an existing Table.
                    var paramIdString = Convert.ToString(value);
                    if (!model.TryGetObjectByKey(Mappings.ParamsById, paramIdString, out IParamsParam _))
                    {
                        results.Add(Error.NonExistingId(this, treeControl, parent, paramIdString));
                        continue;
                    }

                    if (status.HasFlag(GenericStatus.Untrimmed))
                    {
                        results.Add(Error.UntrimmedAttribute(this, table, parent, treeControl.ParameterId.RawValue, rawValue));
                        continue;
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
                        var readNode = (ITreeControlsTreeControlHierarchyTable)context.Result.ReferenceNode;

                        TreeControlsTreeControlHierarchyTable editNode = null;
                        foreach (var treeControl in context.Protocol.TreeControls)
                        {
                            editNode = treeControl.Hierarchy?.Get(readNode);

                            if (editNode != null)
                            {
                                break;
                            }
                        }

                        if (editNode != null)
                        {
                            editNode.ParentAttribute.Value = readNode.ParentAttribute.Value;
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
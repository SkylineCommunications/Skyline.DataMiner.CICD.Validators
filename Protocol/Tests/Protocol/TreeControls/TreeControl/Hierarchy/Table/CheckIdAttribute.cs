namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.Hierarchy.Table.CheckIdAttribute
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

    [Test(CheckId.CheckIdAttribute, Category.TreeControl)]
    internal class CheckIdAttribute : IValidate, ICodeFix
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

                ValidateHelper helper = new ValidateHelper(this, context, model, results, treeControl);
                helper.Validate();
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
                            editNode.Id.Value = readNode.Id.Value;
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

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly IProtocolModel model;
        private readonly List<IValidationResult> results;
        private readonly ITreeControlsTreeControl treeControl;

        private readonly IValueTag<string> path;

        internal ValidateHelper(IValidate test, ValidatorContext context, IProtocolModel model, List<IValidationResult> results, ITreeControlsTreeControl treeControl)
        {
            this.test = test;
            this.context = context;
            this.model = model;
            this.results = results;
            this.treeControl = treeControl;

            path = treeControl.Hierarchy.Path;
        }

        internal void Validate()
        {
            foreach (var table in treeControl.Hierarchy)
            {
                var id = table.Id;
                (GenericStatus status, string rawValue, uint? value) = GenericTests.CheckBasics(id, isRequired: true);

                // Missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(test, treeControl, table, treeControl.ParameterId.RawValue));
                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(test, treeControl, id, treeControl.ParameterId.RawValue));
                    continue;
                }

                // Invalid
                if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(rawValue))
                {
                    results.Add(Error.InvalidValue(test, treeControl, id, rawValue, treeControl.ParameterId.RawValue));
                    continue;
                }

                // TreeControl Table Params
                if (!CheckTableParam(id, tablePid: Convert.ToString(value)))
                {
                    continue;
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(test, table, id, treeControl.ParameterId.RawValue, rawValue));
                    continue;
                }
            }
        }

        private bool CheckTableParam(IValueTag<uint?> idAttribute, string tablePid)
        {
            // Non Existing Param
            if (!model.TryGetObjectByKey(Mappings.ParamsById, tablePid, out IParamsParam tableParam))
            {
                results.Add(Error.NonExistingId(test, treeControl, idAttribute, tablePid));
                return false;
            }

            // TODO: Param Wrong Type
            //if (!tableParam.IsTable())
            //{
            //    results.Add(Error.ReferencedParamWrongType(test, treeControl, path, tableParam.Type?.RawValue, tablePid));
            //    return false;
            //}

            // Param Requiring RTDisplay
            IValidationResult rtDisplayError = Error.ReferencedParamExpectingRTDisplay(test, treeControl, path, tableParam.Id.RawValue);
            context.CrossData.RtDisplay.AddParam(tableParam, rtDisplayError);

            return true;
        }
    }
}
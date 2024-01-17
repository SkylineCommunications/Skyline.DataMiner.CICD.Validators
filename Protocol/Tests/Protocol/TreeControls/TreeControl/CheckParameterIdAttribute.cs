namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.CheckParameterIdAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckParameterIdAttribute, Category.TreeControl)]
    internal class CheckParameterIdAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            IProtocolModel model = context.ProtocolModel;
            var protocol = model?.Protocol;

            if (protocol?.TreeControls == null)
            {
                return results;
            }

            foreach (var treeControl in protocol.TreeControls)
            {
                if (context.ProtocolModel.IsExportedProtocolModel && !treeControl.MainParameterExists(context.ProtocolModel))
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
                        var readNode = (ITreeControlsTreeControl)context.Result.ReferenceNode;
                        var editNode = context.Protocol.TreeControls.Get(readNode);

                        editNode.ParameterId.Value = readNode.ParameterId.Value;
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

        internal ValidateHelper(IValidate test, ValidatorContext context, IProtocolModel model, List<IValidationResult> results, ITreeControlsTreeControl treeControl)
        {
            this.test = test;
            this.context = context;
            this.model = model;
            this.results = results;
            this.treeControl = treeControl;
        }

        internal void Validate()
        {
            var parameterId = treeControl.ParameterId;
            (GenericStatus status, string rawValue, uint? value) = GenericTests.CheckBasics(parameterId, isRequired: true);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingAttribute(test, treeControl, treeControl));
                return;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, treeControl, parameterId));
                return;
            }

            // Invalid Value
            if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(rawValue))
            {
                results.Add(Error.InvalidValue(test, treeControl, parameterId, rawValue));
                return;
            }

            // TreeControl Param
            CheckTreeControlParam(Convert.ToString(value), parameterId);

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(test, treeControl, parameterId, rawValue));
                return;
            }
        }

        private void CheckTreeControlParam(string pid, IReadable parameterId)
        {
            // Non Existing Param
            if (!model.TryGetObjectByKey(Mappings.ParamsById, pid, out IParamsParam treeControlParam))
            {
                results.Add(Error.NonExistingId(test, treeControl, parameterId, pid));
                return;
            }

            // Param Wrong Type
            if (treeControlParam.Type?.Value != EnumParamType.Dummy &&
                treeControlParam.Type?.Value != EnumParamType.Read)
            {
                results.Add(Error.ReferencedParamWrongType(test, treeControl, parameterId, treeControlParam.Type?.RawValue, pid));
            }

            // Param Requiring RTDisplay
            IValidationResult rtDisplayError = Error.ReferencedParamExpectingRTDisplay(test, treeControl, parameterId, pid);
            context.CrossData.RtDisplay.AddParam(treeControlParam, rtDisplayError);
        }
    }
}
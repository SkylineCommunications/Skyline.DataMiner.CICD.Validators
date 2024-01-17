namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.ExtraDetails.LinkedDetails.CheckDetailsTableIdAttribute
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

    [Test(CheckId.CheckDetailsTableIdAttribute, Category.TreeControl)]
    internal class CheckDetailsTableIdAttribute : IValidate, ICodeFix
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (ITreeControlsTreeControl treeControl in context.EachTreeControlWithValidParameterId())
            {
                if (context.ProtocolModel.IsExportedProtocolModel && !treeControl.MainParameterExists(context.ProtocolModel))
                {
                    continue;
                }

                if (treeControl.ExtraDetails == null)
                {
                    continue;
                }

                foreach (var linkedDetails in treeControl.ExtraDetails)
                {
                    // Further Validation
                    ValidateHelper helper = new ValidateHelper(this, context, results, treeControl, linkedDetails);
                    helper.Validate();
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
                        var treeControlReadNode = (ITreeControlsTreeControl)context.Result.ReferenceNode;
                        var treeControlEditNode = context.Protocol.TreeControls.Get(treeControlReadNode);

                        var linkedDetailsReadNode = (ITreeControlsTreeControlExtraDetailsLinkedDetails)context.Result.ExtraData[ExtraData.LinkedDetails];
                        var linkedDetailsEditNode = treeControlEditNode.ExtraDetails.Get(linkedDetailsReadNode);

                        linkedDetailsEditNode.DetailsTableId.Value = linkedDetailsReadNode.DetailsTableId.Value;
                        result.Success = true;

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
        private readonly RelationManager relationManager;
        private readonly List<IValidationResult> results;

        private readonly ITreeControlsTreeControl treeControl;
        private readonly ITreeControlsTreeControlExtraDetailsLinkedDetails linkedDetails;
        private readonly IValueTag<uint?> detailsTableId;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, ITreeControlsTreeControl treeControl, ITreeControlsTreeControlExtraDetailsLinkedDetails linkedDetails)
        {
            this.test = test;
            this.context = context;
            this.model = context.ProtocolModel;
            this.relationManager = model.RelationManager;
            this.results = results;

            this.treeControl = treeControl;
            this.linkedDetails = linkedDetails;
            this.detailsTableId = linkedDetails.DetailsTableId;
        }

        public void Validate()
        {
            (GenericStatus status, string paramIdRaw, uint? paramId) = GenericTests.CheckBasics(detailsTableId, isRequired: true);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingAttribute(test, treeControl, detailsTableId, treeControl.ParameterId.RawValue));
                return;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, treeControl, detailsTableId, treeControl.ParameterId.RawValue));
                return;
            }

            // Invalid
            if (status.HasFlag(GenericStatus.Invalid))
            {
                results.Add(Error.InvalidValue(test, treeControl, detailsTableId, paramIdRaw, treeControl.ParameterId.RawValue));
                return;
            }

            // Param Validation
            ValidateTableParam(Convert.ToString(paramId));

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(test, treeControl, detailsTableId, treeControl.ParameterId.RawValue, paramIdRaw)
                    .WithExtraData(ExtraData.LinkedDetails, linkedDetails));
            }
        }

        private void ValidateTableParam(string paramId)
        {
            // Non Existing PID
            if (!model.TryGetObjectByKey(Mappings.ParamsById, paramId, out IParamsParam param))
            {
                results.Add(Error.NonExistingId(test, treeControl, detailsTableId, paramId));
                return;
            }

            // Param (can either be a tablePid, either a fkColumnPid)
            IParamsParam tableParam;
            if (param.IsTable())
            {
                tableParam = param;
            }
            else if (param.TryGetTable(relationManager, out tableParam))
            {
                // TODO: Check param is a FK to proper table
            }
            else
            {
                // TODO: Wrong type (neither a tablePid, neither a columnPid)
                return;
            }

            // RTDisplay Expected
            IValidationResult rtDisplayError = Error.ReferencedTableExpectingRTDisplay(test, treeControl, detailsTableId, tableParam.Id.RawValue);
            context.CrossData.RtDisplay.AddParam(tableParam, rtDisplayError);
        }
    }

    internal enum ExtraData { LinkedDetails };
}
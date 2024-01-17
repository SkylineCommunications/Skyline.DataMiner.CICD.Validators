namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.ExtraDetails.LinkedDetails.CheckDiscreetColumnIdAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckDiscreetColumnIdAttribute, Category.TreeControl)]
    internal class CheckDiscreetColumnIdAttribute : IValidate, ICodeFix
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

                if (treeControl.ExtraDetails == null)
                {
                    continue;
                }

                foreach (var linkedDetails in treeControl.ExtraDetails)
                {
                    var discreetColumnId = linkedDetails.DiscreetColumnId;
                    (GenericStatus status, string columnPidRaw, uint? columnPid) = GenericTests.CheckBasics(discreetColumnId, isRequired: true);

                    // Missing
                    if (status.HasFlag(GenericStatus.Missing))
                    {
                        results.Add(Error.MissingAttribute(this, treeControl, linkedDetails, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    // Empty
                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyAttribute(this, treeControl, discreetColumnId, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    // Invalid
                    if (status.HasFlag(GenericStatus.Invalid))
                    {
                        results.Add(Error.InvalidValue(this, treeControl, discreetColumnId, columnPidRaw, treeControl.ParameterId.RawValue));
                        continue;
                    }

                    // Non Existing PID
                    var columnPidString = Convert.ToString(columnPid);
                    if (!model.TryGetObjectByKey(Mappings.ParamsById, columnPidString, out IParamsParam columnParam))
                    {
                        results.Add(Error.NonExistingId(this, treeControl, discreetColumnId, columnPidRaw));
                        continue;
                    }

                    // RTDisplay Expected
                    IValidationResult rtDisplayError = Error.ReferencedColumnExpectingRTDisplay(this, treeControl, discreetColumnId, columnPidRaw);
                    context.CrossData.RtDisplay.AddParam(columnParam, rtDisplayError);

                    // Untrimmed
                    if (status.HasFlag(GenericStatus.Untrimmed))
                    {
                        results.Add(Error.UntrimmedAttribute(this, treeControl, discreetColumnId, treeControl.ParameterId.RawValue, columnPidRaw)
                            .WithExtraData(ExtraData.LinkedDetails, linkedDetails));
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
                        var treeControlReadNode = (ITreeControlsTreeControl)context.Result.ReferenceNode;
                        var treeControlEditNode = context.Protocol.TreeControls.Get(treeControlReadNode);

                        var linkedDetailsReadNode = (ITreeControlsTreeControlExtraDetailsLinkedDetails)context.Result.ExtraData[ExtraData.LinkedDetails];
                        var linkedDetailsEditNode = treeControlEditNode.ExtraDetails.Get(linkedDetailsReadNode);

                        linkedDetailsEditNode.DiscreetColumnId.Value = linkedDetailsReadNode.DiscreetColumnId.Value;
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

    internal enum ExtraData { LinkedDetails };
}
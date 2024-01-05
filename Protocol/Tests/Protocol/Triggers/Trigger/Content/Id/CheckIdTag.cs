namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Triggers.Trigger.Content.Id.CheckIdTag
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

    [Test(CheckId.CheckIdTag, Category.Trigger)]
    internal class CheckIdTag : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (ITriggersTrigger trigger in context.EachTriggerWithValidId())
            {
                if (trigger.Content == null || trigger.Content.Count < 1)
                {
                    results.Add(Error.MissingTag(this, trigger, trigger, trigger.Id.RawValue));
                    continue;
                }

                foreach (var idElement in trigger.Content)
                {
                    (GenericStatus status, string targetRawId, uint? targetId) = GenericTests.CheckBasics(idElement, isRequired: false);

                    // Empty
                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyTag(this, idElement, idElement, trigger.Id.RawValue));
                        continue;
                    }

                    // Invalid
                    if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(targetRawId))
                    {
                        results.Add(Error.InvalidValue(this, idElement, idElement, targetRawId, trigger.Id.RawValue));
                        continue;
                    }

                    // Non-Existing Response
                    switch (trigger.Type?.Value)
                    {
                        case Skyline.DataMiner.CICD.Models.Protocol.Enums.EnumTriggerType.Trigger:
                            if (!context.ProtocolModel.TryGetObjectByKey<ITriggersTrigger>(Mappings.TriggersById, Convert.ToString(targetId), out _))
                            {
                                results.Add(Error.NonExistingId(this, idElement, idElement, "Trigger", targetRawId, trigger.Id.RawValue));
                                continue;
                            }
                            break;
                        case Skyline.DataMiner.CICD.Models.Protocol.Enums.EnumTriggerType.Action:
                            if (!context.ProtocolModel.TryGetObjectByKey<IActionsAction>(Mappings.ActionsById, Convert.ToString(targetId), out _))
                            {
                                results.Add(Error.NonExistingId(this, idElement, idElement, "Action", targetRawId, trigger.Id.RawValue));
                                continue;
                            }
                            break;
                    }

                    // Untrimmed
                    if (status.HasFlag(GenericStatus.Untrimmed))
                    {
                        results.Add(Error.UntrimmedTag(this, idElement, idElement, trigger.Id.RawValue, targetRawId));
                        continue;
                    }
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedTag:
                    if (!(context.Result.ReferenceNode is ITriggersTriggerContentId readContentId))
                    {
                        result.Message = "context.Result.Node is not of type ITriggersTriggerContentId";
                        break;
                    }

                    if (context.Protocol?.Triggers == null)
                    {
                        result.Message = "context?.Protocol.Triggers == null";
                        break;
                    }

                    foreach (TriggersTrigger trigger in context.Protocol.Triggers)
                    {
                        var editContentId = trigger.Content.Get(readContentId);
                        if (editContentId == null)
                        {
                            continue;
                        }

                        editContentId.Value = readContentId.Value;
                        result.Success = true;
                    }

                    if (!result.Success)
                    {
                        result.Message = "Matching 'Trigger/Content/Id' tag not found.";
                    }

                    break;
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
}
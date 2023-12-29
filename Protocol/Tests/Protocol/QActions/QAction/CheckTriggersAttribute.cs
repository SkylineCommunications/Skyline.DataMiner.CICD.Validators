namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckTriggersAttribute
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckTriggersAttribute, Category.QAction)]
    internal class CheckTriggersAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IQActionsQAction qAction in context.EachQActionWithValidId())
            {
                var triggers = qAction.Triggers;
                (GenericStatus status, _, _) = GenericTests.CheckBasics(triggers, isRequired: true);

                // Check options
                var options = qAction.GetOptions();
                bool isPrecompile = options?.HasPrecompile == true;
                bool isTriggerdByGroup = options?.HasGroup == true;

                // No need to check this QAction further if it's a preCompile QAction
                if (isPrecompile)
                {
                    // TODO: an excessive attribute should be thrown if triggers attribute is present
                    continue;
                }

                // No need to check this QAction further if triggered by a multi-threaded timer (qactionBefore)
                if (qAction.GetTriggerTimers(context.ProtocolModel.RelationManager).Any())
                {
                    // TODO: an excessive attribute could potentially be thrown if triggers attribute is present
                    // Indeed, when a QAction is directly triggered by the timer via qactionBefore or qactionAfter, it doesn't really make sense to also trigger it on a group.
                    // The QAction triggered on group is meant to get data directly from that group while triggering it with qactionBefore or qactionAfter won't have that data available so such scenario shouldn't happen.
                    continue;
                }

                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(this, qAction, qAction, qAction.Id.RawValue));
                    continue;
                }

                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, qAction, qAction, qAction.Id.RawValue));
                    continue;
                }

                var triggerIds = triggers.Value.Split(';');

                var reportedDuplicateTriggerIds = new List<string>();

                IValidationResult parentInvalidAttributeError = null;

                foreach (var triggerId in triggerIds)
                {
                    (GenericStatus valueStatus, uint convertedValue) = GenericTests.CheckBasics<uint>(triggerId);

                    // Invalid values
                    if (valueStatus.HasFlag(GenericStatus.Invalid))
                    {
                        if (parentInvalidAttributeError == null)
                        {
                            parentInvalidAttributeError = Error.InvalidAttribute(this, qAction, qAction, triggers.Value, qAction.Id.RawValue);
                            results.Add(parentInvalidAttributeError);
                        }

                        // Only add sub error in case the full triggers value is not this triggerId
                        if (triggerId != triggers.Value)
                        {
                            parentInvalidAttributeError.SubResults.Add(Error.InvalidAttribute(this, qAction, qAction, triggerId, qAction.Id.RawValue));
                        }

                        continue;
                    }

                    // Duplicate values
                    if (triggerIds.Count(t => t == triggerId) > 1 && !reportedDuplicateTriggerIds.Contains(triggerId))
                    {
                        reportedDuplicateTriggerIds.Add(triggerId);

                        results.Add(Error.DuplicateId(this, qAction, qAction, triggerId, qAction.Id.RawValue));
                        continue;
                    }

                    if (isTriggerdByGroup)
                    {
                        if (!context.ProtocolModel.TryGetObjectByKey<IGroupsGroup>(Mappings.GroupsById, triggerId, out _))
                        {
                            results.Add(Error.NonExistingGroup(this, qAction, qAction, triggerId, qAction.Id.RawValue));
                            continue;
                        }
                    }
                    else
                    {
                        if (!context.ProtocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, triggerId, out _) && !ParamHelper.IsGeneralParam(convertedValue))
                        {
                            results.Add(Error.NonExistingParam(this, qAction, qAction, triggerId, qAction.Id.RawValue));
                            continue;
                        }
                    }
                }
            }

            return results;
        }

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {

        ////        default:
        ////            result.Message = String.Format("This error ({0}) isn't implemented.", context.Result.ErrorId.ToString());
        ////            break;
        ////    }

        ////    return result;
        ////}

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }
}
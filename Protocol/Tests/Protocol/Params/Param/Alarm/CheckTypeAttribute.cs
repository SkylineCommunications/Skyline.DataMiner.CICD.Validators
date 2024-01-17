namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.CheckTypeAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckTypeAttribute, Category.Param)]
    internal class CheckTypeAttribute : /*IValidate, ICodeFix,*/ ICompare
    {
        ////public List<IValidationResult> Validate(ValidatorContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {
        ////        default:
        ////            result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
        ////            break;
        ////    }

        ////    return result;
        ////}

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();
            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                if (results.AddIfNotNull(CompareHelper.CheckUpdatedNormalization(context, results, oldParam, newParam)))
                {
                    continue;
                }

                if (results.AddIfNotNull(CompareHelper.CheckAddedNormalization(oldParam, newParam)))
                {
                    continue;
                }
            }

            return results;
        }
    }

    internal static class CompareHelper
    {
        public static IValidationResult CheckUpdatedNormalization(MajorChangeCheckContext context, List<IValidationResult> results, IParamsParam oldParam, IParamsParam newParam)
        {
            // No breaking change if alarming is not enabled
            bool bMonitored = oldParam.Alarm?.Monitored?.Value != null && oldParam.Alarm.Monitored.Value.Value;
            if (!bMonitored)
            {
                return null;
            }

            string oldAlarmType = oldParam.Alarm?.Type?.Value;
            string newAlarmType = newParam.Alarm?.Type?.Value;

            uint? id = newParam.Id?.Value;
            if (id == null)
            {
                return null;
            }

            if (String.IsNullOrEmpty(oldAlarmType))
            {
                return null;
            }

            if (String.IsNullOrWhiteSpace(newAlarmType))
            {
                results.Add(ErrorCompare.RemovedNormalizationAlarmType(newParam, newParam, oldAlarmType, Convert.ToString(id)));
                return null;
            }

            if (oldAlarmType != newAlarmType)
            {
                if (NeedsMajorChangeWorkAround(oldAlarmType, newAlarmType))
                {
                    if (!HasMajorChangeWorkAround(context, oldAlarmType, id))
                    {
                        return ErrorCompare.UpdatedNormalizationAlarmType(newParam, newParam, oldAlarmType, Convert.ToString(id), newAlarmType);
                    }
                }
                else
                {
                    return ErrorCompare.UpdatedNormalizationAlarmType(newParam, newParam, oldAlarmType, Convert.ToString(id), newAlarmType);
                }
            }

            return null;
        }

        public static IValidationResult CheckAddedNormalization(IParamsParam oldParam, IParamsParam newParam)
        {
            // Check if in old version the parameter is monitored already.
            bool? oldMonitoringState = oldParam.Alarm?.Monitored?.Value;

            if (oldMonitoringState == null || !oldMonitoringState.Value)
            {
                return null;
            }

            string oldTypeValue = oldParam.Alarm?.Type?.Value;
            string newTypeValue = newParam.Alarm?.Type?.Value;

            if (!String.IsNullOrWhiteSpace(oldTypeValue) && oldTypeValue.Contains(":"))
            {
                return null;
            }

            if (!String.IsNullOrWhiteSpace(newTypeValue) && newTypeValue.Contains(":"))
            {
                string pid = newParam.Id?.RawValue;

                if (pid == null)
                {
                    return null;
                }

                return ErrorCompare.AddedNormalizationAlarmType(newParam.Alarm.Type, newParam.Alarm.Type, newTypeValue, pid);
            }

            return null;
        }

        private static bool NeedsMajorChangeWorkAround(string oldAlarmType, string newAlarmType)
        {
            return oldAlarmType.Contains(":") && !newAlarmType.Contains(":");
        }

        private static bool HasMajorChangeWorkAround(MajorChangeCheckContext context, string alarmType, uint? pid)
        {
            string[] splitAlarmType = alarmType.Split(':');
            uint? normalizeValuePid = null;
            if (splitAlarmType.Length > 1)
            {
                string[] splitNormalizeOptions = splitAlarmType[1].Split(',');
                if (UInt32.TryParse(splitNormalizeOptions[0], out uint normalizedNoneNullable))
                {
                    normalizeValuePid = normalizedNoneNullable;
                }
            }

            var triggers = context?.NewProtocolModel?.Protocol?.Triggers;
            if (triggers == null)
            {
                return false;
            }

            RelationManager relationManager = context.NewProtocolModel.RelationManager;
            foreach (var trigger in triggers)
            {
                EnumTriggerTime? triggerTime = EnumTriggerTimeConverter.Convert(trigger.Time?.Value);
                if (!triggerTime.HasValue || triggerTime.Value != EnumTriggerTime.AfterStartup)
                {
                    continue;
                }

                if (RecurseFindNormalize(relationManager, trigger, 0, 50, pid, normalizeValuePid))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool RecurseFindNormalize(RelationManager relations, IReadable target, int currentLevel, int maxLevel, uint? pid, uint? normalizeValuePid)
        {
            if (currentLevel >= maxLevel)
            {
                return false;
            }

            IActionsAction targetAsAction = target as IActionsAction;
            if (targetAsAction?.Type?.Value == EnumActionType.Normalize &&
                targetAsAction.On?.Id?.Value == Convert.ToString(pid) &&
                targetAsAction.Type?.Id?.Value == normalizeValuePid)
            {
                return true;
            }

            var outgoingLinks = relations.GetForwardLinks(target);

            foreach (Link outgoing in outgoingLinks)
            {
                if (RecurseFindNormalize(relations, outgoing.Target, currentLevel + 1, maxLevel, pid, normalizeValuePid))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
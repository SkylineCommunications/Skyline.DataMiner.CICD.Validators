namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Timers.Timer.Content.Group.CheckGroupTag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckGroupTag, Category.Timer)]
    internal class CheckGroupTag : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (ITimersTimer timer in context.EachTimerWithValidId())
            {
                if (timer.Content == null)
                {
                    continue;
                }

                var linkedContentGroups = timer.GetTimerContentGroups(context.ProtocolModel.RelationManager).ToList();

                foreach (ITimersTimerContentGroup group in timer.Content)
                {
                    (GenericStatus valueStatus, uint convertedValue) = Generic.GenericTests.CheckBasics<uint>(group.Value);

                    if (valueStatus.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyGroupTag(this, group, group, timer.Id.RawValue));
                        continue;
                    }

                    if (!valueStatus.HasFlag(GenericStatus.Invalid))
                    {
                        if (linkedContentGroups.FirstOrDefault(g => g.Id?.Value == convertedValue) == null)
                        {
                            // Linked group does not exist.
                            results.Add(Error.NonExistingIdInGroup(this, timer, group, group.TagName, group.Value, timer.Id.RawValue));
                        }

                        continue;
                    }

                    // Verify whether the group refers to a column.
                    Match regexMatch = Regex.Match(group.Value, "^col:([0-9]+):([0-9]+)$", RegexOptions.IgnoreCase);

                    if (!regexMatch.Success)
                    {
                        // Invalid content.
                        results.Add(Error.InvalidGroupTag(this, group, group, group.Value, timer.Id.RawValue));
                        continue;
                    }

                    // If this value is specified, the timer must be a multi-threaded timer and the "ip" option has to define an existing table.
                    // This table should be retrieved to check whether the column referred in the Group tag exist.
                    IParamsParam threadTable = ValidateHelper.GetMultithreadedTimerTableId(context, timer);

                    if (threadTable == null)
                    {
                        // This means that either multi-threaded timer is missing the "ip" option or that the <Group>col:x:y</Group> option is used in a regular timer.
                        results.Add(Error.InvalidGroupTag(this, group, group, group.Value, timer.Id.RawValue));
                        continue;
                    }

                    string colColumnIndexValue = regexMatch.Groups[1].Captures[0].Value;
                    string colGroupIdValue = regexMatch.Groups[2].Captures[0].Value;

                    bool colColumnIdxIsUInt = UInt32.TryParse(colColumnIndexValue, out uint colColumnIndex);
                    bool colGroupIdIsUInt = UInt32.TryParse(colGroupIdValue, out uint colGroupId);

                    if (!colColumnIdxIsUInt || !colGroupIdIsUInt)
                    {
                        results.Add(Error.InvalidGroupTag(this, group, group, group.Value, timer.Id.RawValue));
                        continue;
                    }

                    List<IValidationResult> subErrors = new List<IValidationResult>();

                    // Verify whether the specified value is correct.
                    if (!ValidateHelper.HasColumnIndex(threadTable, colColumnIndex))
                    {
                        subErrors.Add(Error.NonExistingIdInGroup(this, group, group, $"Column index Table '{threadTable.Id.Value}' (0-based)", colColumnIndexValue, timer.Id.RawValue));
                    }

                    if (!ValidateHelper.GroupExists(context, colGroupId))
                    {
                        subErrors.Add(Error.NonExistingIdInGroup(this, group, group, group.TagName, colGroupIdValue, timer.Id.RawValue));
                    }

                    if (subErrors.Count > 0)
                    {
                        IValidationResult invalidGroupTag = Error.InvalidGroupTag(this, @group, @group, @group.Value, timer.Id.RawValue);
                        invalidGroupTag.WithSubResults(subErrors.ToArray());
                        results.Add(invalidGroupTag);
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

    internal static class ValidateHelper
    {
        public static IParamsParam GetMultithreadedTimerTableId(ValidatorContext context, ITimersTimer timer)
        {
            var options = timer.GetOptions();

            var ipAddress = options?.IPAddress;

            if (ipAddress?.TableParameterId?.Value == null)
            {
                return null;
            }

            // This means a value was specified for this component.
            var referencedParamId = ipAddress.TableParameterId.Value.Value.ToString();
            var model = context.ProtocolModel;

            return model.TryGetObjectByKey(Mappings.ParamsById, referencedParamId, out IParamsParam param) ? param : null;
        }

        public static bool HasColumnIndex(IParamsParam param, uint columnIndex)
        {
            if (param.ArrayOptions == null)
            {
                return false;
            }

            foreach (ITypeColumnOption columnOption in param.ArrayOptions)
            {
                if (columnOption.Idx?.Value == columnIndex)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool GroupExists(ValidatorContext context, uint groupId)
        {
            bool groupExists = true;

            var model = context.ProtocolModel;

            if (!model.TryGetObjectByKey<IGroupsGroup>(Mappings.GroupsById, groupId.ToString(), out _))
            {
                groupExists = false;
            }

            return groupExists;
        }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.Content.CheckContentTag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckContentTag, Category.Group)]
    internal class CheckContentTag : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var group in context.EachGroupWithValidId())
            {
                if (group.Type != null && group.Type?.Value == null)
                {
                    // This case where the Group/Type contains a wrong value will be covered by check on Group/Type
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, group);
                helper.Validate();
            }

            return results;
        }

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

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        private readonly IGroupsGroup group;
        private readonly IGroupsGroupContent content;
        private readonly EnumGroupType groupType;
        private readonly string groupTypeString;

        private string[] multiThreadedGroups;

        private static readonly string[] ValidActionGroupContent = { "Action" };
        private static readonly string[] ValidTriggerGroupContent = { "Trigger" };
        private static readonly string[] ValidPollGroupContent = { "Param", "Pair", "Session" };

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IGroupsGroup group)
            : base(test, context, results)
        {
            this.group = group;

            content = group.Content;
            groupType = group.Type?.Value ?? EnumGroupType.Poll;
            groupTypeString = EnumGroupTypeConverter.ConvertBack(groupType);
        }

        public void Validate()
        {
            /* TODO: Rethink a bit the max size part
             * 
             * In the old validator, we were only checking the size for poll groups (with no distinctions between pairs, sessions, params)
             * - if multipleGet -> 20
             * - else -> 10
             * 
             * We currently reproduced that but I think we could improve it
             *  - 'action' ? in case of QAction, we should probably limit it to a rather small number as well ?
             *  - 'poll action' ? same as above?
             *  - 'trigger' ? same as above?
             *  - 'poll trigger' ? same as above?
             *  - 'poll':
             *      - Session: probably much less than 10? something like 3 or so?
             *      - Pair : probably much less than 10? something like 3 or so?
             *      - Param:
             *          - table : probably 1 or 2 ?
             *          - standalone
             *              - if multipleGet -> 20
             *              - else -> 10
             */

            if (content == null)
            {
                CheckMissingTag();
            }
            else
            {
                switch (groupType)
                {
                    case EnumGroupType.Action:
                    case EnumGroupType.PollAction:
                        ValidateActionGroup();
                        break;
                    case EnumGroupType.PollTrigger:
                    case EnumGroupType.Trigger:
                        ValidateTriggerGroup();
                        break;
                    case EnumGroupType.Poll:
                        ValidatePollGroup();
                        break;
                    default:
                        return;
                }
            }
        }

        private void ValidateActionGroup()
        {
            // InvalidContent
            results.AddRange(CheckAllowedTypes(ValidActionGroupContent));
        }

        private void ValidateTriggerGroup()
        {
            // InvalidContent
            results.AddRange(CheckAllowedTypes(ValidTriggerGroupContent));
        }

        private void ValidatePollGroup()
        {
            var contentTagNames = new HashSet<string>();
            foreach (var contentChild in content)
            {
                var error = CheckContentChildType(contentChild, ValidPollGroupContent);
                if (error != null)
                {
                    results.Add(error);
                    continue;
                }

                contentTagNames.Add(contentChild.TagName);
            }

            // MixedType
            if (contentTagNames.Count > 1)
            {
                results.Add(Error.MixedTypes(test, group, content, String.Join(";", contentTagNames), group.Id.RawValue));
            }

            ValidatePollGroupSize();
        }

        private void ValidatePollGroupSize()
        {
            if (content.MultipleGet?.Value == true)
            {
                if (content.Count > 20)
                {
                    results.Add(Error.MaxItemsMultipleGet(test, group, content, group.Id.RawValue));
                }
            }
            else
            {
                if (content.Count > 10)
                {
                    results.Add(Error.MaxItems(test, group, content, group.Id.RawValue));
                }
            }
        }

        private void CheckMissingTag()
        {
            // When no group is present, it is some times necessary to add an empty one to get some thread going.
            int groupCount = context.ProtocolModel.Protocol.Groups.Count;
            if (groupCount == 1)
            {
                return;
            }

            // Make sure we load multi-threaded groups only once
            if (multiThreadedGroups == null)
            {
                multiThreadedGroups = LoadMultiThreadedGroups(context);
            }

            // Missing tag
            if (!multiThreadedGroups.Contains(group.Id.RawValue))
            {
                results.Add(Error.MissingTag(test, group, group, group.Id.RawValue));
            }
        }

        private static string[] LoadMultiThreadedGroups(ValidatorContext context)
        {
            List<string> result = new List<string>();
            foreach (ITimersTimer timer in context.EachTimerWithValidId())
            {
                var timerOptions = timer.GetOptions();

                if (timerOptions?.IPAddress == null)
                {
                    continue;
                }

                foreach (ITimersTimerContentGroup contentGroup in timer.Content)
                {
                    result.Add(contentGroup.RawValue);
                }
            }

            return result.ToArray();
        }

        private IEnumerable<IValidationResult> CheckAllowedTypes(IReadOnlyCollection<string> allowedContentTypes)
        {
            foreach (var contentChild in content)
            {
                var result = CheckContentChildType(contentChild, allowedContentTypes);
                if (result != null)
                {
                    yield return result;
                }
            }
        }

        private IValidationResult CheckContentChildType(IGroupsGroupContentItem contentChild, IEnumerable<string> allowedContentTypes)
        {
            if (!allowedContentTypes.Contains(contentChild.TagName))
            {
                return Error.IncompatibleContentWithGroupType(test, group, contentChild, groupTypeString, contentChild.TagName, group.Id.RawValue);
            }

            return null;
        }
    }
}

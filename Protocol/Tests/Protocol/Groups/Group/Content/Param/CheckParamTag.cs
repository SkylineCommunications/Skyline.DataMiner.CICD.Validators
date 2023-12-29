namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.Content.Param.CheckParamTag
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckParamTag, Category.Group)]
    internal class CheckParamTag : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IGroupsGroup group in context.EachGroupWithValidId())
            {
                if (group.Content == null)
                {
                    continue;
                }

                foreach (var itemInGroup in group.Content)
                {
                    // Exclude other tags
                    if (!(itemInGroup is IGroupsGroupContentParam param))
                    {
                        continue;
                    }

                    ValidateHelper helper = new ValidateHelper(this, context, results, group, param);

                    if (!helper.CheckBasics())
                    {
                        continue;
                    }

                    helper.CheckPid();
                    helper.CheckSuffixes();
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();
            var extraData = context.Result.ExtraData;

            switch (context.Result.ErrorId)
            {
                case ErrorIds.ObsoleteSuffixTable:
                    var readGroup = (IGroupsGroup)extraData[ExtraData.Group];
                    var readContentParam = (IGroupsGroupContentParam)extraData[ExtraData.GroupContentParam];
                    string oldSuffix = (string)extraData[ExtraData.OldSuffix];
                    string newSuffix = (string)extraData[ExtraData.NewSuffix];

                    var fix = new ObsoleteSuffixTableFix(readContentParam, oldSuffix, newSuffix);

                    GroupsGroup editGroup = context.Protocol.Groups.Get(readGroup);
                    editGroup.Accept(fix);

                    result.Success = true;
                    break;
                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }
    }

    internal enum ExtraData
    {
        Group,
        GroupContentParam,
        OldSuffix,
        NewSuffix,
    }

    internal class ObsoleteSuffixTableFix : Skyline.DataMiner.CICD.Models.Protocol.Edit.ProtocolVisitor
    {
        private readonly IGroupsGroupContentParam paramToFix;
        private readonly string oldSuffix;
        private readonly string newSuffix;

        public ObsoleteSuffixTableFix(IGroupsGroupContentParam paramToFix, string oldSuffix, string newSuffix)
        {
            this.paramToFix = paramToFix;
            this.oldSuffix = oldSuffix;
            this.newSuffix = newSuffix;
        }

        public override void VisitGroupsGroupContentParam(GroupsGroupContentParam obj)
        {
            if (obj.Read == paramToFix)
            {
                obj.Value = obj.Value.Replace(oldSuffix, newSuffix);
            }
        }
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        private static readonly string[] AllowedSuffixes = { "single", "instance", "tablev2", "getnext" };
        private static readonly Dictionary<string, string> ObsoleteSuffixes = new Dictionary<string, string>
        {
            { "table", "tablev2" },
        };

        private readonly IGroupsGroup group;
        private readonly IGroupsGroupContentParam param;

        private readonly string groupId;
        private readonly string rawValue;
        private string[] valueParts;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IGroupsGroup group, IGroupsGroupContentParam param)
            : base(test, context, results)
        {
            this.group = group;
            this.param = param;

            rawValue = param.RawValue;
            groupId = group.Id.RawValue;
        }

        internal bool CheckBasics()
        {
            (GenericStatus status, _, string value) = GenericTests.CheckBasics(param, false);

            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyParamTag(test, param, param, groupId));
                return false;
            }

            if (status.HasFlag(GenericStatus.Invalid))
            {
                results.Add(Error.InvalidParamTag(test, param, param, rawValue, groupId));
                return false;
            }

            valueParts = value.Split(':');

            return true;
        }

        internal void CheckPid()
        {
            string pid = valueParts[0];

            if (!UInt32.TryParse(pid, out _))
            {
                results.Add(Error.InvalidParamTag(test, param, param, rawValue, groupId));
                return;
            }

            if (!context.ProtocolModel.TryGetObjectByKey(Mappings.ParamsById, pid, out IParamsParam _))
            {
                results.Add(Error.NonExistingId(test, param, param, pid, groupId));
            }
        }

        internal void CheckSuffixes()
        {
            if (valueParts.Length < 2)
            {
                return;
            }

            CheckSuffix1();

            if (valueParts.Length > 2)
            {
                results.Add(Error.InvalidParamTag(test, param, param, rawValue, groupId));
            }

            void CheckSuffix1()
            {
                string suffix = valueParts[1];
                if (Array.IndexOf(AllowedSuffixes, suffix) < 0)
                {
                    if (ObsoleteSuffixes.TryGetValue(suffix, out string newSuffix))
                    {
                        IValidationResult obsoleteSuffixTable = Error.ObsoleteSuffixTable(test, param, param, groupId);
                        obsoleteSuffixTable.WithExtraData(ExtraData.Group, group)
                                           .WithExtraData(ExtraData.GroupContentParam, param)
                                           .WithExtraData(ExtraData.OldSuffix, suffix)
                                           .WithExtraData(ExtraData.NewSuffix, newSuffix);
                        results.Add(obsoleteSuffixTable);
                    }
                    else
                    {
                        results.Add(Error.InvalidParamSuffix(test, param, param, suffix, groupId));
                    }
                }

                foreach (Link link in context.ProtocolModel.RelationManager.GetLinks(group))
                {
                    if (!(link.Source is ITimersTimer timer))
                    {
                        continue;
                    }

                    var timerOptions = timer.GetOptions();
                    if (timerOptions?.Each != null || timerOptions?.IPAddress != null || timerOptions?.ThreadPool != null)
                    {
                        continue;
                    }

                    results.Add(Error.SuffixRequiresMultiThreadedTimer(test, param, param, suffix, groupId));
                }
            }
        }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.Content.Trigger.CheckTriggerTag
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

    [Test(CheckId.CheckTriggerTag, Category.Group)]
    internal class CheckTriggerTag : IValidate/*, ICodeFix, ICompare*/
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
                    if (!(itemInGroup is IGroupsGroupContentTrigger trigger))
                    {
                        continue;
                    }

                    (GenericStatus status, string _, string value) = GenericTests.CheckBasics(trigger, false);

                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyTriggerTag(this, trigger, trigger, group.Id.RawValue));
                        continue;
                    }

                    if (status.HasFlag(GenericStatus.Invalid) || !UInt32.TryParse(value, out uint _))
                    {
                        results.Add(Error.InvalidTriggerTag(this, trigger, trigger, value, group.Id.RawValue));
                        continue;
                    }

                    if (!context.ProtocolModel.TryGetObjectByKey<ITriggersTrigger>(Mappings.TriggersById, value, out _))
                    {
                        results.Add(Error.NonExistingId(this, trigger, trigger, trigger.Value, group.Id.RawValue));
                        continue;
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
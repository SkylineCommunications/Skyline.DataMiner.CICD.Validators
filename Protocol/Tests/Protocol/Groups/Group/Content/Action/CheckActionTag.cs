namespace SLDisValidator2.Tests.Protocol.Groups.Group.Content.Action.CheckActionTag
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Generic;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckActionTag, Category.Group)]
    internal class CheckActionTag : IValidate/*, ICodeFix, ICompare*/
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
                    if (!(itemInGroup is IGroupsGroupContentAction action))
                    {
                        continue;
                    }

                    (GenericStatus status, string _, string value) = GenericTests.CheckBasics(action, false);

                    if (status.HasFlag(GenericStatus.Empty))
                    {
                        results.Add(Error.EmptyActionTag(this, action, action, group.Id.RawValue));
                        continue;
                    }

                    if (status.HasFlag(GenericStatus.Invalid) || !UInt32.TryParse(value, out uint _))
                    {
                        results.Add(Error.InvalidActionTag(this, action, action, value, group.Id.RawValue));
                        continue;
                    }

                    if (!context.ProtocolModel.TryGetObjectByKey<IActionsAction>(Mappings.ActionsById, value, out _))
                    {
                        results.Add(Error.NonExistingId(this, action, action, action.Value, group.Id.RawValue));
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
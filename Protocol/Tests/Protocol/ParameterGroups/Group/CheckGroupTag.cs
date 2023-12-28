namespace SLDisValidator2.Tests.Protocol.ParameterGroups.Group.CheckGroupTag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckGroupTag, Category.ParameterGroup)]
    public class CheckGroupTag : IValidate, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParameterGroupsGroup parameterGroup in context.EachParameterGroupWithValidId())
            {
                // After all, below is allowed in order to create standalone ParameterGroups
                //if (parameterGroup.DynamicId?.Value == null && parameterGroup.Params == null)
                //{
                //    results.Add(Error.MissingParamReference(this, parameterGroup, parameterGroup, parameterGroup.Id.RawValue));
                //}

                if (parameterGroup.DynamicId != null && parameterGroup.Params != null)
                {
                    results.Add(Error.IncompatibleParamReferences(this, parameterGroup, parameterGroup, parameterGroup.Id.RawValue));
                }
            }

            return results;
        }

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            uint?[] allMatchingGroupIDs = context.EachMatchingParameterGroup().Select(p => p.oldGroup?.Id?.Value).ToArray();
            var oldGroups = context?.PreviousProtocolModel?.Protocol?.ParameterGroups;
            if (oldGroups == null)
            {
                return results;
            }

            foreach (var oldGroup in oldGroups)
            {
                uint? oldGroupId = oldGroup?.Id?.Value;
                if (oldGroupId != null && !allMatchingGroupIDs.Contains(oldGroupId))
                {
                    results.Add(ErrorCompare.DcfParameterGroupRemoved(oldGroup, oldGroup, Convert.ToString(oldGroupId)));
                }
            }

            return results;
        }
    }
}
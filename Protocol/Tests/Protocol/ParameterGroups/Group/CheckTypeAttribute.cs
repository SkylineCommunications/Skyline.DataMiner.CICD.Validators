namespace SLDisValidator2.Tests.Protocol.ParameterGroups.Group.CheckTypeAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckTypeAttribute, Category.Protocol)]
    public class CheckTypeAttribute : ICompare
    {
        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IParameterGroupsGroup oldGroup, IParameterGroupsGroup newGroup) in context.EachMatchingParameterGroup())
            {
                uint? groupId = newGroup?.Id?.Value;
                if (groupId == null)
                {
                    continue;
                }

                EnumParamGroupType? oldType = oldGroup?.Type?.Value;
                EnumParamGroupType? newType = newGroup?.Type?.Value;

                if (oldType != newType)
                {
                    results.Add(ErrorCompare.DcfParameterGroupTypeChanged(newGroup, newGroup, Convert.ToString(groupId), Convert.ToString(oldType), Convert.ToString(newType)));
                }
            }

            return results;
        }
    }
}
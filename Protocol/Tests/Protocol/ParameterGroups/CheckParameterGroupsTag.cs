namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ParameterGroups.CheckParameterGroupsTag
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckParameterGroupsTag, Category.ParameterGroup)]
    internal class CheckParameterGroupsTag : ICompare
    {
        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var oldProtocol = context.PreviousProtocolModel?.Protocol;
            var newProtocol = context.NewProtocolModel?.Protocol;
            if (oldProtocol == null || newProtocol == null)
            {
                return results;
            }

            var oldFirstGroup = oldProtocol.ParameterGroups?.FirstOrDefault();
            var newFirstGroup = newProtocol.ParameterGroups?.FirstOrDefault();

            if (oldFirstGroup == null && newFirstGroup != null)
            {
                results.Add(ErrorCompare.DcfAdded(newFirstGroup, newFirstGroup));
            }

            return results;
        }
    }
}
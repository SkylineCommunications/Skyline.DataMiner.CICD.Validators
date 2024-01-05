namespace Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal static class MajorChangeCheckContextExtensions
    {
        /// <summary>
        /// Iterates all parameters from the old protocols and returns the old and matching new param, matching on the Id.
        /// </summary>
        /// <param name="context">The MajorChangeCheckContext object containing the protocolModels.</param>
        /// <returns>A named Tuple holding the oldParam and newParam.</returns>
        public static IEnumerable<(IParamsParam oldParam, IParamsParam newParam)> EachMatchingParam(this MajorChangeCheckContext context)
        {
            var newProtocolParams = context?.NewProtocolModel?.Protocol?.Params;
            var previousProtocolParams = context?.PreviousProtocolModel?.Protocol?.Params;
            if (previousProtocolParams == null || newProtocolParams == null)
            {
                yield break;
            }

            foreach (var previousParam in previousProtocolParams)
            {
                if (previousParam?.Id?.Value == null)
                {
                    continue;
                }

                var newParam = newProtocolParams.FirstOrDefault(p => p.Id?.Value == previousParam.Id.Value);
                if (newParam == null)
                {
                    continue;
                }

                yield return (previousParam, newParam);
            }
        }

        /// <summary>
        /// Iterates all ParameterGroups from the old protocols and returns the old and matching new param, matching on the Id.
        /// </summary>
        /// <param name="context">The MajorChangeCheckContext object containing the protocolModels.</param>
        /// <returns>A named Tuple holding the oldGroup and newGroup.</returns>
        public static IEnumerable<(IParameterGroupsGroup oldGroup, IParameterGroupsGroup newGroup)> EachMatchingParameterGroup(this MajorChangeCheckContext context)
        {
            var newProtocolParameterGroups = context?.NewProtocolModel?.Protocol?.ParameterGroups;
            var previousProtocolParameterGroups = context?.PreviousProtocolModel?.Protocol?.ParameterGroups;
            if (newProtocolParameterGroups == null || previousProtocolParameterGroups == null)
            {
                yield break;
            }

            foreach (var previousGroup in previousProtocolParameterGroups)
            {
                var newGroup = newProtocolParameterGroups.FirstOrDefault(p => p.Id?.Value == previousGroup.Id?.Value);
                if (newGroup == null)
                {
                    continue;
                }

                yield return (previousGroup, newGroup);
            }
        }
    }
}
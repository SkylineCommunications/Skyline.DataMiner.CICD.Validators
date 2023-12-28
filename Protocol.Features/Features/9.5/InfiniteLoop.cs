namespace SLDisDmFeatureCheck.Features
{
    using System.Collections.Generic;
    using System.Linq;
    using SLDisDmFeatureCheck.Common;
    using SLDisDmFeatureCheck.Common.Attributes;
    using SLDisDmFeatureCheck.Common.Interfaces;
    using SLDisDmFeatureCheck.Common.Results;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    //[MinDataMinerVersions("9.5.0.0-8068", "9.6.3.0-8092")]
    internal class InfiniteLoop : IFeatureCheck
    {
        public string Title => "Infinite loop detection";

        public string Description => "Infinite loop detection in SNMPManager.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 20419 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.Params
                               ?.Where(param => param?.SNMP?.InvalidResponseHandling?.InfiniteLoop != null)
                               ?.Select(param => (IReadable)param)
                               ?.ToList();

            return new FeatureCheckResult(items);
        }
    }
}
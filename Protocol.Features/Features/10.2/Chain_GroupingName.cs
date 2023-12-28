namespace SLDisDmFeatureCheck.Features
{
    using System.Collections.Generic;
    using System.Linq;
    using SLDisDmFeatureCheck.Common;
    using SLDisDmFeatureCheck.Common.Attributes;
    using SLDisDmFeatureCheck.Common.Interfaces;
    using SLDisDmFeatureCheck.Common.Results;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    [MinDataMinerVersions("10.2.0.0-11517", "10.1.3.0-9963")]
    public class Chain_GroupingName : IFeatureCheck
    {
        public string Title => "Chain Grouping Name";

        public string Description => "Group EPM chains together.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 28751, 28834 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.Chains
                               ?.Where(c => c is IChainsChain chain && chain.GroupingName != null)
                               ?.Select(c => (IReadable)c)
                               ?.ToList();

            return new FeatureCheckResult(items);
        }
    }
}
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
    public class Chain_DefaultSelectionField : IFeatureCheck
    {
        public string Title => "Chain Default Selection Field";

        public string Description => "Select a Field that needs to automatically select a value.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 28751, 28846 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.Chains
                               ?.Where(c => c is IChainsChain chain && chain.DefaultSelectionField != null)
                               ?.Select(c => (IReadable)c)
                               ?.ToList();

            return new FeatureCheckResult(items);
        }
    }
}
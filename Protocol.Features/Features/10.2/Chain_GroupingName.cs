namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    [MinDataMinerVersions("10.2.0.0-11517", "10.1.3.0-9963")]
    internal class ChainGroupingName : IFeatureCheck
    {
        public string Title => "Chain Grouping Name";

        public string Description => "Group EPM chains together.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 28751, 28834 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.Chains
                               ?.Where(c => c is IChainsChain chain && chain.GroupingName != null)
                               .Select(c => (IReadable)c)
                               .ToList();

            return new FeatureCheckResult(items);
        }
    }
}
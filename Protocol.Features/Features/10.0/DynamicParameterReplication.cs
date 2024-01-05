namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("10.0.0.0-9118", "9.6.1.0-7839")]
    internal class DynamicParameterReplication : IFeatureCheck
    {
        public string Title => "Dynamic Parameter Replication";

        public string Description => "You are now able to configure replication on a parameter level.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 19311 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.Params
                               ?.Where(param => param?.Replication?.Element?.Dynamic != null || param?.Replication?.Parameter?.Dynamic != null)
                               .Select(param => (IReadable)param)
                               .ToList();

            return new FeatureCheckResult(items);
        }
    }
}
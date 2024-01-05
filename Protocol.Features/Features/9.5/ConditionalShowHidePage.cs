namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("9.5.0.0-8480", "9.0.3.5-4937")]
    internal class ConditionalShowHidePage : IFeatureCheck
    {
        public string Title => "Conditional Show/Hide Pages";

        public string Description => "Hide or show pages depending on a parameter value.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 12840 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.Display?.Pages
                ?.Select(x => (IReadable)x)
                .ToList();

            return new FeatureCheckResult(items);
        }
    }
}
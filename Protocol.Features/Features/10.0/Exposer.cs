namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("10.0.0.0-9118", "9.6.5.0-8250")]
    internal class Exposer : IFeatureCheck
    {
        public string Title => "Exposer";

        public string Description => "This will help creating the required alarm properties for the new CPE/EPM features.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 21101, 21122, 21206, 21465, 21746 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.Topologies
                               ?.Where(topology => topology?.Any(cell => cell?.Exposer != null) == true)
                               .Select(x => (IReadable)x)
                               .ToList();

            return new FeatureCheckResult(items);
        }
    }
}
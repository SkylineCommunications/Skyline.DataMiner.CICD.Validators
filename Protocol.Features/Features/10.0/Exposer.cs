namespace SLDisDmFeatureCheck.Features
{
    using System.Collections.Generic;
    using System.Linq;
    using SLDisDmFeatureCheck.Common;
    using SLDisDmFeatureCheck.Common.Attributes;
    using SLDisDmFeatureCheck.Common.Interfaces;
    using SLDisDmFeatureCheck.Common.Results;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    //[MinDataMinerVersions("10.0.0.0-9118", "9.6.5.0-8250")]
    public class Exposer : IFeatureCheck
    {
        public string Title => "Exposer";

        public string Description => "This will help creating the required alarm properties for the new CPE/EPM features.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 21101, 21122, 21206, 21465, 21746 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.Topologies
                               ?.Where(topology => topology?.Any(cell => cell?.Exposer != null) == true)
                               ?.Select(x => (IReadable)x)
                               ?.ToList();

            return new FeatureCheckResult(items);
        }
    }
}
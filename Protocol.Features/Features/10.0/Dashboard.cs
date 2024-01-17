namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("10.0.0.0-9118", "9.6.11.0-8649")]
    internal class Dashboard : IFeatureCheck
    {
        public string Title => "Dashboard";

        public string Description => "Dashboards application";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 22855, 22874, 22875, 23080, 23084, 23097, 23103, 23173, 23176, 23245, 23249, 23281, 23293 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.Params
                               ?.Where(param => param?.Dashboard != null)
                               .Select(param => (IReadable)param)
                               .ToList();

            return new FeatureCheckResult(items);
        }
    }
}
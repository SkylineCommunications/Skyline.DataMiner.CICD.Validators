namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("10.1.0.0-9966", "10.0.13.0-9784")]
    internal class InternalLicenses : IFeatureCheck
    {
        public string Title => "Internal Licenses";

        public string Description => "With this tag the element will not be counted towards the element license count.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 27933 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            List<IReadable> items = new List<IReadable>();

            if (context?.Model?.Protocol?.InternalLicenses != null)
            {
                items.AddRange(context.Model.Protocol.InternalLicenses);
            }

            return new FeatureCheckResult(items);
        }
    }
}
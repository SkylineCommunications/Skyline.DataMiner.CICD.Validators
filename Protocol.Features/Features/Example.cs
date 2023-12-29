/*
namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Attributes;

    // https://intranet.skyline.be/DataMiner/Lists/Released%20Versions/AllItems.aspx
    [MinDataMinerVersions("9101.9102.9103.9104", "9001.9002.9003.9004")]
    internal class ItsOver9000 : IFeatureCheck
    {
        public string Title => "It's over 9000!";

        public string Description => "This is an internet meme.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 9999 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = new List<FeatureCheckResultItem>();

            foreach (var param in context?.Model?.Protocol?.Params)
            {
                if (param?.Id?.Value > 9000)
                {
                    items.Add(new FeatureCheckResultItem(param));
                }
            }

            return new FeatureCheckResult(items);
        }
    }
}
*/
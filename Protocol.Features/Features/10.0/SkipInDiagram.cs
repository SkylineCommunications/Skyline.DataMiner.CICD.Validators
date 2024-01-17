namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    //[MinDataMinerVersions("10.0.0.0-9118", "9.6.4.0-8151")]
    internal class SkipInDiagram : IFeatureCheck
    {
        public string Title => "SkipInDiagram";

        public string Description => "You can now skip a level in a CPE diagram by adding the option 'skipInDiagram' in the Field options.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 20893 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.Chains
                               ?.Where(IsChainAndHasOptionInField)
                               .Select(x => (IReadable)x)
                               .ToList();

            return new FeatureCheckResult(items);

            bool IsChainAndHasOptionInField(IChainsItem c)
            {
                // TODO: Use a GetOptions method (to be made)?
                return c is IChainsChain chain && chain.Any(field => field.Options?.Value?.Contains("skipInDiagram") == true);
            }
        }
    }
}
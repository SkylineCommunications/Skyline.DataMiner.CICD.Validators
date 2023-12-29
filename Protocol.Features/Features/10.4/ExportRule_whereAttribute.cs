namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    // TODO: Add build number when 10.4 is released
    [MinDataMinerVersions("10.4.0.0", "10.3.8.0-13183")]
    internal class ExportRuleWhereAttribute : IFeatureCheck
    {
        public string Title => "Export Rules - whereAttribute";

        public string Description => "This allows to validate the value of an attribute when applying an export rule.";

        public IReadOnlyCollection<uint> ReleaseNotes => new uint[] { 36622 };

        public IFeatureCheckResult CheckIfUsed(FeatureCheckContext context)
        {
            var items = context?.Model?.Protocol?.ExportRules
                               ?.Where(rule => rule.WhereAttribute != null)
                               .Select(rule => (IReadable)rule)
                               .ToList();

            return new FeatureCheckResult(items);
        }
    }
}
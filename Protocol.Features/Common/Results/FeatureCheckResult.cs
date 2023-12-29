namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal class FeatureCheckResult : IFeatureCheckResult
    {
        internal FeatureCheckResult()
        {
            FeatureItems = Array.Empty<FeatureCheckResultItem>();
        }

        internal FeatureCheckResult(IReadOnlyCollection<FeatureCheckResultItem> featureItems)
        {
            FeatureItems = featureItems ?? Array.Empty<FeatureCheckResultItem>();
        }

        internal FeatureCheckResult(IReadOnlyCollection<IReadable> featureItems)
            : this(featureItems?.Select(x => new FeatureCheckResultItem(x)).ToList())
        {
        }

        public bool IsUsed => FeatureItems.Count > 0;

        public IReadOnlyCollection<FeatureCheckResultItem> FeatureItems { get; }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Common
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;

    /// <summary>
    /// Represents a feature with additional info and which items are using the feature.
    /// </summary>
    /// <seealso cref="IFeature" />
    public class Feature : IFeature
    {
        private readonly IFeatureCheck featureCheck;
        private readonly MinDataMinerVersionsAttribute minDmaVersionAttribute;
        private readonly MaxDataMinerVersionsAttribute maxDmaVersionAttribute;
        private readonly IFeatureCheckResult featureCheckResult;

        internal Feature(IFeatureCheck featureCheck, IFeatureCheckResult featureCheckResult, MinDataMinerVersionsAttribute minAttr, MaxDataMinerVersionsAttribute maxAttr)
        {
            this.featureCheck = featureCheck;
            this.featureCheckResult = featureCheckResult;
            minDmaVersionAttribute = minAttr;
            maxDmaVersionAttribute = maxAttr;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title => featureCheck.Title;

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description => featureCheck.Description;

        /// <summary>
        /// Gets the release notes.
        /// </summary>
        public IReadOnlyCollection<uint> ReleaseNotes => featureCheck.ReleaseNotes;

        /// <summary>
        /// Gets the minimum main release.
        /// </summary>
        public DataMinerVersion MinMainRelease => minDmaVersionAttribute.MainRelease;

        /// <summary>
        /// Gets the minimum feature release.
        /// </summary>
        public DataMinerVersion MinFeatureRelease => minDmaVersionAttribute.FeatureRelease;

        /// <summary>
        /// Gets the maximum main release. Will be null when there isn't a maximum defined.
        /// </summary>
        public DataMinerVersion MaxMainRelease => maxDmaVersionAttribute?.MainRelease;

        /// <summary>
        /// Gets the maximum feature release. Will be null when there isn't a maximum defined.
        /// </summary>
        public DataMinerVersion MaxFeatureRelease => maxDmaVersionAttribute?.FeatureRelease;

        /// <summary>
        /// Gets the feature items.
        /// </summary>
        public IReadOnlyCollection<FeatureCheckResultItem> FeatureItems => featureCheckResult.FeatureItems;
    }
}
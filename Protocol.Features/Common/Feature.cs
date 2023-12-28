namespace SLDisDmFeatureCheck.Common
{
    using System.Collections.Generic;

    using Attributes;

    using Interfaces;

    using Results;

    using Skyline.DataMiner.CICD.Common;

    // Inherits IFeature so it always will have the info if we add extra.
    internal class Feature : IFeature
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

        public string Title => featureCheck.Title;

        public string Description => featureCheck.Description;

        public IReadOnlyCollection<uint> ReleaseNotes => featureCheck.ReleaseNotes;

        public DataMinerVersion MinMainRelease => minDmaVersionAttribute.MainRelease;

        public DataMinerVersion MinFeatureRelease => minDmaVersionAttribute.FeatureRelease;

        /// <summary>
        /// Gets the maximum main release. Will be null when there isn't a maximum defined.
        /// </summary>
        /// <value>
        /// The maximum main release.
        /// </value>
        public DataMinerVersion MaxMainRelease => maxDmaVersionAttribute?.MainRelease;

        /// <summary>
        /// Gets the maximum feature release. Will be null when there isn't a maximum defined.
        /// </summary>
        /// <value>
        /// The maximum feature release.
        /// </value>
        public DataMinerVersion MaxFeatureRelease => maxDmaVersionAttribute?.FeatureRelease;

        public IReadOnlyCollection<FeatureCheckResultItem> FeatureItems => featureCheckResult.FeatureItems;
    }
}
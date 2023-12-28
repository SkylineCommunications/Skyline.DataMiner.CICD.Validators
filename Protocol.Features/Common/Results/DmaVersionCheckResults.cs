namespace SLDisDmFeatureCheck.Common.Results
{
    using System.Collections.Generic;
    using SLDisDmFeatureCheck.Common.Interfaces;

    internal class DmaVersionCheckResults : IDmaVersionCheckResults
    {
        internal DmaVersionCheckResults()
        {
            Features = new List<Feature>();
        }

        public IReadOnlyCollection<IFeature> Features { get; internal set; }
    }
}
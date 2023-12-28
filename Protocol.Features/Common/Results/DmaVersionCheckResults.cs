namespace SLDisDmFeatureCheck.Common.Results
{
    using System.Collections.Generic;
    using SLDisDmFeatureCheck.Common.Interfaces;

    public class DmaVersionCheckResults : IDmaVersionCheckResults
    {
        internal DmaVersionCheckResults()
        {
            Features = new List<Feature>();
        }

        public IReadOnlyCollection<Feature> Features { get; internal set; }
    }
}
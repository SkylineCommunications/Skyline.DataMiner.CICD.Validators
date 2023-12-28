namespace SLDisDmFeatureCheck.Common.Interfaces
{
    using System.Collections.Generic;

    public interface IDmaVersionCheckResults
    {
        IReadOnlyCollection<Feature> Features { get; }
    }
}
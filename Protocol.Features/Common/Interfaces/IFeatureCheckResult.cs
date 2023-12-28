namespace SLDisDmFeatureCheck.Common.Interfaces
{
    using System.Collections.Generic;
    using Results;

    public interface IFeatureCheckResult
    {
        bool IsUsed { get; }

        IReadOnlyCollection<FeatureCheckResultItem> FeatureItems { get; }
    }
}
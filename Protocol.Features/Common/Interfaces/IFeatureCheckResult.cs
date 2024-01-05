namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces
{
    using System.Collections.Generic;

    using Results;

    internal interface IFeatureCheckResult
    {
        bool IsUsed { get; }

        IReadOnlyCollection<FeatureCheckResultItem> FeatureItems { get; }
    }
}
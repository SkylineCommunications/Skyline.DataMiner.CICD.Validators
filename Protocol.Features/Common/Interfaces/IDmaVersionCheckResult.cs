namespace SLDisDmFeatureCheck.Common.Interfaces
{
    using Skyline.DataMiner.CICD.Common;

    public interface IDmaVersionCheckResult
    {
        DataMinerVersion MainRelease { get; }

        DataMinerVersion FeatureRelease { get; }

        IFeatureCheck Feature { get; }
    }
}
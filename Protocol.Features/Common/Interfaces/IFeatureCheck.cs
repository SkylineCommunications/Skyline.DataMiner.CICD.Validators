namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces
{
    internal interface IFeatureCheck : IFeature
    {
        IFeatureCheckResult CheckIfUsed(FeatureCheckContext context);
    }
}
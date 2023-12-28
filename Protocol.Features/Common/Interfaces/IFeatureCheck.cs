namespace SLDisDmFeatureCheck.Common.Interfaces
{
    internal interface IFeatureCheck : IFeature
    {
        IFeatureCheckResult CheckIfUsed(FeatureCheckContext context);
    }
}
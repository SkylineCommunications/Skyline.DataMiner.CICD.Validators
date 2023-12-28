namespace SLDisDmFeatureCheck.Common.Interfaces
{
    public interface IFeatureCheck : IFeature
    {
        IFeatureCheckResult CheckIfUsed(FeatureCheckContext context);
    }
}
namespace Skyline.DataMiner.CICD.Validators.Common.Suppressions
{
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal static class SuppressionExtensions
    {
        public static bool IsValid(
            this IVersionHistoryBranchesBranchSystemVersionsSystemVersionMajorVersionsMajorVersionMinorVersionsMinorVersionSuppressionsSuppression suppression)
        {
            return suppression.ResultId?.Value != null &&
                   suppression.Location?.Value != null &&
                   suppression.Type?.Value != null &&
                   suppression.Reason?.Value != null;
        }
    }
}
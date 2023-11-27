namespace Skyline.DataMiner.CICD.Validators.Common.Suppressions
{
    using Skyline.DataMiner.CICD.Models.Protocol.Edit;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    /// <summary>
    /// Represents a major change suppression that is included in the version history.
    /// </summary>
    public class VersionHistoryMajorChangeSuppression : VersionHistorySuppression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionHistoryMajorChangeSuppression"/> class.
        /// </summary>
        /// <param name="suppression">The suppression.</param>
        public VersionHistoryMajorChangeSuppression(IVersionHistoryBranchesBranchSystemVersionsSystemVersionMajorVersionsMajorVersionMinorVersionsMinorVersionSuppressionsSuppression suppression) : base(suppression)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionHistoryMajorChangeSuppression"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="result">The result.</param>
        /// <param name="reason">The reason.</param>
        public VersionHistoryMajorChangeSuppression(IReadable node, IValidationResult result, string reason)
        {
            Type = EnumSuppressionType.MajorChange;

            Reason = reason;
            ResultId = result.FullId;

            Location = node.GetIdentifier();
        }

        /// <summary>
        /// Generates an edit XML node from this suppression.
        /// </summary>
        /// <returns>The corresponding edit XML node.</returns>
        public VersionHistoryBranchesBranchSystemVersionsSystemVersionMajorVersionsMajorVersionMinorVersionsMinorVersionSuppressionsSuppression ToEditXml()
        {
            var suppression =
                new VersionHistoryBranchesBranchSystemVersionsSystemVersionMajorVersionsMajorVersionMinorVersionsMinorVersionSuppressionsSuppression
                {
                    ResultId = ResultId,
                    Type = new VersionHistoryBranchesBranchSystemVersionsSystemVersionMajorVersionsMajorVersionMinorVersionsMinorVersionSuppressionsSuppressionType(Type),
                    Location = Location,
                    Reason = Reason
                };

            return suppression;
        }
    }
}
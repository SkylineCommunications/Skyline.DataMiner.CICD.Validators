namespace Skyline.DataMiner.CICD.Validators.Common.Suppressions
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    /// <summary>
    /// Represents a suppression that is included in the version history.
    /// </summary>
    public abstract class VersionHistorySuppression : ISuppression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionHistorySuppression"/> class.
        /// </summary>
        protected VersionHistorySuppression()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionHistorySuppression"/> class.
        /// </summary>
        /// <exception cref="ArgumentException">Invalid location, result ID, reason or type.</exception>
        protected VersionHistorySuppression(IVersionHistoryBranchesBranchSystemVersionsSystemVersionMajorVersionsMajorVersionMinorVersionsMinorVersionSuppressionsSuppression suppression)
        {
            if (suppression == null)
            {
                throw new ArgumentNullException(nameof(suppression));
            }

            Location = suppression.Location?.Value ?? throw new ArgumentException("Invalid location.");
            ResultId = suppression.ResultId?.Value ?? throw new ArgumentException("Invalid result ID.");
            Reason = suppression.Reason?.Value ?? throw new ArgumentException("Invalid reason.");
            Type = suppression.Type?.Value.GetValueOrDefault() ?? throw new ArgumentException("Invalid type.");
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public string Location { get; protected set; }

        /// <summary>
        /// Gets or sets the result ID.
        /// </summary>
        /// <value>The result ID.</value>
        public string ResultId { get; protected set; }

        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        /// <value>The reason.</value>
        public string Reason { get; protected set; }

        /// <summary>
        /// Get or sets the suppression type.
        /// </summary>
        /// <value>The suppression type.</value>
        public EnumSuppressionType Type { get; protected set; }

        /// <summary>
        /// Parses the specified version history based suppression XML edit node.
        /// </summary>
        /// <param name="versionSuppression">The version history based suppression XML edit node.</param>
        /// <param name="suppression">When this method returns, contains the version history based suppression.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="versionSuppression"/> is <see langword="null"/>.</exception>
        public static bool TryParse(
            IVersionHistoryBranchesBranchSystemVersionsSystemVersionMajorVersionsMajorVersionMinorVersionsMinorVersionSuppressionsSuppression
                versionSuppression, out VersionHistorySuppression suppression)
        {
            if (versionSuppression == null)
            {
                throw new ArgumentNullException(nameof(versionSuppression));
            }

            suppression = default;

            if (!versionSuppression.IsValid())
            {
                return false;
            }

            switch (versionSuppression.Type.Value)
            {
                case EnumSuppressionType.MajorChange:
                    suppression = new VersionHistoryMajorChangeSuppression(versionSuppression);
                    return true;

                default:
                    return false;
            }
        }
    }
}
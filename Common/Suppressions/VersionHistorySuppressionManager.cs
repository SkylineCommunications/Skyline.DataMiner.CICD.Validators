namespace Skyline.DataMiner.CICD.Validators.Common.Suppressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    /// <summary>
    /// Suppression manager for version history based suppressions.
    /// </summary>
	public class VersionHistorySuppressionManager
    {
        private readonly IProtocol protocol;
        private readonly IDictionary<string, List<VersionHistorySuppression>> suppressions = new Dictionary<string, List<VersionHistorySuppression>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionHistorySuppressionManager"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <exception cref="ArgumentNullException"><paramref name="protocol"/> is <see langword="null"/>.</exception>
        public VersionHistorySuppressionManager(IProtocol protocol)
        {
            this.protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));

            GetAllSuppressions();
        }

        private void GetAllSuppressions()
        {
            if (protocol.VersionHistory == null)
            {
                // No history, no suppressions.
                return;
            }

            if (!protocol.VersionHistory.TryGetMinorVersion(protocol.Version?.Value, out var minor))
            {
                // No version, no suppressions
                return;
            }

            if (minor?.Suppressions == null)
            {
                return;
            }

            foreach (var minorSuppression in minor.Suppressions)
            {
                if (minorSuppression.ResultId?.Value == null)
                {
                    // Invalid suppression (ResultId is used for the dictionary). TryParse will be false when stuff is missing.
                    continue;
                }

                if (!suppressions.TryGetValue(minorSuppression.ResultId.Value, out List<VersionHistorySuppression> temp))
                {
                    temp = new List<VersionHistorySuppression>();
                    suppressions.Add(minorSuppression.ResultId.Value, temp);
                }

                if (VersionHistorySuppression.TryParse(minorSuppression, out VersionHistorySuppression s))
                {
                    temp.Add(s);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified validator result is suppressed.
        /// </summary>
        /// <param name="result">The validator result.</param>
        /// <returns><c>true</c> if the specified validator result is suppressed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is <see langword="null"/>.</exception>
        public bool IsSuppressed(IValidationResult result)
        {
            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return TryFindSuppression(result, out _);
        }

        /// <summary>
        /// Determines whether all the sub results of the specified validation result are suppressed.
        /// </summary>
        /// <param name="result">The validation result.</param>
        /// <returns><c>true</c> if all the child results of the specified validator result are suppressed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is <see langword="null"/>.</exception>
        public bool AreAllChildrenSuppressed(IValidationResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.SubResults == null || result.SubResults.Count == 0)
            {
                return false;
            }

            foreach (var r in result.SubResults)
            {
                if (!IsSuppressed(r) && !AreAllChildrenSuppressed(r))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Adds the specified suppression.
        /// </summary>
        /// <param name="suppression">The suppression to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="suppression"/> is <see langword="null"/>.</exception>
        public void AddSuppression(VersionHistorySuppression suppression)
        {
            if (suppression == null) throw new ArgumentNullException(nameof(suppression));

            if (!suppressions.TryGetValue(suppression.ResultId, out List<VersionHistorySuppression> temp))
            {
                temp = new List<VersionHistorySuppression>();
                suppressions.Add(suppression.ResultId, temp);
            }

            temp.Add(suppression);
        }

        /// <summary>
        /// Retrieves the suppression for the specified validator result.
        /// </summary>
        /// <param name="result">The validator result.</param>
        /// <param name="suppression">When this method returns, contains the suppression that suppresses this validation result if a suppression was found; otherwise, <see langword="null"/>.</param>
        /// <returns><c>true</c> if the specified validation result has been suppressed; otherwise, <c>false</c>.</returns>
        public bool TryFindSuppression(IValidationResult result, out VersionHistorySuppression suppression)
        {
            suppression = default;

            if (result?.FullId == null)
            {
                // Old validator results.
                return false;
            }

            if (!suppressions.TryGetValue(result.FullId, out List<VersionHistorySuppression> suppressionsForResultCode))
            {
                return false;
            }

            string location = result.PositionNode.GetIdentifier();
            suppression = suppressionsForResultCode.FirstOrDefault(s => s.Location == location);
            return suppression != null;
        }
    }
}
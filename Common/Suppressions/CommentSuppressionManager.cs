namespace Skyline.DataMiner.CICD.Validators.Common.Suppressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Parsers.Common.Xml;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    /// <summary>
    /// Suppression manager for comment-based suppressions.
    /// </summary>
    public class CommentSuppressionManager
    {
        private readonly ILineInfoProvider lineInfoProvider;

        private readonly List<CommentSuppression> suppressions;
        private readonly IDictionary<string, List<CommentSuppression>> dictSuppressions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentSuppression"/> class.
        /// </summary>
        /// <param name="document">The XML document.</param>
        /// <param name="lineInfoProvider">The line info provider.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document"/> or <paramref name="lineInfoProvider"/> is <see langword="null"/>.</exception>
        public CommentSuppressionManager(XmlDocument document, ILineInfoProvider lineInfoProvider)
        {
            this.lineInfoProvider = lineInfoProvider ?? throw new ArgumentNullException(nameof(lineInfoProvider));

            suppressions = CommentSuppression.GetAllSuppressions(document).ToList();
            dictSuppressions = suppressions.GroupBy(s => s.Code).ToDictionary(x => x.Key, x => x.ToList());
        }

        /// <summary>
        /// Gets the comment-based suppressions.
        /// </summary>
        public IReadOnlyCollection<CommentSuppression> Suppressions => suppressions.AsReadOnly();

        /// <summary>
        /// Determines whether the specified validation result is suppressed.
        /// </summary>
        /// <param name="result">The validation result.</param>
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
        /// Determines whether the all sub results of the specified validation result are suppressed.
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
        public void AddSuppression(CommentSuppression suppression)
        {
            if (suppression is null)
            {
                throw new ArgumentNullException(nameof(suppression));
            }

            suppressions.Add(suppression);

            if (!dictSuppressions.TryGetValue(suppression.Code, out var list))
            {
                list = new List<CommentSuppression>();
                dictSuppressions.Add(suppression.Code, list);
            }

            list.Add(suppression);
        }

        /// <summary>
        /// Retrieves the suppression for the specified validator result.
        /// </summary>
        /// <param name="result">The validator result.</param>
        /// <param name="suppression">When this method returns, contains the suppression that suppresses this validation result if a suppression was found; otherwise, <see langword="null"/>.</param>
        /// <returns><c>true</c> if the specified validation result has been suppressed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is <see langword="null"/>.</exception>
        public bool TryFindSuppression(IValidationResult result, out CommentSuppression suppression)
        {
            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var resultCode = String.IsNullOrWhiteSpace(result.FullId) ? Convert.ToString(result.ErrorId) : result.FullId;

            if (dictSuppressions.TryGetValue(resultCode, out var suppressionsForResultCode))
            {
                int pos = GetPosition(result);
                suppression = suppressionsForResultCode.FirstOrDefault(s => pos >= s.Start && pos <= s.End);
                return suppression != null;
            }

            suppression = default;
            return false;
        }

        private int GetPosition(IValidationResult result)
        {
            if (result.Position > 0 && result.Line <= 0)
            {
                return result.Position;
            }

            if (result.Position <= 0 && result.Line > 0)
            {
                int pos = lineInfoProvider.GetOffset(result.Line - 1, 0);
                pos += lineInfoProvider.GetFirstNonWhitespaceOffset(result.Line - 1) ?? 0;

                return pos;
            }

            return -1;
        }
    }
}
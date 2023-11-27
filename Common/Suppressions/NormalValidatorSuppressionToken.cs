namespace Skyline.DataMiner.CICD.Validators.Common.Suppressions
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Skyline.DataMiner.CICD.Parsers.Common.Xml;

    /// <summary>
    /// Represents a normal (i.e. non-postponed) validator suppression token.
    /// </summary>
    public class NormalValidatorSuppressionToken : SuppressionToken, ISuppressionTokenWithReason
    {
        private static readonly Regex _regExtractSuppressValidator = new Regex(@"^(?<close>[\/\\]?)SuppressValidator\s+(?<code>[0-9\.]+)(\s+(?<reason>.+))?$", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalValidatorSuppressionToken"/> class.
        /// </summary>
        /// <param name="document">The XML document.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="isClose">Indicates whether the token is a closing token.</param>
        /// <param name="code">The suppressed code.</param>
        /// <param name="reason">The reason of suppression.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reason"/> is <see langword="null"/>.</exception>
        public NormalValidatorSuppressionToken(XmlDocument document, XmlComment comment, bool isClose, string code, string reason)
            : base(document, comment, SuppressionType.Normal, code, isClose)
        {
            Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalValidatorSuppressionToken"/> class.
        /// </summary>
        /// <param name="document">The XML document.</param>
        /// <param name="position">The comment.</param>
        /// <param name="isClose">Indicates whether the token is a closing token.</param>
        /// <param name="code">The suppressed code.</param>
        /// <param name="reason">The reason of suppression.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reason"/> is <see langword="null"/>.</exception>
        public NormalValidatorSuppressionToken(XmlDocument document, int position, bool isClose, string code, string reason)
           : base(document, position, SuppressionType.Normal, code, isClose)
        {
            Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        }

        /// <summary>
        /// Gets the suppression reason.
        /// </summary>
        /// <value>The suppression reason.</value>
        public string Reason { get; }

        /// <summary>
        /// Gets a value indicating whether the suppression reason is valid.
        /// </summary>
        /// <value><c>true</c> if the suppression reason is valid; otherwise; <c>false</c>.</value>
        /// <remarks>The suppression reason is considered invalid if it <see langword="null"/> or white space.</remarks>
        public override bool IsValid => !String.IsNullOrWhiteSpace(Reason);

        /// <summary>
        /// Parses the specified XML comment and converts it into a normal suppression token.
        /// </summary>
        /// <param name="document">The XML document.</param>
        /// <param name="comment">The XML comment representing a suppression.</param>
        /// <param name="token">When this method returns, contains the suppression token that corresponds with this XML comment, if it could be parsed; otherwise, <see langword="null"/>.</param>
        /// <returns><c>true</c> if the suppression comment could be parsed; otherwise; <c>false</c>.</returns>
        public static bool TryParse(XmlDocument document, XmlComment comment, out NormalValidatorSuppressionToken token)
        {
            string text = comment.InnerText?.Trim() ?? "";

            if (TryMatchRegex(_regExtractSuppressValidator, text, out var m))
            {
                string code = m.Groups["code"].Value;
                string reason = m.Groups["reason"].Value;
                bool isClose = new[] { "\\", "/" }.Contains(m.Groups["close"].Value);

                token = new NormalValidatorSuppressionToken(document, comment, isClose, code, reason);
                return true;
            }

            token = null;
            return false;
        }

        /// <summary>
        /// Generates an XML comment pair containing the specified result code and suppression reason.
        /// </summary>
        /// <param name="resultCode">The result code.</param>
        /// <param name="reason">The reason.</param>
        /// <param name="startComment">The start comment.</param>
        /// <param name="endComment">The end comment.</param>
        /// <exception cref="ArgumentException"><paramref name="resultCode"/> or <paramref name="reason"/> is <see langword="null"/> or white space.</exception>
        public static void CreateXmlComments(string resultCode, string reason, out string startComment, out string endComment)
        {
            if (String.IsNullOrWhiteSpace(resultCode))
            {
                throw new ArgumentException(nameof(resultCode) + " cannot be empty", nameof(resultCode));
            }

            if (String.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException(nameof(reason) + " cannot be empty", nameof(resultCode));
            }

            startComment = $"<!-- SuppressValidator {resultCode} {reason} -->";
            endComment = $"<!-- /SuppressValidator {resultCode} -->";
        }
    }
}
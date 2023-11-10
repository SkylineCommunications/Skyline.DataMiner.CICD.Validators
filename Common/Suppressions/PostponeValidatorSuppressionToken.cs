namespace Skyline.DataMiner.CICD.Validators.Common.Suppressions
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;

	using Skyline.DataMiner.CICD.Parsers.Common.Xml;

	/// <summary>
	/// Represents a postponed suppression.
	/// </summary>
	public class PostponeValidatorSuppressionToken : SuppressionToken, ISuppressionTokenWithReason
	{
		private static readonly Regex _regExtractPostponeValidator = new Regex(@"^(?<close>[\/\\]?)PostponeValidator\s+(?<code>[0-9\.]+)(\s+DCP(?<task>\d+))?(\s+(?<reason>.+))?$", RegexOptions.Compiled | RegexOptions.Singleline);

		/// <summary>
		/// Initializes a new instance of the <see cref="PostponeValidatorSuppressionToken"/> class.
		/// </summary>
		/// <param name="document">The XML document.</param>
		/// <param name="comment">The comment.</param>
		/// <param name="isClose">Indicates whether the token is a closing token.</param>
		/// <param name="code">The suppressed code.</param>
		/// <param name="dcpTask">The DCP task in which the issue will be fixed.</param>
		/// <param name="reason">The suppression reason.</param>
		/// <exception cref="ArgumentNullException"><paramref name="reason"/> is <see langword="null"/>.</exception>
		public PostponeValidatorSuppressionToken(XmlDocument document, XmlComment comment, bool isClose, string code, int dcpTask, string reason)
			: base(document, comment, SuppressionType.Postpone, code, isClose)
		{
			DcpTask = dcpTask;
			Reason = reason ?? throw new ArgumentNullException(nameof(reason));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PostponeValidatorSuppressionToken"/> class.
		/// </summary>
		/// <param name="document">The XML document.</param>
		/// <param name="position">The position.</param>
		/// <param name="isClose">Indicates whether the token is a closing token.</param>
		/// <param name="code">The suppressed code.</param>
		/// <param name="dcpTask">The DCP task in which the issue will be fixed.</param>
		/// <param name="reason">The suppression reason.</param>
		/// <exception cref="ArgumentNullException"><paramref name="reason"/> is <see langword="null"/>.</exception>
		public PostponeValidatorSuppressionToken(XmlDocument document, int position, bool isClose, string code, int dcpTask, string reason)
			: base(document, position, SuppressionType.Postpone, code, isClose)
		{
			DcpTask = dcpTask;
			Reason = reason ?? throw new ArgumentNullException(nameof(reason));
		}

		/// <summary>
		/// Gets the DCP task.
		/// </summary>
		/// <value>The DCP task.</value>
		public int DcpTask { get; }

		/// <summary>
		/// Gets the reason.
		/// </summary>
		/// <value>The reason.</value>
		public string Reason { get; }

		/// <summary>
		/// Gets a value indicating whether the suppression reason is valid.
		/// </summary>
		/// <value><c>true</c> if the suppression reason is valid; otherwise; <c>false</c>.</value>
		/// <remarks>The suppression reason is considered invalid if it <see langword="null"/> or white space or the DCP task is not provided.</remarks>
		public override bool IsValid => DcpTask > 0 && !string.IsNullOrWhiteSpace(Reason);

		/// <summary>
		/// Parses the specified XML comment and converts it into a postponed suppression token.
		/// </summary>
		/// <param name="document">The XML document.</param>
		/// <param name="comment">The XML comment representing a suppression.</param>
		/// <param name="token">When this method returns, contains the suppression token that corresponds with this XML comment, if it could be parsed; otherwise, <see langword="null"/>.</param>
		/// <returns><c>true</c> if the suppression comment could be parsed; otherwise; <c>false</c>.</returns>
		public static bool TryParse(XmlDocument document, XmlComment comment, out PostponeValidatorSuppressionToken token)
		{
			string text = comment.InnerText?.Trim() ?? "";

			if (TryMatchRegex(_regExtractPostponeValidator, text, out var m))
			{
				string code = m.Groups["code"].Value;
				string reason = m.Groups["reason"].Value;
				int.TryParse(m.Groups["task"].Value, out int dcpTask);
				bool isClose = (new[] { "\\", "/" }).Contains(m.Groups["close"].Value);

				token = new PostponeValidatorSuppressionToken(document, comment, isClose, code, dcpTask, reason);
				return true;
			}

			token = null;
			return false;
		}

		/// <summary>
		/// Generates an XML comment pair containing the specified result code DCP task ID and suppression reason.
		/// </summary>
		/// <param name="resultCode">The result code.</param>
		/// <param name="dcpTaskId">The DCP task ID.</param>
		/// <param name="reason">The reason.</param>
		/// <param name="startComment">The start comment.</param>
		/// <param name="endComment">The end comment.</param>
		/// <exception cref="ArgumentException"><paramref name="resultCode"/> or <paramref name="reason"/> is <see langword="null"/> or white space.</exception>
		public static void CreateXmlComments(string resultCode, int dcpTaskId, string reason, out string startComment, out string endComment)
		{
			if (string.IsNullOrWhiteSpace(resultCode))
			{
				throw new ArgumentException(nameof(resultCode) + " cannot be empty", nameof(resultCode));
			}

			if (string.IsNullOrWhiteSpace(reason))
			{
				throw new ArgumentException(nameof(reason) + " cannot be empty", nameof(resultCode));
			}

			startComment = $"<!-- PostponeValidator {resultCode} DCP{dcpTaskId} {reason} -->";
			endComment = $"<!-- /PostponeValidator {resultCode} -->";
		}
	}
}
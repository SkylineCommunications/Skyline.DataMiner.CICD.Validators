namespace Skyline.DataMiner.CICD.Validators.Common.Suppressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Parsers.Common.Xml;

    /// <summary>
    /// Represents a suppression that results in a suppression comment.
    /// </summary>
    public class CommentSuppression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommentSuppression"/> class.
        /// </summary>
        /// <param name="startToken">The start token.</param>
        /// <param name="endToken">The end token.</param>
        /// <exception cref="ArgumentNullException"><paramref name="startToken"/> or <paramref name="endToken"/> is <see langword="null"/>.</exception>
        public CommentSuppression(SuppressionToken startToken, SuppressionToken endToken)
        {
            StartToken = startToken ?? throw new ArgumentNullException(nameof(startToken));
            EndToken = endToken ?? throw new ArgumentNullException(nameof(endToken));

            Start = StartToken.Position;
            End = EndToken.Position;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentSuppression"/> class.
        /// </summary>
        /// <param name="startToken">The start token.</param>
        /// <param name="end">The end.</param>
        /// <exception cref="ArgumentNullException"><paramref name="startToken"/> is <see langword="null"/>.</exception>
        public CommentSuppression(SuppressionToken startToken, int end)
        {
            StartToken = startToken ?? throw new ArgumentNullException(nameof(startToken));

            Start = StartToken.Position;
            End = end;
        }

        /// <summary>
        /// Gets the XML document.
        /// </summary>
        /// <value>The XML document.</value>
        public XmlDocument Document => StartToken.Document;

        /// <summary>
        /// Gets the start token.
        /// </summary>
        /// <value>The start token.</value>
        public SuppressionToken StartToken { get; }

        /// <summary>
        /// Gets the end token.
        /// </summary>
        /// <value>The end token.</value>
        public SuppressionToken EndToken { get; }

        /// <summary>
        /// Gets the start.
        /// </summary>
        /// <value>The start.</value>
        public int Start { get; }

        /// <summary>
        /// Gets the end.
        /// </summary>
        /// <value>The end.</value>
        public int End { get; }

        /// <summary>
        /// Gets the suppression type.
        /// </summary>
        /// <value>The suppression type.</value>
        public SuppressionType Type => StartToken.Type;

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code => StartToken.Code;

        /// <summary>
        /// Retrieves all comment-based suppressions from the specified XML document.
        /// </summary>
        /// <param name="document">The XML document.</param>
        /// <param name="onlyValid">Indicates whether only valid suppressions should be retrieved.</param>
        /// <returns>All comment-based suppressions from the specified XML document.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="document"/> is <see langword="null"/>.</exception>
        public static IEnumerable<CommentSuppression> GetAllSuppressions(XmlDocument document, bool onlyValid = true)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var suppressions = GetAllSuppressionsInternal(document);

            if (onlyValid)
            {
                suppressions = suppressions.Where(s => s.StartToken.IsValid);
            }

            return suppressions;
        }

        private static IEnumerable<CommentSuppression> GetAllSuppressionsInternal(XmlDocument document)
        {
            var openTokens = new Dictionary<(SuppressionType, string), Stack<SuppressionToken>>();

            var tokens = SuppressionToken.GetAllTokens(document);
            foreach (var token in tokens.OrderBy(t => t.Position))
            {
                if (!openTokens.TryGetValue((token.Type, token.Code), out var stack))
                {
                    stack = new Stack<SuppressionToken>();
                    openTokens.Add((token.Type, token.Code), stack);
                }

                if (token.IsOpen)
                {
                    // register open token
                    stack.Push(token);
                }
                else if (stack.Count > 0)
                {
                    var openToken = stack.Pop();
                    yield return new CommentSuppression(openToken, token);
                }
                else
                {
                    // no open token found => ignore end token
                }
            }

            // handle unclosed suppression tokens
            foreach (var stack in openTokens.Values)
            {
                foreach (var openToken in stack)
                {
                    int endPos = TryGetSuppressionEndPosition(openToken) ?? document.LastCharOffset + 1;
                    yield return new CommentSuppression(openToken, endPos);
                }
            }
        }

        private static int? TryGetSuppressionEndPosition(SuppressionToken token)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var parent = token.CommentNode?.ParentNode;
            if (parent == null)
            {
                return null;
            }

            var index = parent.Children.IndexOf(token.CommentNode);
            if (index < 0)
            {
                return null;
            }

            var nextSiblingElement = parent.Children.Skip(index + 1).OfType<XmlElement>().FirstOrDefault();
            if (nextSiblingElement != null)
            {
                return nextSiblingElement.LastCharOffset + 1;
            }
            else
            {
                return parent.LastCharOffset + 1;
            }
        }
    }
}
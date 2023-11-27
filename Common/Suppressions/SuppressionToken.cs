namespace Skyline.DataMiner.CICD.Validators.Common.Suppressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Skyline.DataMiner.CICD.Parsers.Common.Xml;

    /// <summary>
    /// Represents a suppression token.
    /// </summary>
    public abstract class SuppressionToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuppressionToken"/> class.
        /// </summary>
        /// <param name="document">The XML document.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="type">The suppression type.</param>
        /// <param name="code">The code.</param>
        /// <param name="isClose">Indicates whether this is a closing token.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comment"/> of <paramref name="comment"/> is <see langword="null"/>.</exception>
        protected SuppressionToken(XmlDocument document, XmlComment comment, SuppressionType type, string code, bool isClose)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            CommentNode = comment ?? throw new ArgumentNullException(nameof(comment));
            Position = comment.FirstCharOffset;
            Type = type;
            Code = code ?? throw new ArgumentNullException(nameof(code));
            IsClose = isClose;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuppressionToken"/> class.
        /// </summary>
        /// <param name="document">The XML document.</param>
        /// <param name="position">The position.</param>
        /// <param name="type">The suppression type.</param>
        /// <param name="code">The code.</param>
        /// <param name="isClose">Indicates whether this is a closing token.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document"/> or <paramref name="code"/> is <see langword="null"/>.</exception>
        protected SuppressionToken(XmlDocument document, int position, SuppressionType type, string code, bool isClose)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            Position = position;
            Type = type;
            Code = code ?? throw new ArgumentNullException(nameof(code));
            IsClose = isClose;
        }

        /// <summary>
        /// Gets the XML document.
        /// </summary>
        /// <value>The XML document.</value>
        public XmlDocument Document { get; }

        /// <summary>
        /// Gets the comment node.
        /// </summary>
        /// <value>The comment node.</value>
        public XmlComment CommentNode { get; }

        /// <summary>
        /// Gets the suppression type.
        /// </summary>
        /// <value>The suppression type.</value>
        public SuppressionType Type { get; }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; }

        /// <summary>
        /// Gets a value indicating whether this is a closing token.
        /// </summary>
        /// <value><c>true</c> if this is a closing token; otherwise, <c>false</c>.</value>
        public bool IsClose { get; }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public int Position { get; }

        /// <summary>
        /// Gets a value indicating whether this is an opening token.
        /// </summary>
        /// <value><c>true</c> if this is an opening token; otherwise, <c>false</c>.</value>
        public bool IsOpen => !IsClose;

        /// <summary>
        /// Gets a value indicating whether this token is valid.
        /// </summary>
        /// <value><c>true</c> if this token is valid; otherwise, <c>false</c>.</value>
        public abstract bool IsValid { get; }

        /// <summary>
        /// Retrieves all tokens of the specified XML document.
        /// </summary>
        /// <param name="document">The XML document.</param>
        /// <returns>All tokens of the specified XML document.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="document"/> is <see langword="null"/>.</exception>
        public static IEnumerable<SuppressionToken> GetAllTokens(XmlDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var allComments = GetAllComments(document);

            foreach (var comment in allComments)
            {
                if (DetectToken(document, comment, out var token))
                {
                    yield return token;
                }
            }
        }

        internal static bool DetectToken(XmlDocument document, XmlComment comment, out SuppressionToken token)
        {
            if (NormalValidatorSuppressionToken.TryParse(document, comment, out var t1))
            {
                token = t1;
                return true;
            }

            if (PostponeValidatorSuppressionToken.TryParse(document, comment, out var t2))
            {
                token = t2;
                return true;
            }

            token = null;
            return false;
        }

        /// <summary>
        /// Tries to match the specified text to the specified regular expression.
        /// </summary>
        /// <param name="regex">The regular expression.</param>
        /// <param name="text">The text.</param>
        /// <param name="match">When this method returns, contains the match if the there was a match; otherwise, <see langword="null"/>.</param>
        /// <returns></returns>
        protected static bool TryMatchRegex(Regex regex, string text, out Match match)
        {
            var m = regex.Match(text);
            if (m.Success)
            {
                match = m;
                return true;
            }
            else
            {
                match = null;
                return false;
            }
        }

        private static ICollection<XmlComment> GetAllComments(XmlDocument document)
        {
            var allComments = new List<XmlComment>();

            TreeWalker<XmlNode>(document, x => x is XmlContainer c ? c.Children : Enumerable.Empty<XmlNode>(), x =>
            {
                if (x is XmlComment c)
                {
                    allComments.Add(c);
                }
            });

            return allComments;
        }

        private static void TreeWalker<T>(T root, Func<T, IEnumerable<T>> getChildren, Action<T> processNode) where T : class
        {
            Queue<T> nodes = new Queue<T>();
            nodes.Enqueue(root);

            while (nodes.Count > 0)
            {
                T node = nodes.Dequeue();

                processNode(node);

                var children = getChildren(node);
                foreach (var c in children)
                {
                    nodes.Enqueue(c);
                }
            }
        }
    }
}
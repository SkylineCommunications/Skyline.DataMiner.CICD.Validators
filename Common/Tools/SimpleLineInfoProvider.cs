namespace Skyline.DataMiner.CICD.Validators.Common.Tools
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    /// <summary>
    /// Represents a simple line info provider.
    /// </summary>
    public class SimpleLineInfoProvider : ILineInfoProvider
    {
        private readonly string text;
        private readonly Lazy<List<int>> lineBreaks;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLineInfoProvider"/> class.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        public SimpleLineInfoProvider(string text)
        {
            this.text = text ?? throw new ArgumentNullException(nameof(text));

            lineBreaks = new Lazy<List<int>>(GetLinebreakPositions);
        }

        /// <summary>
        /// Gets the number of lines.
        /// </summary>
        /// <value>The number of lines.</value>
        public int Lines => lineBreaks.Value.Count + 1;

        /// <inheritdoc	/>
        public void GetLineAndColumn(int offset, out int lineNumber, out int columnIndex)
        {
            if (offset < 0 || offset > text.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            lineNumber = lineBreaks.Value.BinarySearch(offset);
            if (lineNumber < 0)
            {
                lineNumber = ~lineNumber;
            }

            columnIndex = offset - GetOffset(lineNumber);
        }

        /// <inheritdoc	/>
        public int GetOffset(int lineNumber, int columnIndex)
        {
            if (lineNumber < 0 || lineNumber >= Lines)
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            int offset = GetOffset(lineNumber);
            return offset + columnIndex;
        }

        /// <inheritdoc	/>
        public int? GetFirstNonWhitespaceOffset(int lineNumber)
        {
            if (lineNumber < 0 || lineNumber >= Lines)
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            string text = GetText(lineNumber);

            for (int i = 0; i < text.Length; i++)
            {
                if (!Char.IsWhiteSpace(text[i]))
                {
                    return i;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the offset of the specified line.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <returns>The offset.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="lineNumber"/> is invalid.</exception>
        private int GetOffset(int lineNumber)
        {
            if (lineNumber < 0 || lineNumber >= Lines)
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            if (lineNumber == 0)
            {
                return 0;
            }

            return lineBreaks.Value[lineNumber - 1] + 1;
        }

        /// <summary>
        /// Retrieves the text of the specified line.
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private string GetText(int lineNumber)
        {
            if (lineNumber < 0 || lineNumber >= Lines)
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            int start, end;

            if (lineNumber == 0)
            {
                start = 0;
                end = lineBreaks.Value.Count > 0 ? lineBreaks.Value[0] : text.Length;
            }
            else if (lineNumber == Lines - 1)
            {
                start = GetOffset(lineNumber);
                end = text.Length;
            }
            else
            {
                start = GetOffset(lineNumber);
                end = GetOffset(lineNumber + 1);
            }

            return text.Substring(start, end - start);
        }

        private List<int> GetLinebreakPositions()
        {
            var lineBreaks = new List<int>();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    lineBreaks.Add(i);
                }
            }

            return lineBreaks;
        }
    }
}

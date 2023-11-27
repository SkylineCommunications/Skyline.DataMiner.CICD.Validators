namespace Skyline.DataMiner.CICD.Validators.Common.Interfaces
{
    /// <summary>
    /// Represents a line info provider.
    /// </summary>
    public interface ILineInfoProvider
    {
        /// <summary>
        /// Retrieves the line and column from the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="columnIndex">The column index.</param>
        void GetLineAndColumn(int offset, out int lineNumber, out int columnIndex);

        /// <summary>
        /// Retrieves the offset from the specified line number and column index.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="columnIndex">The column index.</param>
        /// <returns>The offset.</returns>
        int GetOffset(int lineNumber, int columnIndex);

        /// <summary>
        /// Retrieves the first non white space offset starting from the specified line number.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <returns>The first non white space offset starting from the specified line number.</returns>
        int? GetFirstNonWhitespaceOffset(int lineNumber);
    }
}

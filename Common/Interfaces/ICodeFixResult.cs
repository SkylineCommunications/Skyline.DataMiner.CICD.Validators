namespace Skyline.DataMiner.CICD.Validators.Common.Interfaces
{
    /// <summary>
    /// Represents the result of performing a code fix.
    /// </summary>
    public interface ICodeFixResult
    {
        /// <summary>
        /// Gets a value indicating whether the application of the code fix succeeded.
        /// </summary>
        /// <value><c>true</c> if applying the code fix succeeded; otherwise, <c>false</c>.</value>
        bool Success { get; }

        /// <summary>
        /// Gets the message related to application of the code fix.
        /// </summary>
        /// <value>The message related to application of the code fix.</value>
        string Message { get; }
    }
}

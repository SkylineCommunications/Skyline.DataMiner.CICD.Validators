namespace Skyline.DataMiner.CICD.Validators.Common.Suppressions
{
    /// <summary>
    /// Represents a suppression token with a specified reason.
    /// </summary>
    public interface ISuppressionTokenWithReason
    {
        /// <summary>
        /// Gets the reason.
        /// </summary>
        /// <value>The reason.</value>
        string Reason { get; }
    }
}
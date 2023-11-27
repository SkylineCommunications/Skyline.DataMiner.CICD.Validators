namespace Skyline.DataMiner.CICD.Validators.Common.Suppressions
{
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;

    /// <summary>
    /// Represents a suppression.
    /// </summary>
    public interface ISuppression
    {
        /// <summary>
        /// Gets the location of the suppression.
        /// </summary>
        /// <value>The location of the suppression.</value>
        string Location { get; }

        /// <summary>
        /// Gets the result ID.
        /// </summary>
        /// <value>The result ID.</value>
        string ResultId { get; }

        /// <summary>
        /// Gets the reason.
        /// </summary>
        /// <value>The reason.</value>
        string Reason { get; }

        /// <summary>
        /// Gets the suppression type.
        /// </summary>
        /// <value>The suppression type.</value>
        EnumSuppressionType Type { get; }
    }
}
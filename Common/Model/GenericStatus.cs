namespace Skyline.DataMiner.CICD.Validators.Common.Model
{
    using System;

    /// <summary>
    /// Represents the generic status.
    /// </summary>
    [Flags]
    public enum GenericStatus
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,
        /// <summary>
        /// Missing.
        /// </summary>
        Missing = 1,
        /// <summary>
        /// Empty.
        /// </summary>
        Empty = 2,
        /// <summary>
        /// Untrimmed.
        /// </summary>
        Untrimmed = 4,

        /// <summary>
        /// Invalid.
        /// </summary>
        /// <remarks>When the ValueTag.Value is null this indicates the value could not be parsed to the model type (ex: Enum, Int32, etc.).</remarks>
        Invalid = 8,

        //Something = 16,
        //Something = 32,
    }
}
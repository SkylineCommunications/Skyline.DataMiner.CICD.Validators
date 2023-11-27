namespace Skyline.DataMiner.CICD.Validators.Common.Model
{
    /// <summary>
    /// Represents a validator severity.
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = -999, // Not to be used
        /// <summary>
        /// Information.
        /// </summary>
		Information = -1, // Remnant of old validator
        /// <summary>
        /// Bubble up.
        /// </summary>
        BubbleUp = -100, // Default bubble up
        /// <summary>
        /// Warning.
        /// </summary>
        Warning = 0,
        /// <summary>
        /// Minor.
        /// </summary>
        Minor,
        /// <summary>
        /// Major.
        /// </summary>
        Major,
        /// <summary>
        /// Critical.
        /// </summary>
        Critical,
    }
}
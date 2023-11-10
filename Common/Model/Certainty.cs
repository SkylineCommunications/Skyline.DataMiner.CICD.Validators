namespace Skyline.DataMiner.CICD.Validators.Common.Model
{
    /// <summary>
    /// Represents the certainty of a detected issue.
    /// </summary>
    public enum Certainty
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = -1,
        /// <summary>
        /// Certain.
        /// </summary>
        Certain = 100,
        /// <summary>
        /// Uncertain.
        /// </summary>
        Uncertain = 50,
    }
}
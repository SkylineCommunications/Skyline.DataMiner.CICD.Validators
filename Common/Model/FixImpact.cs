namespace Skyline.DataMiner.CICD.Validators.Common.Model
{
    /// <summary>
    /// Represents the impact of a fix.
    /// </summary>
    public enum FixImpact
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined,
        /// <summary>
        /// Non breaking.
        /// </summary>
        NonBreaking,
        /// <summary>
        /// Breaking.
        /// </summary>
        Breaking,
    }
}
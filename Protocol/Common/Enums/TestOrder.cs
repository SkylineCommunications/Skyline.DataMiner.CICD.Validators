namespace Skyline.DataMiner.CICD.Validators.Protocol.Common.Enums
{
    /// <summary>
    /// Order of how the checks will be executed.
    /// </summary>
    public enum TestOrder
    {
        /// <summary>
        /// Will run before the <see cref="Mid"/> checks.
        /// </summary>
        Pre1 = 100,

        /// <summary>
        /// Default value for running checks.
        /// </summary>
        Mid = 200,

        /// <summary>
        /// Will run after <see cref="Mid"/> checks.
        /// </summary>
        Post1 = 300
    }
}
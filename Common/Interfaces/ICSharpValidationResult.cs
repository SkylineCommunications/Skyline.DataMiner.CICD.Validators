namespace Skyline.DataMiner.CICD.Validators.Common.Interfaces
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Represents a validator result related to C# code.
    /// </summary>
    public interface ICSharpValidationResult : IValidationResult
    {
        /// <summary>
        /// Gets the location in source code.
        /// </summary>
        /// <value>The location in source code.</value>
        Location CSharpLocation { get; }
    }
}
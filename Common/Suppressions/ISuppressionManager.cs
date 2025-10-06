namespace Skyline.DataMiner.CICD.Validators.Common.Suppressions
{
    using System;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    /// <summary>
    /// Defines methods for managing suppressions of validation results.
    /// </summary>
    public interface ISuppressionManager
    {
        /// <summary>
        /// Determines whether the specified validator result is suppressed.
        /// </summary>
        /// <param name="result">The validator result.</param>
        /// <returns><c>true</c> if the specified validator result is suppressed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is <see langword="null"/>.</exception>
        bool IsSuppressed(IValidationResult result);

        /// <summary>
        /// Determines whether all the sub results of the specified validation result are suppressed.
        /// </summary>
        /// <param name="result">The validation result.</param>
        /// <returns><c>true</c> if all the child results of the specified validator result are suppressed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is <see langword="null"/>.</exception>
        bool AreAllChildrenSuppressed(IValidationResult result);
    }

    /// <summary>
    /// Defines methods for managing suppressions of validation results.
    /// </summary>
    /// <typeparam name="T">The type of the suppression object. Must be a reference type.</typeparam>
    public interface ISuppressionManager<T> : ISuppressionManager where T : class
    {
        /// <summary>
        /// Adds the specified suppression.
        /// </summary>
        /// <param name="suppression">The suppression to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="suppression"/> is <see langword="null"/>.</exception>
        void AddSuppression(T suppression);

        /// <summary>
        /// Retrieves the suppression for the specified validator result.
        /// </summary>
        /// <param name="result">The validator result.</param>
        /// <param name="suppression">When this method returns, contains the suppression that suppresses this validation result if a suppression was found; otherwise, <see langword="null"/>.</param>
        /// <returns><c>true</c> if the specified validation result has been suppressed; otherwise, <c>false</c>.</returns>
        bool TryFindSuppression(IValidationResult result, out T suppression);
    }
}
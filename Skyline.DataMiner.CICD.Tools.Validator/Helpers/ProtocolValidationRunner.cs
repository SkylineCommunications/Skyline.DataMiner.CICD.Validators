namespace Skyline.DataMiner.CICD.Tools.Validator.Helpers
{
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    /// <summary>
    /// Runs both the legacy and the new protocol validators against a given
    /// <see cref="IProtocolInputData"/> instance and aggregates the results.
    /// </summary>
    internal static class ProtocolValidationRunner
    {
        /// <summary>
        /// Runs the legacy and the new validators in parallel and returns the combined results.
        /// </summary>
        /// <param name="inputData">The protocol input data to validate.</param>
        /// <param name="settings">The validator settings to use.</param>
        /// <param name="cancellationToken">A token to observe cancellation requests.</param>
        /// <returns>The combined validation results from both validators.</returns>
        public static IList<IValidationResult> Run(IProtocolInputData inputData, ValidatorSettings settings, CancellationToken cancellationToken = default)
        {
            Task<IList<IValidationResult>>[] tasks =
            [
                Task.Factory.StartNew(() =>
                {
                    // Legacy validator.
                    var validator = new Validators.Protocol.Legacy.Validator();

                    return validator.RunValidate(inputData, settings, cancellationToken);
                }, cancellationToken),
                Task.Factory.StartNew(() =>
                {
                    // New validator.
                    var validator = new Validators.Protocol.Validator();

                    return validator.RunValidate(inputData, settings, cancellationToken);
                }, cancellationToken)
            ];

            return Task.WhenAll(tasks).Result.SelectMany(x => x).ToList();
        }
    }
}

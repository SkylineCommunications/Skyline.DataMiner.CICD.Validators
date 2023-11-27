namespace Skyline.DataMiner.CICD.Validators.Common.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;

    using Skyline.DataMiner.CICD.Parsers.Common.XmlEdit;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    /// <summary>
    /// Validator interface.
    /// </summary>
	public interface IValidator
    {
        /// <summary>
        /// Runs the validator on the specified input.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="validatorSettings">The validator settings.</param>
        /// <returns>The validator results.</returns>
        IList<IValidationResult> RunValidate(IProtocolInputData input, ValidatorSettings validatorSettings, CancellationToken cancellationToken);

        /// <summary>
        /// Performs a comparison between the two provided connector versions and returns detected major changes, if any.
        /// </summary>
        /// <param name="newData">The data representing the new version.</param>
        /// <param name="oldData">The data representing the previous version.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="validatorSettings">The validator settings.</param>
        /// <returns>Detected major changes, if any.</returns>
        IList<IValidationResult> RunCompare(IProtocolInputData newData, IProtocolInputData oldData, ValidatorSettings validatorSettings, CancellationToken cancellationToken);

        /// <summary>
        /// Performs a code fix.
        /// </summary>
        /// <param name="document">The XML document.</param>
        /// <param name="protocol">The protocol model.</param>
        /// <param name="result">The validation result.</param>
        /// <param name="validatorSettings">The validator settings.</param>
        /// <returns>The code fix result.</returns>
        ICodeFixResult ExecuteCodeFix(XmlDocument document, Models.Protocol.Edit.Protocol protocol, IValidationResult result, ValidatorSettings validatorSettings);
    }
}
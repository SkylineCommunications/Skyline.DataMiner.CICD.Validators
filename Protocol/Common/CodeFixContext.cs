namespace Skyline.DataMiner.CICD.Validators.Protocol.Common
{
    using Skyline.DataMiner.CICD.Models.Protocol.Edit;
    using Skyline.DataMiner.CICD.Parsers.Common.XmlEdit;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    internal class CodeFixContext
    {
        public XmlDocument Document { get; }
        public Protocol Protocol { get; }

        /// <summary>
        /// The validator result that should be fixed.
        /// </summary>
        public ValidationResult Result { get; }

        public ValidatorSettings ValidatorSettings { get; }

        public CodeFixContext(XmlDocument document, Protocol protocol, ValidationResult result, ValidatorSettings validatorSettings)
        {
            Document = document;
            Protocol = protocol;
            Result = result;
            ValidatorSettings = validatorSettings;
        }
    }
}
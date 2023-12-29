namespace Skyline.DataMiner.CICD.Validators.Protocol.Common
{
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Parsers.Common.Xml;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    internal class MajorChangeCheckContext
    {
        public IProtocolInputData NewInputData { get; }

        public IProtocolInputData OldInputData { get; }

        public ValidatorSettings ValidatorSettings { get; }

        public IProtocolModel NewProtocolModel => NewInputData.Model;

        public XmlDocument NewDocument => NewInputData.Document;

        public IProtocolModel PreviousProtocolModel => OldInputData.Model;

        public XmlDocument PreviousDocument => OldInputData.Document;

        public MajorChangeCheckContext(IProtocolInputData newData, IProtocolInputData oldData, ValidatorSettings validatorSettings)
        {
            NewInputData = newData ?? throw new System.ArgumentNullException(nameof(newData));
            OldInputData = oldData ?? throw new System.ArgumentNullException(nameof(oldData));
            ValidatorSettings = validatorSettings ?? throw new System.ArgumentNullException(nameof(validatorSettings));
        }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Common
{
    using System;
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Parsers.Common.Xml;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;

    internal class ValidatorContext
    {
        private readonly Lazy<IReadOnlyDictionary<ProjectId, CompiledQActionProject>> compiledQActions;

        public ValidatorContext(IProtocolInputData input, ValidatorSettings validatorSettings)
        {
            InputData = input ?? throw new ArgumentNullException(nameof(input));
            ValidatorSettings = validatorSettings ?? throw new ArgumentNullException(nameof(validatorSettings));

            compiledQActions = new Lazy<IReadOnlyDictionary<ProjectId, CompiledQActionProject>>(() => InputData.QActionCompilationModel?.Build() ?? new Dictionary<ProjectId, CompiledQActionProject>(0));

            Helpers = new HelperCollection(validatorSettings);
        }

        public IProtocolInputData InputData { get; }

        public XmlDocument Document => InputData.Document;

        public IProtocolModel ProtocolModel => InputData.Model;

        public ValidatorSettings ValidatorSettings { get; }

        public IReadOnlyDictionary<ProjectId, CompiledQActionProject> CompiledQActions => compiledQActions.Value;

        public CrossData.CrossData CrossData { get; } = new CrossData.CrossData();

        internal HelperCollection Helpers { get; }
    }
}
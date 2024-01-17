namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Common
{
    using System;
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    internal class FeatureCheckContext
    {
        public FeatureCheckContext(IProtocolInputData input, IReadOnlyDictionary<ProjectId, CompiledQActionProject> compiledQActions, bool isSolutionBased)
        {
            InputData = input ?? throw new ArgumentNullException(nameof(input));
            IsSolution = isSolutionBased;
            CompiledQActions = compiledQActions;
        }

        public FeatureCheckContext(IProtocolInputData input, bool isSolutionBased)
        {
            InputData = input ?? throw new ArgumentNullException(nameof(input));
            IsSolution = isSolutionBased;

            if (input.QActionCompilationModel != null)
            {
                CompiledQActions = input.QActionCompilationModel.Build();
            }
        }

        public FeatureCheckContext(IProtocolInputData input) : this(input, false)
        {
        }

        public IProtocolInputData InputData { get; }

        public IReadOnlyDictionary<ProjectId, CompiledQActionProject> CompiledQActions { get; }

        public IProtocolModel Model => InputData.Model;

        public bool IsSolution { get; }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Helpers
{
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal class QActionAnalyzerBase : CSharpAnalyzerBase
    {
        protected readonly IValidate test;
        protected readonly ValidatorContext context;
        protected readonly List<IValidationResult> results;

        protected readonly IQActionsQAction qAction;
        protected readonly SemanticModel semanticModel;
        protected readonly Solution solution;

        protected QActionAnalyzerBase(IValidate test, ValidatorContext context, List<IValidationResult> results, IQActionsQAction qAction, SemanticModel semanticModel, Solution solution)
        {
            this.test = test;
            this.context = context;
            this.results = results;

            this.qAction = qAction;
            this.semanticModel = semanticModel;
            this.solution = solution;
        }
    }
}
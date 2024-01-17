namespace Skyline.DataMiner.CICD.Validators.Protocol.Helpers
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal class ValidateHelperBase
    {
        protected readonly IValidate test;
        protected readonly ValidatorContext context;
        protected readonly List<IValidationResult> results;

        protected ValidateHelperBase(IValidate test, ValidatorContext context, List<IValidationResult> results)
        {
            this.test = test;
            this.context = context;
            this.results = results;
        }
    }
}
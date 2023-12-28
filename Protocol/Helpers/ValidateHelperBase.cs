namespace SLDisValidator2.Helpers
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

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
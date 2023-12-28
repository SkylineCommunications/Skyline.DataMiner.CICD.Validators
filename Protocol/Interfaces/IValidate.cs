namespace SLDisValidator2.Interfaces
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    using SLDisValidator2.Common;

    public interface IValidate : IRoot
    {
        List<IValidationResult> Validate(ValidatorContext context);
    }
}
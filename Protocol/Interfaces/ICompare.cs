namespace SLDisValidator2.Interfaces
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    using SLDisValidator2.Common;

    public interface ICompare : IRoot
    {
        List<IValidationResult> Compare(MajorChangeCheckContext context);
    }
}
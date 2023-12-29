namespace Skyline.DataMiner.CICD.Validators.Protocol.Interfaces
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    using Skyline.DataMiner.CICD.Validators.Protocol.Common;

    internal interface ICompare : IRoot
    {
        List<IValidationResult> Compare(MajorChangeCheckContext context);
    }
}
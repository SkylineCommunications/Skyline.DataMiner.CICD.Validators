namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Exceptions.CheckTypeTag
{
    using System.Collections.Generic;
    
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckTypeTag, Category.Param)]
    internal class CheckTypeTag : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var param in context.EachParamWithValidId())
            {
                if (param.IsWrite() && param.Interprete?.Exceptions != null)
                {
                    results.Add(Error.ExceptionIncompatibleWithParamType(this, param, param.Interprete.Exceptions, param.Id.RawValue));
                }
            }

            return results;
        }
    }
}
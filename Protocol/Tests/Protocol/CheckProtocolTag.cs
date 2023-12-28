namespace SLDisValidator2.Tests.Protocol.CheckProtocolTag
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckProtocolTag, Category.Protocol)]
    internal class CheckProtocolTag : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var model = context.ProtocolModel;

            if (model.Protocol == null)
            {
                results.Add(Error.MissingTag(this, null, null));
            }

            return results;
        }
    }
}
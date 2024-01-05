namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckProtocolTag
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

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
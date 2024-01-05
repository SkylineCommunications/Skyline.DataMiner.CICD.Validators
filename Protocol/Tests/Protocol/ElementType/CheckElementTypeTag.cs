namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ElementType.CheckElementTypeTag
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckElementTypeTag, Category.Protocol)]
    internal class CheckElementTypeTag : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            if (context.ProtocolModel.Protocol == null)
            {
                return results;
            }

            var protocol = context.ProtocolModel.Protocol;
            var elementType = protocol.ElementType;

            // Check if tag is there
            if (elementType == null)
            {
                results.Add(Error.MissingTag(this, protocol, protocol));
                return results;
            }

            // Check if tag is filled in
            if (String.IsNullOrWhiteSpace(elementType.Value))
            {
                results.Add(Error.EmptyTag(this, elementType, elementType));
                return results;
            }

            return results;
        }
    }
}
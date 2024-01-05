namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.SNMP.CheckIncludepagesAttribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckIncludepagesAttribute, Category.Protocol)]
    internal class CheckIncludepagesAttribute : IValidate, ICodeFix
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var model = context.ProtocolModel;

            if (model?.Protocol?.SNMP == null)
            {
                return results;
            }

            ISNMP snmp = model.Protocol.SNMP;

            // Check if attribute is there
            if (snmp.Includepages == null)
            {
                results.Add(Error.MissingAttribute(this, snmp, snmp));
                return results;
            }

            (GenericStatus valueStatus, bool _) = GenericTests.CheckBasics<bool>(snmp.Includepages.RawValue);

            if (valueStatus.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(this, snmp, snmp.Includepages));
                return results;
            }

            string[] allowedValues =
            {
                "true",
                "false"
            };
            if (valueStatus.HasFlag(GenericStatus.Invalid) || !allowedValues.Contains(snmp.Includepages.RawValue))
            {
                // Can't parse it to a bool or invalid casing of the value.
                results.Add(Error.InvalidAttribute(this, snmp, snmp.Includepages, snmp.Includepages.RawValue, String.Join("; ", allowedValues)));
                return results;
            }

            return results;

        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.MissingAttribute:
                case ErrorIds.InvalidAttribute:
                case ErrorIds.EmptyAttribute:
                    {
                        if (context.Protocol.SNMP.Includepages?.Value == null)
                        {
                            // When empty or missing tag or unknown value
                            context.Protocol.SNMP.Includepages = new Skyline.DataMiner.CICD.Models.Protocol.Edit.AttributeValue<bool?>(true);
                        }
                        else
                        {
                            // When wrong casing
                            context.Protocol.SNMP.Includepages = new Skyline.DataMiner.CICD.Models.Protocol.Edit.AttributeValue<bool?>(context.Protocol.SNMP.Includepages.Value);
                        }

                        result.Success = true;
                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }
    }
}
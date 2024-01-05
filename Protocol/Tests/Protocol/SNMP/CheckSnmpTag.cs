namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.SNMP.CheckSnmpTag
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckSnmpTag, Category.Protocol)]
    internal class CheckSnmpTag : IValidate, ICodeFix
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var model = context.ProtocolModel;

            if (model?.Protocol == null)
            {
                return results;
            }

            ISNMP snmp = model.Protocol.SNMP;

            (GenericStatus status, string rawValue, EnumSNMP? _) = GenericTests.CheckBasics(snmp, true);

            // Check if tag is there
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingTag(this, model.Protocol, model.Protocol));
                return results;
            }

            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyTag(this, snmp, snmp));
                return results;
            }

            if (status.HasFlag(GenericStatus.Invalid))
            {
                results.Add(Error.InvalidValue(this, snmp, snmp, rawValue, "auto, false"));
                return results;
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.MissingTag:
                    {
                        context.Protocol.SNMP = new Skyline.DataMiner.CICD.Models.Protocol.Edit.SNMP(EnumSNMP.Auto)
                        {
                            //Includepages = new Skyline.DataMiner.CICD.Models.Protocol.Edit.AttributeValue<bool?>(true)
                            Includepages = true
                        };

                        result.Success = true;
                        break;
                    }

                case ErrorIds.EmptyTag:
                case ErrorIds.InvalidValue:
                    {
                        context.Protocol.SNMP.Value = EnumSNMP.Auto;
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
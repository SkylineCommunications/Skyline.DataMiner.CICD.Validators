namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.SNMP.OID.CheckOptionsAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckOptionsAttribute, Category.Param)]
    internal class CheckOptionsAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.SNMP?.OID == null)
                {
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, param);

                if (helper.ValidateBasics())
                {
                    continue;
                }

                helper.ValidateInstanceOption();
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol?.Params == null)
            {
                result.Message = "No Params found!";
                return result;
            }

            var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
            var paramEditNode = context.Protocol.Params.Get(paramReadNode);
            if (paramEditNode == null)
            {
                result.Message = "No Param Edit Node found!";
                return result;
            }
            
            if (paramEditNode.SNMP == null)
            {
                result.Message = "No SNMP tag found.";
                return result;
            }

            if (paramEditNode.SNMP.OID == null)
            {
                result.Message = "No OID tag found.";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.EmptyAttribute:
                    paramEditNode.SNMP.OID.Options = null;
                    result.Success = true;
                    break;

                case ErrorIds.UntrimmedAttribute:
                    // Value is already trimmed
                    paramEditNode.SNMP.OID.Options = paramReadNode.SNMP.OID.Options.Value;
                    result.Success = true;
                    break;

                case ErrorIds.MissingInstanceOption:
                    if (paramEditNode.SNMP.OID.Options == null)
                    {
                        result.Message = "No options attribute found.";
                        break;
                    }

                    paramEditNode.SNMP.OID.Options.Value = paramEditNode.SNMP.OID.Options.Value.Insert(0, "instance;");
                    result.Success = true;
                    break;

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }
        
        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        private readonly IParamsParam param;
        private readonly SnmpOidOptions options;
        private readonly IValueTag<string> optionsAttribute;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param) : base(test, context, results)
        {
            this.param = param;
            optionsAttribute = param.SNMP?.OID?.Options;
            options = param.SNMP?.OID?.GetOptions();
        }

        /// <summary>
        /// Validates the basics.
        /// </summary>
        /// <returns>True when nothing further needs to be validated (e.g.: missing attribute).</returns>
        public bool ValidateBasics()
        {
            (GenericStatus status, string _, string _) = GenericTests.CheckBasics(optionsAttribute, isRequired: true);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                // No error, but no point in checking further.
                return true;    
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, param, optionsAttribute, param.Id.RawValue));
                return true;
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(test, param, optionsAttribute, param.Id.RawValue, optionsAttribute.RawValue));
            }

            return false;
        }

        public void ValidateInstanceOption()
        {
            if (options.HasInstance)
            {
                // Has instance already.
                return;
            }

            if (options.PartialSnmp != null)
            {
                // Has partialSnmp options specified
                results.Add(Error.MissingInstanceOption(test, param, optionsAttribute, param.Id.RawValue));
            }
        }
    }
}
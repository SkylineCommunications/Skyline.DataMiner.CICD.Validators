namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Type.CheckVirtualAttribute
{
    using System;
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

    [Test(CheckId.CheckVirtualAttribute, Category.Param)]
    internal class CheckVirtualAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var param in context.EachParamWithValidId())
            {
                var typeTag = param.Type;
                var virtualAttribute = typeTag?.Virtual;
                if (virtualAttribute == null)
                {
                    continue;
                }

                (GenericStatus status, string optionsRawValue, string _) = GenericTests.CheckBasics(virtualAttribute, isRequired: false);

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, param, virtualAttribute, param.Id.RawValue));
                    return results;
                }

                // Further Validation
                ValidateHelper helper = new ValidateHelper(this, context, results, param);
                helper.Validate();

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, param, virtualAttribute, param.Id.RawValue, optionsRawValue));
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol.Params == null)
            {
                result.Message = "No Param found";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.EmptyAttribute:
                    {
                        var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
                        var paramEditNode = context.Protocol.Params.Get(paramReadNode);

                        paramEditNode.Type.Virtual = null;
                        result.Success = true;

                        break;
                    }

                case ErrorIds.UntrimmedAttribute:
                    {
                        var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
                        var paramEditNode = context.Protocol.Params.Get(paramReadNode);

                        paramEditNode.Type.Virtual.Value = paramReadNode.Type.Virtual.Value;
                        result.Success = true;

                        break;
                    }

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
        private readonly IValueTag<string> virtualAttribute;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param) : base(test, context, results)
        {
            this.param = param;
            virtualAttribute = param.Type.Virtual;
        }

        public void Validate()
        {
            string[] virtualParts = virtualAttribute.Value.Split(':');
            foreach (string virtualPart in virtualParts)
            {
                if (virtualPart.Equals("source", StringComparison.OrdinalIgnoreCase))
                {
                    // Param Requiring RTDisplay
                    IValidationResult rtDisplayError = Error.RTDisplayExpected(test, param, virtualAttribute, param.Id.RawValue);
                    context.CrossData.RtDisplay.AddParam(param, rtDisplayError);
                }
            }
        }
    }
}
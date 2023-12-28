namespace SLDisValidator2.Tests.Protocol.Params.Param.Type.CheckVirtualAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Generic;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckVirtualAttribute, Category.Param)]
    public class CheckVirtualAttribute : IValidate, ICodeFix/*, ICompare*/
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

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly IProtocolModel model;
        private readonly List<IValidationResult> results;

        private readonly IParamsParam param;
        private readonly IValueTag<string> virtualAttribute;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param)
        {
            this.test = test;
            this.context = context;
            this.model = context.ProtocolModel;
            this.results = results;

            this.param = param;
            this.virtualAttribute = param.Type.Virtual;
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
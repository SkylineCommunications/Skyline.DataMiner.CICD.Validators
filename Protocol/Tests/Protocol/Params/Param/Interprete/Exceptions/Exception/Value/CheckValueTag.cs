namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Exceptions.Exception.Value.CheckValueTag
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckValueTag, Category.Param)]
    internal class CheckValueTag : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.Interprete?.Exceptions == null)
                {
                    continue;
                }

                foreach (var exception in param.Interprete.Exceptions)
                {
                    ValidateHelper helper = new ValidateHelper(this, context, results, param, exception);
                    helper.Validate();
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol.Params == null)
            {
                result.Message = "No Param found.";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedTag:
                    {
                        var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
                        var paramEditNode = context.Protocol.Params.Get(paramReadNode);

                        var exceptionReadNode = (IParamsParamInterpreteExceptionsException)context.Result.ExtraData[ExtraData.Exception];
                        var exceptionEditNode = paramEditNode.Interprete.Exceptions.Get(exceptionReadNode);

                        exceptionEditNode.ValueElement.Value = exceptionReadNode.ValueElement.Value;
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

    internal enum ExtraData
    {
        Exception
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        private readonly IParamsParam param;
        private readonly IParamsParamInterpreteExceptionsException exception;

        private readonly IParamsParamInterpreteType interpreteType;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param, IParamsParamInterpreteExceptionsException exception)
            : base(test, context, results)
        {
            this.param = param;
            this.exception = exception;

            interpreteType = param.Interprete.Type;
        }

        public void Validate()
        {
            (GenericStatus status, string _, string _) = GenericTests.CheckBasics(exception.ValueElement, isRequired: true);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingTag(test, param, exception, param.Id?.RawValue));
                return;
            }

            // TODO: Empty (double check if empty tag is actually supported)
            ////if (status.HasFlag(GenericStatus.Empty))
            ////{
            ////    results.Add(Error.EmptyTag(this, param, exception, param.Id?.RawValue));
            ////    return;
            ////}

            // Value
            if (!ValidateValue())
            {
                return;
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                var result = Error.UntrimmedTag(test, param, exception.ValueElement, param.Id?.RawValue, exception.ValueElement.RawValue)
                                  .WithExtraData(ExtraData.Exception, exception);

                results.Add(result);
            }
        }

        private bool ValidateValue()
        {
            if (interpreteType == null)
            {
                return true;
            }

            // Any value is allowed if type is not double
            if (interpreteType.Value != EnumParamInterpretType.Double)
            {
                return true;
            }

            // Value incompatible with type
            if (!Double.TryParse(exception.ValueElement.Value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out _))
            {
                results.Add(Error.ValueIncompatibleWithInterpreteType(test, param, exception, exception.ValueElement.RawValue, interpreteType.RawValue, param.Id?.RawValue));
                return false;
            }

            return true;
        }
    }
}
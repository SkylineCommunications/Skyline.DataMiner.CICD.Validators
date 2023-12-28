namespace SLDisValidator2.Tests.Protocol.Params.Param.Interprete.Exceptions.Exception.CheckValueAttribute
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Generic;
    using SLDisValidator2.Helpers;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckValueAttribute, Category.Param)]
    public class CheckValueAttribute : IValidate/*, ICodeFix, ICompare*/
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

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {

        ////        default:
        ////            result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
        ////            break;
        ////    }

        ////    return result;
        ////}

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
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
            (GenericStatus status, string _, string _) = GenericTests.CheckBasics(exception.ValueAttribute, isRequired: true);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingAttribute(test, param, exception, param.Id?.RawValue));
                return;
            }

            // TODO: Empty (double check if empty attribute is actually supported)
            ////if (status.HasFlag(GenericStatus.Empty))
            ////{
            ////    results.Add(Error.EmptyAttribute(this, param, exception, param.Id?.RawValue));
            ////    return;
            ////}

            // Value
            if (!ValidateValue())
            {
                return;
            }
        }

        private bool ValidateValue()
        {
            // Any value is allowed if type is not double
            if (interpreteType?.Value != EnumParamInterpretType.Double)
            {
                return true;
            }

            // Value incompatible with type
            if (!Double.TryParse(exception.ValueAttribute.Value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out _))
            {
                results.Add(Error.ValueIncompatibleWithInterpreteType(test, exception, exception, exception.ValueAttribute.Value, interpreteType?.RawValue, param.Id?.RawValue));
                return false;
            }

            return true;
        }
    }
}
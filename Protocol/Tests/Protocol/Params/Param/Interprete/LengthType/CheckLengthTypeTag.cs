namespace SLDisValidator2.Tests.Protocol.Params.Param.Interprete.LengthType.CheckLengthTypeTag
{
    using System.Collections.Generic;

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

    [Test(CheckId.CheckLengthTypeTag, Category.Param)]
    public class CheckLengthTypeTag : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.Interprete == null)
                {
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, param);
                helper.Validate();
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

        private readonly IParamsParamInterpreteLengthType lengthType;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param)
            : base(test, context, results)
        {
            this.param = param;

            lengthType = param.Interprete.LengthType;
        }

        public void Validate()
        {
            (GenericStatus status, _, _) = GenericTests.CheckBasics(lengthType, isRequired: true);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingTag(test, param, param.Interprete, param.Id.RawValue));
                return;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyTag(test, param, lengthType, param.Id.RawValue));
                return;
            }

            //  Invalid value
            if (status.HasFlag(GenericStatus.Invalid))
            {
                results.Add(Error.InvalidValue(test, param, lengthType, lengthType.RawValue, param.Id.RawValue));
                return;
            }

            if (ValidateFixedParams())
            {
                return;
            }
        }

        private bool ValidateFixedParams()
        {
            if (param.Type?.Value != EnumParamType.Fixed)
            {
                return false;
            }

            if (lengthType.Value != EnumParamInterpretLengthType.Fixed)
            {
                results.Add(Error.InvalidValue(test, param, lengthType, lengthType.RawValue, param.Id.RawValue));
                return true;
            }

            return false;
        }
    }
}
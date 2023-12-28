namespace SLDisValidator2.Tests.Protocol.Params.Param.Interprete.Length.CheckLengthTag
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

    [Test(CheckId.CheckLengthTag, Category.Param)]
    internal class CheckLengthTag : IValidate/*, ICodeFix, ICompare*/
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

        private readonly IParamsParamInterprete interprete;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param)
            : base(test, context, results)
        {
            this.param = param;

            interprete = param.Interprete;
        }

        public void Validate()
        {
            bool isRequired = (interprete.LengthType?.Value == EnumParamInterpretLengthType.Fixed && param.Type.Value != EnumParamType.ReadBit);

            (GenericStatus status, _, _) = GenericTests.CheckBasics(interprete.Length, isRequired);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingTag(test, param, interprete, param.Id.RawValue));
                return;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyTag(test, param, interprete.Length, param.Id.RawValue));
                return;
            }

            //  Invalid value
            if (status.HasFlag(GenericStatus.Invalid))
            {
                results.Add(Error.InvalidValue(test, param, interprete.Length, Severity.Major, interprete.Length.RawValue, param.Id.RawValue));
                return;
            }

            // Length parameters can never be longer than 4 bytes as otherwise it can cause memory corruption in SLPort
            if (param.Type?.Value == EnumParamType.Length && interprete.Length.Value > 4)
            {
                results.Add(Error.InvalidValue(test, param, interprete.Length, Severity.Critical, interprete.Length.RawValue, param.Id.RawValue));
            }
        }
    }
}
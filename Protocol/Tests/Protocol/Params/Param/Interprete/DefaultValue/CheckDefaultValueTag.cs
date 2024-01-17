namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.DefaultValue.CheckDefaultValueTag
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckDefaultValueTag, Category.Param)]
    internal class CheckDefaultValueTag : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                var defaultValue = param.Interprete?.DefaultValue;
                if (defaultValue == null)
                {
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, param, defaultValue);
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
        private readonly IValueTag<string> defaultValue;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param, IValueTag<string> defaultValue)
            : base(test, context, results)
        {
            this.param = param;
            this.defaultValue = defaultValue;
        }

        public void Validate()
        {
            // Not Yet Supported
            if (param.TryGetTable(context.ProtocolModel.RelationManager, out _))
            {
                results.Add(Error.NotYetSupportedTag(test, param, defaultValue, param.Id?.RawValue));
                return;
            }

            // Unsupported
            if (param.Type?.Value != EnumParamType.Read)
            {
                results.Add(Error.UnsupportedTag(test, param, defaultValue, param.Id?.RawValue));
                return;
            }

            // ValueIncompatibleWithInterpreteType
            if (param.Interprete.Type?.Value == EnumParamInterpretType.Double &&
                !Double.TryParse(defaultValue.Value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out _))
            {
                results.Add(Error.ValueIncompatibleWithInterpreteType(test, param, defaultValue, defaultValue.RawValue, param.Interprete.Type.RawValue, param.Id?.RawValue));
                return;
            }
        }
    }
}
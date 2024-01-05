namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.LengthType.CheckIdAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckIdAttribute, Category.Param)]
    internal class CheckIdAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.Interprete?.LengthType == null)
                {
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, param);
                helper.Validate();
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol.Params == null)
            {
                result.Message = "No Params found!";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    {
                        var readNode = (IParamsParam)context.Result.ReferenceNode;
                        var editNode = context.Protocol.Params.Get(readNode);

                        editNode.Interprete.LengthType.Id.Value = readNode.Interprete.LengthType.Id.Value;
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

        private readonly IParamsParamInterpreteLengthType lengthType;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param)
            : base(test, context, results)
        {
            this.param = param;

            lengthType = param.Interprete.LengthType;
        }

        public void Validate()
        {
            bool isRequired = lengthType.Value == EnumParamInterpretLengthType.OtherParam;
            (GenericStatus status, _, _) = GenericTests.CheckBasics(lengthType.Id, isRequired);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingAttribute(test, param, lengthType, param.Id.RawValue));
                return;
            }

            // Not present but not required
            if (lengthType.Id == null)
            {
                return;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, param, lengthType.Id, param.Id.RawValue));
                return;
            }

            // Referenced Param
            if (!ValidateReferencedParam())
            {
                return;
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(test, param, lengthType.Id, param.Id.RawValue, lengthType.Id.RawValue));
                return;
            }
        }

        private bool ValidateReferencedParam()
        {
            // NonExisting
            if (!context.ProtocolModel.TryGetObjectByKey(Mappings.ParamsById, lengthType.Id.RawValue?.Trim(), out IParamsParam referencedParam))
            {
                results.Add(Error.NonExistingId(test, param, lengthType.Id, lengthType.Id.RawValue, param.Id.RawValue));
                return false;
            }

            // Wrong Type
            var refParamType = referencedParam.Interprete?.Type;
            if (refParamType?.Value != EnumParamInterpretType.Double)
            {
                results.Add(Error.ReferencedParamWrongInterpreteType(test, referencedParam, refParamType ?? (IReadable)referencedParam, refParamType?.RawValue, referencedParam.Id.RawValue));
                return false;
            }

            return true;
        }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.CheckDependencyId
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;


    [Test(CheckId.CheckDependencyId, Category.Param)]
    internal class CheckDependencyId : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            IProtocolModel model = context.ProtocolModel;

            foreach (var param in context.EachParamWithValidId())
            {
                ValidateHelper helper = new ValidateHelper(this, context, model, results, param);
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

                        editNode.Measurement.Discreets.DependencyId.Value = readNode.Measurement.Discreets.DependencyId.Value;
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

        internal ValidateHelper(IValidate test, ValidatorContext context, IProtocolModel model, List<IValidationResult> results, IParamsParam param)
        {
            this.test = test;
            this.context = context;
            this.model = model;
            this.results = results;
            this.param = param;
        }

        internal void Validate()
        {
            if (param.Measurement?.Discreets?.DependencyId == null)
            {
                // No dependencyId attribute to be checked
                return;
            }

            var dependencyIdAttribute = param.Measurement.Discreets.DependencyId;
            (GenericStatus status, string rawValue, uint? value) = GenericTests.CheckBasics(dependencyIdAttribute, false);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, param, dependencyIdAttribute, param.Id.RawValue));
                return;
            }

            // Invalid Value
            if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(rawValue))
            {
                results.Add(Error.InvalidValue(test, param, dependencyIdAttribute, rawValue, param.Id.RawValue));
                return;
            }

            // Referenced Parameter
            CheckReferencedParam(Convert.ToString(value), dependencyIdAttribute);

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(test, param, dependencyIdAttribute, param.Id.RawValue, rawValue));
                return;
            }
        }

        private void CheckReferencedParam(string referencedPid, IReadable dependencyIdAttribute)
        {
            // Non Existing Param
            if (!model.TryGetObjectByKey(Mappings.ParamsById, referencedPid, out IParamsParam referencedParam))
            {
                results.Add(Error.NonExistingId(test, param, dependencyIdAttribute, referencedPid, param.Id.RawValue));
                return;
            }

            // Param Wrong Type
            if (referencedParam.Type?.Value != EnumParamType.Read &&
                referencedParam.Type?.Value != EnumParamType.ReadBit)
            {
                results.Add(Error.ReferencedParamWrongType(test, param, dependencyIdAttribute, referencedParam.Type?.RawValue, referencedPid));
            }

            // Param Requiring RTDisplay
            IValidationResult rtDisplayError = Error.ReferencedParamRTDisplayExpected(test, param, dependencyIdAttribute, referencedPid, param.Id.RawValue);
            context.CrossData.RtDisplay.AddParam(referencedParam, rtDisplayError);
        }
    }
}
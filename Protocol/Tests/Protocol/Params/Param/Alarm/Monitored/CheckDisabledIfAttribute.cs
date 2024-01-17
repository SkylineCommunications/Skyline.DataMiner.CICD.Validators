namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.Monitored.CheckDisabledIfAttribute
{
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

    [Test(CheckId.CheckDisabledIfAttribute, Category.Param)]
    internal class CheckDisabledIfAttribute : IValidate, ICodeFix/*, ICompare*/
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

                        editNode.Alarm.Monitored.DisabledIf.Value = readNode.Alarm.Monitored.DisabledIf.Value;
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
            if (param.Alarm?.Monitored?.DisabledIf == null)
            {
                // No disableIf attribute to be checked
                return;
            }

            var disabledIfAttribute = param.Alarm.Monitored.DisabledIf;
            (GenericStatus status, string rawValue, string value) = GenericTests.CheckBasics(disabledIfAttribute, false);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, param, disabledIfAttribute, param.Id.RawValue));
                return;
            }

            int firstCommaPosition = value.IndexOf(',');

            // Invalid Value
            if (status.HasFlag(GenericStatus.Invalid) || firstCommaPosition < 0)
            {
                results.Add(Error.InvalidValue(test, param, disabledIfAttribute, rawValue, param.Id.RawValue));
                return;
            }

            string referencedPid = value.Substring(0, firstCommaPosition);

            // Referenced Parameter
            CheckReferencedParam(referencedPid, disabledIfAttribute);

            // TODO: Check conditional value (in case of Discreet, the Discreet.Value should be used rather than the Discreet.DisplayValue)
            ////string conditionalValue = value.Substring(firstCommaPosition + 1);

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(test, param, disabledIfAttribute, param.Id.RawValue, rawValue));
                return;
            }
        }

        private void CheckReferencedParam(string referencedPid, IReadable disabledIfAttribute)
        {
            // Non Existing Param
            if (!model.TryGetObjectByKey(Mappings.ParamsById, referencedPid, out IParamsParam referencedParam))
            {
                results.Add(Error.NonExistingId(test, param, disabledIfAttribute, referencedPid, param.Id.RawValue));
                return;
            }

            // Param Wrong Type
            if (referencedParam.IsWrite() || referencedParam.Type?.Value == EnumParamType.Array)
            {
                results.Add(Error.ReferencedParamWrongType(test, param, disabledIfAttribute, referencedParam.Type?.RawValue, referencedPid));
            }

            // Param Requiring RTDisplay
            IValidationResult rtDisplayError = Error.ReferencedParamRTDisplayExpected(test, param, disabledIfAttribute, referencedPid, param.Id.RawValue);
            context.CrossData.RtDisplay.AddParam(referencedParam, rtDisplayError);
        }
    }
}
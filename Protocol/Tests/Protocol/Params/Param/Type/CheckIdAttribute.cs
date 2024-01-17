namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Type.CheckIdAttribute
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckIdAttribute, Category.Param)]
    internal class CheckIdAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var param in context.EachParamWithValidId())
            {
                if (param.Type == null)
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
                result.Message = "No Param found";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    {
                        var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
                        var paramEditNode = context.Protocol.Params.Get(paramReadNode);

                        paramEditNode.Type.Id.Value = paramReadNode.Type.Id.Value;

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
        private readonly List<IValidationResult> results;

        private readonly IParamsParam param;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param)
        {
            this.test = test;
            this.context = context;
            this.results = results;

            this.param = param;
        }

        public void Validate()
        {
            var paramType = param.Type;
            bool isRequired = paramType.Value == EnumParamType.ReadBit
                || paramType.Value == EnumParamType.WriteBit
                || paramType.Value == EnumParamType.Response;

            (GenericStatus status, string idRaw, string id) = GenericTests.CheckBasics(paramType.Id, isRequired);

            // Missing (When required)
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingAttribute(test, param, paramType, paramType.RawValue, param.Id.RawValue));
                return;
            }

            // Missing when not required
            if (paramType.Id == null)
            {
                // No need to further check if attribute is not present.
                return;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, param, paramType, param.Id.RawValue));
                return;
            }

            // Referenced Items
            ValidateReferencedItems(paramType, id, idRaw);

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(test, param, paramType, param.Id.RawValue, idRaw));
            }
        }

        private void ValidateReferencedItems(IParamsParamType paramType, string id, string idRaw)
        {
            switch (paramType.Value)
            {
                case EnumParamType.Array:
                    string[] columnPids = id.Split(';');
                    foreach (string columnPid in columnPids)
                    {
                        if (!context.ProtocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, columnPid, out _))
                        {
                            results.Add(Error.NonExistingColumn(test, param, paramType, columnPid, param.Id.RawValue));
                        }
                    }

                    break;
                case EnumParamType.ReadBit:
                case EnumParamType.WriteBit:
                    if (!context.ProtocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, id, out _))
                    {
                        results.Add(Error.NonExistingParam(test, param, paramType, idRaw, param.Id.RawValue));
                    }

                    // TODO: Check if type referenced param is of Type 'group'

                    break;
                case EnumParamType.Response:
                    if (!context.ProtocolModel.TryGetObjectByKey<IResponsesResponse>(Mappings.ResponsesById, id, out _))
                    {
                        results.Add(Error.NonExistingResponse(test, param, paramType, idRaw, param.Id.RawValue));
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
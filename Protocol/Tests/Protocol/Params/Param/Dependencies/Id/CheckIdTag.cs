namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Dependencies.Id.CheckIdTag
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    enum ExtraData { dependencyId }

    [Test(CheckId.CheckIdTag, Category.Param)]
    internal class CheckIdTag : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var param in context.EachParamWithValidId())
            {
                if (param.Dependencies == null)
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

            if (!(context.Result.ReferenceNode is IParamsParam readParam))
            {
                result.Message = nameof(readParam) + " is null.";
                return result;
            }

            var editParam = context.Protocol?.Params?.Get(readParam);
            if (editParam == null)
            {
                result.Message = nameof(editParam) + " is null.";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.EmptyTag:
                    editParam.Dependencies.Cleanup(cleanupSelf: true);
                    result.Success = true;
                    break;
                case ErrorIds.UntrimmedTag:
                    if (!(context.Result.ExtraData[ExtraData.dependencyId] is IParamsParamDependenciesId readDependencyId))
                    {
                        result.Message = nameof(readDependencyId) + " is null.";
                        return result;
                    }

                    var editDependencyId = editParam.Dependencies?.Get(readDependencyId);
                    if (editDependencyId == null)
                    {
                        result.Message = nameof(editDependencyId) + " is null.";
                        return result;
                    }

                    editDependencyId.Value = readDependencyId.Value;
                    result.Success = true;
                    break;
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
        private readonly RelationManager relationManager;
        private readonly List<IValidationResult> results;

        private readonly IParamsParam param;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param)
        {
            this.test = test;
            this.context = context;
            this.relationManager = context.ProtocolModel.RelationManager;
            this.results = results;

            this.param = param;
        }

        public void Validate()
        {
            // RTDisplay
            context.CrossData.RtDisplay.AddParam(param, Error.RTDisplayExpected(test, param, param.Dependencies, param.Id.RawValue));

            foreach (var dependencyId in param.Dependencies)
            {
                (GenericStatus status, _, _) = GenericTests.CheckBasics(dependencyId, isRequired: false);

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyTag(test, param, dependencyId, param.Id.RawValue)
                        .WithExtraData(ExtraData.dependencyId, dependencyId));
                    continue;
                }

                // Referenced Param
                if (!ValidateReferencedParam(dependencyId))
                {
                    continue;
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedTag(test, param, dependencyId, param.Id.RawValue, dependencyId.RawValue)
                        .WithExtraData(ExtraData.dependencyId, dependencyId));
                }
            }
        }

        private bool ValidateReferencedParam(IParamsParamDependenciesId dependencyId)
        {
            // NonExisting Param
            if (!context.ProtocolModel.TryGetObjectByKey(Mappings.ParamsById, dependencyId.Value, out IParamsParam referencedParam))
            {
                results.Add(Error.NonExistingId(test, param, dependencyId, dependencyId.RawValue, param.Id.RawValue));
                return false;
            }

            // RTDisplay
            context.CrossData.RtDisplay.AddParam(referencedParam, Error.RTDisplayExpectedOnReferencedParam(test, param, dependencyId, referencedParam.Id.RawValue, param.Id.RawValue));

            // Linked params
            if (referencedParam.IsRead() && referencedParam.TryGetWrite(relationManager, out var writeParam))
            {
                context.CrossData.RtDisplay.AddParam(writeParam);
            }
            else if (referencedParam.IsWrite() && referencedParam.TryGetRead(relationManager, out var readParam))
            {
                context.CrossData.RtDisplay.AddParam(readParam);
            }

            return true;
        }
    }
}
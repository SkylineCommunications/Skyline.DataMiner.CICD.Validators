namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.NamingFormat.CheckNamingFormatTag
{
    using System;
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

    [Test(CheckId.CheckNamingFormatTag, Category.Param)]
    internal class CheckNamingFormatTag : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var param in context.EachParamWithValidId())
            {
                var namingFormat = param.ArrayOptions?.NamingFormat;
                if (namingFormat == null)
                {
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, param, namingFormat);
                helper.Validate();
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol?.Params == null)
            {
                result.Message = "No Params found!";
                return result;
            }

            var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
            var paramEditNode = context.Protocol.Params.Get(paramReadNode);
            if (paramEditNode == null)
            {
                result.Message = "Param Edit Node not found!";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedTag:
                    paramEditNode.ArrayOptions.NamingFormat = paramReadNode.ArrayOptions.NamingFormat.Value;
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
        ////private readonly RelationManager relationManager;
        private readonly List<IValidationResult> results;

        private readonly IParamsParam tableParam;
        private readonly IParamsParamArrayOptionsNamingFormat namingFormat;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam tableParam, IParamsParamArrayOptionsNamingFormat namingFormat)
        {
            this.test = test;
            this.context = context;
            ////this.relationManager = context.ProtocolModel.RelationManager;
            this.results = results;

            this.tableParam = tableParam;
            this.namingFormat = namingFormat;
        }

        public void Validate()
        {
            (GenericStatus status, string _, string _) = GenericTests.CheckBasics(namingFormat, isRequired: false);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyTag(test, tableParam, namingFormat, tableParam.Id.RawValue));
                return;
            }

            // NamingFormat parts
            ValidateNamingFormatParts();

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedTag(test, tableParam, namingFormat, tableParam.Id?.RawValue, namingFormat.RawValue));
            }
        }

        private void ValidateNamingFormatParts()
        {
            int dynamicPartsCount = 0;

            string[] namingFormatParts = namingFormat.Value.Split(namingFormat.Value[0]);
            foreach (var namingFormatPart in namingFormatParts)
            {
                if (!UInt32.TryParse(namingFormatPart, out uint _))
                {
                    // DM will consider it a hard-coded display key part if it can't be parsed to a number
                    continue;
                }

                dynamicPartsCount++;

                // Non Existing Reference
                if (!context.ProtocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, namingFormatPart, out _))
                {
                    results.Add(Error.NonExistingParam(test, tableParam, namingFormat, namingFormatPart, tableParam.Id?.RawValue));
                    continue;
                }
            }

            if (dynamicPartsCount == 0)
            {
                results.Add(Error.MissingDynamicPart(test, tableParam, namingFormat, tableParam.Id.RawValue));
            }
        }
    }
}
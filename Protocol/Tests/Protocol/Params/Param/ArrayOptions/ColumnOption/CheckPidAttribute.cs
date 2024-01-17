namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckPidAttribute
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

    internal enum ExtraData
    {
        ColumnOption,
    }

    [Test(CheckId.CheckPidAttribute, Category.Param)]
    internal class CheckPidAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var param in context.EachParamWithValidId())
            {
                if (param.ArrayOptions == null)
                {
                    continue;
                }

                foreach (var columnOption in param.ArrayOptions)
                {
                    ValidateHelper helper = new ValidateHelper(this, context, results, param, columnOption);
                    helper.Validate();
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol.Params == null)
            {
                result.Message = "No Param found.";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    {
                        var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
                        var paramEditNode = context.Protocol.Params.Get(paramReadNode);
                        if (paramEditNode == null)
                        {
                            result.Message = "param Edit Node not found.";
                            return result;
                        }

                        var columnOptionReadNode = (ITypeColumnOption)context.Result.ExtraData[ExtraData.ColumnOption];
                        var columnOptionEditNode = paramEditNode.ArrayOptions.Get(columnOptionReadNode);

                        if (columnOptionEditNode == null)
                        {
                            result.Message = "columnOption Edit Node not found.";
                            return result;
                        }

                        columnOptionEditNode.Pid = columnOptionReadNode.Pid.Value;

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

        private readonly IParamsParam tableParam;
        private readonly ITypeColumnOption columnOption;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam tableParam, ITypeColumnOption columnOption)
        {
            this.test = test;
            this.context = context;
            this.results = results;

            this.tableParam = tableParam;
            this.columnOption = columnOption;
        }

        public void Validate()
        {
            (GenericStatus status, string pidRaw, uint? pid) = GenericTests.CheckBasics(columnOption.Pid, isRequired: true);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingAttribute(test, tableParam, columnOption, tableParam.Id.RawValue));
                return;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, tableParam, columnOption.Pid, tableParam.Id.RawValue));
                return;
            }

            // NonExistingParam
            if (!context.ProtocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, Convert.ToString(pid), out _))
            {
                results.Add(Error.NonExistingParam(test, tableParam, columnOption.Pid, pidRaw, tableParam.Id?.RawValue));
                return;
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(test, tableParam, columnOption.Pid, tableParam.Id?.RawValue, pidRaw)
                    .WithExtraData(ExtraData.ColumnOption, columnOption));
            }
        }
    }
}
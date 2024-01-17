namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.CheckColumns
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckColumns, Category.Param)]
    internal class CheckColumns : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var param in context.EachParamWithValidId())
            {
                if (!param.IsTable())
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
        private readonly RelationManager relationManager;

        private readonly IParamsParam tableParam;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam tableParam)
            : base(test, context, results)
        {
            relationManager = context.ProtocolModel.RelationManager;

            this.tableParam = tableParam;
        }

        public void Validate()
        {
            var columns = tableParam.GetColumns(relationManager, returnBaseColumnsIfDuplicateAs: true);
            foreach (var column in columns)
            {
                ValidateColumn(column);
            }
        }

        private void ValidateColumn((uint? idx, string pid, IParamsParam columnParam) column)
        {
            (_, string pid, IParamsParam columnParam) = column;
            if (columnParam == null)
            {
                // NonExistingParam covered by Param/Type@id && ColumnOption@pid checks
                return;
            }

            if (columnParam.Type?.Value == null)
            {
                // Covered by Param/Type check.
                return;
            }

            if (columnParam.Type.Value != EnumParamType.Read
                && columnParam.Type.Value != EnumParamType.Write
                && columnParam.Type.Value != EnumParamType.Group
                && columnParam.Type.Value != EnumParamType.ReadBit
                && columnParam.Type.Value != EnumParamType.WriteBit)
            {
                results.Add(Error.ColumnInvalidType(test, columnParam, columnParam.Type, columnParam.Type.RawValue, pid));
            }
        }
    }
}
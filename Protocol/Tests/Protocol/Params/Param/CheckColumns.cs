namespace SLDisValidator2.Tests.Protocol.Params.Param.CheckColumns
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckColumns, Category.Param)]
    public class CheckColumns : IValidate/*, ICodeFix, ICompare*/
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

    internal class ValidateHelper
    {
        private readonly IValidate test;
        private readonly ValidatorContext context;
        private readonly RelationManager relationManager;
        private readonly List<IValidationResult> results;

        private readonly IParamsParam tableParam;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam tableParam)
        {
            this.test = test;
            this.context = context;
            this.relationManager = context.ProtocolModel.RelationManager;
            this.results = results;

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

        public void ValidateColumn((uint? idx, string pid, IParamsParam columnParam) column)
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
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckIndexAttribute
{
    using System.Collections.Generic;
    using System.Linq;

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

    [Test(CheckId.CheckIndexAttribute, Category.Param)]
    internal class CheckIndexAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                IParamsParamArrayOptions arrayOptions = param.ArrayOptions;
                if (arrayOptions == null)
                {
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, param, arrayOptions);
                helper.Validate();
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    {
                        if (context.Protocol?.Params != null)
                        {
                            var readNode = (IParamsParam)context.Result.ReferenceNode;
                            var editNode = context.Protocol.Params.Get(readNode);

                            editNode.ArrayOptions.Index = readNode.ArrayOptions.Index.Value;
                            result.Success = true;
                        }

                        break;
                    }

                case ErrorIds.MissingAttribute:
                case ErrorIds.EmptyAttribute:
                    {
                        var readNode = (IParamsParam)context.Result.ReferenceNode;
                        var editNode = context.Protocol.Params.Get(readNode);

                        editNode.ArrayOptions.Index = 0;
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
        private readonly RelationManager relationManager;

        private readonly IParamsParam tableParam;
        private readonly IValueTag<uint?> indexAttribute;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam tableParam, IParamsParamArrayOptions arrayOptions)
            : base(test, context, results)
        {
            this.tableParam = tableParam;
            this.relationManager = context.ProtocolModel.RelationManager;
            this.indexAttribute = arrayOptions.Index;
        }

        public void Validate()
        {
            (GenericStatus status, string indexRawValue, uint? index) = GenericTests.CheckBasics(indexAttribute, isRequired: true);

            // Missing
            if (status.HasFlag(GenericStatus.Missing))
            {
                results.Add(Error.MissingAttribute(test, tableParam, indexAttribute, tableParam.Id.RawValue));
                return;
            }

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(test, tableParam, indexAttribute, tableParam.Id.RawValue));
                return;
            }

            // Invalid
            if (status.HasFlag(GenericStatus.Invalid) || !GenericTests.IsPlainNumbers(indexRawValue))
            {
                results.Add(Error.InvalidAttributeValue(test, tableParam, indexAttribute, tableParam.Id.RawValue, indexRawValue));
                return;
            }

            // Unrecommended PK IDX
            if (index.Value != 0)
            {
                results.Add(Error.UnrecommendedValue(test, tableParam, indexAttribute, indexAttribute.RawValue, tableParam.Id.RawValue, "0"));
                return;
            }

            // Check referenced param
            if (!ValidateReferencedColumn())
            {
                return;
            }

            // Untrimmed
            if (status.HasFlag(GenericStatus.Untrimmed))
            {
                results.Add(Error.UntrimmedAttribute(test, tableParam, indexAttribute, tableParam.Id.RawValue, indexRawValue));
            }
        }

        private bool ValidateReferencedColumn()
        {
            // Table has no column with such IDX
            (uint? idx, _, IParamsParam pkColumnParam) = tableParam.GetColumns(relationManager, returnBaseColumnsIfDuplicateAs: true)
                                                                   .FirstOrDefault(c => c.idx == indexAttribute.Value);
            if (idx == null)
            {
                results.Add(Error.NonExistingColumn(test, tableParam, indexAttribute, indexAttribute.RawValue, tableParam.Id.RawValue));
                return false;
            }

            // Non existing Param
            if (pkColumnParam == null)
            {
                // Non Existing Param covered by Param/Type@id && ColumnOptions@pid checks.
                return true;
            }

            // PK Param/Type
            var paramType = pkColumnParam.Type;
            if (paramType == null)
            {
                // Covered by Param/Type check
                ////results.Add(Error.InvalidColumnType(test, pkColumnParam, pkColumnParam, "UNDEFINED", pkColumnParam.Id.RawValue));
            }
            else if (paramType.Value == EnumParamType.Write
                || paramType.Value == EnumParamType.Group
                || paramType.Value == EnumParamType.ReadBit
                || paramType.Value == EnumParamType.WriteBit)
            {
                // PK Param/Type Invalid (Only check for types that are valid for normal columns but not for PKs, other ones are covered by Param.CheckColumns)
                results.Add(Error.InvalidColumnType(test, pkColumnParam, paramType, paramType.RawValue, pkColumnParam.Id.RawValue));
            }

            // PK Interprete/Type
            var interpretType = pkColumnParam.Interprete?.Type;
            if (interpretType == null)
            {
                results.Add(Error.InvalidColumnInterpreteType(test, pkColumnParam, pkColumnParam, "UNDEFINED", pkColumnParam.Id.RawValue));
            }
            else if (interpretType.Value.HasValue
                && interpretType.Value != EnumParamInterpretType.String)
            {
                results.Add(Error.InvalidColumnInterpreteType(test, pkColumnParam, interpretType, interpretType.RawValue, pkColumnParam.Id.RawValue));
            }

            // PK Measurement/Type
            if (tableParam.GetRTDisplay() || tableParam.WillBeExported())
            {
                var measurementType = pkColumnParam.Measurement?.Type;
                if (measurementType == null)
                {
                    results.Add(Error.InvalidColumnMeasurementType(test, pkColumnParam, pkColumnParam, "UNDEFINED", pkColumnParam.Id.RawValue));
                }
                else if (measurementType.Value.HasValue
                    && measurementType.Value != EnumParamMeasurementType.String
                    && measurementType.Value != EnumParamMeasurementType.Number)
                {
                    results.Add(Error.InvalidColumnMeasurementType(test, pkColumnParam, measurementType, measurementType.RawValue, pkColumnParam.Id.RawValue));
                }
            }

            return true;
        }
    }
}
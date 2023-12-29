namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Range.CheckRangeTag
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckRangeTag, Category.Param)]
    internal class CheckRangeTag : IValidate
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.Display == null)
                {
                    continue;
                }

                ValidateHelper helper = new ValidateHelper(this, context, results, param);
                helper.Validate();
            }

            return results;
        }
    }

    internal class ValidateHelper : ValidateHelperBase
    {
        private readonly RelationManager relationManager;
        private readonly IParamsParam param;

        private readonly IParamsParamDisplay display;
        private readonly IParamsParamDisplayRange range;

        public ValidateHelper(IValidate test, ValidatorContext context, List<IValidationResult> results, IParamsParam param)
            : base(test, context, results)
        {
            relationManager = context.ProtocolModel.RelationManager;
            this.param = param;

            display = param.Display;
            range = display.Range;
        }

        public void Validate()
        {
            if (range == null)
            {
                CheckMissingTag();
            }
            else if (range.Low == null && range.High == null)
            {
                results.Add(Error.EmptyTag(test, param, range, param.Id.RawValue));
            }
            else
            {
                CheckExcessiveTag();
                CheckLowVsHigh();
            }
        }

        private void CheckMissingTag()
        {
            if (param.IsPositioned(relationManager))
            {
                if (param.TryGetTable(relationManager, out var tableParam))
                {
                    // Index Column
                    if (tableParam.TryGetPrimaryKeyColumn(relationManager, out var indexColumn) &&
                        indexColumn == param)
                    {
                        return;
                    }

                    // Normal column, so don't skip it.
                }
            }
            else if (param.IsWrite() && param.TryGetRead(relationManager, out IParamsParam readParameter) &&
                readParameter.IsPositioned(relationManager))
            {
                // In case of a write parameter of a column
            }
            else
            {
                return;
            }

            // Check the Measurement Type
            if (param.IsNumber())
            {
                if (!param.IsDateTime() && !param.IsTime())
                {
                    // Normal Number Parameter
                    results.Add(Error.MissingTag(test, display, display, param.Measurement?.Type?.ReadNode.InnerText, param.Id.RawValue));
                }
            }
            else if (param.IsProgress() || param.IsAnalog())
            {
                results.Add(Error.MissingTag(test, display, display, param.Measurement?.Type?.ReadNode.InnerText, param.Id.RawValue));
            }
        }

        private void CheckExcessiveTag()
        {
            if (!param.IsPositioned(relationManager))
            {
                // No need to check on hidden parameters. Those can have Ranges for documentation purpose
                return;
            }

            if (param.TryGetTable(relationManager, out var tableParam))
            {
                // Index Column
                if (tableParam.TryGetPrimaryKeyColumn(relationManager, out var indexColumn) &&
                    indexColumn == param)
                {
                    results.Add(Error.UnsupportedTag(test, range, range, "primary key", param.Id.RawValue));
                    return;
                }

                // Normal column
            }

            if (!param.IsProgress() && !param.IsAnalog() && !param.IsString() && !param.IsNumber())
            {
                results.Add(Error.UnsupportedTag(test, range, range, param.Measurement?.Type?.ReadNode.InnerText, param.Id.RawValue));
                return;
            }
        }

        private void CheckLowVsHigh()
        {
            if (range.Low != null && range.High != null &&
                range.High.Value <= range.Low.Value)
            {
                results.Add(Error.LowShouldBeSmallerThanHigh(test, param, range, range.Low.RawValue, range.High.RawValue, param.Id.RawValue));
                return;
            }
        }
    }
}
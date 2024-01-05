namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Type.CheckLinkAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Edit;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckLinkAttribute, Category.Param)]
    internal class CheckLinkAttribute : IValidate, ICodeFix
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (!param.IsMatrix())
                {
                    continue;
                }

                if (param.Type?.Value == EnumParamType.Write)
                {
                    continue;
                }

                var measurementType = param.Measurement?.Type;
                var link = measurementType?.Link;
                if (link == null)
                {
                    results.Add(Error.MissingAttribute(this, param, measurementType, param.Id.RawValue));
                }
                else
                {
                    if (!link.Value.EndsWith(".xml"))
                    {
                        results.Add(Error.InvalidAttribute(this, param, measurementType, param.Id.RawValue));
                    }
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.MissingAttribute:
                    FixHelper.FixMissingLinkAttribute(context);
                    result.Success = true;
                    break;

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }
    }

    internal static class FixHelper
    {
        public static void FixMissingLinkAttribute(CodeFixContext context)
        {
            IParamsParam readParam = (IParamsParam)context.Result.ReferenceNode;
            ParamsParam writeParam = context.Protocol.Params.Get(readParam);

            if (writeParam == null)
            {
                return;
            }

            if (writeParam.Measurement == null)
            {
                writeParam.Measurement = new ParamsParamMeasurement();
            }

            if (writeParam.Measurement.Type == null)
            {
                writeParam.Measurement.Type = new ParamsParamMeasurementType(EnumParamMeasurementType.Matrix);
            }

            if (writeParam.Measurement.Type.Link == null)
            {
                writeParam.Measurement.Type.Link = new AttributeValue<string>("labels.xml");
            }
        }
    }
}
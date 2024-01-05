namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.CheckDiscreetsTag
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckDiscreetsTag, Category.Param)]
    internal class CheckDiscreetsTag : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                var type = param.Measurement?.Type;
                if (type?.Value == null)
                {
                    // No need to check if there isn't a Type.
                    continue;
                }

                bool hasNoDiscreetsTag = param.Measurement.Discreets == null;

                string typeString = EnumParamMeasurementTypeConverter.ConvertBack(type.Value.Value);
                switch (type.Value)
                {
                    case EnumParamMeasurementType.Button:
                    case EnumParamMeasurementType.Discreet:
                    case EnumParamMeasurementType.Pagebutton:
                        if (hasNoDiscreetsTag)
                        {
                            results.Add(Error.MissingTag(this, param, param.Measurement, typeString, param.Id.RawValue));
                            continue;
                        }

                        break;

                        // Potentially we can add an excessive error later on?
                }
            }

            return results;
        }

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {

        ////        default:
        ////            result.Message = String.Format("This error ({0}) isn't implemented.", context.Result.ErrorId.ToString());
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
}
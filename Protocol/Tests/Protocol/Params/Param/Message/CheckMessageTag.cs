namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Message.CheckMessageTag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckMessageTag, Category.Param)]
    internal class CheckMessageTag : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param.ConfirmPopup?.Value == Skyline.DataMiner.CICD.Models.Protocol.Enums.EnumParamConfirmPopup.Always)
                {
                    // TODO: we might want to create a new minor here mentioning we prefer the use of the Message tag over the use of the confirmPopup attribute
                    continue;
                }

                var discreets = param.Measurement?.Discreets;
                if (!param.IsButton() || discreets == null || !discreets.Any())
                {
                    continue;
                }

                var subResults = new List<IValidationResult>();

                foreach (IParamsParamMeasurementDiscreetsDiscreet discreet in discreets)
                {
                    if (discreet == null)
                    {
                        continue;
                    }

                    string displayValue = discreet.Display?.Value;

                    if (Helper.IsCriticalActionCaption(displayValue))
                    {
                        subResults.Add(Error.MissingTag_Sub(this, param, discreet.Display, displayValue));
                    }
                }

                if (subResults.Any() && String.IsNullOrWhiteSpace(param.Message?.Value))
                {
                    results.Add(Error.MissingTag(this, param, param, param.Id.RawValue).WithSubResults(subResults.ToArray()));
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
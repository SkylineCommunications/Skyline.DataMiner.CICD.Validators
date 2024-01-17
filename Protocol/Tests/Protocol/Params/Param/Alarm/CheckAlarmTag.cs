namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.CheckAlarmTag
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckAlarmTag, Category.Param)]
    internal class CheckAlarmTag : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                if (param?.Alarm?.Monitored?.Value != true)
                {
                    continue;
                }

                if (String.IsNullOrWhiteSpace(param.Alarm.Info?.Value) &&
                    String.IsNullOrWhiteSpace(param.Alarm.CL?.Value) &&
                    String.IsNullOrWhiteSpace(param.Alarm.MaL?.Value) &&
                    String.IsNullOrWhiteSpace(param.Alarm.MiL?.Value) &&
                    String.IsNullOrWhiteSpace(param.Alarm.WaL?.Value) &&
                    String.IsNullOrWhiteSpace(param.Alarm.Normal?.Value) &&
                    String.IsNullOrWhiteSpace(param.Alarm.WaH?.Value) &&
                    String.IsNullOrWhiteSpace(param.Alarm.MiH?.Value) &&
                    String.IsNullOrWhiteSpace(param.Alarm.MaH?.Value) &&
                    String.IsNullOrWhiteSpace(param.Alarm.CH?.Value))
                {
                    results.Add(Error.MissingDefaultThreshold(this, param.Alarm, param.Alarm, param.Id.RawValue));
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
}
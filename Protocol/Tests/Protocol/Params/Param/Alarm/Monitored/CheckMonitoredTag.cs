namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.Monitored.CheckMonitoredTag
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckMonitoredTag, Category.Param)]
    internal class CheckMonitoredTag : IValidate, ICodeFix, ICompare
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var param in context.EachParamWithValidId())
            {
                var alarm = param.Alarm;
                if (alarm == null)
                {
                    continue;
                }

                var monitored = param.Alarm.Monitored;
                (GenericStatus status, string monitoredRawValue, bool? monitoredValue) = GenericTests.CheckBasics(monitored, isRequired: true);

                // Missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingTag(this, param, alarm, param.Id.RawValue));
                    continue;
                }

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyTag(this, param, monitored, param.Id.RawValue));
                    continue;
                }

                // Invalid
                if (status.HasFlag(GenericStatus.Invalid))
                {
                    results.Add(Error.InvalidTag(this, param, monitored, monitoredRawValue, param.Id.RawValue));
                    continue;
                }

                if (monitoredValue == true)
                {
                    // RTDisplay Expected
                    IValidationResult rtDisplayError = Error.RTDisplayExpected(this, param, monitored, param.Id.RawValue);
                    context.CrossData.RtDisplay.AddParam(param, rtDisplayError);
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedTag(this, param, monitored, param.Id.RawValue, monitoredRawValue));
                }

            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol?.Params == null)
            {
                result.Message = "No Param found";
                return result;
            }

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedTag:
                    {
                        var paramReadNode = (IParamsParam)context.Result.ReferenceNode;
                        var paramEditNode = context.Protocol.Params.Get(paramReadNode);

                        paramEditNode.Alarm.Monitored.Value = paramReadNode.Alarm.Monitored.Value;
                        result.Success = true;

                        break;
                    }

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        public List<IValidationResult> Compare(MajorChangeCheckContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((IParamsParam oldParam, IParamsParam newParam) in context.EachMatchingParam())
            {
                string newPid = newParam?.Id?.RawValue;

                bool hasMonitoringOld = oldParam?.Alarm?.Monitored?.Value == true;
                bool hasMonitoredNew = newParam?.Alarm?.Monitored?.Value == true;

                if (hasMonitoringOld && !hasMonitoredNew)
                {
                    results.Add(ErrorCompare.RemovedAlarming(newParam, newParam, newPid));
                }
            }

            return results;
        }
    }
}
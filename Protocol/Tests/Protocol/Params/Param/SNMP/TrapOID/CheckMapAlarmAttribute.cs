namespace SLDisValidator2.Tests.Protocol.Params.Param.SNMP.TrapOID.CheckMapAlarmAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Common.Attributes;
    using SLDisValidator2.Common.Extensions;
    using SLDisValidator2.Generic;
    using SLDisValidator2.Interfaces;

    [Test(CheckId.CheckMapAlarmAttribute, Category.Param)]
    internal class CheckMapAlarmAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (var param in context.EachParamWithValidId())
            {
                var mapAlarm = param.SNMP?.TrapOID?.MapAlarm;
                if (mapAlarm == null)
                {
                    continue;
                }

                (GenericStatus status, string mapAlarmRawValue, string mapAlarmValue) = GenericTests.CheckBasics(mapAlarm, isRequired: false);

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, param, mapAlarm, param.Id.RawValue));
                    continue;
                }

                // RTDisplay required
                if (mapAlarmValue.StartsWith("true", StringComparison.OrdinalIgnoreCase))
                {
                    IValidationResult rtDisplayError = Error.RTDisplayExpected(this, param, mapAlarm, param.Id.RawValue);
                    context.CrossData.RtDisplay.AddParam(param, rtDisplayError);
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, param, mapAlarm, param.Id.RawValue, mapAlarmRawValue));
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    var readNode = (IParamsParam)context.Result.ReferenceNode;
                    var editNode = context.Protocol.Params.Get(readNode);

                    editNode.SNMP.TrapOID.MapAlarm.Value = readNode.SNMP.TrapOID.MapAlarm.Value;
                    result.Success = true;
                    break;
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
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.SNMP.Type.CheckInconsistentSnmpReadWriteTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckInconsistentSnmpReadWriteTypes, Category.Param)]
    internal class CheckInconsistentSnmpReadWriteTypes : IValidate /*, ICodeFix, ICompare*/
    {
        // Please comment out the interfaces that aren't used together with the respective methods.

        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            // Snapshot all SNMP-enabled params that have a valid (parsed) SNMP type and a non-empty OID.
            var snmpParams = context.EachParamWithValidId()
                .Where(p => p.SNMP?.Enabled?.Value == true
                         && p.SNMP.Type?.Value != null
                         && !String.IsNullOrWhiteSpace(p.SNMP.OID?.RawValue))
                .ToList();

            var writeParamsByOid = snmpParams
                .Where(p => p.Type?.Value == EnumParamType.Write)
                .GroupBy(GetSnmpTargetKey, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            foreach (var readParam in snmpParams.Where(p => p.Type?.Value == EnumParamType.Read))
            {
                string targetKey = GetSnmpTargetKey(readParam);

                if (!writeParamsByOid.TryGetValue(targetKey, out var writeParams))
                {
                    // No matching write param for this SNMP target, nothing to compare.
                    continue;
                }

                var readType = readParam.SNMP.Type.Value;

                foreach (var writeParam in writeParams)
                {
                    var writeType = writeParam.SNMP.Type.Value;

                    if (readType == writeType)
                    {
                        continue;
                    }

                    IValidationResult error = Error.InconsistentSnmpReadWriteTypes(this, null, null)
                        .WithSubResults(
                            Error.InconsistentSnmpReadWriteTypes_Sub(this, readParam, readParam.SNMP.Type, readParam.SNMP.Type.RawValue, "read", readParam.Id.RawValue),
                            Error.InconsistentSnmpReadWriteTypes_Sub(this, writeParam, writeParam.SNMP.Type, writeParam.SNMP.Type.RawValue, "write", writeParam.Id.RawValue));

                    results.Add(error);
                }
            }

            return results;
        }

        private static string GetSnmpTargetKey(IParamsParam param)
        {
            string oid = param.SNMP.OID.RawValue?.Trim();
            string oidId = param.SNMP.OID.Id?.RawValue?.Trim();
            string options = param.SNMP.OID.Options?.RawValue?.Trim();

            return String.Join("|", oid, oidId, options);
        }

        //public ICodeFixResult Fix(CodeFixContext context)
        //{
        //    CodeFixResult result = new CodeFixResult();

        //    switch (context.Result.ErrorId)
        //    {

        //        default:
        //            result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
        //            break;
        //    }

        //    return result;
        //}
        
        //public List<IValidationResult> Compare(MajorChangeCheckContext context)
        //{
        //    List<IValidationResult> results = new List<IValidationResult>();

        //    return results;
        //}
    }
}
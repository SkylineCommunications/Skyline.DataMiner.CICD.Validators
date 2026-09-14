namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.SNMP.Type.CheckInconsistentSnmpReadWriteTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
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

            // Snapshot all SNMP-enabled params so we can look up write params by OID.
            var snmpParams = context.EachParamWithValidId()
                .Where(p => p.SNMP?.Enabled?.Value == true && p.SNMP?.Type != null && p.SNMP?.OID != null)
                .ToList();

            var writeParamsByOid = snmpParams
                .Where(p => p.Type?.Value == EnumParamType.Write)
                .GroupBy(p => p.SNMP.OID.RawValue)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var readParam in snmpParams.Where(p => p.Type?.Value == EnumParamType.Read))
            {
                string oid = readParam.SNMP.OID.RawValue;

                if (!writeParamsByOid.TryGetValue(oid, out var writeParam))
                {
                    // No matching write param for this OID, nothing to compare.
                    continue;
                }

                var readType = readParam.SNMP.Type;
                var writeType = writeParam.SNMP.Type;

                if (readType.Value == writeType.Value)
                {
                    continue;
                }

                string paramName = readParam.Name?.RawValue ?? readParam.Id.RawValue;

                results.Add(Error.InconsistentSnmpReadWriteTypes(
                    this,
                    readParam,
                    readParam,
                    paramName,
                    readType.RawValue,
                    writeType.RawValue));
            }

            return results;
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
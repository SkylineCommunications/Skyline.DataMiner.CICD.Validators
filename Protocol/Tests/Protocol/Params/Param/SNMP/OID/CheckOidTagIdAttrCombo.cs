namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.SNMP.OID.CheckOidTagIdAttrCombo
{
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckOidTagIdAttrCombo, Category.Param)]
    internal class CheckOidTagIdAttrCombo : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                var snmpOidElem = param.SNMP?.OID;

                if (snmpOidElem == null)
                {
                    // No need to check anything as there is no tag.
                    continue;
                }

                var snmpOidIdAttr = snmpOidElem.Id;

                bool hasIdAttribute = snmpOidIdAttr != null;

                if (param.IsTable())
                {
                    if (!param.IsSubtable() && hasIdAttribute)
                    {
                        results.Add(Error.ExcessiveAttribute(this, snmpOidElem, snmpOidElem, param.Id.RawValue));
                        continue;
                    }
                }
                else
                {
                    // Standalone or column parameter.
                    bool hasWildcard = snmpOidElem.Value?.Contains('*') == true;

                    if (hasWildcard != hasIdAttribute)
                    {
                        string oidIdRawValue = snmpOidIdAttr?.ReadNode?.GetAttributeValue("id");
                        results.Add(Error.InvalidCombo(this, snmpOidElem, snmpOidElem, snmpOidElem.Value, oidIdRawValue, param.Id.RawValue));
                        continue;
                    }
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
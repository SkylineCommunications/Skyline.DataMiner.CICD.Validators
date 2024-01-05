namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.SNMP.OID.CheckIdAttribute
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckIdAttribute, Category.Param)]
    internal class CheckIdAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        // Regex to detect wrong values such as '01', '+1', etc, which are invalid but can be parsed to uint
        // Instead of this regex, we could consider making the protocol model more strict by using stricter NumberStyle in parser or so.
        static readonly Regex Regex = new Regex(@"^\s*[1-9]\d*\s*$|^\s*0\s*$", RegexOptions.Compiled);

        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IParamsParam param in context.EachParamWithValidId())
            {
                var snmpOidIdAttr = param.SNMP?.OID?.Id;

                if (snmpOidIdAttr == null)
                {
                    continue;
                }

                (GenericStatus status, string rawValue, uint? referencedParamId) = GenericTests.CheckBasics(snmpOidIdAttr, isRequired: false);

                // Empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, param.SNMP.OID, param.SNMP.OID, param.Id.RawValue));
                    continue;
                }

                // Invalid
                if (!Regex.IsMatch(rawValue))
                {
                    results.Add(Error.InvalidAttributeValue(this, param.SNMP.OID, param.SNMP.OID, param.Id.RawValue, rawValue));
                    continue;
                }

                // Non Existing Param
                if (!context.ProtocolModel.TryGetObjectByKey(Mappings.ParamsById, Convert.ToString(referencedParamId), out IParamsParam referencedParam))
                {
                    results.Add(Error.NonExistingParam(this, param.SNMP.OID, param.SNMP.OID, rawValue, param.Id.RawValue));
                    continue;
                }

                // Unsupported Param Type
                if (referencedParam.Type == null
                    || (referencedParam.Type.Value != EnumParamType.Read
                        && referencedParam.Type.Value != EnumParamType.Bus)
                    || referencedParam.TryGetTable(context.ProtocolModel.RelationManager, out IParamsParam _))
                {
                    results.Add(Error.UnsupportedParam(this, param.SNMP.OID, param.SNMP.OID, rawValue, param.Id.RawValue));
                    continue;
                }

                // Untrimmed
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    IValidationResult error = Error.UntrimmedAttribute(this, param, param.SNMP.OID, param.Id.RawValue, rawValue)
                        .WithExtraData(ExtraData.RawValue, rawValue);

                    results.Add(error);
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol.Params == null)
            {
                result.Message = "No Params found!";
                return result;
            }

            var readNode = (IParamsParam)context.Result.ReferenceNode;
            var editNode = context.Protocol.Params.Get(readNode);

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedAttribute:
                    editNode.SNMP.OID.Id = Convert.ToUInt32(((string)context.Result.ExtraData[ExtraData.RawValue]).Trim());
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

    internal enum ExtraData
    {
        RawValue
    }
}
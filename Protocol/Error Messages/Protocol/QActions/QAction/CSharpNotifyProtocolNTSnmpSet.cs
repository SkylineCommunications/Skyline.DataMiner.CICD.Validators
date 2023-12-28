namespace SLDisValidator2.Tests.Protocol.QActions.QAction.CSharpNotifyProtocolNTSnmpSet
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    internal static class Error
    {
        internal static IValidationResult DeltIncompatible(IValidate test, IReadable referenceNode, IReadable positionNode, string qactionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CSharpNotifyProtocolNTSnmpSet,
                ErrorId = ErrorIds.DeltIncompatible,
                FullId = "3.29.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invocation of method '{0}.{1}' is not compatible with '{2}'. QAction ID '{3}'.", "SLProtocol", "NotifyProtocol(292/*NT_SNMP_SET*/, ...)", "DELT", qactionId),
                HowToFix = "",
                ExampleCode = "object[] elementInfo = new object[] { elementId, ipPort, multipleSet, instance, connectionId, setCommunityString, enableRetries, agentId };" + Environment.NewLine + "object[] oidInfo = new object[] { new object[] { oid, newValue, snmpType } };" + Environment.NewLine + "" + Environment.NewLine + "object[] result = (object[])protocol.NotifyProtocol(292/*NT_SNMP_SET*/, elementInfo, oidInfo);",
                Details = "To make this call DELT compatible, the DMA ID needs to be provided as argument." + Environment.NewLine + "See Example code." + Environment.NewLine + "" + Environment.NewLine + "More information about the syntax can be found in the DataMiner Development Library.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint DeltIncompatible = 1;
    }

    public static class CheckId
    {
        public const uint CSharpNotifyProtocolNTSnmpSet = 29;
    }
}
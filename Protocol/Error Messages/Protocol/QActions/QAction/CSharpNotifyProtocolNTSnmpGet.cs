// <auto-generated>This is auto-generated code by Validator Management Tool. Do not modify.</auto-generated>
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpNotifyProtocolNTSnmpGet
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal static class Error
    {
        public static IValidationResult DeltIncompatible(IValidate test, IReadable referenceNode, IReadable positionNode, string qactionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CSharpNotifyProtocolNTSnmpGet,
                ErrorId = ErrorIds.DeltIncompatible,
                FullId = "3.30.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invocation of method '{0}.{1}' is not compatible with '{2}'. QAction ID '{3}'.", "SLProtocol", "NotifyProtocol(295/*NT_SNMP_GET*/, ...)", "DELT", qactionId),
                HowToFix = "",
                ExampleCode = "object[] elementInfo = new object[] { elementId, ipPort, multipleGet, instance, connectionId, getCommunityString, splitErrors, agentId };" + Environment.NewLine + "string[] oidInfo = new string[] { \"1.3.6.1.2.1.1.4.0\" };" + Environment.NewLine + "" + Environment.NewLine + "object[] result = (object[])protocol.NotifyProtocol(295/*NT_SNMP_GET*/, elementInfo, oidInfo);",
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

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CSharpNotifyProtocolNTSnmpGet = 30;
    }
}
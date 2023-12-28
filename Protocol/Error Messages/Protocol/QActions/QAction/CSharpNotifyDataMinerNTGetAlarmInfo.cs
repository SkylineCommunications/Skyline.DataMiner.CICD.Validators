namespace SLDisValidator2.Tests.Protocol.QActions.QAction.CSharpNotifyDataMinerNTGetAlarmInfo
{
    using System;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    internal static class Error
    {
        internal static IValidationResult DeltIncompatible(IValidate test, IReadable referenceNode, IReadable positionNode, string qactionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CSharpNotifyDataMinerNTGetAlarmInfo,
                ErrorId = ErrorIds.DeltIncompatible,
                FullId = "3.26.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invocation of method '{0}.{1}' is not compatible with '{2}'. QAction ID '{3}'.", "SLProtocol", "NotifyDataMiner(48/*NT_GET_ALARM_INFO*/, ...)", "DELT", qactionId),
                HowToFix = "",
                ExampleCode = "uint[] elementInfo = new uint[] { dmaId, elementId };" + Environment.NewLine + "uint[] parameterIds = new uint[] { 100, 300 };" + Environment.NewLine + "" + Environment.NewLine + "object[] result = (object[]) protocol.NotifyDataMiner(48 /* NT_GET_ALARM_INFO */, elementInfo, parameterIds);",
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
        public const uint CSharpNotifyDataMinerNTGetAlarmInfo = 26;
    }
}
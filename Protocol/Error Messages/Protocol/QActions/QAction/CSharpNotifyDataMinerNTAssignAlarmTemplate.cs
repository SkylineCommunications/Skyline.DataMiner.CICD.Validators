namespace SLDisValidator2.Tests.Protocol.QActions.QAction.CSharpNotifyDataMinerNTAssignAlarmTemplate
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
                CheckId = CheckId.CSharpNotifyDataMinerNTAssignAlarmTemplate,
                ErrorId = ErrorIds.DeltIncompatible,
                FullId = "3.19.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invocation of method '{0}.{1}' is not compatible with '{2}'. QAction ID '{3}'.", "SLProtocol", "NotifyDataMiner(Queued)(117/*NT_ASSIGN_ALARM_TEMPLATE*/, ...)", "DELT", qactionId),
                HowToFix = "",
                ExampleCode = "uint[] elementDetails = { agentId, elementId };" + Environment.NewLine + "string[] alarmTemplate = new string[] { \"Alarm Template 1\" };" + Environment.NewLine + "" + Environment.NewLine + "protocol.NotifyDataMiner(117/*NT_ASSIGN_ALARM_TEMPLATE*/, elementDetails, alarmTemplate);",
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
        public const uint CSharpNotifyDataMinerNTAssignAlarmTemplate = 19;
    }
}
namespace SLDisValidator2.Tests.Protocol.QActions.QAction.CSharpNotifyDataMinerNTSetElementState
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
                CheckId = CheckId.CSharpNotifyDataMinerNTSetElementState,
                ErrorId = ErrorIds.DeltIncompatible,
                FullId = "3.16.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invocation of method '{0}.{1}' is not compatible with '{2}'. QAction ID '{3}'.", "SLProtocol", "NotifyDataMiner(Queued)(115/*NT_SET_ELEMENT_STATE*/, ...)", "DELT", qactionId),
                HowToFix = "",
                ExampleCode = "uint[] elementDetails = new uint[] { elementId, state, deleteOptions, dmaID };" + Environment.NewLine + "protocol.NotifyDataMiner(115 /*NT_SET_ELEMENT_STATE*/ , elementDetails, null);",
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
        public const uint CSharpNotifyDataMinerNTSetElementState = 16;
    }
}
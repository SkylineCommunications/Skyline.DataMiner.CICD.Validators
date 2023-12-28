namespace SLDisValidator2.Tests.Protocol.QActions.QAction.CSharpSLProtocolCheckTrigger
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    internal static class Error
    {
        internal static IValidationResult NonExistingTrigger(IValidate test, IReadable referenceNode, IReadable positionNode, string triggerId, string qactionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CSharpSLProtocolCheckTrigger,
                ErrorId = ErrorIds.NonExistingTrigger,
                FullId = "3.3.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Method '{0}' references a non-existing '{1}' with {2} '{3}'. QAction ID '{4}'.", "SLProtocol.CheckTrigger", "Trigger", "ID", triggerId, qactionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "SLProtocol.CheckTrigger is used to make a trigger go off." + Environment.NewLine + "Make sure to provide it with an ID of a trigger that exists.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint NonExistingTrigger = 1;
    }

    public static class CheckId
    {
        public const uint CSharpSLProtocolCheckTrigger = 3;
    }
}
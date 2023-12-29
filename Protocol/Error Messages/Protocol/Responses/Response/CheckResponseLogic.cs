namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Responses.Response.CheckResponseLogic
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal static class Error
    {
        internal static IValidationResult MissingCrcResponseAction(IValidate test, IReadable referenceNode, IReadable positionNode, string responseId, string paramPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckResponseLogic,
                ErrorId = ErrorIds.MissingCrcResponseAction,
                FullId = "11.1.1",
                Category = Category.Response,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("No '{0}' Action triggered before Response '{1}'. '{0}' Param '{2}'.", "CRC", responseId, paramPid),
                HowToFix = "Make sure a CRC action is triggered after response.",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint MissingCrcResponseAction = 1;
    }

    public static class CheckId
    {
        public const uint CheckResponseLogic = 1;
    }
}
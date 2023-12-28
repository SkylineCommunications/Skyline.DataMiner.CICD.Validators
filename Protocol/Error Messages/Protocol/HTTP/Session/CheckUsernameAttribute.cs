namespace SLDisValidator2.Tests.Protocol.HTTP.Session.CheckUsernameAttribute
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    public static class Error
    {
        public static IValidationResult NonExistingId(IValidate test, IReadable referenceNode, IReadable positionNode, string pid, string sessionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckUsernameAttribute,
                ErrorId = ErrorIds.NonExistingId,
                FullId = "8.5.1",
                Category = Category.HTTP,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Attribute '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "userName", "Param", "ID", pid, "HTTP Session", "ID", sessionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "Use this attribute to specify a hardcoded username or the id of an existing parameter containing the username.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    public static class ErrorIds
    {
        public const uint NonExistingId = 1;
    }

    public static class CheckId
    {
        public const uint CheckUsernameAttribute = 5;
    }
}
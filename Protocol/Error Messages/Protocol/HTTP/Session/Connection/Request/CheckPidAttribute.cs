namespace SLDisValidator2.Tests.Protocol.HTTP.Session.Connection.Request.CheckPidAttribute
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    public static class Error
    {
        public static IValidationResult NonExistingId(IValidate test, IReadable referenceNode, IReadable positionNode, string pid, string sessionId, string connectionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckPidAttribute,
                ErrorId = ErrorIds.NonExistingId,
                FullId = "8.9.1",
                Category = Category.HTTP,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Attribute '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'. {7} {8} '{9}'.", "Request@pid", "Param", "ID", pid, "HTTP Session", "ID", sessionId, "Connection", "ID", connectionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "Use this attribute to specify the id of an existing parameter containing the url to be used for this request.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult EmptyAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string sessionId, string connectionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckPidAttribute,
                ErrorId = ErrorIds.EmptyAttribute,
                FullId = "8.9.2",
                Category = Category.HTTP,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty attribute '{0}' in {1} '{2}'. {3} {4} '{5}'.", "Request@pid", "HTTP Session", sessionId, "Connection", "ID", connectionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "Use this attribute to specify the id of an existing parameter containing the url to be used for this request.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string pidValue, string httpSessionId, string connectionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckPidAttribute,
                ErrorId = ErrorIds.InvalidAttribute,
                FullId = "8.9.3",
                Category = Category.HTTP,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{1}' in attribute '{0}'. {2} {4} '{3}'. {5} {7} '{6}'.", "Request@pid", pidValue, "HTTP Session", httpSessionId, "ID", "Connection", connectionId, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "Use this attribute to specify the id of an existing parameter containing the url to be used for this request.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    public static class ErrorIds
    {
        public const uint NonExistingId = 1;
        public const uint EmptyAttribute = 2;
        public const uint InvalidAttribute = 3;
    }

    public static class CheckId
    {
        public const uint CheckPidAttribute = 9;
    }
}
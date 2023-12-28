namespace SLDisValidator2.Tests.Protocol.Groups.Group.CheckConnectionPidAttribute
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    internal static class Error
    {
        internal static IValidationResult EmptyAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string groupId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckConnectionPidAttribute,
                ErrorId = ErrorIds.EmptyAttribute,
                FullId = "4.7.1",
                Category = Category.Group,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty attribute '{0}' in {1} '{2}'.", "connectionPid", "Group", groupId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The connectionPid attribute should contain the id of an existing parameter with the id of the connection.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult InvalidAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string connectionPid, string groupId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckConnectionPidAttribute,
                ErrorId = ErrorIds.InvalidAttribute,
                FullId = "4.7.2",
                Category = Category.Group,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{1}' in attribute '{0}'. {2} {4} '{3}'.", "connectionPid", connectionPid, "Group", groupId, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "The connectionPid attribute should contain the id of an existing parameter with the id of the connection.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult NonExistingId(IValidate test, IReadable referenceNode, IReadable positionNode, string pid, string groupId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckConnectionPidAttribute,
                ErrorId = ErrorIds.NonExistingId,
                FullId = "4.7.3",
                Category = Category.Group,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Attribute '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "connectionPid", "Param", "ID", pid, "Group", "ID", groupId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The connectionPid attribute should contain the id of an existing parameter with the id of the connection.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint EmptyAttribute = 1;
        public const uint InvalidAttribute = 2;
        public const uint NonExistingId = 3;
    }

    public static class CheckId
    {
        public const uint CheckConnectionPidAttribute = 7;
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.Content.Action.CheckActionTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal static class Error
    {
        internal static IValidationResult NonExistingId(IValidate test, IReadable referenceNode, IReadable positionNode, string actionId, string groupId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTag,
                ErrorId = ErrorIds.NonExistingId,
                FullId = "4.3.1",
                Category = Category.Group,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Tag '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "Content/Action", "Action", "ID", actionId, "Group", "ID", groupId),
                HowToFix = "",
                ExampleCode = "",
                Details = "Group/Content tag should contain a list of 'Param', 'Pair', 'Session', 'Trigger' or 'Action' tags. Note that only one type of them is allowed for a specific Group." + Environment.NewLine + " - 'Param' tags should refer to the id of an existing Param." + Environment.NewLine + " - 'Pair' tags should refer to the id of an existing Pair." + Environment.NewLine + " - 'Session' tags should refer to the id of an existing HTTP/Session." + Environment.NewLine + " - 'Trigger' tags should refer to the id of an existing Trigger." + Environment.NewLine + " - 'Action' tags should refer to the id of an existing Action.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult EmptyActionTag(IValidate test, IReadable referenceNode, IReadable positionNode, string groupId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTag,
                ErrorId = ErrorIds.EmptyActionTag,
                FullId = "4.3.2",
                Category = Category.Group,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty tag '{0}' in {1} '{2}'.", "Content/Action", "Group", groupId),
                HowToFix = "",
                ExampleCode = "",
                Details = "Group/Content tag should contain a list of 'Param', 'Pair', 'Session', 'Trigger' or 'Action' tags. Note that only one type of them is allowed for a specific Group." + Environment.NewLine + " - 'Param' tags should refer to the id of an existing Param." + Environment.NewLine + " - 'Pair' tags should refer to the id of an existing Pair." + Environment.NewLine + " - 'Session' tags should refer to the id of an existing HTTP/Session." + Environment.NewLine + " - 'Trigger' tags should refer to the id of an existing Trigger." + Environment.NewLine + " - 'Action' tags should refer to the id of an existing Action.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult InvalidActionTag(IValidate test, IReadable referenceNode, IReadable positionNode, string value, string groupId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckActionTag,
                ErrorId = ErrorIds.InvalidActionTag,
                FullId = "4.3.3",
                Category = Category.Group,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{0}' in tag '{1}'. {2} {4} '{3}'.", value, "Content/Action", "Group", groupId, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "Group/Content tag should contain a list of 'Param', 'Pair', 'Session', 'Trigger' or 'Action' tags. Note that only one type of them is allowed for a specific Group." + Environment.NewLine + " - 'Param' tags should refer to the id of an existing Param." + Environment.NewLine + " - 'Pair' tags should refer to the id of an existing Pair." + Environment.NewLine + " - 'Session' tags should refer to the id of an existing HTTP/Session." + Environment.NewLine + " - 'Trigger' tags should refer to the id of an existing Trigger." + Environment.NewLine + " - 'Action' tags should refer to the id of an existing Action.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint NonExistingId = 1;
        public const uint EmptyActionTag = 2;
        public const uint InvalidActionTag = 3;
    }

    public static class CheckId
    {
        public const uint CheckActionTag = 3;
    }
}
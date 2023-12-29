namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.Content.Trigger.CheckTriggerTag
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
        public static IValidationResult NonExistingId(IValidate test, IReadable referenceNode, IReadable positionNode, string triggerId, string groupId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckTriggerTag,
                ErrorId = ErrorIds.NonExistingId,
                FullId = "4.6.1",
                Category = Category.Group,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Tag '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "Content/Trigger", "Trigger", "ID", triggerId, "Group", "ID", groupId),
                HowToFix = "",
                ExampleCode = "",
                Details = "Group/Content tag should contain a list of 'Param', 'Pair', 'Session', 'Trigger' or 'Action' tags. Note that only one type of them is allowed for a specific Group." + Environment.NewLine + " - 'Param' tags should refer to the id of an existing Param." + Environment.NewLine + " - 'Pair' tags should refer to the id of an existing Pair." + Environment.NewLine + " - 'Session' tags should refer to the id of an existing HTTP/Session." + Environment.NewLine + " - 'Trigger' tags should refer to the id of an existing Trigger." + Environment.NewLine + " - 'Action' tags should refer to the id of an existing Action.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult EmptyTriggerTag(IValidate test, IReadable referenceNode, IReadable positionNode, string groupId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckTriggerTag,
                ErrorId = ErrorIds.EmptyTriggerTag,
                FullId = "4.6.2",
                Category = Category.Group,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty tag '{0}' in {1} '{2}'.", "Content/Trigger", "Group", groupId),
                HowToFix = "",
                ExampleCode = "",
                Details = "Group/Content tag should contain a list of 'Param', 'Pair', 'Session', 'Trigger' or 'Action' tags. Note that only one type of them is allowed for a specific Group." + Environment.NewLine + " - 'Param' tags should refer to the id of an existing Param." + Environment.NewLine + " - 'Pair' tags should refer to the id of an existing Pair." + Environment.NewLine + " - 'Session' tags should refer to the id of an existing HTTP/Session." + Environment.NewLine + " - 'Trigger' tags should refer to the id of an existing Trigger." + Environment.NewLine + " - 'Action' tags should refer to the id of an existing Action.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidTriggerTag(IValidate test, IReadable referenceNode, IReadable positionNode, string value, string groupId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckTriggerTag,
                ErrorId = ErrorIds.InvalidTriggerTag,
                FullId = "4.6.3",
                Category = Category.Group,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{0}' in tag '{1}'. {2} {4} '{3}'.", value, "Content/Trigger", "Group", groupId, "ID"),
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
        public const uint EmptyTriggerTag = 2;
        public const uint InvalidTriggerTag = 3;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckTriggerTag = 6;
    }
}
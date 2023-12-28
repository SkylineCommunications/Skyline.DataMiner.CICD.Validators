namespace SLDisValidator2.Tests.Protocol.Timers.Timer.Content.Group.CheckGroupTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    public static class Error
    {
        public static IValidationResult NonExistingIdInGroup(IValidate test, IReadable referenceNode, IReadable positionNode, string referenceKind, string referenceId, string timerId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckGroupTag,
                ErrorId = ErrorIds.NonExistingIdInGroup,
                FullId = "7.5.2",
                Category = Category.Timer,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Tag '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "Group", referenceKind, "ID", referenceId, "Timer", "ID", timerId),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult EmptyGroupTag(IValidate test, IReadable referenceNode, IReadable positionNode, string timerId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckGroupTag,
                ErrorId = ErrorIds.EmptyGroupTag,
                FullId = "7.5.3",
                Category = Category.Timer,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty tag '{0}' in {1} '{2}'.", "Group", "Timer", timerId),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidGroupTag(IValidate test, IReadable referenceNode, IReadable positionNode, string tagValue, string timerId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckGroupTag,
                ErrorId = ErrorIds.InvalidGroupTag,
                FullId = "7.5.4",
                Category = Category.Timer,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{0}' in tag '{1}'. {2} {4} '{3}'.", tagValue, "Content/Group", "Timer", timerId, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    public static class ErrorIds
    {
        public const uint NonExistingIdInGroup = 2;
        public const uint EmptyGroupTag = 3;
        public const uint InvalidGroupTag = 4;
    }

    public static class CheckId
    {
        public const uint CheckGroupTag = 5;
    }
}
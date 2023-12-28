namespace SLDisValidator2.Tests.Protocol.Params.Param.Type.CheckTypeTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    internal static class Error
    {
        internal static IValidationResult MissingTag(IValidate test, IReadable referenceNode, IReadable positionNode, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckTypeTag,
                ErrorId = ErrorIds.MissingTag,
                FullId = "2.61.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing tag '{0}' in {1} '{2}'.", "Param/Type", "Param", pid),
                HowToFix = "",
                ExampleCode = "",
                Details = "All parameters must have a properly filled-in 'Param/Type' tag.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult EmptyTag(IValidate test, IReadable referenceNode, IReadable positionNode, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckTypeTag,
                ErrorId = ErrorIds.EmptyTag,
                FullId = "2.61.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty tag '{0}' in {1} '{2}'.", "Param/Type", "Param", pid),
                HowToFix = "",
                ExampleCode = "",
                Details = "All parameters must have a properly filled-in 'Param/Type' tag.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult UntrimmedTag(IValidate test, IReadable referenceNode, IReadable positionNode, string pid, string untrimmedValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckTypeTag,
                ErrorId = ErrorIds.UntrimmedTag,
                FullId = "2.61.3",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed tag '{0}' in {1} '{2}'. Current value '{3}'.", "Param/Type", "Param", pid, untrimmedValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "All parameters must have a properly filled-in 'Param/Type' tag.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult InvalidValue(IValidate test, IReadable referenceNode, IReadable positionNode, string tagValue, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckTypeTag,
                ErrorId = ErrorIds.InvalidValue,
                FullId = "2.61.4",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{0}' in tag '{1}'. {2} {4} '{3}'.", tagValue, "Param/Type", "Param", pid, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "All parameters must have a properly filled-in 'Param/Type' tag.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint MissingTag = 1;
        public const uint EmptyTag = 2;
        public const uint UntrimmedTag = 3;
        public const uint InvalidValue = 4;
    }

    public static class CheckId
    {
        public const uint CheckTypeTag = 61;
    }
}
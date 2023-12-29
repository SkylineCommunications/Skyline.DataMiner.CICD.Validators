namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Pairs.Pair.Content.Command.CheckCommandTag
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
        public static IValidationResult MissingTag(IValidate test, IReadable referenceNode, IReadable positionNode, string pairId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckCommandTag,
                ErrorId = ErrorIds.MissingTag,
                FullId = "9.4.1",
                Category = Category.Pair,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing tag '{0}' in {1} '{2}'.", "Content/Command", "Pair", pairId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The Content tag of every pairs should contain one and only one Command tag which should have as value an unsigned number and refer to the id of an existing Command." + Environment.NewLine + "Note that only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult EmptyTag(IValidate test, IReadable referenceNode, IReadable positionNode, string pairId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckCommandTag,
                ErrorId = ErrorIds.EmptyTag,
                FullId = "9.4.2",
                Category = Category.Pair,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty tag '{0}' in {1} '{2}'.", "Content/Command", "Pair", pairId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The Content tag of every pairs should contain one and only one Command tag which should have as value an unsigned number and refer to the id of an existing Command." + Environment.NewLine + "Note that only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UntrimmedTag(IValidate test, IReadable referenceNode, IReadable positionNode, string pairId, string untrimmedValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckCommandTag,
                ErrorId = ErrorIds.UntrimmedTag,
                FullId = "9.4.3",
                Category = Category.Pair,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed tag '{0}' in {1} '{2}'. Current value '{3}'.", "Content/Command", "Pair", pairId, untrimmedValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "The Content tag of every pairs should contain one and only one Command tag which should have as value an unsigned number and refer to the id of an existing Command." + Environment.NewLine + "Note that only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidValue(IValidate test, IReadable referenceNode, IReadable positionNode, string tagValue, string pairId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckCommandTag,
                ErrorId = ErrorIds.InvalidValue,
                FullId = "9.4.4",
                Category = Category.Pair,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{0}' in tag '{1}'. {2} {4} '{3}'.", tagValue, "Content/Command", "Pair", pairId, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "The Content tag of every pairs should contain one and only one Command tag which should have as value an unsigned number and refer to the id of an existing Command." + Environment.NewLine + "Note that only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult NonExistingId(IValidate test, IReadable referenceNode, IReadable positionNode, string commandId, string pairId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckCommandTag,
                ErrorId = ErrorIds.NonExistingId,
                FullId = "9.4.5",
                Category = Category.Pair,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Tag '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "Content/Command", "Command", "ID", commandId, "Pair", "ID", pairId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The Content tag of every pairs should contain one and only one Command tag which should have as value an unsigned number and refer to the id of an existing Command." + Environment.NewLine + "Note that only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).",
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
        public const uint NonExistingId = 5;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckCommandTag = 4;
    }
}
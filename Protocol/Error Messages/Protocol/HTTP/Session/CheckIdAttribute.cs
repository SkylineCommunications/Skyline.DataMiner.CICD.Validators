namespace SLDisValidator2.Tests.Protocol.HTTP.Session.CheckIdAttribute
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    internal static class Error
    {
        internal static IValidationResult MissingAttribute(IValidate test, IReadable referenceNode, IReadable positionNode)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckIdAttribute,
                ErrorId = ErrorIds.MissingAttribute,
                FullId = "8.16.1",
                Category = Category.HTTP,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Missing attribute '{0}'.", "HTTP/Session@id"),
                HowToFix = "",
                ExampleCode = "",
                Details = "The id attribute is used internally as the identifier for each session." + Environment.NewLine + "It is therefore mandatory and needs to follow a number of rules:" + Environment.NewLine + "- Each session should have a unique id." + Environment.NewLine + "- Should be an unsigned integer." + Environment.NewLine + "- Only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult EmptyAttribute(IValidate test, IReadable referenceNode, IReadable positionNode)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckIdAttribute,
                ErrorId = ErrorIds.EmptyAttribute,
                FullId = "8.16.2",
                Category = Category.HTTP,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Empty attribute '{0}'.", "HTTP/Session@id"),
                HowToFix = "",
                ExampleCode = "",
                Details = "The id attribute is used internally as the identifier for each session." + Environment.NewLine + "It is therefore mandatory and needs to follow a number of rules:" + Environment.NewLine + "- Each session should have a unique id." + Environment.NewLine + "- Should be an unsigned integer." + Environment.NewLine + "- Only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult UntrimmedAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string attributeValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckIdAttribute,
                ErrorId = ErrorIds.UntrimmedAttribute,
                FullId = "8.16.3",
                Category = Category.HTTP,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed attribute '{0}'. Current value '{1}'.", "HTTP/Session@id", attributeValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "The id attribute is used internally as the identifier for each session." + Environment.NewLine + "It is therefore mandatory and needs to follow a number of rules:" + Environment.NewLine + "- Each session should have a unique id." + Environment.NewLine + "- Should be an unsigned integer." + Environment.NewLine + "- Only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult InvalidValue(IValidate test, IReadable referenceNode, IReadable positionNode, string sessionId, string sessionName)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckIdAttribute,
                ErrorId = ErrorIds.InvalidValue,
                FullId = "8.16.4",
                Category = Category.HTTP,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{1}' in attribute '{0}'. {2} {4} '{3}'.", "HTTP/Session@id", sessionId, "Session", sessionName, "name"),
                HowToFix = "",
                ExampleCode = "",
                Details = "The id attribute is used internally as the identifier for each session." + Environment.NewLine + "It is therefore mandatory and needs to follow a number of rules:" + Environment.NewLine + "- Each session should have a unique id." + Environment.NewLine + "- Should be an unsigned integer." + Environment.NewLine + "- Only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult DuplicatedId(IValidate test, IReadable referenceNode, IReadable positionNode, string sessionId, string sessionNames)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckIdAttribute,
                ErrorId = ErrorIds.DuplicatedId,
                FullId = "8.16.5",
                Category = Category.HTTP,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("More than one {0} with same ID '{1}'. {0} Names '{2}'.", "HTTP Session", sessionId, sessionNames),
                HowToFix = "",
                ExampleCode = "",
                Details = "The id attribute is used internally as the identifier for each session." + Environment.NewLine + "It is therefore mandatory and needs to follow a number of rules:" + Environment.NewLine + "- Each session should have a unique id." + Environment.NewLine + "- Should be an unsigned integer." + Environment.NewLine + "- Only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint MissingAttribute = 1;
        public const uint EmptyAttribute = 2;
        public const uint UntrimmedAttribute = 3;
        public const uint InvalidValue = 4;
        public const uint DuplicatedId = 5;
    }

    public static class CheckId
    {
        public const uint CheckIdAttribute = 16;
    }
}
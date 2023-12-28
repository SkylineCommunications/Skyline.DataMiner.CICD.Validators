namespace SLDisValidator2.Tests.Protocol.Actions.Action.On.CheckOnTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    public static class Error
    {
        public static IValidationResult MissingTag(IValidate test, IReadable referenceNode, IReadable positionNode, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOnTag,
                ErrorId = ErrorIds.MissingTag,
                FullId = "6.6.1",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing tag '{0}' in {1} '{2}'.", "On", "Action", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/On' tag is mandatory and should contain one of the following values:" + Environment.NewLine + "command, group, pair, parameter, protocol, response, timer.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult EmptyTag(IValidate test, IReadable referenceNode, IReadable positionNode, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOnTag,
                ErrorId = ErrorIds.EmptyTag,
                FullId = "6.6.2",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty tag '{0}' in {1} '{2}'.", "On", "Action", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/On' tag is mandatory and should contain one of the following values:" + Environment.NewLine + "command, group, pair, parameter, protocol, response, timer.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UntrimmedTag(IValidate test, IReadable referenceNode, IReadable positionNode, string actionId, string untrimmedValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOnTag,
                ErrorId = ErrorIds.UntrimmedTag,
                FullId = "6.6.3",
                Category = Category.Action,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed tag '{0}' in {1} '{2}'. Current value '{3}'.", "On", "Action", actionId, untrimmedValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/On' tag is mandatory and should contain one of the following values:" + Environment.NewLine + "command, group, pair, parameter, protocol, response, timer.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidValue(IValidate test, IReadable referenceNode, IReadable positionNode, string actionOn, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOnTag,
                ErrorId = ErrorIds.InvalidValue,
                FullId = "6.6.4",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{0}' in tag '{1}'. {2} {4} '{3}'.", actionOn, "On", "Action", actionId, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/On' tag is mandatory and should contain one of the following values:" + Environment.NewLine + "command, group, pair, parameter, protocol, response, timer.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    public static class ErrorIds
    {
        public const uint MissingTag = 1;
        public const uint EmptyTag = 2;
        public const uint UntrimmedTag = 3;
        public const uint InvalidValue = 4;
    }

    public static class CheckId
    {
        public const uint CheckOnTag = 6;
    }
}
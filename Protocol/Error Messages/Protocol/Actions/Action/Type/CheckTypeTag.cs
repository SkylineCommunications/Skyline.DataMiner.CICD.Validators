namespace SLDisValidator2.Tests.Protocol.Actions.Action.Type.CheckTypeTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    internal static class Error
    {
        internal static IValidationResult MissingTag(IValidate test, IReadable referenceNode, IReadable positionNode, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckTypeTag,
                ErrorId = ErrorIds.MissingTag,
                FullId = "6.5.1",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing tag '{0}' in {1} '{2}'.", "Type", "Action", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/Type' tag is mandatory and should contain one of the following values:" + Environment.NewLine + "add to execute, aggregate, append, append data, change length, clear, clear length info, clear on display, close, copy, copy reverse, crc, execute, execute next, execute one, execute one top, execute one now, force execute, go, increment, length, lock, unlock, make, merge, multiply, normalize, open, pow, priority lock, priority unlock, read, read file, read stuffing, replace, replace data, reschedule, restart timer, reverse, run actions, save, set, set and get with wait, set info, set next, set with wait, sleep, start, stop, stop current group, stuffing, swap column, timeout, wmi.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult EmptyTag(IValidate test, IReadable referenceNode, IReadable positionNode, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckTypeTag,
                ErrorId = ErrorIds.EmptyTag,
                FullId = "6.5.2",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty tag '{0}' in {1} '{2}'.", "Type", "Action", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/Type' tag is mandatory and should contain one of the following values:" + Environment.NewLine + "add to execute, aggregate, append, append data, change length, clear, clear length info, clear on display, close, copy, copy reverse, crc, execute, execute next, execute one, execute one top, execute one now, force execute, go, increment, length, lock, unlock, make, merge, multiply, normalize, open, pow, priority lock, priority unlock, read, read file, read stuffing, replace, replace data, reschedule, restart timer, reverse, run actions, save, set, set and get with wait, set info, set next, set with wait, sleep, start, stop, stop current group, stuffing, swap column, timeout, wmi.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult UntrimmedTag(IValidate test, IReadable referenceNode, IReadable positionNode, string actionId, string untrimmedValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckTypeTag,
                ErrorId = ErrorIds.UntrimmedTag,
                FullId = "6.5.3",
                Category = Category.Action,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed tag '{0}' in {1} '{2}'. Current value '{3}'.", "Type", "Action", actionId, untrimmedValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/Type' tag is mandatory and should contain one of the following values:" + Environment.NewLine + "add to execute, aggregate, append, append data, change length, clear, clear length info, clear on display, close, copy, copy reverse, crc, execute, execute next, execute one, execute one top, execute one now, force execute, go, increment, length, lock, unlock, make, merge, multiply, normalize, open, pow, priority lock, priority unlock, read, read file, read stuffing, replace, replace data, reschedule, restart timer, reverse, run actions, save, set, set and get with wait, set info, set next, set with wait, sleep, start, stop, stop current group, stuffing, swap column, timeout, wmi.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult InvalidValue(IValidate test, IReadable referenceNode, IReadable positionNode, string actionType, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckTypeTag,
                ErrorId = ErrorIds.InvalidValue,
                FullId = "6.5.4",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{0}' in tag '{1}'. {2} {4} '{3}'.", actionType, "Type", "Action", actionId, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Action/Type' tag is mandatory and should contain one of the following values:" + Environment.NewLine + "add to execute, aggregate, append, append data, change length, clear, clear length info, clear on display, close, copy, copy reverse, crc, execute, execute next, execute one, execute one top, execute one now, force execute, go, increment, length, lock, unlock, make, merge, multiply, normalize, open, pow, priority lock, priority unlock, read, read file, read stuffing, replace, replace data, reschedule, restart timer, reverse, run actions, save, set, set and get with wait, set info, set next, set with wait, sleep, start, stop, stop current group, stuffing, swap column, timeout, wmi.",
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
        public const uint CheckTypeTag = 5;
    }
}
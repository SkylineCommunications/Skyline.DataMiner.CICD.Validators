namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Triggers.Trigger.CheckAfterStartupFlow
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
        public static IValidationResult InvalidAfterStartupTriggerCondition(IValidate test, IReadable referenceNode, IReadable positionNode, string triggerId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckAfterStartupFlow,
                ErrorId = ErrorIds.InvalidAfterStartupTriggerCondition,
                FullId = "5.7.1",
                Category = Category.Trigger,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("After startup Trigger can't have a Condition. Trigger ID '{0}'.", triggerId),
                HowToFix = "Move the condition to a later step.",
                ExampleCode = "",
                Details = "A condition should not be used until the after startup flow went through the protocol thread (via a poll group).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidAfterStartupActionCondition(IValidate test, IReadable referenceNode, IReadable positionNode, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckAfterStartupFlow,
                ErrorId = ErrorIds.InvalidAfterStartupActionCondition,
                FullId = "5.7.2",
                Category = Category.Trigger,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("After startup Action can't have a Condition. Action ID '{0}'.", actionId),
                HowToFix = "Move the condition to a later step.",
                ExampleCode = "",
                Details = "A condition should not be used until the after startup flow went through the protocol thread (via a poll group).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidAfterStartupTriggerType(IValidate test, IReadable referenceNode, IReadable positionNode, string triggerId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckAfterStartupFlow,
                ErrorId = ErrorIds.InvalidAfterStartupTriggerType,
                FullId = "5.7.3",
                Category = Category.Trigger,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("After startup Trigger must have a Type tag with value 'action'. Trigger ID '{0}'", triggerId),
                HowToFix = "",
                ExampleCode = "",
                Details = "After startup flow must be:" + Environment.NewLine + "Trigger: after startup On protocol -> Action: 'execute next/execute one top/execute/execute one now' On Group -> Group: poll/poll action/poll trigger.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidAfterStartupActionOn(IValidate test, IReadable referenceNode, IReadable positionNode, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckAfterStartupFlow,
                ErrorId = ErrorIds.InvalidAfterStartupActionOn,
                FullId = "5.7.4",
                Category = Category.Trigger,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("After startup Action must have an On tag with value 'group'. Action ID '{0}'.", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "After startup flow must be:" + Environment.NewLine + "Trigger: after startup On protocol -> Action: 'execute next/execute one top/execute/execute one now' On Group -> Group: poll/poll action/poll trigger.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidAfterStartupActionType(IValidate test, IReadable referenceNode, IReadable positionNode, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckAfterStartupFlow,
                ErrorId = ErrorIds.InvalidAfterStartupActionType,
                FullId = "5.7.5",
                Category = Category.Trigger,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("After startup Action must have a Type tag with value 'execute next' or 'execute'. Action ID '{0}'.", actionId),
                HowToFix = "",
                ExampleCode = "",
                Details = "After startup flow must be:" + Environment.NewLine + "Trigger: after startup On protocol -> Action: 'execute next/execute one top/execute/execute one now' On Group -> Group: poll/poll action/poll trigger.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidAfterStartupGroupType(IValidate test, IReadable referenceNode, IReadable positionNode, string groupId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckAfterStartupFlow,
                ErrorId = ErrorIds.InvalidAfterStartupGroupType,
                FullId = "5.7.6",
                Category = Category.Trigger,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("After startup Group must have a Type tag with value 'poll', 'poll trigger' or 'poll action'. Group ID '{0}'.", groupId),
                HowToFix = "",
                ExampleCode = "",
                Details = "After startup flow must be:" + Environment.NewLine + "Trigger: after startup On protocol -> Action: 'execute next/execute one top/execute/execute one now' On Group -> Group: poll/poll action/poll trigger.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint InvalidAfterStartupTriggerCondition = 1;
        public const uint InvalidAfterStartupActionCondition = 2;
        public const uint InvalidAfterStartupTriggerType = 3;
        public const uint InvalidAfterStartupActionOn = 4;
        public const uint InvalidAfterStartupActionType = 5;
        public const uint InvalidAfterStartupGroupType = 6;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckAfterStartupFlow = 7;
    }
}
// <auto-generated>This is auto-generated code by Validator Management Tool. Do not modify.</auto-generated>
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Actions.Action.Condition.CheckConditionTag
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
        public static IValidationResult InvalidCondition(IValidate test, IReadable referenceNode, IReadable positionNode, string conditionString, string invalidityReason, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckConditionTag,
                ErrorId = ErrorIds.InvalidCondition,
                FullId = "6.4.1",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid condition '{0}'. Reason '{1}'. {2} {3} '{4}'.", conditionString, invalidityReason, "Action", "ID", actionId),
                HowToFix = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult NonExistingId(IValidate test, IReadable referenceNode, IReadable positionNode, string paramId, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckConditionTag,
                ErrorId = ErrorIds.NonExistingId,
                FullId = "6.4.2",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Tag '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "Action/Condition", "Param", "PID", paramId, "Action", "ID", actionId),
                HowToFix = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult ConditionCanBeSimplified(IValidate test, IReadable referenceNode, IReadable positionNode, string conditionString, string actionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckConditionTag,
                ErrorId = ErrorIds.ConditionCanBeSimplified,
                FullId = "6.4.3",
                Category = Category.Action,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Condition '{0}' can be simplified. {1} {2} '{3}'.", conditionString, "Action", "ID", actionId),
                HowToFix = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint InvalidCondition = 1;
        public const uint NonExistingId = 2;
        public const uint ConditionCanBeSimplified = 3;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckConditionTag = 4;
    }
}
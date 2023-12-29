namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Commands.Command.Content.Param.CheckParamTag
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
        public static IValidationResult NonExistingId(IValidate test, IReadable referenceNode, IReadable positionNode, string referencedPid, string commandId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckParamTag,
                ErrorId = ErrorIds.NonExistingId,
                FullId = "10.3.1",
                Category = Category.Command,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Tag '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "Content/Param", "Param", "ID", referencedPid, "Command", "ID", commandId),
                HowToFix = "",
                ExampleCode = "",
                Details = "Command/Content tag should contain a list of 'Param' tags. The 'Param' tags should refer to the id of an existing Param.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult EmptyParamTag(IValidate test, IReadable referenceNode, IReadable positionNode, string commandId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckParamTag,
                ErrorId = ErrorIds.EmptyParamTag,
                FullId = "10.3.2",
                Category = Category.Command,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty tag '{0}' in {1} '{2}'.", "Content/Param", "Command", commandId),
                HowToFix = "",
                ExampleCode = "",
                Details = "Command/Content tag should contain a list of 'Param' tags. The 'Param' tags should refer to the id of an existing Param.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidParamTag(IValidate test, IReadable referenceNode, IReadable positionNode, string value, string commandId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckParamTag,
                ErrorId = ErrorIds.InvalidParamTag,
                FullId = "10.3.3",
                Category = Category.Command,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{0}' in tag '{1}'. {2} {4} '{3}'.", value, "Content/Param", "Command", commandId, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "Command/Content tag should contain a list of 'Param' tags. The 'Param' tags should refer to the id of an existing Param.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint NonExistingId = 1;
        public const uint EmptyParamTag = 2;
        public const uint InvalidParamTag = 3;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckParamTag = 3;
    }
}
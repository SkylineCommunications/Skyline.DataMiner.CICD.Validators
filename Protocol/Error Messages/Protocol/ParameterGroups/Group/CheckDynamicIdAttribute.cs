namespace SLDisValidator2.Tests.Protocol.ParameterGroups.Group.CheckDynamicIdAttribute
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    public static class Error
    {
        public static IValidationResult NonExistingId(IValidate test, IReadable referenceNode, IReadable positionNode, string tablePid, string parameterGroupId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDynamicIdAttribute,
                ErrorId = ErrorIds.NonExistingId,
                FullId = "16.1.1",
                Category = Category.ParameterGroup,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Attribute '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "dynamicId", "Table", "PID", tablePid, "ParameterGroup", "ID", parameterGroupId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The ParameterGroups/Group@dynamicId should always refer to an existing table Param.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string attributeValue, string parameterGroupId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDynamicIdAttribute,
                ErrorId = ErrorIds.InvalidAttribute,
                FullId = "16.1.2",
                Category = Category.ParameterGroup,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{1}' in attribute '{0}'. {2} {4} '{3}'.", "dynamicId", attributeValue, "ParameterGroup", parameterGroupId, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "The ParameterGroup/Groups/Group/Params/Param tags should all refer to existing parameters.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult EmptyAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string groupId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDynamicIdAttribute,
                ErrorId = ErrorIds.EmptyAttribute,
                FullId = "16.1.3",
                Category = Category.ParameterGroup,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty attribute '{0}' in {1} '{2}'.", "dynamicId", "ParameterGroup", groupId),
                HowToFix = "",
                ExampleCode = "",
                Details = "The ParameterGroup/Groups/Group/Params/Param tags should all refer to existing parameters.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    public static class ErrorIds
    {
        public const uint NonExistingId = 1;
        public const uint InvalidAttribute = 2;
        public const uint EmptyAttribute = 3;
    }

    public static class CheckId
    {
        public const uint CheckDynamicIdAttribute = 1;
    }
}
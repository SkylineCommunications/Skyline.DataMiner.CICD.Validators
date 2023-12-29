namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.CheckDependencyId
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
        public static IValidationResult EmptyAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDependencyId,
                ErrorId = ErrorIds.EmptyAttribute,
                FullId = "2.54.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty attribute '{0}' in {1} '{2}'.", "Discreets@dependencyId", "Param", pid),
                HowToFix = "",
                ExampleCode = "",
                Details = "Discreets@dependencyId attribute should contain the ID of a Param." + Environment.NewLine + "The referenced Param is expected to:" + Environment.NewLine + "- Be of type 'read' or 'read bit'." + Environment.NewLine + "- Have RTDisplay tag set to 'true'.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UntrimmedAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string pid, string untrimmedValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDependencyId,
                ErrorId = ErrorIds.UntrimmedAttribute,
                FullId = "2.54.2",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed attribute '{0}' in {1} '{2}'. Current value '{3}'.", "Discreets@dependencyId", "Param", pid, untrimmedValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "Discreets@dependencyId attribute should contain the ID of a Param." + Environment.NewLine + "The referenced Param is expected to:" + Environment.NewLine + "- Be of type 'read' or 'read bit'." + Environment.NewLine + "- Have RTDisplay tag set to 'true'.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidValue(IValidate test, IReadable referenceNode, IReadable positionNode, string attributeValue, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDependencyId,
                ErrorId = ErrorIds.InvalidValue,
                FullId = "2.54.3",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{1}' in attribute '{0}'. {2} {4} '{3}'.", "Discreets@dependencyId", attributeValue, "Param", pid, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "Discreets@dependencyId attribute should contain the ID of a Param." + Environment.NewLine + "The referenced Param is expected to:" + Environment.NewLine + "- Be of type 'read' or 'read bit'." + Environment.NewLine + "- Have RTDisplay tag set to 'true'.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult NonExistingId(IValidate test, IReadable referenceNode, IReadable positionNode, string referencedPid, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDependencyId,
                ErrorId = ErrorIds.NonExistingId,
                FullId = "2.54.4",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Attribute '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "Discreets@dependencyId", "Param", "ID", referencedPid, "Param", "ID", pid),
                HowToFix = "",
                ExampleCode = "",
                Details = "Discreets@dependencyId attribute should contain the ID of a Param." + Environment.NewLine + "The referenced Param is expected to:" + Environment.NewLine + "- Be of type 'read' or 'read bit'." + Environment.NewLine + "- Have RTDisplay tag set to 'true'.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult ReferencedParamWrongType(IValidate test, IReadable referenceNode, IReadable positionNode, string referencedParamType, string referencedPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDependencyId,
                ErrorId = ErrorIds.ReferencedParamWrongType,
                FullId = "2.54.5",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid Param Type '{0}' on Param referenced by a 'Discreets@dependencyId' attribute. Param ID '{1}'.", referencedParamType, referencedPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "Discreets@dependencyId attribute should contain the ID of a Param." + Environment.NewLine + "The referenced Param is expected to:" + Environment.NewLine + "- Be of type 'read' or 'read bit'." + Environment.NewLine + "- Have RTDisplay tag set to 'true'.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult ReferencedParamRTDisplayExpected(IValidate test, IReadable referenceNode, IReadable positionNode, string referencedPid, string referencingPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDependencyId,
                ErrorId = ErrorIds.ReferencedParamRTDisplayExpected,
                FullId = "2.54.6",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("RTDisplay(true) expected on Param '{0}' referenced by a 'Discreets@dependencyId' attribute. Param ID '{1}'.", referencedPid, referencingPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "Discreets@dependencyId attribute should contain the ID of a Param." + Environment.NewLine + "The referenced Param is expected to:" + Environment.NewLine + "- Be of type 'read' or 'read bit'." + Environment.NewLine + "- Have RTDisplay tag set to 'true'.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint EmptyAttribute = 1;
        public const uint UntrimmedAttribute = 2;
        public const uint InvalidValue = 3;
        public const uint NonExistingId = 4;
        public const uint ReferencedParamWrongType = 5;
        public const uint ReferencedParamRTDisplayExpected = 6;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckDependencyId = 54;
    }
}
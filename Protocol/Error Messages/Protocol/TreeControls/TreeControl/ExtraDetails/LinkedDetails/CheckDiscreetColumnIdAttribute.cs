namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.ExtraDetails.LinkedDetails.CheckDiscreetColumnIdAttribute
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
        public static IValidationResult MissingAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string treeControlPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDiscreetColumnIdAttribute,
                ErrorId = ErrorIds.MissingAttribute,
                FullId = "18.7.1",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing attribute '{0}' in {1} '{2}'.", "LinkedDetails@discreetColumnId", "TreeControl", treeControlPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "LinkedDetails@discreetColumnId attribute should contain a column PID. The value contained in that column will then be compared to the value specified in LinkedDetails@value attribute." + Environment.NewLine + "Such column should have its RTDisplay tag set to true.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult EmptyAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string treeControlPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDiscreetColumnIdAttribute,
                ErrorId = ErrorIds.EmptyAttribute,
                FullId = "18.7.2",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty attribute '{0}' in {1} '{2}'.", "LinkedDetails@discreetColumnId", "TreeControl", treeControlPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "LinkedDetails@discreetColumnId attribute should contain a column PID. The value contained in that column will then be compared to the value specified in LinkedDetails@value attribute." + Environment.NewLine + "Such column should have its RTDisplay tag set to true.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UntrimmedAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string treeControlPid, string untrimmedValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDiscreetColumnIdAttribute,
                ErrorId = ErrorIds.UntrimmedAttribute,
                FullId = "18.7.3",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed attribute '{0}' in {1} '{2}'. Current value '{3}'.", "LinkedDetails@discreetColumnId", "TreeControl", treeControlPid, untrimmedValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "LinkedDetails@discreetColumnId attribute should contain a column PID. The value contained in that column will then be compared to the value specified in LinkedDetails@value attribute." + Environment.NewLine + "Such column should have its RTDisplay tag set to true.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidValue(IValidate test, IReadable referenceNode, IReadable positionNode, string attributeValue, string treeControlPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDiscreetColumnIdAttribute,
                ErrorId = ErrorIds.InvalidValue,
                FullId = "18.7.4",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{1}' in attribute '{0}'. {2} {4} '{3}'.", "LinkedDetails@discreetColumnId", attributeValue, "TreeControl", treeControlPid, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "LinkedDetails@discreetColumnId attribute should contain a column PID. The value contained in that column will then be compared to the value specified in LinkedDetails@value attribute." + Environment.NewLine + "Such column should have its RTDisplay tag set to true.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult NonExistingId(IValidate test, IReadable referenceNode, IReadable positionNode, string columnPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDiscreetColumnIdAttribute,
                ErrorId = ErrorIds.NonExistingId,
                FullId = "18.7.5",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Attribute '{0}@{1}' references a non-existing '{2}' with {3} '{4}'.", "ExtraDetails/LinkedDetails", "discreetColumnId", "Column", "PID", columnPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "LinkedDetails@discreetColumnId attribute should contain a column PID. The value contained in that column will then be compared to the value specified in LinkedDetails@value attribute." + Environment.NewLine + "Such column should have its RTDisplay tag set to true.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult ReferencedColumnExpectingRTDisplay(IValidate test, IReadable referenceNode, IReadable positionNode, string columnPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDiscreetColumnIdAttribute,
                ErrorId = ErrorIds.ReferencedColumnExpectingRTDisplay,
                FullId = "18.7.6",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("RTDisplay(true) expected on column referenced by TreeControl 'LinkedDetails@discreetColumnId' attribute. Column PID '{0}'.", columnPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "LinkedDetails@discreetColumnId attribute should contain a column PID. The value contained in that column will then be compared to the value specified in LinkedDetails@value attribute." + Environment.NewLine + "Such column should have its RTDisplay tag set to true.",
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
        public const uint NonExistingId = 5;
        public const uint ReferencedColumnExpectingRTDisplay = 6;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckDiscreetColumnIdAttribute = 7;
    }
}
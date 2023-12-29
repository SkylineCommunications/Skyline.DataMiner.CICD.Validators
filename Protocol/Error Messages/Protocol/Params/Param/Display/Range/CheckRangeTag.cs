namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Range.CheckRangeTag
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
        public static IValidationResult MissingTag(IValidate test, IReadable referenceNode, IReadable positionNode, string paramDisplayType, string paramPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckRangeTag,
                ErrorId = ErrorIds.MissingTag,
                FullId = "2.11.1",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "Missing tag 'Display/Range' in some parameters.",
                Description = String.Format("Missing '{0}' tag for '{1}' Param with ID '{2}'.", "Display/Range", paramDisplayType, paramPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UnsupportedTag(IValidate test, IReadable referenceNode, IReadable positionNode, string paramDisplayType, string paramPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckRangeTag,
                ErrorId = ErrorIds.UnsupportedTag,
                FullId = "2.11.2",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Unsupported '{0}' tag for '{1}' Param with ID '{2}'.", "Display/Range", paramDisplayType, paramPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "For certain types of Param, a range does not make sense (ex: table param).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult EmptyTag(IValidate test, IReadable referenceNode, IReadable positionNode, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckRangeTag,
                ErrorId = ErrorIds.EmptyTag,
                FullId = "2.11.3",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty tag '{0}' in {1} '{2}'.", "Display/Range", "Param", pid),
                HowToFix = "Either add 'Range/Low' and/or 'Range/High' tag(s), either remove the empty 'Display/Range' tag.",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult LowShouldBeSmallerThanHigh(IValidate test, IReadable referenceNode, IReadable positionNode, string rangeLow, string rangeHigh, string paramId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckRangeTag,
                ErrorId = ErrorIds.LowShouldBeSmallerThanHigh,
                FullId = "2.11.4",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Range/Low '{0}' should be smaller than Range/High '{1}'. Param ID '{2}'.", rangeLow, rangeHigh, paramId),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint MissingTag = 1;
        public const uint UnsupportedTag = 2;
        public const uint EmptyTag = 3;
        public const uint LowShouldBeSmallerThanHigh = 4;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckRangeTag = 11;
    }
}
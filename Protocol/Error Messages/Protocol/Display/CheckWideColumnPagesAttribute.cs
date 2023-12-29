namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Display.CheckWideColumnPagesAttribute
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
        public static IValidationResult EmptyAttribute(IValidate test, IReadable referenceNode, IReadable positionNode)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckWideColumnPagesAttribute,
                ErrorId = ErrorIds.EmptyAttribute,
                FullId = "1.29.1",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty attribute '{0}'.", "Protocol/Display@wideColumnPage"),
                HowToFix = "",
                ExampleCode = "",
                Details = "The Protocol/Display@wideColumnPages allows to define a semicolon list of pages that should take the whole width available even if it only contains 1 column." + Environment.NewLine + "It should refer to pages that are present in the Protocol/Display@pageOrder attribute and on which at least one parameter is displayed.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UntrimmedAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string untrimmedValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckWideColumnPagesAttribute,
                ErrorId = ErrorIds.UntrimmedAttribute,
                FullId = "1.29.2",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed attribute '{0}'. Current value '{1}'.", "wideColumnsPages", untrimmedValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "The Protocol/Display@wideColumnPages allows to define a semicolon list of pages that should take the whole width available even if it only contains 1 column." + Environment.NewLine + "It should refer to pages that are present in the Protocol/Display@pageOrder attribute and on which at least one parameter is displayed.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UnexistingPage(IValidate test, IReadable referenceNode, IReadable positionNode, string pageName)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckWideColumnPagesAttribute,
                ErrorId = ErrorIds.UnexistingPage,
                FullId = "1.29.3",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("The page '{0}' specified in 'Protocol/Display@wideColumnPages' does not exist.", pageName),
                HowToFix = "",
                ExampleCode = "",
                Details = "The Protocol/Display@wideColumnPages allows to define a semicolon list of pages that should take the whole width available even if it only contains 1 column." + Environment.NewLine + "It should refer to pages that are present in the Protocol/Display@pageOrder attribute and on which at least one parameter is displayed.",
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
        public const uint UnexistingPage = 3;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckWideColumnPagesAttribute = 29;
    }
}
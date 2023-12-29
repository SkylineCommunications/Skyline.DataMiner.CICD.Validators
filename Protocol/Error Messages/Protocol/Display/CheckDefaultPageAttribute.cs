namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Display.CheckDefaultPageAttribute
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
        public static IValidationResult MissingAttribute(IValidate test, IReadable referenceNode, IReadable positionNode)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDefaultPageAttribute,
                ErrorId = ErrorIds.MissingAttribute,
                FullId = "1.21.1",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing attribute '{0}'.", "defaultPage"),
                HowToFix = "Specify a default page using the defaultPage attribute.",
                ExampleCode = "<Display defaultPage=\"General\" pageOrder=\"General;----------;Data Page 1;Data Page 2;----------;WebInterface#http://[Polling Ip]/\" />",
                Details = "Skyline recommends the following structure for driver pages:" + Environment.NewLine + "- General" + Environment.NewLine + "- -----------" + Environment.NewLine + "- Data Page(s)" + Environment.NewLine + "- -----------" + Environment.NewLine + "- WebInterface",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult EmptyAttribute(IValidate test, IReadable referenceNode, IReadable positionNode)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDefaultPageAttribute,
                ErrorId = ErrorIds.EmptyAttribute,
                FullId = "1.21.2",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty attribute '{0}'.", "defaultPage"),
                HowToFix = "Specify a default page in the defaultPage attribute.",
                ExampleCode = "<Display defaultPage=\"General\" pageOrder=\"General;----------;Data Page 1;Data Page 2;----------;WebInterface#http://[Polling Ip]/\" />",
                Details = "Skyline recommends the following structure for driver pages:" + Environment.NewLine + "- General" + Environment.NewLine + "- -----------" + Environment.NewLine + "- Data Page(s)" + Environment.NewLine + "- -----------" + Environment.NewLine + "- WebInterface",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UnexistingPage(IValidate test, IReadable referenceNode, IReadable positionNode, string pageName)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDefaultPageAttribute,
                ErrorId = ErrorIds.UnexistingPage,
                FullId = "1.21.3",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("The specified defaultPage '{0}' does not exist.", pageName),
                HowToFix = "",
                ExampleCode = "<Display defaultPage=\"General\" pageOrder=\"General;----------;Data Page 1;Data Page 2;----------;WebInterface#http://[Polling Ip]/\" />",
                Details = "Skyline recommends the following structure for driver pages:" + Environment.NewLine + "- General" + Environment.NewLine + "- -----------" + Environment.NewLine + "- Data Page(s)" + Environment.NewLine + "- -----------" + Environment.NewLine + "- WebInterface",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UntrimmedAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string attributeValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDefaultPageAttribute,
                ErrorId = ErrorIds.UntrimmedAttribute,
                FullId = "1.21.4",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed attribute '{0}'. Current value '{1}'.", "defaultPage", attributeValue),
                HowToFix = "",
                ExampleCode = "<Display defaultPage=\"General\" pageOrder=\"General;----------;Data Page 1;Data Page 2;----------;WebInterface#http://[Polling Ip]/\" />",
                Details = "Skyline recommends the following structure for driver pages:" + Environment.NewLine + "- General" + Environment.NewLine + "- -----------" + Environment.NewLine + "- Data Page(s)" + Environment.NewLine + "- -----------" + Environment.NewLine + "- WebInterface",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidDefaultPage(IValidate test, IReadable referenceNode, IReadable positionNode)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDefaultPageAttribute,
                ErrorId = ErrorIds.InvalidDefaultPage,
                FullId = "1.21.5",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "The default page should be a page with name 'General'.",
                HowToFix = "Define a page with name 'General' and specify it as the default page.",
                ExampleCode = "<Display defaultPage=\"General\" pageOrder=\"General;----------;Data Page 1;Data Page 2;----------;WebInterface#http://[Polling Ip]/\" />",
                Details = "Skyline recommends the following structure for driver pages:" + Environment.NewLine + "- General" + Environment.NewLine + "- -----------" + Environment.NewLine + "- Data Page(s)" + Environment.NewLine + "- -----------" + Environment.NewLine + "- WebInterface",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UnsupportedPage(IValidate test, IReadable referenceNode, IReadable positionNode, string pageName)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDefaultPageAttribute,
                ErrorId = ErrorIds.UnsupportedPage,
                FullId = "1.21.6",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Unsupported popup page '{0}' in defaultPage attribute.", pageName),
                HowToFix = "",
                ExampleCode = "<Display defaultPage=\"General\" pageOrder=\"General;----------;Data Page 1;Data Page 2;----------;WebInterface#http://[Polling Ip]/\" />",
                Details = "Skyline recommends the following structure for driver pages:" + Environment.NewLine + "- General" + Environment.NewLine + "- -----------" + Environment.NewLine + "- Data Page(s)" + Environment.NewLine + "- -----------" + Environment.NewLine + "- WebInterface",
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
        public const uint UnexistingPage = 3;
        public const uint UntrimmedAttribute = 4;
        public const uint InvalidDefaultPage = 5;
        public const uint UnsupportedPage = 6;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckDefaultPageAttribute = 21;
    }
}
namespace SLDisValidator2.Tests.Protocol.Params.Param.ArrayOptions.NamingFormat.CheckNamingFormatTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    internal static class Error
    {
        internal static IValidationResult EmptyTag(IValidate test, IReadable referenceNode, IReadable positionNode, string tablePid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckNamingFormatTag,
                ErrorId = ErrorIds.EmptyTag,
                FullId = "2.65.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty tag '{0}' in {1} '{2}'.", "ArrayOptions/NamingFormat", "Table", tablePid),
                HowToFix = "",
                ExampleCode = "",
                Details = "'ArrayOptions/NamingFormat' tag should start by a separator followed by a separated list of display key parts." + Environment.NewLine + "- Numeric parts will be considered as dynamic display key parts and should refer to an existing Param." + Environment.NewLine + "- Non-Numeric parts will be considered as hard-coded display key parts." + Environment.NewLine + "" + Environment.NewLine + "NamingFormat should contain, at least, 1 dynamic part.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult UntrimmedTag(IValidate test, IReadable referenceNode, IReadable positionNode, string tablePid, string untrimmedValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckNamingFormatTag,
                ErrorId = ErrorIds.UntrimmedTag,
                FullId = "2.65.2",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed tag '{0}' in {1} '{2}'. Current value '{3}'.", "ArrayOptions/NamingFormat", "Table", tablePid, untrimmedValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "'ArrayOptions/NamingFormat' tag should start by a separator followed by a separated list of display key parts." + Environment.NewLine + "- Numeric parts will be considered as dynamic display key parts and should refer to an existing Param." + Environment.NewLine + "- Non-Numeric parts will be considered as hard-coded display key parts." + Environment.NewLine + "" + Environment.NewLine + "NamingFormat should contain, at least, 1 dynamic part.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult NonExistingParam(IValidate test, IReadable referenceNode, IReadable positionNode, string referencedPid, string tablePid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckNamingFormatTag,
                ErrorId = ErrorIds.NonExistingParam,
                FullId = "2.65.3",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Tag '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "ArrayOptions/NamingFormat", "Param", "ID", referencedPid, "Table", "PID", tablePid),
                HowToFix = "",
                ExampleCode = "",
                Details = "'ArrayOptions/NamingFormat' tag should start by a separator followed by a separated list of display key parts." + Environment.NewLine + "- Numeric parts will be considered as dynamic display key parts and should refer to an existing Param." + Environment.NewLine + "- Non-Numeric parts will be considered as hard-coded display key parts." + Environment.NewLine + "" + Environment.NewLine + "NamingFormat should contain, at least, 1 dynamic part.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult MissingDynamicPart(IValidate test, IReadable referenceNode, IReadable positionNode, string tablePid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckNamingFormatTag,
                ErrorId = ErrorIds.MissingDynamicPart,
                FullId = "2.65.4",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing dynamic part(s) in 'ArrayOptions/NamingFormat' tag. Table PID '{0}'.", tablePid),
                HowToFix = "",
                ExampleCode = "",
                Details = "'ArrayOptions/NamingFormat' tag should start by a separator followed by a separated list of display key parts." + Environment.NewLine + "- Numeric parts will be considered as dynamic display key parts and should refer to an existing Param." + Environment.NewLine + "- Non-Numeric parts will be considered as hard-coded display key parts." + Environment.NewLine + "" + Environment.NewLine + "NamingFormat should contain, at least, 1 dynamic part.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint EmptyTag = 1;
        public const uint UntrimmedTag = 2;
        public const uint NonExistingParam = 3;
        public const uint MissingDynamicPart = 4;
    }

    public static class CheckId
    {
        public const uint CheckNamingFormatTag = 65;
    }
}
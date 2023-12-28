namespace SLDisValidator2.Tests.Protocol.Params.Param.Interprete.Length.CheckLengthTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    internal static class Error
    {
        internal static IValidationResult MissingTag(IValidate test, IReadable referenceNode, IReadable positionNode, string paramId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckLengthTag,
                ErrorId = ErrorIds.MissingTag,
                FullId = "2.74.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing tag '{0}' in {1} '{2}'.", "Length", "Param", paramId),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult EmptyTag(IValidate test, IReadable referenceNode, IReadable positionNode, string paramId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckLengthTag,
                ErrorId = ErrorIds.EmptyTag,
                FullId = "2.74.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty tag '{0}' in {1} '{2}'.", "Length", "Param", paramId),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult InvalidValue(IValidate test, IReadable referenceNode, IReadable positionNode, Severity severity, string tagValue, string paramId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckLengthTag,
                ErrorId = ErrorIds.InvalidValue,
                FullId = "2.74.3",
                Category = Category.Param,
                Severity = severity,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{0}' in tag '{1}'. {2} {4} '{3}'.", tagValue, "Length", "Param", paramId, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "The 'Length' tag needs to be an unsigned integer." + Environment.NewLine + "When the parameter is of type 'length', the 'Length' tag can not be larger than 4.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint MissingTag = 1;
        public const uint EmptyTag = 2;
        public const uint InvalidValue = 3;
    }

    public static class CheckId
    {
        public const uint CheckLengthTag = 74;
    }
}
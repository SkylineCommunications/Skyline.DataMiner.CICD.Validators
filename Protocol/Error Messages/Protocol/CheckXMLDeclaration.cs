namespace SLDisValidator2.Tests.Protocol.CheckXMLDeclaration
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    internal static class Error
    {
        internal static IValidationResult InvalidDeclaration(IValidate test, IReadable referenceNode, IReadable positionNode, string currentEncoding, string possibleValues)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckXMLDeclaration,
                ErrorId = ErrorIds.InvalidDeclaration,
                FullId = "1.18.1",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid XML encoding '{0}'. Possible values '{1}'.", currentEncoding, possibleValues),
                HowToFix = "Remove the XML declaration if not set to UTF-8.",
                ExampleCode = "",
                Details = "",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint InvalidDeclaration = 1;
    }

    public static class CheckId
    {
        public const uint CheckXMLDeclaration = 18;
    }
}
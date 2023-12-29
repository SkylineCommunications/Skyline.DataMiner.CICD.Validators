namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckXMLDeclaration
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
        public static IValidationResult InvalidDeclaration(IValidate test, IReadable referenceNode, IReadable positionNode, string currentEncoding, string possibleValues)
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

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckXMLDeclaration = 18;
    }
}
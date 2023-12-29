namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Chains.CheckChildNameAttributes
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal static class Error
    {
        internal static IValidationResult DuplicatedValue(IValidate test, IReadable referenceNode, IReadable positionNode, string chainName)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckChildNameAttributes,
                ErrorId = ErrorIds.DuplicatedValue,
                FullId = "15.1.1",
                Category = Category.Chain,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Duplicated {0} Name '{1}'.", "Chain child", chainName),
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
        public const uint DuplicatedValue = 1;
    }

    public static class CheckId
    {
        public const uint CheckChildNameAttributes = 1;
    }
}
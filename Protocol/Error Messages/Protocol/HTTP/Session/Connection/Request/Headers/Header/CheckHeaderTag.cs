namespace SLDisValidator2.Tests.Protocol.HTTP.Session.Connection.Request.Headers.Header.CheckHeaderTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    public static class Error
    {
        public static IValidationResult UntrimmedTag(IValidate test, IReadable referenceNode, IReadable positionNode, string tagValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckHeaderTag,
                ErrorId = ErrorIds.UntrimmedTag,
                FullId = "8.2.1",
                Category = Category.HTTP,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed tag '{0}'. Current value '{1}'.", "Header", tagValue),
                HowToFix = "Trim the Header tag value.",
                ExampleCode = "",
                Details = "",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    public static class ErrorIds
    {
        public const uint UntrimmedTag = 1;
    }

    public static class CheckId
    {
        public const uint CheckHeaderTag = 2;
    }
}
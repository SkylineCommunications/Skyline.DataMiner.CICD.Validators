namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Display.CheckDisplayTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal static class Error
    {
        internal static IValidationResult MissingTag(IValidate test, IReadable referenceNode, IReadable positionNode)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDisplayTag,
                ErrorId = ErrorIds.MissingTag,
                FullId = "1.28.1",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing tag '{0}'.", "Protocol/Display"),
                HowToFix = "",
                ExampleCode = "",
                Details = "The Protocol/Display tag is mandatory in order to define how pages should be displayed (default page, page order, etc).",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint MissingTag = 1;
    }

    public static class CheckId
    {
        public const uint CheckDisplayTag = 28;
    }
}
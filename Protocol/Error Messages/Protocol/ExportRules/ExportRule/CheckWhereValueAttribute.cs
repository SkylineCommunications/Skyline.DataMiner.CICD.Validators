namespace SLDisValidator2.Tests.Protocol.ExportRules.ExportRule.CheckWhereValueAttribute
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    public static class Error
    {
        public static IValidationResult MissingAttribute(IValidate test, IReadable referenceNode, IReadable positionNode)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckWhereValueAttribute,
                ErrorId = ErrorIds.MissingAttribute,
                FullId = "17.2.1",
                Category = Category.ExportRule,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing attribute '{0}'.", "ExportRule@whereValue"),
                HowToFix = "",
                ExampleCode = "",
                Details = "As soon as whereTag or whereAttribute is being used, the whereValue is needed.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    public static class ErrorIds
    {
        public const uint MissingAttribute = 1;
    }

    public static class CheckId
    {
        public const uint CheckWhereValueAttribute = 2;
    }
}
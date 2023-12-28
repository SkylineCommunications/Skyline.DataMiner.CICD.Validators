namespace SLDisValidator2.Tests.Protocol.Params.Param.ArrayOptions.CheckPartialAttribute
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;


    internal static class ErrorCompare
    {
        internal static IValidationResult EnabledPartial(IReadable referenceNode, IReadable positionNode, string paramId)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckPartialAttribute,
                ErrorId = ErrorIds.EnabledPartial,
                FullId = "2.26.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Partial Table option was enabled on table '{0}'.", paramId),
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
        public const uint EnabledPartial = 1;
    }

    public static class CheckId
    {
        public const uint CheckPartialAttribute = 26;
    }
}
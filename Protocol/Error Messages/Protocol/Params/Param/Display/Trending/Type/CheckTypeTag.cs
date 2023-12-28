namespace SLDisValidator2.Tests.Protocol.Params.Param.Display.Trending.Type.CheckTypeTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;

    public static class ErrorCompare
    {
        public static IValidationResult UpdatedTrendType(IReadable referenceNode, IReadable positionNode, string oldTrendType, string paramId, string newTrendType)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckTypeTag,
                ErrorId = ErrorIds.UpdatedTrendType,
                FullId = "2.30.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Trend Type '{0}' on Param '{1}' was changed into '{2}'.", oldTrendType, paramId, newTrendType),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    public static class ErrorIds
    {
        public const uint UpdatedTrendType = 1;
    }

    public static class CheckId
    {
        public const uint CheckTypeTag = 30;
    }
}
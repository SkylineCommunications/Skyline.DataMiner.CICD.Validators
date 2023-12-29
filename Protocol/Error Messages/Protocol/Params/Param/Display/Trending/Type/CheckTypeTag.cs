namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Trending.Type.CheckTypeTag
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;


    internal static class ErrorCompare
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

    internal static class ErrorIds
    {
        public const uint UpdatedTrendType = 1;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckTypeTag = 30;
    }
}
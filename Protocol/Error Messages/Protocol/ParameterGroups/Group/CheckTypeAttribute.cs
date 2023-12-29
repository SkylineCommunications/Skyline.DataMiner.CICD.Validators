namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ParameterGroups.Group.CheckTypeAttribute
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
        public static IValidationResult DcfParameterGroupTypeChanged(IReadable referenceNode, IReadable positionNode, string groupId, string oldType, string newType)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckTypeAttribute,
                ErrorId = ErrorIds.DcfParameterGroupTypeChanged,
                FullId = "16.4.1",
                Category = Category.ParameterGroup,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("DCF Group type for ParameterGroup '{0}' was changed from '{1}' into '{2}'.", groupId, oldType, newType),
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
        public const uint DcfParameterGroupTypeChanged = 1;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckTypeAttribute = 4;
    }
}
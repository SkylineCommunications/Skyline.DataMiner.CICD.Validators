namespace SLDisValidator2.Tests.Protocol.ParameterGroups.CheckParameterGroupsTag
{
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;

    public static class ErrorCompare
    {
        public static IValidationResult DcfAdded(IReadable referenceNode, IReadable positionNode)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckParameterGroupsTag,
                ErrorId = ErrorIds.DcfAdded,
                FullId = "16.2.1",
                Category = Category.ParameterGroup,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = "DCF was added.",
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
        public const uint DcfAdded = 1;
    }

    public static class CheckId
    {
        public const uint CheckParameterGroupsTag = 2;
    }
}
namespace SLDisValidator2.Tests.Protocol.ParameterGroups.Group.CheckDynamicIndexAttribute
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    public static class Error
    {
        public static IValidationResult MissingDynamicIdAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string parameterGroupId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckDynamicIndexAttribute,
                ErrorId = ErrorIds.MissingDynamicIdAttribute,
                FullId = "16.8.1",
                Category = Category.ParameterGroup,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Filtering via 'Group@dynamicIndex' attribute requires a 'Group@dynamicId' attribute. ParameterGroup ID '{0}'.", parameterGroupId),
                HowToFix = "",
                ExampleCode = "",
                Details = "'Group@dynamicIndex' attribute allows to filter on Display Keys before creating dynamic DCF interfaces." + Environment.NewLine + "Such filter is applied on the table referred to via the 'Group@dynamicId' attribute." + Environment.NewLine + "This means that the presence of a 'Group@dynamicIndex' attribute while there is no 'Group@dynamicIndex' doesn't make sense.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    public static class ErrorIds
    {
        public const uint MissingDynamicIdAttribute = 1;
    }

    public static class CheckId
    {
        public const uint CheckDynamicIndexAttribute = 8;
    }
}
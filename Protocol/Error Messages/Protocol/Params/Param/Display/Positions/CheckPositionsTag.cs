namespace SLDisValidator2.Tests.Protocol.Params.Param.Display.Positions.CheckPositionsTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    public static class Error
    {
        public static IValidationResult EmptyTag(IValidate test, IReadable referenceNode, IReadable positionNode, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckPositionsTag,
                ErrorId = ErrorIds.EmptyTag,
                FullId = "2.57.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing tag '{0}' in {1} '{2}'.", "Display/Positions", "Param", pid),
                HowToFix = "",
                ExampleCode = "",
                Details = "A 'Display/Positions' should always have, at least, one Position tag.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult RTDisplayExpected(IValidate test, IReadable referenceNode, IReadable positionNode, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckPositionsTag,
                ErrorId = ErrorIds.RTDisplayExpected,
                FullId = "2.57.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("RTDisplay(true) expected on Param '{0}' which is positioned.", pid),
                HowToFix = "",
                ExampleCode = "",
                Details = "A positioned Param (with Display/Positions tag) requires its RTDisplay to be set to true in order to be properly displayed." + Environment.NewLine + "Note that an exception to this rule can be made when a Param is only meant to be displayed on a DVE protocol and not on the main one.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    public static class ErrorIds
    {
        public const uint EmptyTag = 1;
        public const uint RTDisplayExpected = 2;
    }

    public static class CheckId
    {
        public const uint CheckPositionsTag = 57;
    }
}
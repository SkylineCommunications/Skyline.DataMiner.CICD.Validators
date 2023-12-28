namespace SLDisValidator2.Tests.Protocol.Params.Param.Information.Includes.CheckIncludesTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    internal static class Error
    {
        internal static IValidationResult ObsoleteTag(IValidate test, IReadable referenceNode, IReadable positionNode, string pid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckIncludesTag,
                ErrorId = ErrorIds.ObsoleteTag,
                FullId = "2.66.1",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Obsolete tag '{0}'. {1} {2} '{3}'.", "Information/Includes", "Param", "ID", pid),
                HowToFix = "",
                ExampleCode = "",
                Details = "'Information/Includes' tag was only used in the past by SystemDisplay. Today, it is considered obsolete.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint ObsoleteTag = 1;
    }

    public static class CheckId
    {
        public const uint CheckIncludesTag = 66;
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Type.CheckTypeTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Skyline.DataMiner.CICD.Validators.Protocol.Common;


    internal static class ErrorCompare
    {
        internal static IValidationResult UpdatedValue(IReadable referenceNode, IReadable positionNode, string paramId, string oldTypeValue, string newTypeValue)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckTypeTag,
                ErrorId = ErrorIds.UpdatedValue,
                FullId = "2.20.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Interprete type on Param '{0}' has been changed from '{1}' to '{2}'.", paramId, oldTypeValue, newTypeValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult RemovedTag(IReadable referenceNode, IReadable positionNode, string paramId)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckTypeTag,
                ErrorId = ErrorIds.RemovedTag,
                FullId = "2.20.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Interprete type on Param '{0}' has been removed.", paramId),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult AddedTag(IReadable referenceNode, IReadable positionNode, string typeValue, string paramId)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckTypeTag,
                ErrorId = ErrorIds.AddedTag,
                FullId = "2.20.3",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("New Type tag '{0}' for interprete in Param '{1}' was added.", typeValue, paramId),
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
        public const uint UpdatedValue = 1;
        public const uint RemovedTag = 2;
        public const uint AddedTag = 3;
    }

    public static class CheckId
    {
        public const uint CheckTypeTag = 20;
    }
}
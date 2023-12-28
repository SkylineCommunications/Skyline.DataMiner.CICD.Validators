namespace SLDisValidator2.Tests.Protocol.Params.Param.Interprete.Others.CheckOthersTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;


    internal static class ErrorCompare
    {
        internal static IValidationResult UpdateOtherId(IReadable referenceNode, IReadable positionNode, string oldId, string newId, string valueTag, string paramPid)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckOthersTag,
                ErrorId = ErrorIds.UpdateOtherId,
                FullId = "2.45.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Uncertain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Id attribute '{0}' has been changed to '{1}' for Other tag with Value tag '{2}'. Param ID '{3}'.", oldId, newId, valueTag, paramPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult UpdateOtherDisplay(IReadable referenceNode, IReadable positionNode, string oldDisplayTag, string newDisplayTag, string valueTag, string paramPid)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckOthersTag,
                ErrorId = ErrorIds.UpdateOtherDisplay,
                FullId = "2.45.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Uncertain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Display tag '{0}' has been changed to '{1}' for Other tag with Value tag '{2}'. Param ID '{3}'.", oldDisplayTag, newDisplayTag, valueTag, paramPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult DeletedValue(IReadable referenceNode, IReadable positionNode, string oldValue, string paramPid)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckOthersTag,
                ErrorId = ErrorIds.DeletedValue,
                FullId = "2.45.3",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Other with Value tag '{0}' has been deleted. Param '{1}'.", oldValue, paramPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "If a value is removed from the Other tags, it will have impact on an alarm template for which that value was configured as a threshold value.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult AddedOthers(IReadable referenceNode, IReadable positionNode, string newValue, string paramPid)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckOthersTag,
                ErrorId = ErrorIds.AddedOthers,
                FullId = "2.45.4",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Other with Value tag '{0}' has been added. Param '{1}'.", newValue, paramPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "If a new value is added to the Other tags and that value is withing the previously allowed range of values for that parameter, it might have an impact on configured alarm templates.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint UpdateOtherId = 1;
        public const uint UpdateOtherDisplay = 2;
        public const uint DeletedValue = 3;
        public const uint AddedOthers = 4;
    }

    public static class CheckId
    {
        public const uint CheckOthersTag = 45;
    }
}
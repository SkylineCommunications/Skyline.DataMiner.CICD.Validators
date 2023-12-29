namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckArrayOptionsTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Skyline.DataMiner.CICD.Validators.Protocol.Common;


    internal static class ErrorCompare
    {
        internal static IValidationResult DisplayColumnChangedToNaming(IReadable referenceNode, IReadable positionNode, string columnIdx, string tableId, string namingValue)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckArrayOptionsTag,
                ErrorId = ErrorIds.DisplayColumnChangedToNaming,
                FullId = "2.15.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("DisplayColumn attribute with column idx '{0}' on table '{1}' was changed into naming options: '{2}'.", columnIdx, tableId, namingValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult DisplayColumnChangeToNamingFormat(IReadable referenceNode, IReadable positionNode, string columnIdx, string tableId, string namingFormatValue)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckArrayOptionsTag,
                ErrorId = ErrorIds.DisplayColumnChangeToNamingFormat,
                FullId = "2.15.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("DisplayColumn attribute with column idx '{0}' on table '{1}' was changed into NamingFormat: '{2}'.", columnIdx, tableId, namingFormatValue),
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
        public const uint DisplayColumnChangedToNaming = 1;
        public const uint DisplayColumnChangeToNamingFormat = 2;
    }

    public static class CheckId
    {
        public const uint CheckArrayOptionsTag = 15;
    }
}
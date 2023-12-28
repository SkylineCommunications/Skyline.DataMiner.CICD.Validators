namespace SLDisValidator2.Tests.Protocol.Params.Param.Database.ColumnDefinition.CheckColumnDefinitionTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;

    internal static class ErrorCompare
    {
        internal static IValidationResult ChangedLoggerDataType(IReadable referenceNode, IReadable positionNode, string oldType, string tablePid, string newType)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckColumnDefinitionTag,
                ErrorId = ErrorIds.ChangedLoggerDataType,
                FullId = "2.28.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Database type '{0}' for columns on table '{1}' was changed into '{2}'.", oldType, tablePid, newType),
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
        public const uint ChangedLoggerDataType = 1;
    }

    public static class CheckId
    {
        public const uint CheckColumnDefinitionTag = 28;
    }
}
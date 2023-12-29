namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckColumnOptionTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Skyline.DataMiner.CICD.Validators.Protocol.Common;


    internal static class ErrorCompare
    {
        internal static IValidationResult RemovedColumnOptionTag(IReadable referenceNode, IReadable positionNode, string columnPid, string tablePid)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckColumnOptionTag,
                ErrorId = ErrorIds.RemovedColumnOptionTag,
                FullId = "2.35.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Column with PID '{0}' was removed from table '{1}'.", columnPid, tablePid),
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
        public const uint RemovedColumnOptionTag = 1;
    }

    public static class CheckId
    {
        public const uint CheckColumnOptionTag = 35;
    }
}
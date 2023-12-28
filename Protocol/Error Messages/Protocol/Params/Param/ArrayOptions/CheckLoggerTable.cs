namespace SLDisValidator2.Tests.Protocol.Params.Param.ArrayOptions.CheckLoggerTable
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;

    public static class ErrorCompare
    {
        public static IValidationResult RemovedLoggerColumn(IReadable referenceNode, IReadable positionNode, string columnPid, string tablePid)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckLoggerTable,
                ErrorId = ErrorIds.RemovedLoggerColumn,
                FullId = "2.27.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Column with PID '{0}' was removed from logger table '{1}'.", columnPid, tablePid),
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
        public const uint RemovedLoggerColumn = 1;
    }

    public static class CheckId
    {
        public const uint CheckLoggerTable = 27;
    }
}
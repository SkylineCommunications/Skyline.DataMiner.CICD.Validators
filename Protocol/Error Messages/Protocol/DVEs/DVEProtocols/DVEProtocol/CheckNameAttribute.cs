namespace SLDisValidator2.Tests.Protocol.DVEs.DVEProtocols.DVEProtocol.CheckNameAttribute
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;

    public static class ErrorCompare
    {
        public static IValidationResult UpdatedValue(IReadable referenceNode, IReadable positionNode, string dveProtocolName, string tableId, string newDveProtocolName)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckNameAttribute,
                ErrorId = ErrorIds.UpdatedValue,
                FullId = "1.10.1",
                Category = Category.Protocol,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("DVE Protocol with Name '{0}' for Table '{1}' was changed into '{2}'.", dveProtocolName, tableId, newDveProtocolName),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult RemovedItem(IReadable referenceNode, IReadable positionNode, string dveProtocolName, string tableId)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckNameAttribute,
                ErrorId = ErrorIds.RemovedItem,
                FullId = "1.10.2",
                Category = Category.Protocol,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("DVE Protocol with Name '{0}' for Table '{1}' was removed.", dveProtocolName, tableId),
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
        public const uint UpdatedValue = 1;
        public const uint RemovedItem = 2;
    }

    public static class CheckId
    {
        public const uint CheckNameAttribute = 10;
    }
}
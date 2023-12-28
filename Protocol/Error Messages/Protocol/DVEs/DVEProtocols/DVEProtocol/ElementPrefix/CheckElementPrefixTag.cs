namespace SLDisValidator2.Tests.Protocol.DVEs.DVEProtocols.DVEProtocol.ElementPrefix.CheckElementPrefixTag
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;

    internal static class ErrorCompare
    {
        internal static IValidationResult AddedElementPrefix(IReadable referenceNode, IReadable positionNode, string dveProtocolName, string tableId)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckElementPrefixTag,
                ErrorId = ErrorIds.AddedElementPrefix,
                FullId = "1.16.1",
                Category = Category.Protocol,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("ElementPrefix was added to DVE Protocol with Name '{0}' for Table '{1}'.", dveProtocolName, tableId),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult RemovedElementPrefix(IReadable referenceNode, IReadable positionNode, string dveProtocolName, string tableId)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckElementPrefixTag,
                ErrorId = ErrorIds.RemovedElementPrefix,
                FullId = "1.16.2",
                Category = Category.Protocol,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("ElementPrefix was removed from DVE Protocol with Name '{0}' for Table '{1}'.", dveProtocolName, tableId),
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
        public const uint AddedElementPrefix = 1;
        public const uint RemovedElementPrefix = 2;
    }

    public static class CheckId
    {
        public const uint CheckElementPrefixTag = 16;
    }
}
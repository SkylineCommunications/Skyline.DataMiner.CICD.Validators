namespace SLDisValidator2.Tests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckIdxAttribute
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;


    internal static class ErrorCompare
    {
        internal static IValidationResult UpdatedIdxValue(IReadable referenceNode, IReadable positionNode, string columnPid, string oldSLProtocolPosition, string newSLProtocolPosition, string tablePid)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckIdxAttribute,
                ErrorId = ErrorIds.UpdatedIdxValue,
                FullId = "2.25.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Column with PID '{0}' had its SLProtocol position changed from '{1}' into '{2}'. Table PID '{3}'.", columnPid, oldSLProtocolPosition, newSLProtocolPosition, tablePid),
                HowToFix = "",
                ExampleCode = "",
                Details = "The SLProtocol position is based on the idx of the columns and will typically match with it." + Environment.NewLine + "However, note that columns with type=\"displaykey\" are not known to SLProtocol." + Environment.NewLine + "This means that even though the SLProtocol position is based on idx value, it will not alway match with it.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult UpdatedIdxValue_Parent(IReadable referenceNode, IReadable positionNode, string tablePid)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckIdxAttribute,
                ErrorId = ErrorIds.UpdatedIdxValue_Parent,
                FullId = "2.25.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Some columns have their SLProtocol position changed. Table PID '{0}'.", tablePid),
                HowToFix = "",
                ExampleCode = "",
                Details = "The SLProtocol position is based on the idx of the columns and will typically match with it." + Environment.NewLine + "However, note that columns with type=\"displaykey\" are not known to SLProtocol." + Environment.NewLine + "This means that even though the SLProtocol position is based on idx value, it will not alway match with it.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint UpdatedIdxValue = 1;
        public const uint UpdatedIdxValue_Parent = 2;
    }

    public static class CheckId
    {
        public const uint CheckIdxAttribute = 25;
    }
}
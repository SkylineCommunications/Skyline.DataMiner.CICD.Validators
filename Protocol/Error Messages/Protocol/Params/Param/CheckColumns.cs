namespace SLDisValidator2.Tests.Protocol.Params.Param.CheckColumns
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    internal static class Error
    {
        internal static IValidationResult ColumnInvalidType(IValidate test, IReadable referenceNode, IReadable positionNode, string columnType, string columnPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckColumns,
                ErrorId = ErrorIds.ColumnInvalidType,
                FullId = "2.64.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{0}' in tag '{1}' for {2}. Possible values '{3}'. {4} {5} '{6}'.", columnType, "Param/Type", "column", "read, write, group, read bit, write bit", "Column", "PID", columnPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "A column should be of Param/Type 'read', 'write', 'group', 'read bit' or 'write bit'.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint ColumnInvalidType = 1;
    }

    public static class CheckId
    {
        public const uint CheckColumns = 64;
    }
}
namespace SLDisValidator2.Tests.Protocol.Params.Param.Name.CheckColumnNames
{
    using System;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    internal static class Error
    {
        internal static IValidationResult MissingTableNameAsPrefixes(IValidate test, IReadable referenceNode, IReadable positionNode, string tableName, string tablePid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckColumnNames,
                ErrorId = ErrorIds.MissingTableNameAsPrefixes,
                FullId = "2.53.1",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing table name '{0}' in front of column names. Table PID '{1}'.", tableName, tablePid),
                HowToFix = "",
                ExampleCode = "",
                Details = "The name of column parameters should start with the name of the table they belong to.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        internal static IValidationResult MissingTableNameAsPrefix(IValidate test, IReadable referenceNode, IReadable positionNode, string tableName, string columnName, string columnPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckColumnNames,
                ErrorId = ErrorIds.MissingTableNameAsPrefix,
                FullId = "2.53.2",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Missing table name '{0}' in front of column name '{1}'. Column PID '{2}'.", tableName, columnName, columnPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "The name of column parameters should start with the name of the table they belong to.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint MissingTableNameAsPrefixes = 1;
        public const uint MissingTableNameAsPrefix = 2;
    }

    public static class CheckId
    {
        public const uint CheckColumnNames = 53;
    }
}
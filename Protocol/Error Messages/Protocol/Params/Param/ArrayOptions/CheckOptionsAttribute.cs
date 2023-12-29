namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckOptionsAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    internal static class Error
    {
        public static IValidationResult EmptyAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string tablePid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.EmptyAttribute,
                FullId = "2.17.1",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty attribute '{0}' in {1} '{2}'.", "ArrayOptions@options", "Param", tablePid),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UntrimmedAttribute(IValidate test, IReadable referenceNode, IReadable positionNode, string tablePid, string untrimmedValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.UntrimmedAttribute,
                FullId = "2.17.2",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed attribute '{0}' in {1} '{2}'. Current value '{3}'.", "ArrayOptions@options", "Table", tablePid, untrimmedValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult NamingEmpty(IValidate test, IReadable referenceNode, IReadable positionNode, string tablePid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.NamingEmpty,
                FullId = "2.17.3",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty option '{0}' in attribute '{1}'. {2} {3} '{4}'.", "naming", "ArrayOptions@options", "Table", "PID", tablePid),
                HowToFix = "",
                ExampleCode = "",
                Details = "The naming option in the ArrayOptions@options attribute should contain a list of PIDs referencing existing parameters.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult NamingRefersToNonExistingParam(IValidate test, IReadable referenceNode, IReadable positionNode, string referencedPid, string tablePid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.NamingRefersToNonExistingParam,
                FullId = "2.17.4",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Option '{0}' in attribute '{1}' references a non-existing '{2}' with {3} '{4}'. {5} {6} '{7}'.", "naming", "ArrayOptions@options", "Param", "ID", referencedPid, "Table", "PID", tablePid),
                HowToFix = "",
                ExampleCode = "",
                Details = "The naming option in the ArrayOptions@options attribute should contain a list of PIDs referencing existing parameters.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult PreserveStateShouldBeAvoided(IValidate test, IReadable referenceNode, IReadable positionNode, string tablePid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.PreserveStateShouldBeAvoided,
                FullId = "2.17.6",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Unrecommended use of the \"preserve state\" option on table '{0}'.", tablePid),
                HowToFix = "Use a QAction to compare the previous and new values of the cells, and calculate the state of each row.",
                ExampleCode = "",
                Details = "The use of the \"preserve state\" option on tables should be avoided as it requires sig­nificantly more processing.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult ViewTableInvalidReference(IValidate test, IReadable referenceNode, IReadable positionNode, Severity severity, string viewOption, string viewTablePid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.ViewTableInvalidReference,
                FullId = "2.17.7",
                Category = Category.Param,
                Severity = severity,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Table view option '{0}' must refer to an existing table excluding the view table itself. View table PID '{1}'.", viewOption, viewTablePid),
                HowToFix = "Make sure the view table refers to an existing table, excluding view tables.",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult ViewTableFilterChangeInvalidColumns(IValidate test, IReadable referenceNode, IReadable positionNode, string columnPid, string viewTablePid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.ViewTableFilterChangeInvalidColumns,
                FullId = "2.17.8",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Column '{0}' specified in the filterChange option must refer to a column of the view table '{1}'.", columnPid, viewTablePid),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult ViewTableDirectViewInvalidColumn(IValidate test, IReadable referenceNode, IReadable positionNode, string columnPid, string viewTablePid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.ViewTableDirectViewInvalidColumn,
                FullId = "2.17.9",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Column '{0}' specified in the directView option of view table '{1}' must refer to a column of another table.", columnPid, viewTablePid),
                HowToFix = "",
                ExampleCode = "",
                Details = "",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorCompare
    {
        public static IValidationResult RemovedLoggerTableDatabaseLink(IReadable referenceNode, IReadable positionNode, string paramPid)
        {
            return new ValidationResult
            {
                Test = null,
                CheckId = CheckId.CheckOptionsAttribute,
                ErrorId = ErrorIds.RemovedLoggerTableDatabaseLink,
                FullId = "2.17.5",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.MajorChangeChecker,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = String.Format("Database link for logger table '{0}' was removed.", paramPid),
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
        public const uint EmptyAttribute = 1;
        public const uint UntrimmedAttribute = 2;
        public const uint NamingEmpty = 3;
        public const uint NamingRefersToNonExistingParam = 4;
        public const uint RemovedLoggerTableDatabaseLink = 5;
        public const uint PreserveStateShouldBeAvoided = 6;
        public const uint ViewTableInvalidReference = 7;
        public const uint ViewTableFilterChangeInvalidColumns = 8;
        public const uint ViewTableDirectViewInvalidColumn = 9;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckOptionsAttribute = 17;
    }
}
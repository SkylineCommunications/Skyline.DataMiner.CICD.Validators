namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.OverrideIconColumns.CheckOverrideIconColumnsTag
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
        public static IValidationResult EmptyTag(IValidate test, IReadable referenceNode, IReadable positionNode, string treeControlPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOverrideIconColumnsTag,
                ErrorId = ErrorIds.EmptyTag,
                FullId = "18.12.1",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Empty tag '{0}' in {1} '{2}'.", "OverrideIconColumns", "TreeControl", treeControlPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "'TreeControl/OverrideIconColumns' tag should contain a comma separated list of column PIDs that should be used to define the icons in the TreeControl structure." + Environment.NewLine + "The column PIDs should belong to one of the tables in the TreeControl Hierarchy and only one column per table should be used.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UntrimmedTag(IValidate test, IReadable referenceNode, IReadable positionNode, string treeControlPid, string untrimmedValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOverrideIconColumnsTag,
                ErrorId = ErrorIds.UntrimmedTag,
                FullId = "18.12.2",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed tag '{0}' in {1} '{2}'. Current value '{3}'.", "OverrideIconColumns", "TreeControl", treeControlPid, untrimmedValue),
                HowToFix = "",
                ExampleCode = "",
                Details = "'TreeControl/OverrideIconColumns' tag should contain a comma separated list of column PIDs that should be used to define the icons in the TreeControl structure." + Environment.NewLine + "The column PIDs should belong to one of the tables in the TreeControl Hierarchy and only one column per table should be used.",
                HasCodeFix = true,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidValue(IValidate test, IReadable referenceNode, IReadable positionNode, string overrideIconColumnsValue, string treeControlPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOverrideIconColumnsTag,
                ErrorId = ErrorIds.InvalidValue,
                FullId = "18.12.3",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{0}' in tag '{1}'. {2} {4} '{3}'.", overrideIconColumnsValue, "OverrideIconColumns", "TreeControl", treeControlPid, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "'TreeControl/OverrideIconColumns' tag should contain a comma separated list of column PIDs that should be used to define the icons in the TreeControl structure." + Environment.NewLine + "The column PIDs should belong to one of the tables in the TreeControl Hierarchy and only one column per table should be used.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult NonExistingIds(IValidate test, IReadable referenceNode, IReadable positionNode, string treeControlPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOverrideIconColumnsTag,
                ErrorId = ErrorIds.NonExistingIds,
                FullId = "18.12.4",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Tag '{0}' references non-existing IDs. {1} {3} '{2}'.", "OverrideIconColumns", "TreeControl", treeControlPid, "ID"),
                HowToFix = "",
                ExampleCode = "",
                Details = "'TreeControl/OverrideIconColumns' tag should contain a comma separated list of column PIDs that should be used to define the icons in the TreeControl structure." + Environment.NewLine + "The column PIDs should belong to one of the tables in the TreeControl Hierarchy and only one column per table should be used.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult NonExistingIds_Sub(IValidate test, IReadable referenceNode, IReadable positionNode, string columnPid, string treeControlPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOverrideIconColumnsTag,
                ErrorId = ErrorIds.NonExistingIds_Sub,
                FullId = "18.12.5",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Tag '{0}' references a non-existing '{1}' with {2} '{3}'. {4} {5} '{6}'.", "OverrideIconColumns", "Column", "PID", columnPid, "TreeControl", "ID", treeControlPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "'TreeControl/OverrideIconColumns' tag should contain a comma separated list of column PIDs that should be used to define the icons in the TreeControl structure." + Environment.NewLine + "The column PIDs should belong to one of the tables in the TreeControl Hierarchy and only one column per table should be used.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult DuplicateId(IValidate test, IReadable referenceNode, IReadable positionNode, string duplicateId, string treeControlId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOverrideIconColumnsTag,
                ErrorId = ErrorIds.DuplicateId,
                FullId = "18.12.6",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Duplicate value '{0}' in tag '{1}'. {2} {3} '{4}'.", duplicateId, "OverrideIconColumns", "TreeControl", "ID", treeControlId),
                HowToFix = "",
                ExampleCode = "",
                Details = "'TreeControl/OverrideIconColumns' tag should contain a comma separated list of column PIDs that should be used to define the icons in the TreeControl structure." + Environment.NewLine + "The column PIDs should belong to one of the tables in the TreeControl Hierarchy and only one column per table should be used.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult DuplicateOverrideIconColumns(IValidate test, IReadable referenceNode, IReadable positionNode, string tablePid, string treeControlPid)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOverrideIconColumnsTag,
                ErrorId = ErrorIds.DuplicateOverrideIconColumns,
                FullId = "18.12.7",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Duplicate OverrideIconColumns IDs for Table '{0}'. TreeControl ID '{1}'.", tablePid, treeControlPid),
                HowToFix = "",
                ExampleCode = "",
                Details = "'TreeControl/OverrideIconColumns' tag should contain a comma separated list of column PIDs that should be used to define the icons in the TreeControl structure." + Environment.NewLine + "The column PIDs should belong to one of the tables in the TreeControl Hierarchy and only one column per table should be used.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult UntrimmedValueInTag_Sub(IValidate test, IReadable referenceNode, IReadable positionNode, string untrimmedValue)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOverrideIconColumnsTag,
                ErrorId = ErrorIds.UntrimmedValueInTag_Sub,
                FullId = "18.12.8",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Untrimmed value '{0}' in tag '{1}'.", untrimmedValue, "OverrideIconColumns"),
                HowToFix = "",
                ExampleCode = "",
                Details = "'TreeControl/OverrideIconColumns' tag should contain a comma separated list of column PIDs that should be used to define the icons in the TreeControl structure." + Environment.NewLine + "The column PIDs should belong to one of the tables in the TreeControl Hierarchy and only one column per table should be used.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult InvalidValueInTag_Sub(IValidate test, IReadable referenceNode, IReadable positionNode, string invalidPart)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOverrideIconColumnsTag,
                ErrorId = ErrorIds.InvalidValueInTag_Sub,
                FullId = "18.12.9",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Invalid value '{1}' in tag '{0}'.", "OverrideIconColumns", invalidPart),
                HowToFix = "",
                ExampleCode = "",
                Details = "'TreeControl/OverrideIconColumns' tag should contain a comma separated list of column PIDs that should be used to define the icons in the TreeControl structure." + Environment.NewLine + "The column PIDs should belong to one of the tables in the TreeControl Hierarchy and only one column per table should be used.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult DuplicateOverrideIconColumns_Sub(IValidate test, IReadable referenceNode, IReadable positionNode, string duplicateId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOverrideIconColumnsTag,
                ErrorId = ErrorIds.DuplicateOverrideIconColumns_Sub,
                FullId = "18.12.10",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Duplicate OverrideIconColumns ID '{0}'.", duplicateId),
                HowToFix = "",
                ExampleCode = "",
                Details = "'TreeControl/OverrideIconColumns' tag should contain a comma separated list of column PIDs that should be used to define the icons in the TreeControl structure." + Environment.NewLine + "The column PIDs should belong to one of the tables in the TreeControl Hierarchy and only one column per table should be used.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }

        public static IValidationResult IrrelevantColumn(IValidate test, IReadable referenceNode, IReadable positionNode, string columnPid, string treeControlId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CheckOverrideIconColumnsTag,
                ErrorId = ErrorIds.IrrelevantColumn,
                FullId = "18.12.11",
                Category = Category.TreeControl,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Irrelevant column with PID '{0}' in 'TreeControl/OverrideIconColumns'. TreeControl ID '{1}'.", columnPid, treeControlId),
                HowToFix = "",
                ExampleCode = "",
                Details = "'TreeControl/OverrideIconColumns' tag should contain a comma separated list of column PIDs that should be used to define the icons in the TreeControl structure." + Environment.NewLine + "The column PIDs should belong to one of the tables in the TreeControl Hierarchy and only one column per table should be used.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint EmptyTag = 1;
        public const uint UntrimmedTag = 2;
        public const uint InvalidValue = 3;
        public const uint NonExistingIds = 4;
        public const uint NonExistingIds_Sub = 5;
        public const uint DuplicateId = 6;
        public const uint DuplicateOverrideIconColumns = 7;
        public const uint UntrimmedValueInTag_Sub = 8;
        public const uint InvalidValueInTag_Sub = 9;
        public const uint DuplicateOverrideIconColumns_Sub = 10;
        public const uint IrrelevantColumn = 11;
    }

    /// <summary>
    /// Contains the identifiers of the checks.
    /// </summary>
    public static class CheckId
    {
        /// <summary>
        /// The check identifier.
        /// </summary>
        public const uint CheckOverrideIconColumnsTag = 12;
    }
}
namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.CheckVolatileTables
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckVolatileTables;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckVolatileTables();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckVolatileTables_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckVolatileTables_IncompatibleVolatileOption()
        {
            var expectedSubResults = new List<IValidationResult>
            {
                Error.IncompatibleVolatileOption(null, null, null, "ColumnOption/options", "save",        "Table",  "PID",  "1000"),
                Error.IncompatibleVolatileOption(null, null, null, "ColumnOption/options", "foreignKey",  "Table",  "PID",  "1000"),
                Error.IncompatibleVolatileOption(null, null, null, "ColumnOption/options", "element",     "Table",  "PID",  "1000"),
            };

            var parent = Error.IncompatibleVolatileTable(
                test: null,
                referenceNode: null,
                positionNode: null,
                item2Value: "1000"
            ).WithSubResults(expectedSubResults.ToArray());

            var data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "IncompatibleVolatileOption",
                ExpectedResults = new List<IValidationResult> { parent },
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckVolatileTables_IncompatibleVolatileTable()
        {
            var expectedSubResults = new List<IValidationResult>
            {
                Error.IncompatibleVolatileOption(null, null, null, "Param@trending",       "true",        "Column", "PID", "1002"),
                Error.IncompatibleVolatileOption(null, null, null, "Alarm/Monitored",      "true",        "Column", "PID", "1003"),
                Error.IncompatibleVolatileOption(null, null, null, "ParameterGroups/Group@dynamicId", "dynamicId", "Table", "PID", "1000"),
                Error.IncompatibleVolatileOption(null, null, null, "ExportRule@table",     "ExportRule",  "Table",  "PID",  "1000"),
            };

            var parent = Error.IncompatibleVolatileTable(
                test: null,
                referenceNode: null,
                positionNode: null,
                item2Value: "1000"
            ).WithSubResults(expectedSubResults.ToArray());

            var data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "IncompatibleVolatileTable",
                ExpectedResults = new List<IValidationResult> { parent },
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckVolatileTables_SuggestedVolatileOption()
        {
            var data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "SuggestedVolatileOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.SuggestedVolatileOption(null, null, null, "1000"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckVolatileTables_IncompatibleVolatileOption()
        {
            // Create ErrorMessage
            var message = Error.IncompatibleVolatileOption(
               test: null,
               referenceNode: null,
               positionNode: null,
               item2Title: "ColumnOption/options",
               item2Value: "save",
               itemKind: "Table",
               idOrPid: "PID",
               itemId: "1000");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = "Incompatible 'ArrayOptions/options' option 'volatile' with 'ColumnOption/options' option 'save'. Table PID '1000'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckVolatileTables_IncompatibleVolatileTable()
        {
            var message = Error.IncompatibleVolatileTable(
                test: null,
                referenceNode: null,
                positionNode: null,
                item2Value: "1000");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = "Incompatible 'ArrayOptions/options' value 'volatile' with 'Table PID' value '1000'.",
                HasCodeFix = false,
            };

            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckVolatileTables_SuggestedVolatileOption()
        {
            var message = Error.SuggestedVolatileOption(
                test: null,
                referenceNode: null,
                positionNode: null,
                itemId: "1000");

            var expected = new ValidationResult
            {
                Severity = Severity.Minor,
                Certainty = Certainty.Uncertain,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = "Suggested 'ArrayOptions/options' option 'volatile' in Table PID '1000'.",
                HasCodeFix = false,
            };

            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckVolatileTables();

        [TestMethod]
        public void Param_CheckVolatileTables_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckVolatileTables_CheckId() => Generic.CheckId(check, CheckId.CheckVolatileTables);
    }
}
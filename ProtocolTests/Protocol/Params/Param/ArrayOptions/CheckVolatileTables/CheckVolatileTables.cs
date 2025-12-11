namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.CheckVolatileTables
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
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
        public void Param_CheckVolatileTables_IncompatibleVolatileTable()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "IncompatibleVolatileTable",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.IncompatibleVolatileTable(null, null, null, tablePID: "1000").WithSubResults(
                        Error.IncompatibleVolatileTable_ColumnOption(null, null, null,  "save",         "1"),   // Save
                        Error.IncompatibleVolatileTable_ColumnOption(null, null, null,  "foreignKey",   "2"),   // FK
                        Error.IncompatibleVolatileTable_ColumnOption(null, null, null,  "element",      "3"),   // DVE
                        Error.IncompatibleVolatileTable_DCF(null, null, null,           "1000",         "1"),   // DCF
                        Error.IncompatibleVolatileTable_Alarming(null, null, null,      "true",         "1005") // Alarm
                    ),

                    Error.IncompatibleVolatileTable(null, null, null, tablePID: "2000").WithSubResults(
                        Error.IncompatibleVolatileTable_ForeignKeyDestination(null, null, null, "2000", "2")
                    ),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckVolatileTables_IncompatibleVolatileTable_Alarming()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "IncompatibleVolatileTable_Alarming",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.IncompatibleVolatileTable(null, null, null, tablePID: "1000").WithSubResults(
                        Error.IncompatibleVolatileTable_Alarming(null, null, null, "true", "1002")
                    ),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckVolatileTables_IncompatibleVolatileTable_ColumnOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "IncompatibleVolatileTable_ColumnOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.IncompatibleVolatileTable(null, null, null, tablePID: "1000").WithSubResults(
                        Error.IncompatibleVolatileTable_ColumnOption(null, null, null,  "save",         "1"),   // Save
                        Error.IncompatibleVolatileTable_ColumnOption(null, null, null,  "foreignKey",   "2"),   // FK
                        Error.IncompatibleVolatileTable_ColumnOption(null, null, null,  "element",      "3")    // DVE
                    ),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckVolatileTables_IncompatibleVolatileTable_DCF()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "IncompatibleVolatileTable_DCF",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.IncompatibleVolatileTable(null, null, null, tablePID: "1000").WithSubResults(
                        Error.IncompatibleVolatileTable_DCF(null, null, null, "1000", "1")
                    ),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckVolatileTables_IncompatibleVolatileTable_ForeignKeyDestination()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "IncompatibleVolatileTable_ForeignKeyDestination",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.IncompatibleVolatileTable(null, null, null, tablePID: "1000").WithSubResults(
                        Error.IncompatibleVolatileTable_ColumnOption(null, null, null,  "foreignKey",   "1")
                    ),

                    Error.IncompatibleVolatileTable(null, null, null, tablePID: "2000").WithSubResults(
                        Error.IncompatibleVolatileTable_ForeignKeyDestination(null, null, null, "2000", "1")
                    ),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckVolatileTables_SuggestedVolatileOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
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
        public void Param_CheckVolatileTables_IncompatibleVolatileTable()
        {
            // Create ErrorMessage
            var message = Error.IncompatibleVolatileTable(null, null, null, "tablePID");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = "Incompatible 'ArrayOptions@options' option 'volatile'. Table PID 'tablePID'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckVolatileTables_IncompatibleVolatileTable_Alarming()
        {
            // Create ErrorMessage
            var message = Error.IncompatibleVolatileTable_Alarming(null, null, null, "monitoredValue", "columnPID");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = "Incompatible 'Alarm/Monitored' value 'monitoredValue'. Column PID 'columnPID'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckVolatileTables_IncompatibleVolatileTable_ColumnOption()
        {
            // Create ErrorMessage
            var message = Error.IncompatibleVolatileTable_ColumnOption(null, null, null, "optionName", "columnIdx");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = "Incompatible 'ColumnOption@options' option 'optionName'. Column IDX 'columnIdx'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckVolatileTables_IncompatibleVolatileTable_DCF()
        {
            // Create ErrorMessage
            var message = Error.IncompatibleVolatileTable_DCF(null, null, null, "dynamicID", "parameterGroupID");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = "Incompatible 'ParameterGroups/Group@dynamicId' value 'dynamicID'. ParameterGroup ID 'parameterGroupID'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckVolatileTables_IncompatibleVolatileTable_ForeignKeyDestination()
        {
            // Create ErrorMessage
            var message = Error.IncompatibleVolatileTable_ForeignKeyDestination(null, null, null, "fkValue", "columnIdx");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = "Incompatible 'ColumnOption@options foreignKey' value 'fkValue'. Column IDX 'columnIdx'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckVolatileTables_SuggestedVolatileOption()
        {
            // Create ErrorMessage
            var message = Error.SuggestedVolatileOption(null, null, null, "tablePID");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Minor,
                Certainty = Certainty.Uncertain,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = "Suggested 'ArrayOptions/options' option 'volatile' in Table PID 'tablePID'.",
                HasCodeFix = false,
            };

            // Assert
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
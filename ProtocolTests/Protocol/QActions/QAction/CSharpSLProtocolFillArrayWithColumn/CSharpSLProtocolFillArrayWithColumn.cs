namespace ProtocolTests.Protocol.QActions.QAction.CSharpSLProtocolFillArrayWithColumn
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpSLProtocolFillArrayWithColumn;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpSLProtocolFillArrayWithColumn();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_Valid()
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
        public void QAction_CSharpSLProtocolFillArrayWithColumn_ColumnManagedByDataMiner()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ColumnManagedByDataMiner",
                ExpectedResults = new List<IValidationResult>
                {
                    // DVE Element
                    Error.ColumnManagedByDataMiner(null, null, null, "10003", "ColumnOption@option", "element"),
                    Error.ColumnManagedByDataMiner(null, null, null, "10003", "ColumnOption@option", "element"),

                    // State
                    Error.ColumnManagedByDataMiner(null, null, null, "10004", "ColumnOption@type", "state"),

                    // DisplayKey
                    Error.ColumnManagedByDataMiner(null, null, null, "9999", "ColumnOption@type", "displaykey"),

                    // Concatenation
                    Error.ColumnManagedByDataMiner(null, null, null, "10007", "ColumnOption@type", "concatenation")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_ColumnManagedByProtocolItem()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ColumnManagedByProtocolItem",
                ExpectedResults = new List<IValidationResult>
                {
                    // RTT
                    Error.ColumnManagedByProtocolItem(null, null, null, "11003", "Timer", "1", "Timer@options", "ping/rttColumn"),
                    Error.ColumnManagedByProtocolItem(null, null, null, "11003", "Timer", "1", "Timer@options", "ping/rttColumn"),
                    
		            // Timestamp
                    Error.ColumnManagedByProtocolItem(null, null, null, "11004", "Timer", "1", "Timer@options", "ping/timestampColumn"),

                    // Jitter
                    Error.ColumnManagedByProtocolItem(null, null, null, "11005", "Timer", "1", "Timer@options", "ping/jitterColumn"),

                    // Latency
                    Error.ColumnManagedByProtocolItem(null, null, null, "11006", "Timer", "1", "Timer@options", "ping/latencyColumn"),

                    // PacketLossRateColumn
                    Error.ColumnManagedByProtocolItem(null, null, null, "11007", "Timer", "1", "Timer@options", "ping/packetLossRateColumn"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_HardCodedColumnPid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "HardCodedColumnPid",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.HardCodedColumnPid(null, null, null, "1002", "103"),
                    Error.HardCodedColumnPid(null, null, null, "1103", "103"),
                    Error.HardCodedColumnPid(null, null, null, "2002", "103"),

                    // Wrapper: Not covered yet
                    //Error.HardCodedColumnPid(null, null, null, "1002", "103"),
                    //Error.HardCodedColumnPid(null, null, null, "2002", "103"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_HardCodedTablePid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "HardCodedTablePid",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.HardCodedTablePid(null, null, null, "1000", "103"),
                    Error.HardCodedTablePid(null, null, null, "1100", "103"),
                    Error.HardCodedTablePid(null, null, null, "2000", "103"),
                    
                    // Wrapper: Not covered yet
                    //Error.HardCodedTablePid(null, null, null, "1000", "103"),
                    //Error.HardCodedTablePid(null, null, null, "2000", "103"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_NonExistingColumn()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingColumn",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingColumn(null, null, null, "1009", "103"),
                    Error.NonExistingColumn(null, null, null, "1109", "103"),
                    Error.NonExistingColumn(null, null, null, "2009", "103"),

                    Error.HardCodedColumnPid(null, null, null, "1009", "103"),
                    Error.HardCodedColumnPid(null, null, null, "1109", "103"),
                    Error.HardCodedColumnPid(null, null, null, "2009", "103"),

                    // Wrapper: Not covered yet
                    //Error.NonExistingColumn(null, null, null, "1009", "103"),
                    //Error.NonExistingColumn(null, null, null, "2009", "103"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_NonExistingTable()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingTable",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingTable(null, null, null, "999", "103"),
                    Error.NonExistingTable(null, null, null, "1099", "103"),
                    Error.NonExistingTable(null, null, null, "1999", "103"),

                    Error.HardCodedTablePid(null, null, null, "999", "103"),
                    Error.HardCodedTablePid(null, null, null, "1099", "103"),
                    Error.HardCodedTablePid(null, null, null, "1999", "103"),
                    
                    // Wrapper: Not covered yet
                    //Error.NonExistingTable(null, null, null, "999", "103"),
                    //Error.NonExistingTable(null, null, null, "1999", "103"),

                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_ParamMissingHistorySet()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ParamMissingHistorySet",
                ExpectedResults = new List<IValidationResult>
                {
                    // Hard-coded
                    Error.ParamMissingHistorySet(null, null, null, "1002"),
                    Error.ParamMissingHistorySet(null, null, null, "1103"),
                    Error.HardCodedColumnPid(null, null, null, "1002", "103"),
                    Error.HardCodedColumnPid(null, null, null, "1103", "103"),

                    // Local Variable
                    Error.ParamMissingHistorySet(null, null, null, "1002"),
                    Error.ParamMissingHistorySet(null, null, null, "1103"),

                    // Generic Constant
                    Error.ParamMissingHistorySet(null, null, null, "1002"),
                    Error.ParamMissingHistorySet(null, null, null, "1103"),

                    // Parameter Class
                    Error.ParamMissingHistorySet(null, null, null, "1002"),
                    Error.ParamMissingHistorySet(null, null, null, "1103"),

                    // Wrappers (Not yet covered)
                    //Error.ParamMissingHistorySet(null, null, null, "1002"),
                    //Error.ParamMissingHistorySet(null, null, null, "1103"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_UnrecommendedSetOnSnmpParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedSetOnSnmpParam",
                ExpectedResults = new List<IValidationResult>
                {
                    // SNMP
                    Error.UnrecommendedSetOnSnmpParam(null, null, null, "102"),

                    Error.UnrecommendedSetOnSnmpParam(null, null, null, "103"),
                    Error.UnrecommendedSetOnSnmpParam(null, null, null, "106"),

                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CSharpSLProtocolFillArrayWithColumn();

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_ParamMissingHistorySet()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "ParamMissingHistorySet",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_HardCodedColumnPid()
        {
            // Create ErrorMessage
            var message = Error.HardCodedColumnPid(null, null, null, "1", "2");

            var expected = new ValidationResult()
            {
                ErrorId = 5,
                FullId = "3.11.5",
                Category = Category.QAction,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unrecommended use of magic number '1', use 'Parameter' class instead. QAction ID '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_HardCodedTablePid()
        {
            // Create ErrorMessage
            var message = Error.HardCodedTablePid(null, null, null, "1", "2");

            var expected = new ValidationResult()
            {
                ErrorId = 4,
                FullId = "3.11.4",
                Category = Category.QAction,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unrecommended use of magic number '1', use 'Parameter' class instead. QAction ID '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_NonExistingColumn()
        {
            // Create ErrorMessage
            var message = Error.NonExistingColumn(null, null, null, "1", "2");

            var expected = new ValidationResult()
            {
                ErrorId = 2,
                FullId = "3.11.2",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'SLProtocol.FillArrayWithColumn' references a non-existing 'column' with PID '1'. QAction ID '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_NonExistingTable()
        {
            // Create ErrorMessage
            var message = Error.NonExistingTable(null, null, null, "1", "2");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "3.11.1",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'SLProtocol.FillArrayWithColumn' references a non-existing 'table' with PID '1'. QAction ID '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_ParamMissingHistorySet()
        {
            // Create ErrorMessage
            var message = Error.ParamMissingHistorySet(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 3,
                FullId = "3.11.3",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "SLProtocol.FillArrayWithColumn overload with 'DateTime? timeInfo' argument requires 'Param@historySet=true'. column PID '1'.",
                HowToFix = "",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_UnrecommendedSetOnSnmpParam()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedSetOnSnmpParam(null, null, null, "1");

            var expected = new ValidationResult
            {
                ErrorId = 8,
                FullId = "3.11.8",
                Category = Category.QAction,
                Severity = Severity.Minor,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unrecommended set on column '1' with 'ColumnOption@type' containing 'snmp'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CSharpSLProtocolFillArrayWithColumn();

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpSLProtocolFillArrayWithColumn_CheckId() => Generic.CheckId(check, CheckId.CSharpSLProtocolFillArrayWithColumn);
    }
}
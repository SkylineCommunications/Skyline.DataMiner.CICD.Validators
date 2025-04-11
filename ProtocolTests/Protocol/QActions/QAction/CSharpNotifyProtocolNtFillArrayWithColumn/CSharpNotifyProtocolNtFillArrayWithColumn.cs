namespace ProtocolTests.Protocol.QActions.QAction.CSharpNotifyProtocolNtFillArrayWithColumn
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpNotifyProtocolNtFillArrayWithColumn;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpNotifyProtocolNtFillArrayWithColumn();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_Valid()
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
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_ColumnManagedByDataMiner()
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
                    Error.ColumnManagedByDataMiner(null, null, null, "10003", "ColumnOption@option", "element"),
                    Error.ColumnManagedByDataMiner(null, null, null, "10003", "ColumnOption@option", "element"),
                    Error.ColumnManagedByDataMiner(null, null, null, "10003", "ColumnOption@option", "element"),

                    Error.ColumnManagedByDataMiner(null, null, null, "10006", "ColumnOption@option", "severity"),

                    // State
                    Error.ColumnManagedByDataMiner(null, null, null, "10004", "ColumnOption@type", "state"),

                    // DisplayKey
                    Error.ColumnManagedByDataMiner(null, null, null, "9999", "ColumnOption@type", "displaykey"),

                    // Concatenation
                    Error.ColumnManagedByDataMiner(null, null, null, "10007", "ColumnOption@type", "concatenation"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_ColumnManagedByProtocolItem()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ColumnManagedByProtocolItem",
                ExpectedResults = new List<IValidationResult>
                {
                    #region QAction 300 (SetOnSingleColumn)
                    // RTT
                    Error.ColumnManagedByProtocolItem(null, null, null, "11003", "Timer", "1", "Timer@options", "ping/rttColumn"),
                    Error.ColumnManagedByProtocolItem(null, null, null, "11003", "Timer", "1", "Timer@options", "ping/rttColumn"),
                    Error.ColumnManagedByProtocolItem(null, null, null, "11003", "Timer", "1", "Timer@options", "ping/rttColumn"),
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
                    #endregion

                    #region QAction 301 (SetOnMultipleColumns)
                    // All columns
                    Error.ColumnManagedByProtocolItem(null, null, null, "11003", "Timer", "1", "Timer@options", "ping/rttColumn"),
                    Error.ColumnManagedByProtocolItem(null, null, null, "11004", "Timer", "1", "Timer@options", "ping/timestampColumn"),
                    Error.ColumnManagedByProtocolItem(null, null, null, "11005", "Timer", "1", "Timer@options", "ping/jitterColumn"),
                    Error.ColumnManagedByProtocolItem(null, null, null, "11006", "Timer", "1", "Timer@options", "ping/latencyColumn"),
                    Error.ColumnManagedByProtocolItem(null, null, null, "11007", "Timer", "1", "Timer@options", "ping/packetLossRateColumn"),
                    #endregion
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_ColumnMissingHistorySet()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ColumnMissingHistorySet",
                ExpectedResults = new List<IValidationResult>
                {
                    #region QAction 103: History sets on method call level

                    // NotifyProtocol
                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Nohistoryset.tablePid, Parameter.Nohistoryset.Pid.nohistoryset_column2, Parameter.Nohistoryset.Pid.nohistoryset_column3, new object[] { useClearAndLeaveFalse, date_dt } }, new object[] { primaryKeys, columnValues, columnValues2 })",
                        "103").WithSubResults(
                        Error.ColumnMissingHistorySet(null, null, null, "1002"),
                        Error.ColumnMissingHistorySet(null, null, null, "1003")),

                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Historysetfalse.tablePid, Parameter.Historysetfalse.Pid.historysetfalse_column2, Parameter.Historysetfalse.Pid.historysetfalse_column3, new object[] { useClearAndLeaveFalse, date_dt } }, new object[] { primaryKeys, columnValues, columnValues2 })",
                        "103").WithSubResults(
                        Error.ColumnMissingHistorySet(null, null, null, "1102"),
                        Error.ColumnMissingHistorySet(null, null, null, "1103")),

                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Mytable.tablePid, Parameter.Mytable.Pid.mytable_column2_historysettrue, Parameter.Mytable.Pid.mytable_column3_historysetfalse, Parameter.Mytable.Pid.mytable_column4_nohistoryset, new object[] { useClearAndLeaveFalse, date_dt } }, new object[] { primaryKeys, columnValues, columnValues2, columnValues3 })",
                        "103").WithSubResults(
                        Error.ColumnMissingHistorySet(null, null, null, "10003"),
                        Error.ColumnMissingHistorySet(null, null, null, "10004")),

                    #endregion

                    #region QAction 104: History sets on cell level
                    
		            // History sets on cell level - no optional field

                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Nohistoryset.tablePid, Parameter.Nohistoryset.Pid.nohistoryset_column2, Parameter.Nohistoryset.Pid.nohistoryset_column3 }, new object[] { primaryKeys, columnValuesAndDateTimes, columnValues })",
                        "104").WithSubResults(
                            Error.ColumnMissingHistorySet(null, null, null, "1002")
                            //Error.ColumnMissingHistorySet(null, null, null, "1003")   // use columnValues (no dateTime)
                            ),

                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Historysetfalse.tablePid, Parameter.Historysetfalse.Pid.historysetfalse_column2, Parameter.Historysetfalse.Pid.historysetfalse_column3 }, new object[] { primaryKeys, columnValuesAndDateTimes, columnValuesAndDateTimes })",
                        "104").WithSubResults(
                        Error.ColumnMissingHistorySet(null, null, null, "1102"),
                        Error.ColumnMissingHistorySet(null, null, null, "1103")),

                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Mytable.tablePid, Parameter.Mytable.Pid.mytable_column2_historysettrue, Parameter.Mytable.Pid.mytable_column3_historysetfalse, Parameter.Mytable.Pid.mytable_column4_nohistoryset }, new object[] { primaryKeys, columnValuesAndDateTimes, columnValuesAndDateTimes, columnValuesAndDateTimes })",
                        "104").WithSubResults(
                        Error.ColumnMissingHistorySet(null, null, null, "10003"),
                        Error.ColumnMissingHistorySet(null, null, null, "10004")),

		            // History sets on cell level - optional bool field
                    
                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Nohistoryset.tablePid, Parameter.Nohistoryset.Pid.nohistoryset_column2, Parameter.Nohistoryset.Pid.nohistoryset_column3, useClearAndLeaveFalse }, new object[] { primaryKeys, columnValues, columnValuesAndDateTimes })",
                        "104").WithSubResults(
                            //Error.ColumnMissingHistorySet(null, null, null, "1002"),   // use columnValues (no dateTime)
                            Error.ColumnMissingHistorySet(null, null, null, "1003")),

                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Historysetfalse.tablePid, Parameter.Historysetfalse.Pid.historysetfalse_column2, Parameter.Historysetfalse.Pid.historysetfalse_column3, useClearAndLeaveFalse }, new object[] { primaryKeys, columnValuesAndDateTimes, columnValues })",
                        "104").WithSubResults(
                            Error.ColumnMissingHistorySet(null, null, null, "1102")
                            //Error.ColumnMissingHistorySet(null, null, null, "1103")   // use columnValues (no dateTime)
                            ),

                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Mytable.tablePid, Parameter.Mytable.Pid.mytable_column2_historysettrue, Parameter.Mytable.Pid.mytable_column3_historysetfalse, Parameter.Mytable.Pid.mytable_column4_nohistoryset, useClearAndLeaveFalse }, new object[] { primaryKeys, columnValuesAndDateTimes, columnValues, columnValuesAndDateTimes })",
                        "104").WithSubResults(
                            //Error.ColumnMissingHistorySet(null, null, null, "10002"),   // Has HistorySet="true"
                            //Error.ColumnMissingHistorySet(null, null, null, "10003"),   // use columnValues (no dateTime)
                            Error.ColumnMissingHistorySet(null, null, null, "10004")),

		            // History sets on cell level - optional array field {bool}
                    
                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Nohistoryset.tablePid, Parameter.Nohistoryset.Pid.nohistoryset_column2, Parameter.Nohistoryset.Pid.nohistoryset_column3, new object[] { useClearAndLeaveFalse } }, new object[] { primaryKeys, columnValuesAndDateTimes, columnValuesAndDateTimes })",
                        "104").WithSubResults(Error.ColumnMissingHistorySet(null, null, null, "1002"), Error.ColumnMissingHistorySet(null, null, null, "1003")),

                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Historysetfalse.tablePid, Parameter.Historysetfalse.Pid.historysetfalse_column2, Parameter.Historysetfalse.Pid.historysetfalse_column3, new object[] { useClearAndLeaveFalse } }, new object[] { primaryKeys, columnValues, columnValuesAndDateTimes })",
                        "104").WithSubResults(
                            //Error.ColumnMissingHistorySet(null, null, null, "1102"),   // use columnValues (no dateTime)
                            Error.ColumnMissingHistorySet(null, null, null, "1103")),

                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Mytable.tablePid, Parameter.Mytable.Pid.mytable_column2_historysettrue, Parameter.Mytable.Pid.mytable_column3_historysetfalse, Parameter.Mytable.Pid.mytable_column4_nohistoryset, new object[] { useClearAndLeaveFalse } }, new object[] { primaryKeys, columnValuesAndDateTimes, columnValuesAndDateTimes, columnValues })",
                        "104").WithSubResults(
                            //Error.ColumnMissingHistorySet(null, null, null, "10002"),   // Has HistorySet="true"
                            Error.ColumnMissingHistorySet(null, null, null, "10003")
                            //Error.ColumnMissingHistorySet(null, null, null, "10004")   // use columnValues (no dateTime)
                            ),

		            // History sets on cell level - optional array field {bool, null}
                    
                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Nohistoryset.tablePid, Parameter.Nohistoryset.Pid.nohistoryset_column2, Parameter.Nohistoryset.Pid.nohistoryset_column3, new object[] { useClearAndLeaveFalse, null } }, new object[] { primaryKeys, columnValuesAndDateTimes, columnValuesAndDateTimes })",
                        "104").WithSubResults(
                        Error.ColumnMissingHistorySet(null, null, null, "1002"),
                        Error.ColumnMissingHistorySet(null, null, null, "1003")),

                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Historysetfalse.tablePid, Parameter.Historysetfalse.Pid.historysetfalse_column2, Parameter.Historysetfalse.Pid.historysetfalse_column3, new object[] { useClearAndLeaveFalse, null } }, new object[] { primaryKeys, columnValuesAndDateTimes, columnValuesAndDateTimes })",
                        "104").WithSubResults(
                        Error.ColumnMissingHistorySet(null, null, null, "1102"),
                        Error.ColumnMissingHistorySet(null, null, null, "1103")),

                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Mytable.tablePid, Parameter.Mytable.Pid.mytable_column2_historysettrue, Parameter.Mytable.Pid.mytable_column3_historysetfalse, Parameter.Mytable.Pid.mytable_column4_nohistoryset, new object[] { useClearAndLeaveFalse, null } }, new object[] { primaryKeys, columnValues, columnValuesAndDateTimes, columnValuesAndDateTimes })",
                        "104").WithSubResults(
                        Error.ColumnMissingHistorySet(null, null, null, "10003"),
                        Error.ColumnMissingHistorySet(null, null, null, "10004"))

                    #endregion
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_HardCodedColumnPid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "HardCodedColumnPid",
                ExpectedResults = new List<IValidationResult>
                {
                    // QAction 102
                    Error.HardCodedColumnPid(null, null, null, "1002", "102"),
                    //Error.HardCodedColumnPid(null, null, null, "1002", "102"),    // Wrappers not yet covered

                    // QAction 103
                    Error.HardCodedColumnPid(null, null, null, "2002", "103"),
                    //Error.HardCodedColumnPid(null, null, null, "2002", "103"),    // Wrappers not yet covered
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_HardCodedTablePid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "HardCodedTablePid",
                ExpectedResults = new List<IValidationResult>
                {
                    // QAction 102
                    Error.HardCodedTablePid(null, null, null, "1000", "102"),
                    Error.HardCodedTablePid(null, null, null, "1100", "102"),

                    // QAction 103
                    Error.HardCodedTablePid(null, null, null, "2000", "103"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_NonExistingColumn()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingColumn",
                ExpectedResults = new List<IValidationResult>
                {
                    // QAction 102
                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Nohistoryset.tablePid, 1009, false }, new object[] { keys, values })",
                        "102").WithSubResults(
                        Error.NonExistingColumn(null, null, null, "1009", "102"),
                        Error.HardCodedColumnPid(null, null, null, "1009", "102")),

                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Historysetfalse.tablePid, MyParams.NonExistingColumn_999, 1110, new object[] { true } }, new object[] { keys, values, values })",
                        "102").WithSubResults(
                        Error.NonExistingColumn(null, null, null, "999", "102"),
                        Error.HardCodedColumnPid(null, null, null, "999", "102"),
                        Error.NonExistingColumn(null, null, null, "1110", "102"),
                        Error.HardCodedColumnPid(null, null, null, "1110", "102")),

                    // QAction 103
                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Historysettrue.tablePid, 2009, new object[] { false } }, new object[] { keys, values })",
                        "103").WithSubResults(
                            Error.NonExistingColumn(null, null, null, "2009", "103"),
                            Error.HardCodedColumnPid(null, null, null, "2009","103"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_NonExistingTable()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingTable",
                ExpectedResults = new List<IValidationResult>
                {
                    // QAction 102
                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { 999, Parameter.Nohistoryset.Pid.nohistoryset_column2, false }, new object[] { keys, values })",
                        "102").WithSubResults(
                        Error.NonExistingTable(null, null, null, "999", "102"),
                        Error.HardCodedTablePid(null, null, null, "999", "102")),

                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { MyParams.NonExistingTable_900, Parameter.Historysetfalse.Pid.historysetfalse_column2, Parameter.Historysetfalse.Pid.historysetfalse_column3_1103, new object[] { false } }, new object[] { keys, values, values })",
                        "102").WithSubResults(
                        Error.NonExistingTable(null, null, null, "900", "102"),
                        Error.HardCodedTablePid(null, null, null, "900", "102")),
                    
                    // QAction 103
                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { 1999, Parameter.Historysettrue.Pid.historysettrue_column2, false }, new object[] { keys, values })",
                        "103").WithSubResults(
                        Error.NonExistingTable(null, null, null, "1999", "103"),
                        Error.HardCodedTablePid(null, null, null, "1999", "103")),
                    
                    // QAction 104
                    Error.NonExistingTable(null, null, null, "0", "104"),
                    
                    // Wrong error for now.
                    Error.ColumnManagedByDataMiner(null, null, null, "1", "ColumnOption@type", "")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_UnexpectedImplementation()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnexpectedImplementation",
                ExpectedResults = new List<IValidationResult>
                {
                    // Several Errors in a Method

                    // TableHistorySet_SeveralColumns
                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Historysetfalse.tablePid, Parameter.Historysetfalse.Pid.historysetfalse_column2, Parameter.Historysetfalse.Pid.historysetfalse_column3, new object[] { false, date_dt } }, new object[] { keys, values, values })",
                        "102").WithSubResults(
                        Error.ColumnMissingHistorySet(null, null, null, "1102"),
                        Error.ColumnMissingHistorySet(null, null, null, "1103")),

                    // CellHistorySet_SeveralColumns
                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Nohistoryset.tablePid, Parameter.Nohistoryset.Pid.nohistoryset_column2 ,Parameter.Nohistoryset.Pid.nohistoryset_column3, new object[] { false } }, new object[] { keys, values2, values3 })",
                        "103").WithSubResults(
                        Error.ColumnMissingHistorySet(null, null, null, "1002"),
                        Error.ColumnMissingHistorySet(null, null, null, "1003")),

                    // HardCodedColumnPid_SeveralColumns
                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Historysettrue.tablePid, MyParams.MyColumn_2002, 2003, true }, new object[] { keys, values, values })",
                        "104").WithSubResults(
                        Error.HardCodedColumnPid(null, null, null, "2002", "104"),
                        Error.HardCodedColumnPid(null, null, null, "2003", "104")),

                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Historysetfalse.tablePid, 1102, 1103}, new object[] { keys, values, values })",
                        "104").WithSubResults(
                            Error.HardCodedColumnPid(null, null, null, "1102", "104"),
                            Error.HardCodedColumnPid(null, null, null, "1103", "104")),

                    // HardCodedColumnPid_And_HistorySet
                    Error.UnexpectedImplementation(null, null, null,
                        "(220, new object[] { Parameter.Historysetfalse.tablePid, 1102, 1103, new object[] { true , date_dt }}, new object[] { keys, values, values })",
                        "105").WithSubResults(
                            Error.HardCodedColumnPid(null, null, null, "1102", "105"),
                            Error.HardCodedColumnPid(null, null, null, "1103", "105"),
                            Error.ColumnMissingHistorySet(null, null, null, "1102"),
                            Error.ColumnMissingHistorySet(null, null, null, "1103"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_UnrecommendedSetOnSnmpParam()
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
        private readonly ICodeFix codeFix = new CSharpNotifyProtocolNtFillArrayWithColumn();

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_ColumnMissingHistorySet()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "ColumnMissingHistorySet",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_ColumnMissingHistorySet()
        {
            // Create ErrorMessage
            var message = Error.ColumnMissingHistorySet(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 4,
                FullId = "3.34.4",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "NotifyProtocol(220/*NT_FILL_ARRAY_WITH_COLUMN*/, ...) method with one or more DateTime(s) given to it requires 'Param@historySet=true' on column with PID '1'.",
                HowToFix = "",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_HardCodedColumnPid()
        {
            // Create ErrorMessage
            var message = Error.HardCodedColumnPid(null, null, null, "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 6,
                FullId = "3.34.6",
                Category = Category.QAction,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unrecommended use of magic number '2', use 'Parameter' class instead. QAction ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_HardCodedTablePid()
        {
            // Create ErrorMessage
            var message = Error.HardCodedTablePid(null, null, null, "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 5,
                FullId = "3.34.5",
                Category = Category.QAction,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unrecommended use of magic number '2', use 'Parameter' class instead. QAction ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_NonExistingColumn()
        {
            // Create ErrorMessage
            var message = Error.NonExistingColumn(null, null, null, "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 3,
                FullId = "3.34.3",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'NotifyProtocol(220/*NT_FILL_ARRAY_WITH_COLUMN*/, ...)' references a non-existing 'column' with PID '2'. QAction ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_NonExistingTable()
        {
            // Create ErrorMessage
            var message = Error.NonExistingTable(null, null, null, "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 2,
                FullId = "3.34.2",
                Category = Category.QAction,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'NotifyProtocol(220/*NT_FILL_ARRAY_WITH_COLUMN*/, ...)' references a non-existing 'table' with PID '2'. QAction ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_UnexpectedImplementation()
        {
            // Create ErrorMessage
            var message = Error.UnexpectedImplementation(null, null, null, "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "3.34.1",
                Category = Category.QAction,
                Severity = Severity.BubbleUp,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Method 'NotifyProtocol(220/*NT_FILL_ARRAY_WITH_COLUMN*/, ...)' with arguments '2' is not implemented as expected. QAction ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_UnrecommendedSetOnSnmpParam()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedSetOnSnmpParam(null, null, null, "1");

            var expected = new ValidationResult
            {
                ErrorId = 9,
                FullId = "3.34.9",
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
        private readonly IRoot check = new CSharpNotifyProtocolNtFillArrayWithColumn();

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpNotifyProtocolNtFillArrayWithColumn_CheckId() => Generic.CheckId(check, CheckId.CSharpNotifyProtocolNtFillArrayWithColumn);
    }
}
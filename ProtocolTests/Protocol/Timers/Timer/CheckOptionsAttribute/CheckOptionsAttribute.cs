namespace ProtocolTests.Protocol.Timers.Timer.CheckOptionsAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Timers.Timer.CheckOptionsAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckOptionsAttribute();

        #region Valid Checks

        [TestMethod]
        public void Timer_CheckOptionsAttribute_Valid()
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
        public void Timer_CheckOptionsAttribute_DuplicateOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "threadPool:10;ip:2000,1;each:100;each:200").WithSubResults(
                        Error.DuplicateOption(null, null, null, "each"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_DuplicateOptionInPingOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateOptionInPingOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "threadPool:10;ip:2000,1;each:100;ping:rttColumn=4,rttColumn=5").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:rttColumn=4,rttColumn=5").WithSubResults(
                            Error.DuplicateOptionInPingOption(null, null, null, "rttColumn")))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_InvalidAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "3", "ip:2000,1;each:100;threadPool:aaa;qaction;qactionBefore:20;qactionAfter:bbb;randomOption").WithSubResults(
                        Error.InvalidThreadPoolOption(null, null, null, "threadPool:aaa").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "<size>", "aaa")),
                        Error.UseOfObsoleteQActionOption(null, null, null),
                        Error.InvalidQActionOption(null, null, null, "qaction"),
                        Error.NonExistingIdInOption(null, null, null, "qactionBefore", "QAction", "20"),
                        Error.InvalidQActionAfterOption(null, null, null, "qactionAfter:bbb"),
                        Error.UnknownOption(null, null, null, "randomOption"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_InvalidDynamicThreadPoolOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidDynamicThreadPoolOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "threadPool:10;ip:2000,1;each:100;dynamicThreadPool").WithSubResults(
                        Error.InvalidDynamicThreadPoolOption(null, null, null, "dynamicThreadPool")),
                    Error.InvalidAttribute(null, null, null, "2", "threadPool:10;ip:2000,1;each:100;dynamicThreadPool:").WithSubResults(
                        Error.InvalidDynamicThreadPoolOption(null, null, null, "dynamicThreadPool:")),
                    Error.InvalidAttribute(null, null, null, "3", "threadPool:10;ip:2000,1;each:100;dynamicThreadPool:abc").WithSubResults(
                        Error.InvalidDynamicThreadPoolOption(null, null, null, "dynamicThreadPool:abc"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_InvalidEachOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidEachOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "threadPool:10;ip:2000,1;each").WithSubResults(
                        Error.InvalidEachOption(null, null, null, "each")),
                    Error.InvalidAttribute(null, null, null, "2", "threadPool:10;ip:2000,1;each:").WithSubResults(
                        Error.InvalidEachOption(null, null, null, "each:")),
                    Error.InvalidAttribute(null, null, null, "3", "threadPool:10;ip:2000,1;each:abc").WithSubResults(
                        Error.InvalidEachOption(null, null, null, "each:abc"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_InvalidIgnoreIfOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidIgnoreIfOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "ip:2000,1;each:1000;threadPool:10;ignoreIf").WithSubResults(
                        Error.InvalidIgnoreIfOption(null, null, null, "ignoreIf")),
                    Error.InvalidAttribute(null, null, null, "2", "ip:2000,1;each:1000;threadPool:10;ignoreIf:").WithSubResults(
                        Error.InvalidIgnoreIfOption(null, null, null, "ignoreIf:")),
                    Error.InvalidAttribute(null, null, null, "10", "ip:2000,1;each:1000;threadPool:10;ignoreIf:abc,2").WithSubResults(
                        Error.InvalidIgnoreIfOption(null, null, null, "ignoreIf:abc,2").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "<columnIdx>", "abc"))),
                    Error.InvalidAttribute(null, null, null, "11", "ip:2000,1;each:1000;threadPool:10;ignoreIf:8,abc").WithSubResults(
                        Error.InvalidIgnoreIfOption(null, null, null, "ignoreIf:8,abc").WithSubResults(
                            Error.NonExistingColumnIdxInOption(null, null, null, "<columnIdx>", "8", "2000"))),
                    Error.InvalidAttribute(null, null, null, "20", "ip:2000,1;each:1000;threadPool:10;ignoreIf:2").WithSubResults(
                        Error.InvalidIgnoreIfOption(null, null, null, "ignoreIf:2").WithSubResults(
                            Error.MissingValueInOption(null, null, null, "<value>")))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_InvalidInstanceOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidInstanceOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "ip:2000,1;each:1000;threadPool:10;instance").WithSubResults(
                        Error.InvalidInstanceOption(null, null, null, "instance")),
                    Error.InvalidAttribute(null, null, null, "2", "ip:2000,1;each:1000;threadPool:10;instance:").WithSubResults(
                        Error.InvalidInstanceOption(null, null, null, "instance:")),
                    Error.InvalidAttribute(null, null, null, "10", "ip:2000,1;each:1000;threadPool:10;instance:aaa,0").WithSubResults(
                        Error.InvalidInstanceOption(null, null, null, "instance:aaa,0").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "<tablePid>", "aaa"))),
                    Error.InvalidAttribute(null, null, null, "11", "ip:2000,1;each:1000;threadPool:10;instance:2000,bbb").WithSubResults(
                        Error.InvalidInstanceOption(null, null, null, "instance:2000,bbb").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "<columnIdx>", "bbb"))),
                    Error.InvalidAttribute(null, null, null, "12", "ip:2000,1;each:1000;threadPool:10;instance:ccc,ddd").WithSubResults(
                        Error.InvalidInstanceOption(null, null, null, "instance:ccc,ddd").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "<tablePid>", "ccc"),
                            Error.InvalidValueInOption(null, null, null, "<columnIdx>", "ddd"))),
                    Error.InvalidAttribute(null, null, null, "20", "ip:2000,1;each:1000;threadPool:10;instance:2000").WithSubResults(
                        Error.InvalidInstanceOption(null, null, null, "instance:2000").WithSubResults(
                            Error.MissingValueInOption(null, null, null, "<columnIdx>"))),
                    Error.InvalidAttribute(null, null, null, "30", "ip:2000,1;each:1000;threadPool:10;instance:1000,1").WithSubResults(
                        Error.InvalidInstanceOption(null, null, null, "instance:1000,1").WithSubResults(
                            Error.NonExistingIdInOption(null, null, null, "<tablePid>", "Param", "1000"))),
                    Error.InvalidAttribute(null, null, null, "31", "ip:2000,1;each:1000;threadPool:10;instance:2000,2").WithSubResults(
                        Error.InvalidInstanceOption(null, null, null, "instance:2000,2").WithSubResults(
                            Error.NonExistingColumnIdxInOption(null, null, null, "<columnIdx>", "2", "2000"))),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_InvalidIpOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidIpOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "threadPool:10;each:100;ip").WithSubResults(
                        Error.InvalidIpOption(null, null, null, "ip")),
                    Error.InvalidAttribute(null, null, null, "2", "threadPool:10;each:100;ip:").WithSubResults(
                        Error.InvalidIpOption(null, null, null, "ip:")),

                    Error.InvalidAttribute(null, null, null, "10", "threadPool:10;each:100;ip:aaa,1").WithSubResults(
                        Error.InvalidIpOption(null, null, null, "ip:aaa,1").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "<tablePid>", "aaa"))),
                    Error.InvalidAttribute(null, null, null, "11", "threadPool:10;each:100;ip:2000,bbb").WithSubResults(
                        Error.InvalidIpOption(null, null, null, "ip:2000,bbb").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "<columnIdx>", "bbb"))),
                    Error.InvalidAttribute(null, null, null, "12", "threadPool:10;each:100;ip:ccc,ddd").WithSubResults(
                        Error.InvalidIpOption(null, null, null, "ip:ccc,ddd").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "<tablePid>", "ccc"),
                            Error.InvalidValueInOption(null, null, null, "<columnIdx>", "ddd"))),

                    Error.InvalidAttribute(null, null, null, "20", "threadPool:10;each:100;ip:1000,0").WithSubResults(
                        Error.InvalidIpOption(null, null, null, "ip:1000,0").WithSubResults(
                            Error.NonExistingIdInOption(null, null, null, "<tablePid>", "Param", "1000"))),
                    Error.InvalidAttribute(null, null, null, "21", "threadPool:10;each:100;ip:2000,1").WithSubResults(
                        Error.InvalidIpOption(null, null, null, "ip:2000,1").WithSubResults(
                            Error.NonExistingColumnIdxInOption(null, null, null, "<columnIdx>", "1", "2000"))),

                    Error.InvalidAttribute(null, null, null, "30", "threadPool:10;each:100;ip:2000").WithSubResults(
                        Error.InvalidIpOption(null, null, null, "ip:2000").WithSubResults(
                            Error.MissingValueInOption(null, null, null, "<columnIdx>")))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_InvalidPingOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidPingOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "ip:2000,1;each:1000;threadPool:10;ping").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping")),
                    Error.InvalidAttribute(null, null, null, "2", "ip:2000,1;each:1000;threadPool:10;ping:").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:")),

                    Error.InvalidAttribute(null, null, null, "10", "ip:2000,1;each:1000;threadPool:10;ping:rttColumn=10").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:rttColumn=10").WithSubResults(
                            Error.NonExistingColumnPositionInOption(null, null, null, "rttColumn", "10", "2000"))),
                    Error.InvalidAttribute(null, null, null, "11", "ip:2000,1;each:1000;threadPool:10;ping:timeoutPid=abc").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:timeoutPid=abc").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "timeoutPid", "abc"),
                            Error.UseOfObsoleteTimeoutPidOptionInPingOption(null, null, null))),
                    Error.InvalidAttribute(null, null, null, "12", "ip:2000,1;each:1000;threadPool:10;ping:ttl=-1").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:ttl=-1").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "ttl", "-1"))),
                    Error.InvalidAttribute(null, null, null, "13", "ip:2000,1;each:1000;threadPool:10;ping:timeout=-1").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:timeout=-1").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "timeout", "-1"))),
                    Error.InvalidAttribute(null, null, null, "14", "ip:2000,1;each:1000;threadPool:10;ping:timestampcolumn=-1").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:timestampcolumn=-1").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "timestampColumn", "-1"))),
                    Error.InvalidAttribute(null, null, null, "15", "ip:2000,1;each:1000;threadPool:10;ping:type=abc").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:type=abc").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "type", "abc"))),
                    Error.InvalidAttribute(null, null, null, "16", "ip:2000,1;each:1000;threadPool:10;ping:size=-1").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:size=-1").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "size", "-1"))),
                    Error.InvalidAttribute(null, null, null, "17", "ip:2000,1;each:1000;threadPool:10;ping:continueSNMPOnTimeout=no").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:continueSNMPOnTimeout=no").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "continueSnmpOnTimeout", "no"))),
                    Error.InvalidAttribute(null, null, null, "18", "ip:2000,1;each:1000;threadPool:10;ping:jitterColumn=20").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:jitterColumn=20").WithSubResults(
                            Error.NonExistingColumnPositionInOption(null, null, null, "jitterColumn", "20", "2000"))),
                    Error.InvalidAttribute(null, null, null, "19", "ip:2000,1;each:1000;threadPool:10;ping:latencyColumn=21").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:latencyColumn=21").WithSubResults(
                            Error.NonExistingColumnPositionInOption(null, null, null, "latencyColumn", "21", "2000"))),
                    Error.InvalidAttribute(null, null, null, "20", "ip:2000,1;each:1000;threadPool:10;ping:packetLossRateColumn=22").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:packetLossRateColumn=22").WithSubResults(
                            Error.NonExistingColumnPositionInOption(null, null, null, "packetLossRateColumn", "22", "2000"))),
                    Error.InvalidAttribute(null, null, null, "21", "ip:2000,1;each:1000;threadPool:10;ping:amountPacketsMeasurements=abc").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:amountPacketsMeasurements=abc").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "amountPacketsMeasurements", "abc"))),
                    Error.InvalidAttribute(null, null, null, "22", "ip:2000,1;each:1000;threadPool:10;ping:amountPacketsMeasurementsPid=10").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:amountPacketsMeasurementsPid=10").WithSubResults(
                            Error.NonExistingIdInOption(null, null, null, "amountPacketsMeasurementsPid", "Param", "10"))),
                    Error.InvalidAttribute(null, null, null, "23", "ip:2000,1;each:1000;threadPool:10;ping:amountPackets=abc").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:amountPackets=abc").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "amountPackets", "abc"))),
                    Error.InvalidAttribute(null, null, null, "24", "ip:2000,1;each:1000;threadPool:10;ping:amountPacketsPid=10").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:amountPacketsPid=10").WithSubResults(
                            Error.NonExistingIdInOption(null, null, null, "amountPacketsPid", "Param", "10"))),
                    Error.InvalidAttribute(null, null, null, "25", "ip:2000,1;each:1000;threadPool:10;ping:excludeWorstResults=110").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:excludeWorstResults=110").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "excludeWorstResults", "110"))),
                    Error.InvalidAttribute(null, null, null, "26", "ip:2000,1;each:1000;threadPool:10;ping:excludeWorstResultsPid=11").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:excludeWorstResultsPid=11").WithSubResults(
                            Error.NonExistingIdInOption(null, null, null, "excludeWorstResultsPid", "Param", "11"))),

                    Error.InvalidAttribute(null, null, null, "100", "ip:2000,1;each:1000;threadPool:10;ping:rttColumn=10,timeoutPid=abc,ttl=-1").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:rttColumn=10,timeoutPid=abc,ttl=-1").WithSubResults(
                            Error.NonExistingColumnPositionInOption(null, null, null, "rttColumn", "10", "2000"),
                            Error.InvalidValueInOption(null, null, null, "timeoutPid", "abc"),
                            Error.UseOfObsoleteTimeoutPidOptionInPingOption(null, null, null),
                            Error.InvalidValueInOption(null, null, null, "ttl", "-1"))),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_InvalidPollingRateOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidPollingRateOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "ip:2000,1;each:1000;threadPool:10;pollingRate").WithSubResults(
                        Error.InvalidPollingRateOption(null, null, null, "pollingRate")),
                    Error.InvalidAttribute(null, null, null, "2", "ip:2000,1;each:1000;threadPool:10;pollingRate:").WithSubResults(
                        Error.InvalidPollingRateOption(null, null, null, "pollingRate:")),

                    Error.InvalidAttribute(null, null, null, "10", "ip:2000,1;each:1000;threadPool:10;pollingRate:abc,def,ghi").WithSubResults(
                        Error.InvalidPollingRateOption(null, null, null, "pollingRate:abc,def,ghi").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "<interval>", "abc"),
                            Error.InvalidValueInOption(null, null, null, "<maxCount>", "def"),
                            Error.InvalidValueInOption(null, null, null, "<releaseCount>", "ghi"))),
                    Error.InvalidAttribute(null, null, null, "100", "ip:2000,1;each:1000;threadPool:10;pollingRate:15").WithSubResults(
                        Error.InvalidPollingRateOption(null, null, null, "pollingRate:15").WithSubResults(
                            Error.MissingValueInOption(null, null, null, "<maxCount>"),
                            Error.MissingValueInOption(null, null, null, "<releaseCount>")))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_InvalidQActionAfterOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidQActionAfterOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "ip:2000,1;each:1000;threadPool:10;qactionAfter").WithSubResults(
                        Error.InvalidQActionAfterOption(null, null, null, "qactionAfter")),
                    Error.InvalidAttribute(null, null, null, "2", "ip:2000,1;each:1000;threadPool:10;qactionAfter:").WithSubResults(
                        Error.InvalidQActionAfterOption(null, null, null, "qactionAfter:")),

                    Error.InvalidAttribute(null, null, null, "10", "ip:2000,1;each:1000;threadPool:10;qactionAfter:abc").WithSubResults(
                        Error.InvalidQActionAfterOption(null, null, null, "qactionAfter:abc")),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_InvalidQActionBeforeOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidQActionBeforeOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "ip:2000,1;each:1000;threadPool:10;qactionBefore").WithSubResults(
                        Error.InvalidQActionBeforeOption(null, null, null, "qactionBefore")),
                    Error.InvalidAttribute(null, null, null, "2", "ip:2000,1;each:1000;threadPool:10;qactionBefore:").WithSubResults(
                        Error.InvalidQActionBeforeOption(null, null, null, "qactionBefore:")),

                    Error.InvalidAttribute(null, null, null, "10", "ip:2000,1;each:1000;threadPool:10;qactionBefore:abc").WithSubResults(
                        Error.InvalidQActionBeforeOption(null, null, null, "qactionBefore:abc")),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_InvalidQActionOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidQActionOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "ip:2000,1;each:1000;threadPool:10;qaction").WithSubResults(
                        Error.InvalidQActionOption(null, null, null, "qaction"),
                        Error.UseOfObsoleteQActionOption(null, null, null)),
                    Error.InvalidAttribute(null, null, null, "2", "ip:2000,1;each:1000;threadPool:10;qaction:").WithSubResults(
                        Error.InvalidQActionOption(null, null, null, "qaction:"),
                        Error.UseOfObsoleteQActionOption(null, null, null)),
                    Error.InvalidAttribute(null, null, null, "10", "ip:2000,1;each:1000;threadPool:10;qaction:abc").WithSubResults(
                        Error.InvalidQActionOption(null, null, null, "qaction:abc"),
                        Error.UseOfObsoleteQActionOption(null, null, null))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_InvalidThreadPoolOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidThreadPoolOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "ip:2000,1;each:1000;threadPool").WithSubResults(
                        Error.InvalidThreadPoolOption(null, null, null, "threadPool")),
                    Error.InvalidAttribute(null, null, null, "2", "ip:2000,1;each:1000;threadPool:").WithSubResults(
                        Error.InvalidThreadPoolOption(null, null, null, "threadPool:")),

                    Error.InvalidAttribute(null, null, null, "10", "ip:2000,1;each:1000;threadPool:abc").WithSubResults(
                        Error.InvalidThreadPoolOption(null, null, null, "threadPool:abc").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "<size>", "abc"))),
                    Error.InvalidAttribute(null, null, null, "11", "ip:2000,1;each:1000;threadPool:10,5,-1,-1,-1,-1,abc,def").WithSubResults(
                        Error.InvalidThreadPoolOption(null, null, null, "threadPool:10,5,-1,-1,-1,-1,abc,def").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "<counterPid>", "abc"),
                            Error.InvalidValueInOption(null, null, null, "<queueSize>", "def"))),

                    Error.ThreadPoolCalculationIntervalDefined(null, null, null, "11")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_InvalidValueInOptions()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValueInOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "ip:2000,1;each:1000;threadPool:10;ignoreIf:abc,2").WithSubResults(
                        Error.InvalidIgnoreIfOption(null, null, null, "ignoreIf:abc,2").WithSubResults(
                            Error.InvalidValueInOption(null, null, null, "<columnIdx>", "abc")))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_MissingEachOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingEachOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "threadPool:10;ip:2000,1").WithSubResults(
                        Error.MissingEachOption(null, null, null, "ip', 'threadPool", "1")),
                    Error.InvalidAttribute(null, null, null, "2", "threadPool:10").WithSubResults(
                        Error.MissingEachOption(null, null, null, "threadPool", "2"),
                        Error.MissingIpOption(null, null, null, "threadPool", "2")),
                    Error.InvalidAttribute(null, null, null, "3", "ip:2000,1").WithSubResults(
                        Error.MissingEachOption(null, null, null, "ip", "3"),
                        Error.MissingThreadPoolOption(null, null, null, "ip", "3")),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_MissingIpOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingIpOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "threadPool:10;each:10").WithSubResults(
                        Error.MissingIpOption(null, null, null, "each', 'threadPool", "1")),
                    Error.InvalidAttribute(null, null, null, "2", "threadPool:10").WithSubResults(
                        Error.MissingIpOption(null, null, null, "threadPool", "2"),
                        Error.MissingEachOption(null, null, null, "threadPool", "2")),
                    Error.InvalidAttribute(null, null, null, "3", "each:10").WithSubResults(
                        Error.MissingIpOption(null, null, null, "each", "3"),
                        Error.MissingThreadPoolOption(null, null, null, "each", "3"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_MissingThreadPoolOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingThreadPoolOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "ip:2000,1;each:10").WithSubResults(
                        Error.MissingThreadPoolOption(null, null, null, "each', 'ip", "1")),
                    Error.InvalidAttribute(null, null, null, "2", "ip:2000,1").WithSubResults(
                        Error.MissingThreadPoolOption(null, null, null, "ip", "2"),
                        Error.MissingEachOption(null, null, null, "ip", "2")),
                    Error.InvalidAttribute(null, null, null, "3", "each:10").WithSubResults(
                        Error.MissingThreadPoolOption(null, null, null, "each", "3"),
                        Error.MissingIpOption(null, null, null, "each", "3"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_MissingValueInOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingValueInOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "ip:2000,1;each:1000;threadPool:10;ignoreIf:2").WithSubResults(
                        Error.InvalidIgnoreIfOption(null, null, null, "ignoreIf:2").WithSubResults(
                            Error.MissingValueInOption(null, null, null, "<value>")))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_NonExistingIdInDynamicThreadPoolOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdInDynamicThreadPoolOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "threadPool:10;ip:2000,1;each:100;dynamicThreadPool:100").WithSubResults(
                        Error.NonExistingIdInDynamicThreadPoolOption(null, null, null, "100"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_NonExistingIdInOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdInOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "ip:2000,1;each:1000;threadPool:10;qaction:11").WithSubResults(
                        Error.NonExistingIdInOption(null, null, null, "qaction", "QAction", "11"),
                        Error.UseOfObsoleteQActionOption(null, null, null)),
                    Error.InvalidAttribute(null, null, null, "2", "ip:2000,1;each:1000;threadPool:10;qactionBefore:12").WithSubResults(
                        Error.NonExistingIdInOption(null, null, null, "qactionBefore", "QAction", "12")),
                    Error.InvalidAttribute(null, null, null, "3", "ip:2000,1;each:1000;threadPool:10;qactionAfter:13").WithSubResults(
                        Error.NonExistingIdInOption(null, null, null, "qactionAfter", "QAction", "13"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_ThreadPoolCalculationIntervalDefined()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ThreadPoolCalculationIntervalDefined",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ThreadPoolCalculationIntervalDefined(null, null, null, "1"),
                    Error.ThreadPoolCalculationIntervalDefined(null, null, null, "2"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_UnknownOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnknownOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "threadPool:10;ip:2000,1;each:100;typo:icmp").WithSubResults(
                        Error.UnknownOption(null, null, null, "typo:icmp"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_UnknownOptionInPingOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnknownOptionInPingOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "threadPool:10;ip:2000,1;each:100;ping:aech=200").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:aech=200").WithSubResults(
                            Error.UnknownOptionInPingOption(null, null, null, "aech=200")))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_UseOfObsoleteQActionOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UseOfObsoleteQActionOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "ip:2000,1;each:1000;threadPool:10;qaction:20").WithSubResults(
                        Error.UseOfObsoleteQActionOption(null, null, null))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_UseOfObsoleteTimeoutPidOptionInPingOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UseOfObsoleteTimeoutPidOptionInPingOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "1", "ip:2000,1;each:1000;threadPool:10;ping:timeoutPid=10").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:timeoutPid=10").WithSubResults(
                            Error.UseOfObsoleteTimeoutPidOptionInPingOption(null, null, null)))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_NonExistingColumnIdxInOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingColumnIdxInOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "21", "threadPool:10;each:100;ip:2000,1").WithSubResults(
                        Error.InvalidIpOption(null, null, null, "ip:2000,1").WithSubResults(
                            Error.NonExistingColumnIdxInOption(null, null, null, "<columnIdx>", "1", "2000")))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Timer_CheckOptionsAttribute_NonExistingColumnPositionInOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingColumnPositionInOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "18", "ip:2000,1;each:1000;threadPool:10;ping:jitterColumn=20").WithSubResults(
                        Error.InvalidPingOption(null, null, null, "ping:jitterColumn=20").WithSubResults(
                            Error.NonExistingColumnPositionInOption(null, null, null, "jitterColumn", "20", "2000")))
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckOptionsAttribute();

        [TestMethod]
        public void Timer_CheckOptionsAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Timer);

        [TestMethod]
        public void Timer_CheckOptionsAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckOptionsAttribute);
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Timer_CheckOptionsAttribute_NonExistingIdInDynamicThreadPoolOption()
        {
            // Create ErrorMessage
            var message = Error.NonExistingIdInDynamicThreadPoolOption(null, null, null, "1");

            string description = "Option 'dynamicThreadPool' references a non-existing 'Param' with ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Timers.Timer.CheckOptionsAttribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Helpers;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    using static Skyline.DataMiner.CICD.Models.Protocol.Read.TimerOptions;

    [Test(CheckId.CheckOptionsAttribute, Category.Timer)]
    internal class CheckOptionsAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (ITimersTimer timer in context.EachTimerWithValidId())
            {
                TimerOptions options = timer.GetOptions();
                if (options == null)
                {
                    continue;
                }

                List<IValidationResult> subResults = new List<IValidationResult>();

                foreach (string unknownOption in options.UnknownOptions)
                {
                    subResults.Add(Error.UnknownOption(this, timer, timer, unknownOption));
                }

                foreach (string duplicateOption in options.DuplicateOptions)
                {
                    subResults.Add(Error.DuplicateOption(this, timer, timer, duplicateOption));
                }

                ValidateTimerHelper helper = new ValidateTimerHelper(this, context, timer, results, subResults, options);

                helper.CheckEachOption();
                helper.CheckDynamicThreadPoolOption();

                IParamsParam ipOptionTableParam = helper.CheckIpOption();
                helper.CheckIgnoreIfOption(ipOptionTableParam);
                helper.CheckPingOption(ipOptionTableParam);
                helper.CheckInstanceOption();

                helper.CheckPollingRateOption();

                helper.CheckQActionOption();
                helper.CheckQActionBeforeOption();
                helper.CheckQActionAfterOption();

                helper.CheckThreadPoolOption();

                helper.CheckRequiringIpOption();
                helper.CheckRequiringEachOption();
                helper.CheckRequiringThreadPoolOption();

                if (subResults.Count > 0)
                {
                    IValidationResult invalidAttribute = Error.InvalidAttribute(this, timer, timer, timer.Id.RawValue, options.OriginalValue);
                    invalidAttribute.WithSubResults(subResults.ToArray());
                    results.Add(invalidAttribute);
                }
            }

            return results;
        }

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {

        ////        default:
        ////            result.Message = String.Format("This error ({0}) isn't implemented.", context.Result.ErrorId.ToString());
        ////            break;
        ////    }

        ////    return result;
        ////}

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }

    internal class ValidateTimerHelper : ValidateHelperBase
    {
        private readonly ITimersTimer timer;
        private readonly List<IValidationResult> subResults;
        private readonly TimerOptions options;

        private readonly string timerId;

        private readonly Dictionary<string, IValidationResult> optionsRequiringIpOption = new Dictionary<string, IValidationResult>();
        private readonly Dictionary<string, IValidationResult> optionsRequiringThreadPoolOption = new Dictionary<string, IValidationResult>();
        private readonly Dictionary<string, IValidationResult> optionsRequiringEachOption = new Dictionary<string, IValidationResult>();

        public ValidateTimerHelper(
            IValidate test,
            ValidatorContext context,
            ITimersTimer timer,
            List<IValidationResult> results,
            List<IValidationResult> subResults,
            TimerOptions options)
            : base(test, context, results)
        {
            this.timer = timer;
            this.subResults = subResults;
            this.options = options;

            timerId = timer.Id.RawValue;
        }

        internal void CheckEachOption()
        {
            var eachOption = options.Each;

            if (eachOption == null || options.DuplicateOptions.Contains("each"))
            {
                return;
            }

            VerifyPresenceMandatoryAttributes("each");

            if (String.IsNullOrWhiteSpace(eachOption.OptionValue) || eachOption.Period == null)
            {
                // This means an invalid value was provided that could not be parsed into an unsigned integer.
                subResults.Add(Error.InvalidEachOption(test, timer, timer, eachOption.OriginalValue));
            }
        }

        internal void CheckDynamicThreadPoolOption()
        {
            var dynamicThreadPool = options.DynamicThreadPool;

            if (dynamicThreadPool == null || options.DuplicateOptions.Contains("dynamicThreadPool"))
            {
                // This means the option was specified in the protocol.
                return;
            }

            VerifyPresenceMandatoryAttributes("dynamicThreadPool");

            if (String.IsNullOrWhiteSpace(dynamicThreadPool.OptionValue) || dynamicThreadPool.ParameterId == null)
            {
                // This means an invalid value was provided that could not be parsed into an unsigned integer.
                subResults.Add(Error.InvalidDynamicThreadPoolOption(test, timer, timer, dynamicThreadPool.OriginalValue));
                return;
            }

            string referencedParameterId = dynamicThreadPool.ParameterId.Value.ToString();

            var model = context.ProtocolModel;

            // Verify whether the specified value is correct.
            if (model.TryGetObjectByKey(Mappings.ParamsById, referencedParameterId, out IParamsParam _))
            {
                // TODO: Verify whether the specified parameter is a standalone parameter.
                // This could result in a new error message "InvalidIdInDynamicThreadPoolOption.
            }
            else
            {
                subResults.Add(Error.NonExistingIdInDynamicThreadPoolOption(test, timer, timer, referencedParameterId));
            }
        }

        private void VerifyPresenceMandatoryAttributes(string optionName)
        {
            // The following options are mandatory when working with multi-threaded timers: "ip", "each", "threadPool".
            if (options.IPAddress == null)
            {
                optionsRequiringIpOption.Add(optionName, Error.MissingIpOption(test, timer, timer, optionName, timerId));
            }

            if (options.ThreadPool == null)  // "threadPool" option is mandatory for multi-threaded timers.
            {
                optionsRequiringThreadPoolOption.Add(optionName, Error.MissingThreadPoolOption(test, timer, timer, optionName, timerId));
            }

            if (options.Each == null)
            {
                optionsRequiringEachOption.Add(optionName, Error.MissingEachOption(test, timer, timer, optionName, timerId));
            }
        }

        internal void CheckIgnoreIfOption(IParamsParam ipOptionTableParam)
        {
            var ignoreIf = options.IgnoreIf;

            if (ignoreIf == null || options.DuplicateOptions.Contains("ignoreIf"))
            {
                return;
            }

            VerifyPresenceMandatoryAttributes("ignoreIf");

            if (String.IsNullOrWhiteSpace(ignoreIf.OptionValue))
            {
                subResults.Add(Error.InvalidIgnoreIfOption(test, timer, timer, ignoreIf.OriginalValue));
                return;
            }

            List<IValidationResult> subSubResults = new List<IValidationResult>();

            if (ignoreIf.ColumnIdx == null)
            {
                subSubResults.Add(Error.InvalidValueInOption(test, timer, timer, "<columnIdx>", ignoreIf.OriginalValue));
            }
            else
            {
                // Entering this else clause means a value was specified for this component.
                if (ignoreIf.ColumnIdx.Value == null)
                {
                    // This means an invalid value was provided (which could not be parsed into an unsigned integer).
                    subSubResults.Add(Error.InvalidValueInOption(test, timer, timer, "<columnIdx>", ignoreIf.ColumnIdx.OriginalValue));
                }
                else
                {
                    uint referencedColumnIdx = ignoreIf.ColumnIdx.Value.Value;

                    // Verify whether the specified value is correct.
                    if (!HasColumnIdx(ipOptionTableParam, referencedColumnIdx))
                    {
                        subSubResults.Add(Error.NonExistingColumnIdxInOption(test, timer, timer, "<columnIdx>", Convert.ToString(referencedColumnIdx), Convert.ToString(ipOptionTableParam?.Id?.Value)));
                    }
                }
            }

            if (ignoreIf.Value == null)
            {
                // This means an invalid value was provided that could not be parsed into a column idx and value separated by a comma.
                subSubResults.Add(Error.MissingValueInOption(test, timer, timer, "<value>"));
            }

            if (subSubResults.Count > 0)
            {
                IValidationResult invalidIgnoreIfOption = Error.InvalidIgnoreIfOption(test, timer, timer, ignoreIf.OriginalValue);
                invalidIgnoreIfOption.WithSubResults(subSubResults.ToArray());
                subResults.Add(invalidIgnoreIfOption);
            }
        }

        internal void CheckInstanceOption()
        {
            var instance = options.Instance;

            if (instance == null || options.DuplicateOptions.Contains("instance"))
            {
                return;
            }

            VerifyPresenceMandatoryAttributes("instance");

            if (String.IsNullOrWhiteSpace(instance.OptionValue))
            {
                subResults.Add(Error.InvalidInstanceOption(test, timer, timer, instance.OriginalValue));
                return;
            }

            List<IValidationResult> subSubResults = new List<IValidationResult>();

            IParamsParam referencedTableParam = null;

            if (instance.TableParameterId == null)
            {
                subSubResults.Add(Error.InvalidValueInOption(test, timer, timer, "<tablePid>", instance.OriginalValue));
            }
            else
            {
                if (instance.TableParameterId.Value == null)
                {
                    subSubResults.Add(Error.InvalidValueInOption(test, timer, timer, "<tablePid>", instance.TableParameterId.OriginalValue));
                }
                else // This means a value was specified for this component.
                {
                    if (instance.TableParameterId.Value == null)
                    {
                        // This means an invalid value was provided (which could not be parsed into an unsigned integer).
                        subSubResults.Add(Error.InvalidValueInOption(test, timer, timer, "<tablePid>", instance.TableParameterId.OriginalValue));
                    }
                    else
                    {
                        var referencedParamId = instance.TableParameterId.Value.Value.ToString();
                        var model = context.ProtocolModel;

                        if (model.TryGetObjectByKey(Mappings.ParamsById, referencedParamId, out IParamsParam param))
                        {
                            referencedTableParam = param;
                            // TODO: Check if referenced parameter is a table parameter.
                        }
                        else
                        {
                            subSubResults.Add(Error.NonExistingIdInOption(test, timer, timer.Options, "<tablePid>", "Param", referencedParamId));
                        }
                    }
                }
            }

            if (instance.ColumnIdx == null)
            {
                subSubResults.Add(Error.MissingValueInOption(test, timer, timer, "<columnIdx>"));
            }
            else
            {
                if (instance.ColumnIdx.Value == null)
                {
                    subSubResults.Add(Error.InvalidValueInOption(test, timer, timer, "<columnIdx>", instance.ColumnIdx.OriginalValue));
                }
                else
                {
                    uint referencedColumnIdx = instance.ColumnIdx.Value.Value;

                    // Verify whether the specified value is correct.
                    if (referencedTableParam != null && !HasColumnIdx(referencedTableParam, referencedColumnIdx))
                    {
                        subSubResults.Add(Error.NonExistingColumnIdxInOption(test, timer, timer.Options, "<columnIdx>", instance.ColumnIdx.OriginalValue, referencedTableParam.Id.RawValue));
                    }
                }
            }

            if (subSubResults.Count > 0)
            {
                IValidationResult invalidInstanceOption = Error.InvalidInstanceOption(test, timer, timer, instance.OriginalValue);
                invalidInstanceOption.WithSubResults(subSubResults.ToArray());
                subResults.Add(invalidInstanceOption);
            }
        }

        internal IParamsParam CheckIpOption()
        {
            var ipAddress = options.IPAddress;

            if (ipAddress == null || options.DuplicateOptions.Contains("ip"))
            {
                return null;
            }

            VerifyPresenceMandatoryAttributes("ip");

            if (String.IsNullOrWhiteSpace(ipAddress.OptionValue))
            {
                subResults.Add(Error.InvalidIpOption(test, timer, timer, ipAddress.OriginalValue));
                return null;
            }

            List<IValidationResult> subSubResults = new List<IValidationResult>();

            IParamsParam referencedTableParam = null;
            if (ipAddress.TableParameterId == null)
            {
                subSubResults.Add(Error.InvalidValueInOption(test, timer, timer, "<tablePid>", ipAddress.OptionValue));
            }
            else
            {
                // This means a value was specified for this component.
                if (ipAddress.TableParameterId.Value == null)
                {
                    // This means an invalid value was provided (which could not be parsed into an unsigned integer).
                    subSubResults.Add(Error.InvalidValueInOption(test, timer, timer, "<tablePid>", ipAddress.TableParameterId.OriginalValue));
                }
                else
                {
                    var referencedParamId = ipAddress.TableParameterId.Value.Value.ToString();
                    var model = context.ProtocolModel;

                    if (!model.TryGetObjectByKey(Mappings.ParamsById, referencedParamId, out IParamsParam param))
                    {
                        subSubResults.Add(Error.NonExistingIdInOption(test, timer, timer.Options, "<tablePid>", "Param", referencedParamId));
                    }
                    else
                    {
                        referencedTableParam = param;
                        // TODO: Check if referenced parameter is a table parameter.
                    }
                }
            }

            if (ipAddress.ColumnIdx == null)
            {
                subSubResults.Add(Error.MissingValueInOption(test, timer, timer, "<columnIdx>"));
            }
            else // This means a value was specified for this component.
            {
                if (ipAddress.ColumnIdx.Value == null) // This means an invalid value was provided (which could not be parsed into an unsigned integer).
                {
                    subSubResults.Add(Error.InvalidValueInOption(test, timer, timer, "<columnIdx>", ipAddress.ColumnIdx.OriginalValue));
                }
                else
                {
                    uint referencedColumnIdx = ipAddress.ColumnIdx.Value.Value;

                    // Verify whether the specified value is correct.
                    if (referencedTableParam != null && !HasColumnIdx(referencedTableParam, referencedColumnIdx))
                    {
                        subSubResults.Add(Error.NonExistingColumnIdxInOption(test, timer, timer.Options, "<columnIdx>", ipAddress.ColumnIdx.OriginalValue, referencedTableParam.Id.RawValue));
                    }
                }
            }

            if (subSubResults.Count > 0)
            {
                IValidationResult invalidIpOption = Error.InvalidIpOption(test, timer, timer, ipAddress.OriginalValue);
                invalidIpOption.WithSubResults(subSubResults.ToArray());
                subResults.Add(invalidIpOption);
            }

            return referencedTableParam;
        }

        internal void CheckPingOption(IParamsParam ipOptionTableParam)
        {
            var ping = options.Ping;

            if (ping == null || options.DuplicateOptions.Contains("ping"))  // This means the option was specified in the protocol.
            {
                return;
            }

            VerifyPresenceMandatoryAttributes("ping");

            if (String.IsNullOrWhiteSpace(ping.OptionValue))
            {
                subResults.Add(Error.InvalidPingOption(test, timer, timer, ping.OriginalValue));
                return;
            }

            List<IValidationResult> subSubResults = new List<IValidationResult>();

            foreach (string unknownOption in ping.UnknownOptions)
            {
                subSubResults.Add(Error.UnknownOptionInPingOption(test, timer, timer, unknownOption));
            }

            foreach (string duplicateOption in ping.DuplicateOptions)
            {
                subSubResults.Add(Error.DuplicateOptionInPingOption(test, timer, timer, duplicateOption));
            }

            CheckColumnReferenceValue(ipOptionTableParam, ping.RttColumnPosition, subSubResults, "rttColumn", 1, ping.DuplicateOptions);
            CheckParameterIdValue(ping.TimeoutParameterId, subSubResults, "timeoutPid", null, true, ping.DuplicateOptions);

            if (ping.TimeoutParameterId != null)
            {
                subSubResults.Add(Error.UseOfObsoleteTimeoutPidOptionInPingOption(test, timer, timer));
            }

            subSubResults.AddIfNotNull(CheckUIntValue(ping.TimeToLive, "ttl", true, duplicateOptions: ping.DuplicateOptions));
            subSubResults.AddIfNotNull(CheckUIntValue(ping.Timeout, "timeout", true, duplicateOptions: ping.DuplicateOptions));
            CheckColumnReferenceValue(ipOptionTableParam, ping.TimeStampColumnPosition, subSubResults, "timestampColumn", 1, ping.DuplicateOptions);
            CheckDiscreteValue(ping.PingType, subSubResults, "type", true, new string[] { "ICMP", "WINSOCK" }, ping.DuplicateOptions);
            subSubResults.AddIfNotNull(CheckUIntValue(ping.PayloadSize, "size", true, duplicateOptions: ping.DuplicateOptions));
            CheckDiscreteValue(ping.ContinueSnmpOnTimeout, subSubResults, "continueSnmpOnTimeout", true, new string[] { "TRUE", "FALSE" }, ping.DuplicateOptions);
            CheckColumnReferenceValue(ipOptionTableParam, ping.JitterColumnPosition, subSubResults, "jitterColumn", 1, ping.DuplicateOptions);
            CheckColumnReferenceValue(ipOptionTableParam, ping.LatencyColumnPosition, subSubResults, "latencyColumn", 1, ping.DuplicateOptions);
            CheckColumnReferenceValue(ipOptionTableParam, ping.PacketLossRateColumnPosition, subSubResults, "packetLossRateColumn", 1, ping.DuplicateOptions);
            subSubResults.AddIfNotNull(CheckUIntValue(ping.AmountPackets, "amountPackets", true, duplicateOptions: ping.DuplicateOptions));
            CheckParameterIdValue(ping.AmountPacketsPid, subSubResults, "amountPacketsPid", null, true, ping.DuplicateOptions);
            subSubResults.AddIfNotNull(CheckUIntValue(ping.AmountPacketsMeasurements, "amountPacketsMeasurements", true, duplicateOptions: ping.DuplicateOptions));
            CheckParameterIdValue(ping.AmountPacketsMeasurementsPid, subSubResults, "amountPacketsMeasurementsPid", null, true, ping.DuplicateOptions);
            subSubResults.AddIfNotNull(CheckUIntValue(ping.ExcludeWorstResults, "excludeWorstResults", true, duplicateOptions: ping.DuplicateOptions, lowLimit: 0, highLimit: 100));
            CheckParameterIdValue(ping.ExcludeWorstResultsParameterId, subSubResults, "excludeWorstResultsPid", null, true, ping.DuplicateOptions);

            if (subSubResults.Count > 0)
            {
                IValidationResult invalidPingOption = Error.InvalidPingOption(test, timer, timer, ping.OriginalValue);
                invalidPingOption.WithSubResults(subSubResults.ToArray());
                subResults.Add(invalidPingOption);
            }
        }

        internal void CheckPollingRateOption()
        {
            var pollingRateOption = options.PollingRate;

            if (pollingRateOption == null || options.DuplicateOptions.Contains("pollingRate"))
            {
                return;
            }

            VerifyPresenceMandatoryAttributes("pollingRate");

            if (String.IsNullOrEmpty(pollingRateOption.OptionValue))
            {
                subResults.Add(Error.InvalidPollingRateOption(test, timer, timer, pollingRateOption.OriginalValue));
                return;
            }

            List<IValidationResult> subSubErrors = new List<IValidationResult>();

            subSubErrors.AddIfNotNull(CheckUIntValue(pollingRateOption.Interval, "<interval>", false));
            subSubErrors.AddIfNotNull(CheckUIntValue(pollingRateOption.MaxCount, "<maxCount>", false));
            subSubErrors.AddIfNotNull(CheckUIntValue(pollingRateOption.ReleaseCount, "<releaseCount>", false));

            if (subSubErrors.Count > 0)
            {
                // This means an invalid value was provided that could not be parsed into an unsigned integer.
                IValidationResult invalidPollingRateOption = Error.InvalidPollingRateOption(test, timer, timer, pollingRateOption.OriginalValue);
                invalidPollingRateOption.WithSubResults(subSubErrors.ToArray());
                subResults.Add(invalidPollingRateOption);
            }
        }

        internal void CheckQActionOption()
        {
            var qAction = options.QAction;

            if (qAction == null || options.DuplicateOptions.Contains("qaction"))
            {
                return;
            }

            VerifyPresenceMandatoryAttributes("qaction");

            if (qAction.Id == null)
            {
                return;
            }

            // This means a value was specified for this component.
            subResults.Add(Error.UseOfObsoleteQActionOption(test, timer, timer));

            if (String.IsNullOrWhiteSpace(qAction.Id.OriginalValue) || qAction.Id.Value == null)
            {
                // This means an invalid value was provided (which could not be parsed into an unsigned integer).
                subResults.Add(Error.InvalidQActionOption(test, timer, timer, qAction.OriginalValue));
                return;
            }

            uint referencedQAction = qAction.Id.Value.Value;
            var model = context.ProtocolModel;

            // Verify whether the parameter exists.
            if (!model.TryGetObjectByKey<IQActionsQAction>(Mappings.QActionsById, referencedQAction.ToString(), out _))
            {
                subResults.Add(Error.NonExistingIdInOption(test, timer, timer, "qaction", "QAction", qAction.Id.OriginalValue));
            }
        }

        internal void CheckQActionAfterOption()
        {
            var qAction = options.QActionAfter;

            if (qAction == null || options.DuplicateOptions.Contains("qactionAfter"))
            {
                return;
            }

            VerifyPresenceMandatoryAttributes("qactionAfter");

            if (qAction.Id == null)
            {
                return;
            }

            // This means a value was specified for this component.
            if (String.IsNullOrWhiteSpace(qAction.Id.OriginalValue) || qAction.Id.Value == null)
            {
                // This means an invalid value was provided (which could not be parsed into an unsigned integer).
                subResults.Add(Error.InvalidQActionAfterOption(test, timer, timer, qAction.OriginalValue));
                return;
            }

            uint referencedQAction = qAction.Id.Value.Value;
            var model = context.ProtocolModel;

            // Verify whether the parameter exists.
            if (!model.TryGetObjectByKey<IQActionsQAction>(Mappings.QActionsById, referencedQAction.ToString(), out _))
            {
                subResults.Add(Error.NonExistingIdInOption(test, timer, timer, "qactionAfter", "QAction", qAction.Id.OriginalValue));
            }
        }

        internal void CheckQActionBeforeOption()
        {
            var qAction = options.QActionBefore;

            if (qAction == null || options.DuplicateOptions.Contains("qactionBefore"))
            {
                return;
            }

            VerifyPresenceMandatoryAttributes("qactionBefore");

            if (qAction.Id == null)
            {
                return;
            }

            // This means a value was specified for this component.
            if (String.IsNullOrWhiteSpace(qAction.Id.OriginalValue) || qAction.Id.Value == null)
            {
                // This means an invalid value was provided (which could not be parsed into an unsigned integer).
                subResults.Add(Error.InvalidQActionBeforeOption(test, timer, timer, qAction.OriginalValue));
                return;
            }

            uint referencedQAction = qAction.Id.Value.Value;
            var model = context.ProtocolModel;

            // Verify whether the parameter exists.
            if (!model.TryGetObjectByKey<IQActionsQAction>(Mappings.QActionsById, referencedQAction.ToString(), out _))
            {
                subResults.Add(Error.NonExistingIdInOption(test, timer, timer, "qactionBefore", "QAction", qAction.Id.OriginalValue));
            }
        }

        internal void CheckThreadPoolOption()
        {
            var threadPool = options.ThreadPool;

            if (threadPool == null || options.DuplicateOptions.Contains("threadPool"))
            {
                return;
            }

            VerifyPresenceMandatoryAttributes("threadPool");

            if (String.IsNullOrWhiteSpace(threadPool.OptionValue))
            {
                subResults.Add(Error.InvalidThreadPoolOption(test, timer, timer, threadPool.OriginalValue));
                return;
            }

            List<IValidationResult> subSubResults = new List<IValidationResult>();

            var allowedOtherValues = new List<string> { "-1" };

            subSubResults.AddIfNotNull(CheckUIntValue(threadPool.Size, "<size>", false));

            if (threadPool.CalculationInteral?.Value != null)
            {
                results.Add(Error.ThreadPoolCalculationIntervalDefined(test, timer, timer, timerId));
            }

            CheckParameterIdValue(threadPool.UsageParameterId, subSubResults, "<usagePid>", allowedOtherValues, true);
            CheckParameterIdValue(threadPool.WaitingParameterId, subSubResults, "<waitingPid>", allowedOtherValues, true);
            CheckParameterIdValue(threadPool.MaxDurationParameterId, subSubResults, "<maxDurationPid>", allowedOtherValues, true);
            CheckParameterIdValue(threadPool.AverageDurationParameterId, subSubResults, "<avgDurationPid>", allowedOtherValues, true);
            CheckParameterIdValue(threadPool.CounterParameterId, subSubResults, "<counterPid>", allowedOtherValues, true);

            string optionalValue = null;

            if (AreStatisticParametersDefined())
            {
                optionalValue = "-1";
            }

            subSubResults.AddIfNotNull(CheckUIntValue(threadPool.CalculationInteral, "<calculationInterval>", true, optionalValue));
            subSubResults.AddIfNotNull(CheckUIntValue(threadPool.QueueSize, "<queueSize>", true));

            if (subSubResults.Count > 0)
            {
                IValidationResult subResult = Error.InvalidThreadPoolOption(test, timer, timer, threadPool.OriginalValue);
                subResult.WithSubResults(subSubResults.ToArray());
                subResults.Add(subResult);
            }
        }

        private bool AreStatisticParametersDefined()
        {
            var threadPool = options.ThreadPool;

            return threadPool.UsageParameterId != null ||
                   threadPool.WaitingParameterId != null ||
                   threadPool.MaxDurationParameterId != null ||
                   threadPool.AverageDurationParameterId != null ||
                   threadPool.CounterParameterId != null;
        }

        private IValidationResult CheckUIntValue(WrappedNullableUInt32 optionDenotingNumericValue, string optionName, bool isOptional, string exceptionValue = null, ICollection<string> duplicateOptions = null, uint? lowLimit = null, uint? highLimit = null)
        {
            if (optionDenotingNumericValue == null) // No value specified for this component.
            {
                if (!isOptional)
                {
                    return Error.MissingValueInOption(test, timer, timer, optionName);
                }

                return null;
            }

            if (duplicateOptions?.Contains(optionName) == true)
            {
                return null;
            }

            // A value was specified.
            if (optionDenotingNumericValue.Value == null)
            {
                if (exceptionValue == null || !optionDenotingNumericValue.OriginalValue.Equals(exceptionValue))
                {
                    // This means an invalid value was provided (which could not be parsed into an unsigned integer).
                    return Error.InvalidValueInOption(test, timer, timer, optionName, optionDenotingNumericValue.OriginalValue);
                }
            }
            else
            {
                uint value = optionDenotingNumericValue.Value.Value;

                if (value < lowLimit || value > highLimit)
                {
                    return Error.InvalidValueInOption(test, timer, timer, optionName, optionDenotingNumericValue.OriginalValue);
                }
            }

            return null;
        }

        private void CheckDiscreteValue(string optionValue, ICollection<IValidationResult> subSubResults, string optionName, bool isOptional, IEnumerable<string> possibleValues, ICollection<string> duplicateOptions = null)
        {
            if (optionValue == null) // No value specified for this component.
            {
                if (!isOptional)
                {
                    subSubResults.Add(Error.MissingValueInOption(test, timer, timer, optionName));
                }
            }
            else // A value was specified.
            {
                if (duplicateOptions?.Contains(optionName) != true && !possibleValues.Any(s => s.Equals(optionValue, StringComparison.OrdinalIgnoreCase)))
                {
                    // This means an invalid value was provided (which could not be parsed into an unsigned integer).
                    subSubResults.Add(Error.InvalidValueInOption(test, timer, timer, optionName, optionValue));
                }
            }
        }

        private void CheckParameterIdValue(WrappedNullableUInt32 optionDenotingParameterId, ICollection<IValidationResult> subSubResults, string optionName, IReadOnlyCollection<string> otherAllowedRawValues, bool isOptional, ISet<string> duplicateOptions = null)
        {
            if (optionDenotingParameterId == null)
            {
                if (!isOptional)
                {
                    subSubResults.Add(Error.MissingValueInOption(test, timer, timer, optionName));
                }

                return;
            }

            // This means a value was specified for this component.
            if (duplicateOptions != null && duplicateOptions.Contains(optionName))
            {
                return;
            }

            if (optionDenotingParameterId.Value == null)
            {
                // This means an invalid value was provided (which could not be parsed into an unsigned integer).
                if (!(otherAllowedRawValues != null && otherAllowedRawValues.Contains(optionDenotingParameterId.OriginalValue)))
                {
                    // This means the provided value is also not an allowed other value (e.g. -1).
                    subSubResults.Add(Error.InvalidValueInOption(test, timer, timer, optionName, optionDenotingParameterId.OriginalValue));
                }

                return;
            }

            uint referencedParameter = optionDenotingParameterId.Value.Value;

            var model = context.ProtocolModel;

            // Verify whether the parameter exists.
            if (!model.TryGetObjectByKey(Mappings.ParamsById, referencedParameter.ToString(), out IParamsParam _))
            {
                subSubResults.Add(Error.NonExistingIdInOption(test, timer, timer.Options, optionName, "Param", optionDenotingParameterId.OriginalValue));
            }
        }

        private void CheckColumnReferenceValue(IParamsParam tableParam, WrappedNullableUInt32 optionDenotingRef, ICollection<IValidationResult> subSubResults, string optionName, int startIndex, IEnumerable<string> duplicateOptions = null)
        {
            if (optionDenotingRef == null || duplicateOptions?.Contains("rttColumn") == true)
            {
                return;
            }

            // This means a single value was specified for this component.
            if (optionDenotingRef.Value == null)
            {
                // This means an invalid value was provided (which could not be parsed into an unsigned integer).
                subSubResults.Add(Error.InvalidValueInOption(test, timer, timer, optionName, optionDenotingRef.OriginalValue));
                return;
            }

            if (startIndex == 0)
            {
                uint referencedColumnIdx = optionDenotingRef.Value.Value;

                // Verify whether the specified value is correct.
                if (!HasColumnIdx(tableParam, referencedColumnIdx))
                {
                    subSubResults.Add(Error.NonExistingColumnIdxInOption(test, timer, timer.Options, optionName, Convert.ToString(referencedColumnIdx), tableParam.Id.RawValue));
                }
            }
            else
            {
                uint referencedColumnPos = optionDenotingRef.Value.Value;

                // Verify whether the specified value is correct.
                if (!HasColumnIdx(tableParam, referencedColumnPos - 1))
                {
                    subSubResults.Add(Error.NonExistingColumnPositionInOption(test, timer, timer.Options, optionName, Convert.ToString(referencedColumnPos), tableParam.Id.RawValue));
                }
            }
        }

        private static bool HasColumnIdx(IParamsParam param, uint columnIdx)
        {
            if (param?.ArrayOptions == null)
            {
                return false;
            }

            return param.ArrayOptions.Any(columnOption => columnOption.Idx?.Value == columnIdx);
        }

        public void CheckRequiringIpOption()
        {
            if (optionsRequiringIpOption.Count <= 0)
            {
                return;
            }

            if (optionsRequiringIpOption.Count == 1)
            {
                subResults.Add(optionsRequiringIpOption.Values.First());
                return;
            }

            string optionNames = String.Join("', '", optionsRequiringIpOption.Keys);

            // No subResults as nesting would be too deep.
            subResults.Add(Error.MissingIpOption(test, timer, timer, optionNames, timerId));
        }

        public void CheckRequiringEachOption()
        {
            if (optionsRequiringEachOption.Count <= 0)
            {
                return;
            }

            if (optionsRequiringEachOption.Count == 1)
            {
                subResults.Add(optionsRequiringEachOption.Values.First());
                return;
            }

            string optionNames = String.Join("', '", optionsRequiringEachOption.Keys);

            // No subResults as nesting would be too deep.
            subResults.Add(Error.MissingEachOption(test, timer, timer, optionNames, timerId));
        }

        public void CheckRequiringThreadPoolOption()
        {
            if (optionsRequiringThreadPoolOption.Count <= 0)
            {
                return;
            }

            if (optionsRequiringThreadPoolOption.Count == 1)
            {
                subResults.Add(optionsRequiringThreadPoolOption.Values.First());
                return;
            }

            string optionNames = String.Join("', '", optionsRequiringThreadPoolOption.Keys);

            // No subResults as nesting would be too deep.
            subResults.Add(Error.MissingThreadPoolOption(test, timer, timer, optionNames, timerId));
        }
    }
}
namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Timers.Timer.Time.CheckTimeTag
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckTimeTag, Category.Timer)]
    internal class CheckTimeTag : IValidate, ICodeFix
    {
        private const uint maxAllowedTimerTimeValue = 2073600000; // +- 24 Days

        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            SortedDictionary<uint, List<ITimersTimer>> normalTimersByTime = new SortedDictionary<uint, List<ITimersTimer>>();
            List<ITimersTimer> loopTimers = new List<ITimersTimer>();
            var timers = context.ProtocolModel?.Protocol?.Timers;
            ValidateHelper helper = new ValidateHelper(this);
            foreach (ITimersTimer timer in context.EachTimerWithValidId())
            {
                (GenericStatus status, string rawValue, string value) = GenericTests.CheckBasics(timer.Time, true);

                // Check if tag is missing
                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingTag(this, timer, timer, timer.Id.RawValue));
                    continue;
                }

                // Check if tag is empty
                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyTag(this, timer, timer.Time, timer.Id.RawValue));
                    continue;
                }

                if (String.Equals(value, "loop", StringComparison.OrdinalIgnoreCase))
                {
                    loopTimers.Add(timer);
                }
                else
                {
                    // Check if value is valid
                    if (!UInt32.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out uint timerTimeValueInt) || timerTimeValueInt == 0)
                    {
                        results.Add(Error.InvalidTagValue(this, timer, timer.Time, rawValue, "loop, 1-" + maxAllowedTimerTimeValue));
                        continue;
                    }

                    if (timerTimeValueInt < 1000)
                    {
                        results.Add(Error.TooFastTimer(this, timer, timer.Time, rawValue, timer.Id.RawValue));
                    }
                    else if (timerTimeValueInt > maxAllowedTimerTimeValue)
                    {
                        results.Add(Error.TimerTimeCannotBeLargerThan24Days(this, timer, timer.Time, rawValue, timer.Id.RawValue));
                    }

                    // At this point, the specified value should be valid.
                    if (ValidateHelper.IsMultithreadedTimer(timer))
                    {
                        // Validate multi threaded timer
                    }
                    else
                    {
                        if (!normalTimersByTime.ContainsKey(timerTimeValueInt))
                        {
                            normalTimersByTime.Add(timerTimeValueInt, new List<ITimersTimer>());
                        }

                        normalTimersByTime[timerTimeValueInt].Add(timer);
                    }
                }

                // Check if tag starts/ends with whitespace
                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedTag(this, timer, timer.Time, timer.Id.RawValue, rawValue));
                    continue;
                }
            }

            // Check for duplicated loop timers
            if (loopTimers.Count > 1)
            {
                results.Add(helper.MakeDuplicateTimersError(timers, "loop", loopTimers));
            }

            if (normalTimersByTime.Count <= 0)
            {
                return results;
            }

            // Check for duplicated normal timers
            foreach (KeyValuePair<uint, List<ITimersTimer>> kvp in normalTimersByTime)
            {
                uint timerTime = kvp.Key;
                List<ITimersTimer> duplicateTimers = kvp.Value;

                if (duplicateTimers.Count > 1)
                {
                    results.Add(helper.MakeDuplicateTimersError(timers, Convert.ToString(timerTime), duplicateTimers));
                }
            }

            // Check for too similar normal timers.
            List<ITimersTimer> similarTimers = normalTimersByTime.ElementAt(0).Value;
            bool foundSimilarTimers = false;    // This boolean is to make sure we don't match duplicate timers also as too similar timers.

            for (int i = 0; i < normalTimersByTime.Count - 1; i++)
            {
                var currentTimer = normalTimersByTime.ElementAt(i);
                var nextTimer = normalTimersByTime.ElementAt(i + 1);

                // Initialize with the first non too fast timer
                if (currentTimer.Key < 1000)
                {
                    // We ignore every timer that is faster than 1s
                    similarTimers = new List<ITimersTimer>(nextTimer.Value);
                    continue;
                }

                // Compare
                if ((nextTimer.Key - currentTimer.Key) < 1000)
                {
                    similarTimers.AddRange(nextTimer.Value);

                    if (currentTimer.Key != nextTimer.Key)
                    {
                        foundSimilarTimers = true;
                    }
                }
                else
                {
                    if (foundSimilarTimers)
                    {
                        results.Add(helper.MakeSimilarTimersError(timers, similarTimers));
                        foundSimilarTimers = false;     // Reset for next similar group search
                    }

                    similarTimers = new List<ITimersTimer>(nextTimer.Value);     // Reset for next similar group search
                }
            }

            if (foundSimilarTimers)
            {
                results.Add(helper.MakeSimilarTimersError(timers, similarTimers));
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol.Timers == null)
            {
                result.Message = "No Timers found!";
                return result;
            }

            var readNode = (ITimersTimer)context.Result.ReferenceNode;
            var editNode = context.Protocol.Timers.Get(readNode);

            switch (context.Result.ErrorId)
            {
                case ErrorIds.UntrimmedTag:
                    if (editNode.Time != null)
                    {
                        editNode.Time.Value = readNode.Time.Value.Trim();
                        result.Success = true;
                    }

                    break;

                case ErrorIds.TimerTimeCannotBeLargerThan24Days:
                    if (editNode.Time != null)
                    {
                        editNode.Time.Value = Convert.ToString(maxAllowedTimerTimeValue);
                        result.Success = true;
                    }

                    break;

                default:
                    {
                        result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                        break;
                    }
            }

            return result;
        }
    }

    internal class ValidateHelper
    {
        private readonly IValidate test;

        public ValidateHelper(IValidate test)
        {
            this.test = test;
        }

        public IValidationResult MakeDuplicateTimersError(ITimers timers, string timerTime, List<ITimersTimer> duplicateTimers)
        {
            List<IValidationResult> subResults = new List<IValidationResult>(duplicateTimers.Count);

            List<uint?> ids = new List<uint?>(duplicateTimers.Count);
            foreach (ITimersTimer duplicateTimer in duplicateTimers)
            {
                ids.Add(duplicateTimer.Id?.Value);

                subResults.Add(Error.DuplicateTimer(test, duplicateTimer, duplicateTimer, timerTime, duplicateTimer.Id?.Value?.ToString()));
            }

            string timerIds = String.Join(", ", ids);
            IValidationResult error = Error.DuplicateTimer(test, timers, timers, timerTime, timerIds);
            error.WithSubResults(subResults.ToArray());
            return error;
        }

        public IValidationResult MakeSimilarTimersError(ITimers timers, List<ITimersTimer> similarTimers)
        {
            List<IValidationResult> subResults = new List<IValidationResult>(similarTimers.Count);
            List<uint?> similarTimerIds = new List<uint?>(similarTimers.Count);
            List<string> similarTimerTimes = new List<string>(similarTimers.Count);

            foreach (ITimersTimer similarTimer in similarTimers)
            {
                subResults.Add(Error.TooSimilarTimers(test, similarTimer, similarTimer, similarTimer.Id?.Value?.ToString(), similarTimer.Time?.Value?.ToString()));
                similarTimerIds.Add(similarTimer.Id?.Value);
                similarTimerTimes.Add(similarTimer.Time?.Value);
            }

            IValidationResult error = Error.TooSimilarTimers(test, timers, timers, String.Join(", ", similarTimerIds), String.Join(", ", similarTimerTimes));
            error.WithSubResults(subResults.ToArray());
            return error;
        }

        public static bool IsMultithreadedTimer(ITimersTimer timer)
        {
            var options = timer?.GetOptions();

            if (options == null)
            {
                return false;
            }

            return options.Each != null || options.ThreadPool != null || options.IPAddress != null;
        }
    }
}
namespace ProtocolTests.Protocol.Timers.Timer.Time.CheckTimeTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Timers.Timer.Time.CheckTimeTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckTimeTag();

        #region Valid Checks

        [TestMethod]
        public void Timer_CheckTimeTag_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Timer_CheckTimeTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Timer_CheckTimeTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Timer_CheckTimeTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "1", " 1000  "),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Timer_CheckTimeTag_UntrimmedAndInvalid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAndInvalid",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidTagValue(null, null, null, " 0  ", "loop, 1-2073600000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Timer_CheckTimeTag_InvalidTagValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidTagValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidTagValue(null, null, null, "lalala", "loop, 1-2073600000"),
                    Error.InvalidTagValue(null, null, null, "0", "loop, 1-2073600000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Timer_CheckTimeTag_TimerTimeCannotBeLargerThan24Days()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "TimerTimeCannotBeLargerThan24Days",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.TimerTimeCannotBeLargerThan24Days(null, null, null, "2073600001", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Timer_CheckTimeTag_DuplicateTimer()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateTimer",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateTimer(null, null, null, "loop", "1001, 1002").WithSubResults(
                        Error.DuplicateTimer(null, null, null, "loop", "1001"),
                        Error.DuplicateTimer(null, null, null, "loop", "1002")),
                    Error.DuplicateTimer(null, null, null, "60000", "1, 2").WithSubResults(
                        Error.DuplicateTimer(null, null, null, "60000", "1"),
                        Error.DuplicateTimer(null, null, null, "60000", "2")),
                    Error.DuplicateTimer(null, null, null, "300000", "11, 12, 13").WithSubResults(
                        Error.DuplicateTimer(null, null, null, "300000", "11"),
                        Error.DuplicateTimer(null, null, null, "300000", "12"),
                        Error.DuplicateTimer(null, null, null, "300000", "13")),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Timer_CheckTimeTag_TooFastTimer()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "TooFastTimer",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.TooFastTimer(null, null, null, "500", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Timer_CheckTimeTag_TooSimilarTimers()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "TooSimilarTimers",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.TooSimilarTimers(null, null, null, "1, 2, 3", "1000, 1001, 2000").WithSubResults(
                        Error.TooSimilarTimers(null, null, null, "1", "1000"),
                        Error.TooSimilarTimers(null, null, null, "2", "1001"),
                        Error.TooSimilarTimers(null, null, null, "3", "2000")),
                    Error.TooSimilarTimers(null, null, null, "11, 12, 13", "5000, 5099, 6050").WithSubResults(
                        Error.TooSimilarTimers(null, null, null, "11", "5000"),
                        Error.TooSimilarTimers(null, null, null, "12", "5099"),
                        Error.TooSimilarTimers(null, null, null, "13", "6050"))
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckTimeTag();

        [TestMethod]
        public void Timer_CheckTimeTag_UntrimmedTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedTag",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Timer_CheckTimeTag_TimerTimeCannotBeLargerThan24Days()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "TimerTimeCannotBeLargerThan24Days",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckTimeTag();

        [TestMethod]
        public void Timer_CheckTimeTag_CheckCategory() => Generic.CheckCategory(root, Category.Timer);

        [TestMethod]
        public void Timer_CheckTimeTag_CheckId() => Generic.CheckId(root, CheckId.CheckTimeTag);
    }
}
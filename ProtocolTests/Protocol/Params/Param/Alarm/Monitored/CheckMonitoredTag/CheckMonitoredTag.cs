namespace ProtocolTests.Protocol.Params.Param.Alarm.Monitored.CheckMonitoredTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.Monitored.CheckMonitoredTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckMonitoredTag();

        #region Valid
        [TestMethod]
        public void Param_CheckMonitoredTag_Valid()
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

        #region Invalid
        [TestMethod]
        public void Param_CheckMonitoredTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "100"),    // Empty
                    Error.EmptyTag(null, null, null, "101"),    // Spaces
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckMonitoredTag_InvalidTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidTag(null, null, null, "invalidValue", "100"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckMonitoredTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "100"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Param_CheckMonitoredTag_RTDisplayExpected()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "101"),
                    Error.RTDisplayExpected(null, null, null, "102"),
                    Error.RTDisplayExpected(null, null, null, "103"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckMonitoredTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "100", " true"),

                    Error.UntrimmedTag(null, null, null, "200", "false "),
                    Error.UntrimmedTag(null, null, null, "201", " false "),
                }
            };

            Generic.Validate(test, data);
        }
        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckMonitoredTag();

        [TestMethod]
        public void Param_CheckMonitoredTag_UntrimmedTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedTag",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckMonitoredTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckMonitoredTag_Valid()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckMonitoredTag_RemovedAlarming()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedAlarming",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedAlarming(null, null, "1"),
                    ErrorCompare.RemovedAlarming(null, null, "2"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckMonitoredTag();

        [TestMethod]
        public void Param_CheckMonitoredTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckMonitoredTag_CheckId() => Generic.CheckId(root, CheckId.CheckMonitoredTag);
    }
}
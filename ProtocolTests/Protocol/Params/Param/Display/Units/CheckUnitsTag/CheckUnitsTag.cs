namespace ProtocolTests.Protocol.Params.Param.Display.Units.CheckUnitsTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Units.CheckUnitsTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckUnitsTag();

        [TestMethod]
        public void Param_CheckUnitsTag_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckUnitsTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "1"),
                    Error.EmptyTag(null, null, null, "2"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckUnitsTag_InvalidTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidTag(null, null, null, "aaf", "1"),
                    Error.InvalidTag(null, null, null, "bbx", "2"),
                    Error.InvalidTag(null, null, null, "bbx", "3"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckUnitsTag_OutdatedValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "OutdatedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.OutdatedValue(null, null, null, "am/yr", "am/Year", "2"),
                    Error.OutdatedValue(null, null, null, "am/yr", "am/Year", "3"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckUnitsTag_UnsupportedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnsupportedTag(null, null, null, "button", "1"),
                    Error.UnsupportedTag(null, null, null, "chart", "2"),
                    Error.UnsupportedTag(null, null, null, "digital threshold", "3"),
                    Error.UnsupportedTag(null, null, null, "discreet", "4"),
                    Error.UnsupportedTag(null, null, null, "matrix", "5"),
                    Error.UnsupportedTag(null, null, null, "pagebutton", "6"),
                    Error.UnsupportedTag(null, null, null, "string", "8"),
                    Error.UnsupportedTag(null, null, null, "table", "9"),
                    Error.UnsupportedTag(null, null, null, "title", "10"),
                    Error.UnsupportedTag(null, null, null, "togglebutton", "11"),
                    Error.UnsupportedTag(null, null, null, "table", "1000"),
                    Error.UnsupportedTag(null, null, null, "togglebutton", "1002"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckUnitsTag_ExcessiveTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ExcessiveTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ExcessiveTag(null, null, null, "Units", "missing Measurement tag", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckUnitsTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "number", "2"),
                    Error.MissingTag(null, null, null, "analog", "4"),
                    Error.MissingTag(null, null, null, "progress", "5"),
                    Error.MissingTag(null, null, null, "number", "102"),
                    Error.MissingTag(null, null, null, "analog", "104"),
                    Error.MissingTag(null, null, null, "progress", "105"),
                    Error.MissingTag(null, null, null, "number", "202"),
                    Error.MissingTag(null, null, null, "number", "252"),
                    Error.MissingTag(null, null, null, "analog", "204"),
                    Error.MissingTag(null, null, null, "analog", "254"),
                    Error.MissingTag(null, null, null, "progress", "205"),
                    Error.MissingTag(null, null, null, "progress", "255"),
                    Error.MissingTag(null, null, null, "number", "1002"),
                    Error.MissingTag(null, null, null, "analog", "1003"),
                    Error.MissingTag(null, null, null, "progress", "1004"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckUnitsTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "1", " %"),
                    Error.UntrimmedTag(null, null, null, "2", "% "),
                    Error.UntrimmedTag(null, null, null, "3", " % "),
                }
            };

            Generic.Validate(test, data);
        }
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckUnitsTag();

        [TestMethod]
        public void Param_CheckUnitsTag_OutdatedValue()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "OutdatedValue",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckUnitsTag_UntrimmedTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedTag",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckUnitsTag();

        [TestMethod]
        public void Param_CheckUnitsTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckUnitsTag_CheckId() => Generic.CheckId(root, CheckId.CheckUnitsTag);
    }
}
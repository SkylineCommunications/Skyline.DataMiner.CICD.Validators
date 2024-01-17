namespace ProtocolTests.Protocol.SNMP.CheckSnmpTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.SNMP.CheckSnmpTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckSnmpTag();

        [TestMethod]
        public void Protocol_CheckSnmpTag_Valid()
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
        public void Protocol_CheckSnmpTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckSnmpTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckSnmpTag_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "abas", "auto, false"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckSnmpTag_InvalidTrueValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidTrueValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "true", "auto, false"),
                }
            };

            Generic.Validate(test, data);
        }
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckSnmpTag();

        [TestMethod]
        public void Protocol_CheckSnmpTag_MissingTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingTag",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Protocol_CheckSnmpTag_EmptyTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyTag",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Protocol_CheckSnmpTag_InvalidValue()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "InvalidValue",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Protocol_CheckSnmpTag_InvalidTrueValue()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "InvalidTrueValue",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckSnmpTag();

        [TestMethod]
        public void Protocol_CheckSnmpTag_CheckCategory() => Generic.CheckCategory(root, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckSnmpTag_CheckId() => Generic.CheckId(root, CheckId.CheckSnmpTag);
    }
}
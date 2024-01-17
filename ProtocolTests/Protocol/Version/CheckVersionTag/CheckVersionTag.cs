namespace ProtocolTests.Protocol.Version.CheckVersionTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Version.CheckVersionTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckVersionTag();

        [TestMethod]
        public void Protocol_CheckVersionTag_Valid()
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
        public void Protocol_CheckVersionTag_MissingTag()
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
        public void Protocol_CheckVersionTag_EmptyTag()
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
        public void Protocol_CheckVersionTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, " 1.0.0.1 "),
                }
            };

            Generic.Validate(test, data);
        }
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckVersionTag();

        [TestMethod]
        public void Protocol_CheckVersionTag_MissingTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingTag",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Protocol_CheckVersionTag_EmptyTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyTag",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Protocol_CheckVersionTag_UntrimmedTag()
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
        private readonly IRoot root = new CheckVersionTag();

        [TestMethod]
        public void Protocol_CheckVersionTag_CheckCategory() => Generic.CheckCategory(root, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckVersionTag_CheckId() => Generic.CheckId(root, CheckId.CheckVersionTag);
    }
}
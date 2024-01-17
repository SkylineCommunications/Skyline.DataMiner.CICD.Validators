namespace ProtocolTests.Protocol.Provider.CheckProviderTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Provider.CheckProviderTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckProviderTag();

        [TestMethod]
        public void Protocol_CheckProviderTag_Valid()
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
        public void Protocol_CheckProviderTag_ValidSkyline()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidSkyline",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckProviderTag_ValidNonSkyline()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidNonSkyline",
                ExpectedResults = new List<IValidationResult>(),
                IsSkylineUser = false,
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckProviderTag_MissingTag()
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
        public void Protocol_CheckProviderTag_EmptyTag()
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
        public void Protocol_CheckProviderTag_InvalidTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidTag(null, null, null, "Other Provider", "Skyline Communications"),
                }
            };

            Generic.Validate(test, data);
        }
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckProviderTag();

        [TestMethod]
        public void Protocol_CheckProviderTag_MissingTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingTag",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckProviderTag();

        [TestMethod]
        public void Protocol_CheckProviderTag_CheckCategory() => Generic.CheckCategory(root, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckProviderTag_CheckId() => Generic.CheckId(root, CheckId.CheckProviderTag);
    }
}
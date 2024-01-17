namespace ProtocolTests.Protocol.ElementType.CheckElementTypeTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ElementType.CheckElementTypeTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckElementTypeTag();

        [TestMethod]
        public void Protocol_CheckElementTypeTag_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckElementTypeTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckElementTypeTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckElementTypeTag_WhiteSpacesTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "WhiteSpacesTag.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckElementTypeTag();

        [TestMethod]
        public void Protocol_CheckElementTypeTag_CheckCategory() => Generic.CheckCategory(root, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckElementTypeTag_CheckId() => Generic.CheckId(root, CheckId.CheckElementTypeTag);
    }
}
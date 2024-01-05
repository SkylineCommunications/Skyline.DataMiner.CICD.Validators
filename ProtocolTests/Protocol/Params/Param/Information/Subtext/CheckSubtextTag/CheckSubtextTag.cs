namespace ProtocolTests.Protocol.Params.Param.Information.Subtext.CheckSubtextTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Information.Subtext.CheckSubtextTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckSubtextTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckSubtextTag_Valid()
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
        public void Param_CheckSubtextTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "1"),
                    Error.EmptyTag(null, null, null, "6"),
                    Error.EmptyTag(null, null, null, "8"),
                    Error.EmptyTag(null, null, null, "1000"),
                    Error.EmptyTag(null, null, null, "1001"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckSubtextTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "100"),
                    Error.MissingTag(null, null, null, "101"),
                    Error.MissingTag(null, null, null, "102"),
                    Error.MissingTag(null, null, null, "103"),
                    Error.MissingTag(null, null, null, "104"),
                    Error.MissingTag(null, null, null, "105"),

                    Error.MissingTag(null, null, null, "200"),
                    Error.MissingTag(null, null, null, "201"),

                    Error.MissingTag(null, null, null, "300"),
                    Error.MissingTag(null, null, null, "301"),

                    Error.MissingTag(null, null, null, "600"),

                    Error.MissingTag(null, null, null, "1000"),
                    Error.MissingTag(null, null, null, "1001"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckSubtextTag_WhiteSpacesTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "WhiteSpacesTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "1"),
                    Error.EmptyTag(null, null, null, "6"),
                    Error.EmptyTag(null, null, null, "8"),
                    Error.EmptyTag(null, null, null, "1000"),
                    Error.EmptyTag(null, null, null, "1001"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckSubtextTag();

        [TestMethod]
        public void Param_CheckSubtextTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckSubtextTag_CheckId() => Generic.CheckId(root, CheckId.CheckSubtextTag);
    }
}
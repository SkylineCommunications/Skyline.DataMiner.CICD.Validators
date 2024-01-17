namespace ProtocolTests.Protocol.HTTP.Session.Connection.Request.Headers.Header.CheckHeaderTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.HTTP.Session.Connection.Request.Headers.Header.CheckHeaderTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckHeaderTag();

        #region Valid Checks

        [TestMethod]
        public void HTTP_CheckHeaderTag_Valid()
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
        public void HTTP_CheckHeaderTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "    dsfqfdq    "),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckHeaderTag();

        [TestMethod]
        public void HTTP_CheckHeaderTag_UntrimmedTag()
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
        private readonly IRoot root = new CheckHeaderTag();

        [TestMethod]
        public void HTTP_CheckHeaderTag_CheckCategory() => Generic.CheckCategory(root, Category.HTTP);

        [TestMethod]
        public void HTTP_CheckHeaderTag_CheckId() => Generic.CheckId(root, CheckId.CheckHeaderTag);
    }
}
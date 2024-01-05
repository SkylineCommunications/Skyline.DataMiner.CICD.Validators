namespace ProtocolTests.Protocol.HTTP.Session.Connection.Request.Headers.CheckHeaders
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.HTTP.Session.Connection.Request.Headers.CheckHeaders;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckHeaders();

        #region Valid Checks

        [TestMethod]
        public void HTTP_CheckHeaders_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void HTTP_CheckHeaders_DuplicateHeaderKeys()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateHeaderKeys",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateHeaderKeys(null, null, null, "Accept-Language", "1", "1", true).WithSubResults(
                        Error.DuplicateHeaderKeys(null, null, null, "Accept-Language", "1", "1", false),
                        Error.DuplicateHeaderKeys(null, null, null, "Accept-Language", "1", "1", false),
                        Error.DuplicateHeaderKeys(null, null, null, "Accept-Language", "1", "1", false),
                        Error.DuplicateHeaderKeys(null, null, null, "Accept-Language", "1", "1", false)),
                    Error.DuplicateHeaderKeys(null, null, null, "Max-Forwards", "1", "1", true).WithSubResults(
                        Error.DuplicateHeaderKeys(null, null, null, "Max-Forwards", "1", "1", false),
                        Error.DuplicateHeaderKeys(null, null, null, "Max-Forwards", "1", "1", false),
                        Error.DuplicateHeaderKeys(null, null, null, "Max-Forwards", "1", "1", false))
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void HTTP_CheckHeaders_MissingHeaderForVerb()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingHeaderForVerb",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingHeaderForVerb(null, null, null, "Content-Type", "POST", "1", "1"),
                    Error.MissingHeaderForVerb(null, null, null, "Content-Type", "PUT", "1", "2"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckHeaders();

        [TestMethod]
        public void HTTP_CheckHeaders_DuplicateHeaderKeys()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "DuplicateHeaderKeys",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckHeaders();

        [TestMethod]
        public void HTTP_CheckHeaders_CheckCategory() => Generic.CheckCategory(root, Category.HTTP);

        [TestMethod]
        public void HTTP_CheckHeaders_CheckId() => Generic.CheckId(root, CheckId.CheckHeaders);
    }
}
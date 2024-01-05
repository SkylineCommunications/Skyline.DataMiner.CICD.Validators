namespace ProtocolTests.Protocol.HTTP.Session.Connection.Request.Headers.Header.CheckKeyAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.HTTP.Session.Connection.Request.Headers.Header.CheckKeyAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckKeyAttribute();

        #region Valid Checks

        [TestMethod]
        public void HTTP_CheckKeyAttribute_Valid()
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
        public void HTTP_CheckKeyAttribute_EmptyKeyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyKeyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyKeyAttribute(null, null, null, "1", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void HTTP_CheckKeyAttribute_InvalidHeaderKeyForVerb()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidHeaderKeyForVerb",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidHeaderKeyForVerb(null, null, null, "Content-Length", "GET", "1", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void HTTP_CheckKeyAttribute_MissingKeyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingKeyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingKeyAttribute(null, null, null, "1", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void HTTP_CheckKeyAttribute_RedundantHeaderKey()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RedundantHeaderKey",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RedundantHeaderKey(null, null, null, "User-Agent", "1", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void HTTP_CheckKeyAttribute_UnknownHeaderKey()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnknownHeaderKey",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnknownHeaderKey(null, null, null, "DMQSFlkjiaopjklma", "1", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void HTTP_CheckKeyAttribute_UnsupportedHeaderKey()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedHeaderKey",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnsupportedHeaderKey(null, null, null, Certainty.Certain, "Content-Length", "1", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void HTTP_CheckKeyAttribute_UnsupportedHeaderKeyAcceptEncoding()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedHeaderKeyAcceptEncoding",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnsupportedHeaderKey(null, null, null, Certainty.Uncertain, "Accept-Encoding", "1", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void HTTP_CheckKeyAttribute_UntrimmedHeaderKey()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedHeaderKey",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedHeaderKey(null, null, null, "   Pragma   ", "1", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckKeyAttribute();

        [TestMethod]
        public void HTTP_CheckKeyAttribute_UntrimmedHeaderKey()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedHeaderKey",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckKeyAttribute();

        [TestMethod]
        public void HTTP_CheckKeyAttribute_CheckCategory() => Generic.CheckCategory(root, Category.HTTP);

        [TestMethod]
        public void HTTP_CheckKeyAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckKeyAttribute);
    }
}
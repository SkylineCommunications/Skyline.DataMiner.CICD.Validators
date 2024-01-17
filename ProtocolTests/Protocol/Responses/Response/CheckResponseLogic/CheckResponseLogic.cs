namespace ProtocolTests.Protocol.Responses.Response.CheckResponseLogic
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Responses.Response.CheckResponseLogic;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckResponseLogic();

        #region Valid Checks

        [TestMethod]
        public void Response_CheckResponseLogic_Valid()
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
        public void Response_CheckResponseLogic_ValidOnEach()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidOnEach",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Response_CheckResponseLogic_ValidResponseContainsHeaderTrailer()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidResponseContainsHeaderTrailer",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Response_CheckResponseLogic_ValidResponseNoHeaderTrailerDefined()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidResponseNoHeaderTrailerDefined",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Response_CheckResponseLogic_MissingCrcResponseAction()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingCrcResponseAction",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingCrcResponseAction(null, null, null, "1", "2"),
                    Error.MissingCrcResponseAction(null, null, null, "2", "2"),
                    Error.MissingCrcResponseAction(null, null, null, "3", "2"),
                    Error.MissingCrcResponseAction(null, null, null, "4", "2"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Response_CheckResponseLogic_MissingCrcResponseActionEachOvewrite()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingCrcResponseActionEachOvewrite",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingCrcResponseAction(null, null, null, "2", "2"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Response_CheckResponseLogic_MissingSmartSerialHeaderTrailerResponse()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "SmartSerialResponseShouldContainHeaderTrailer",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.SmartSerialResponseShouldContainHeaderTrailer(null, null, null, "1", "2"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Response_CheckResponseLogic_MissingSmartSerialTrailerResponse()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "SmartSerialResponseShouldContainTrailer",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.SmartSerialResponseShouldContainHeaderTrailer(null, null, null, "1", "2"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckResponseLogic();

        [TestMethod]
        public void Response_CheckResponseLogic_CheckCategory() => Generic.CheckCategory(root, Category.Response);

        [TestMethod]
        public void Response_CheckResponseLogic_CheckId() => Generic.CheckId(root, CheckId.CheckResponseLogic);
    }
}
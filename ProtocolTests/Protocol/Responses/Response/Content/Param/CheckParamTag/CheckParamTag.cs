namespace ProtocolTests.Protocol.Responses.Response.Content.Param.CheckParamTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Responses.Response.Content.Param.CheckParamTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckParamTag();

        #region Valid Checks

        [TestMethod]
        public void Response_CheckParamTag_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Response_CheckParamTag_EmptyParamTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyParamTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyParamTag(null, null, null, "1"),
                    Error.EmptyParamTag(null, null, null, "2"),
                    Error.EmptyParamTag(null, null, null, "2")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Response_CheckParamTag_InvalidParamTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidParamTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidParamTag(null, null, null, "test1", "1"),
                    Error.InvalidParamTag(null, null, null, "test2", "2"),
                    Error.InvalidParamTag(null, null, null, "test3", "2"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Response_CheckParamTag_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1"),
                    Error.NonExistingId(null, null, null, "1001", "2"),
                    Error.NonExistingId(null, null, null, "1002", "2"),
                    Error.NonExistingId(null, null, null, "1003", "3")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Response_CheckParamTag_NonExistingIdNoParamsTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoParamsTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1"),
                    Error.NonExistingId(null, null, null, "1001", "2"),
                    Error.NonExistingId(null, null, null, "1002", "2")
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Response_CheckParamTag_EmptyParamTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyParamTag(null, null, null, "0");

            string description = "Empty tag 'Content/Param' in Response '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Response_CheckParamTag_InvalidParamTag()
        {
            // Create ErrorMessage
            var message = Error.InvalidParamTag(null, null, null, "test", "1");

            string description = "Invalid value 'test' in tag 'Content/Param'. Response ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Response_CheckParamTag_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "0", "1");

            string description = "Tag 'Content/Param' references a non-existing 'Param' with ID '0'. Response ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckParamTag();

        [TestMethod]
        public void Response_CheckParamTag_CheckCategory() => Generic.CheckCategory(check, Category.Response);

        [TestMethod]
        public void Response_CheckParamTag_CheckId() => Generic.CheckId(check, CheckId.CheckParamTag);
    }
}
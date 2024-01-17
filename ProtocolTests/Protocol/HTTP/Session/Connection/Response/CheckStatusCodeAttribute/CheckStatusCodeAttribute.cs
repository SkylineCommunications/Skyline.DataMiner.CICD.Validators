namespace ProtocolTests.Protocol.HTTP.Session.Connection.Response.CheckStatusCodeAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.HTTP.Session.Connection.Response.CheckStatusCodeAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckStatusCodeAttribute();

        #region Valid Checks

        [TestMethod]
        public void HTTP_CheckStatusCodeAttribute_Valid()
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
        public void HTTP_CheckStatusCodeAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1", "11")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void HTTP_CheckStatusCodeAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "1", "11")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void HTTP_CheckStatusCodeAttribute_InvalidAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "test", "1", "11")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void HTTP_CheckStatusCodeAttribute_NonExistingIdNoParamsTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoParamsTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1", "11")
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
        public void HTTP_CheckStatusCodeAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "0", "11");

            string description = "Empty attribute 'Response@statusCode' in HTTP Session '0'. Connection ID '11'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void HTTP_CheckStatusCodeAttribute_InvalidAttribute()
        {
            // Create ErrorMessage
            var message = Error.InvalidAttribute(null, null, null, "0", "1", "11");

            string description = "Invalid value '0' in attribute 'Response@statusCode'. HTTP Session ID '1'. Connection ID '11'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void HTTP_CheckStatusCodeAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "0", "1", "11");

            string description = "Attribute 'Response@statusCode' references a non-existing 'Param' with ID '0'. HTTP Session ID '1'. Connection ID '11'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckStatusCodeAttribute();

        [TestMethod]
        public void HTTP_CheckStatusCodeAttribute_CheckCategory() => Generic.CheckCategory(check, Category.HTTP);

        [TestMethod]
        public void HTTP_CheckStatusCodeAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckStatusCodeAttribute);
    }
}
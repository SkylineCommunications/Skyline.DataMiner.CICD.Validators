namespace ProtocolTests.Protocol.HTTP.Session.Connection.Response.Content.CheckPidAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.HTTP.Session.Connection.Response.Content.CheckPidAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckPidAttribute();

        #region Valid Checks

        [TestMethod]
        public void HTTP_CheckPidAttribute_Valid()
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
        public void HTTP_CheckPidAttribute_NonExistingId()
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
        public void HTTP_CheckPidAttribute_EmptyAttribute()
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
        public void HTTP_CheckPidAttribute_InvalidAttribute()
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
        public void HTTP_CheckPidAttribute_NonExistingIdNoParamsTag()
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
        public void HTTP_CheckPidAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "0", "11");

            string description = "Empty attribute 'Response/Content@pid' in HTTP Session '0'. Connection ID '11'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void HTTP_CheckPidAttribute_InvalidAttribute()
        {
            // Create ErrorMessage
            var message = Error.InvalidAttribute(null, null, null, "0", "1", "11");

            string description = "Invalid value '0' in attribute 'Response/Content@pid'. HTTP Session ID '1'. Connection ID '11'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void HTTP_CheckPidAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "0", "1", "11");

            string description = "Attribute 'Response/Content@pid' references a non-existing 'Param' with ID '0'. HTTP Session ID '1'. Connection ID '11'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckPidAttribute();

        [TestMethod]
        public void HTTP_CheckPidAttribute_CheckCategory() => Generic.CheckCategory(check, Category.HTTP);

        [TestMethod]
        public void HTTP_CheckPidAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckPidAttribute);
    }
}
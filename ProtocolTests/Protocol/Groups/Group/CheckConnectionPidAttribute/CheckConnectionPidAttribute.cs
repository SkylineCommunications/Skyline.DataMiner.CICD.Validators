namespace ProtocolTests.Protocol.Groups.Group.CheckConnectionPidAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.CheckConnectionPidAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckConnectionPidAttribute();

        #region Valid Checks

        [TestMethod]
        public void Group_CheckConnectionPidAttribute_Valid()
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
        public void Group_CheckConnectionPidAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "1")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckConnectionPidAttribute_InvalidAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "test", "1")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckConnectionPidAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckConnectionPidAttribute_NonExistingIdNoParamsTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoParamsTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1")
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
        public void Group_CheckConnectionPidAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "0");

            string description = "Empty attribute 'connectionPid' in Group '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Group_CheckConnectionPidAttribute_InvalidAttribute()
        {
            // Create ErrorMessage
            var message = Error.InvalidAttribute(null, null, null, "0", "1");

            string description = "Invalid value '0' in attribute 'connectionPid'. Group ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Group_CheckConnectionPidAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "0", "1");

            string description = "Attribute 'connectionPid' references a non-existing 'Param' with ID '0'. Group ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckConnectionPidAttribute();

        [TestMethod]
        public void Group_CheckConnectionPidAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Group);

        [TestMethod]
        public void Group_CheckConnectionPidAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckConnectionPidAttribute);
    }
}
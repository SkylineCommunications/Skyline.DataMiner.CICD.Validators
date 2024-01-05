namespace ProtocolTests.Protocol.Groups.Group.Content.Action.CheckActionTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.Content.Action.CheckActionTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckActionTag();

        #region Valid Checks

        [TestMethod]
        public void Group_CheckActionTag_Valid()
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
        public void Group_CheckActionTag_EmptyActionTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyActionTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyActionTag(null, null, null, "1"),
                    Error.EmptyActionTag(null, null, null, "2"),
                    Error.EmptyActionTag(null, null, null, "3"),
                    Error.EmptyActionTag(null, null, null, "3")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckActionTag_InvalidActionTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidActionTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidActionTag(null, null, null, "test1", "1"),
                    Error.InvalidActionTag(null, null, null, "test2", "2"),
                    Error.InvalidActionTag(null, null, null, "test3", "3"),
                    Error.InvalidActionTag(null, null, null, "test4", "3"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckActionTag_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1"),
                    Error.NonExistingId(null, null, null, "1002", "2"),
                    Error.NonExistingId(null, null, null, "1003", "3"),
                    Error.NonExistingId(null, null, null, "1004", "3")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckActionTag_NonExistingIdNoActionsTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoActionsTag",
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
        public void Group_CheckActionTag_EmptyActionTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyActionTag(null, null, null, "0");

            string description = "Empty tag 'Content/Action' in Group '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Group_CheckActionTag_InvalidActionTag()
        {
            // Create ErrorMessage
            var message = Error.InvalidActionTag(null, null, null, "test", "1");

            string description = "Invalid value 'test' in tag 'Content/Action'. Group ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Group_CheckActionTag_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "0", "1");

            string description = "Tag 'Content/Action' references a non-existing 'Action' with ID '0'. Group ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckActionTag();

        [TestMethod]
        public void Group_CheckActionTag_CheckCategory() => Generic.CheckCategory(check, Category.Group);

        [TestMethod]
        public void Group_CheckActionTag_CheckId() => Generic.CheckId(check, CheckId.CheckActionTag);
    }
}
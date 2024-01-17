namespace ProtocolTests.Protocol.Groups.Group.Content.Trigger.CheckTriggerTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.Content.Trigger.CheckTriggerTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckTriggerTag();

        #region Valid Checks

        [TestMethod]
        public void Group_CheckTriggerTag_Valid()
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
        public void Group_CheckTriggerTag_EmptyTriggerTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTriggerTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTriggerTag(null, null, null, "1"),
                    Error.EmptyTriggerTag(null, null, null, "2"),
                    Error.EmptyTriggerTag(null, null, null, "3"),
                    Error.EmptyTriggerTag(null, null, null, "3")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckTriggerTag_InvalidTriggerTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidTriggerTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidTriggerTag(null, null, null, "test1", "1"),
                    Error.InvalidTriggerTag(null, null, null, "test2", "2"),
                    Error.InvalidTriggerTag(null, null, null, "test3", "3"),
                    Error.InvalidTriggerTag(null, null, null, "test4", "3"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckTriggerTag_NonExistingId()
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
        public void Group_CheckTriggerTag_NonExistingIdNoTriggersTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoTriggersTag",
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
        public void Group_CheckTriggerTag_EmptyTriggerTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTriggerTag(null, null, null, "0");

            string description = "Empty tag 'Content/Trigger' in Group '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Group_CheckTriggerTag_InvalidTriggerTag()
        {
            // Create ErrorMessage
            var message = Error.InvalidTriggerTag(null, null, null, "test", "1");

            string description = "Invalid value 'test' in tag 'Content/Trigger'. Group ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Group_CheckTriggerTag_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "0", "1");

            string description = "Tag 'Content/Trigger' references a non-existing 'Trigger' with ID '0'. Group ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckTriggerTag();

        [TestMethod]
        public void Group_CheckTriggerTag_CheckCategory() => Generic.CheckCategory(check, Category.Group);

        [TestMethod]
        public void Group_CheckTriggerTag_CheckId() => Generic.CheckId(check, CheckId.CheckTriggerTag);
    }
}
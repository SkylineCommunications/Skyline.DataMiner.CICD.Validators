namespace ProtocolTests.Protocol.Groups.Group.Content.Session.CheckSessionTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.Content.Session.CheckSessionTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckSessionTag();

        #region Valid Checks

        [TestMethod]
        public void Group_CheckSessionTag_Valid()
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
        public void Group_CheckSessionTag_EmptySessionTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptySessionTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptySessionTag(null, null, null, "1"),
                    Error.EmptySessionTag(null, null, null, "2"),
                    Error.EmptySessionTag(null, null, null, "3"),
                    Error.EmptySessionTag(null, null, null, "3")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckSessionTag_InvalidSessionTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidSessionTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidSessionTag(null, null, null, "test1", "1"),
                    Error.InvalidSessionTag(null, null, null, "test2", "2"),
                    Error.InvalidSessionTag(null, null, null, "test3", "3"),
                    Error.InvalidSessionTag(null, null, null, "test4", "3"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckSessionTag_NonExistingId()
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
        public void Group_CheckSessionTag_NonExistingIdNoHttpTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoHttpTag",
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
        public void Group_CheckSessionTag_EmptySessionTag()
        {
            // Create ErrorMessage
            var message = Error.EmptySessionTag(null, null, null, "0");

            string description = "Empty tag 'Content/Session' in Group '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Group_CheckSessionTag_InvalidSessionTag()
        {
            // Create ErrorMessage
            var message = Error.InvalidSessionTag(null, null, null, "test", "1");

            string description = "Invalid value 'test' in tag 'Content/Session'. Group ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Group_CheckSessionTag_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "0", "1");

            string description = "Tag 'Content/Session' references a non-existing 'HTTP Session' with ID '0'. Group ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckSessionTag();

        [TestMethod]
        public void Group_CheckSessionTag_CheckCategory() => Generic.CheckCategory(check, Category.Group);

        [TestMethod]
        public void Group_CheckSessionTag_CheckId() => Generic.CheckId(check, CheckId.CheckSessionTag);
    }
}
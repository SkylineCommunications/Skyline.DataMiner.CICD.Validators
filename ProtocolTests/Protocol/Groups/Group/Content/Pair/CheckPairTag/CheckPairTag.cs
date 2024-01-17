namespace ProtocolTests.Protocol.Groups.Group.Content.Pair.CheckPairTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Groups.Group.Content.Pair.CheckPairTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckPairTag();

        #region Valid Checks

        [TestMethod]
        public void Group_CheckPairTag_Valid()
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
        public void Group_CheckPairTag_EmptyPairTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyPairTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyPairTag(null, null, null, "1"),
                    Error.EmptyPairTag(null, null, null, "2"),
                    Error.EmptyPairTag(null, null, null, "3"),
                    Error.EmptyPairTag(null, null, null, "3")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckPairTag_InvalidPairTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidPairTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidPairTag(null, null, null, "test1", "1"),
                    Error.InvalidPairTag(null, null, null, "test2", "2"),
                    Error.InvalidPairTag(null, null, null, "test3", "3"),
                    Error.InvalidPairTag(null, null, null, "test4", "3")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Group_CheckPairTag_NonExistingId()
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
        public void Group_CheckPairTag_NonExistingIdNoPairsTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoPairsTag",
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
        public void Group_CheckPairTag_EmptyPairTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyPairTag(null, null, null, "0");

            string description = "Empty tag 'Content/Pair' in Group '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Group_CheckPairTag_InvalidPairTag()
        {
            // Create ErrorMessage
            var message = Error.InvalidPairTag(null, null, null, "test", "1");

            string description = "Invalid value 'test' in tag 'Content/Pair'. Group ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Group_CheckPairTag_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "0", "1");

            string description = "Tag 'Content/Pair' references a non-existing 'Pair' with ID '0'. Group ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckPairTag();

        [TestMethod]
        public void Group_CheckPairTag_CheckCategory() => Generic.CheckCategory(check, Category.Group);

        [TestMethod]
        public void Group_CheckPairTag_CheckId() => Generic.CheckId(check, CheckId.CheckPairTag);
    }
}
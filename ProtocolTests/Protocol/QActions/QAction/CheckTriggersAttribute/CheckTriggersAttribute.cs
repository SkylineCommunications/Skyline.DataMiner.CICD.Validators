namespace ProtocolTests.Protocol.QActions.QAction.CheckTriggersAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckTriggersAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckTriggersAttribute();

        #region Valid Checks

        [TestMethod]
        public void QAction_CheckTriggersAttribute_Valid()
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
        public void QAction_CheckTriggersAttribute_DuplicateId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateId(null, null, null, "1", "1")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CheckTriggersAttribute_EmptyAttribute()
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
        public void QAction_CheckTriggersAttribute_InvalidAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "aaa", "1"),
                    Error.InvalidAttribute(null, null, null, "1;bbb", "2").WithSubResults(
                        Error.InvalidAttribute(null, null, null, "bbb", "2")),
                    Error.InvalidAttribute(null, null, null, "ccc;1", "3").WithSubResults(
                        Error.InvalidAttribute(null, null, null, "ccc", "3")),
                    Error.InvalidAttribute(null, null, null, "1;aaa;bbb;ccc;2", "4").WithSubResults(
                        Error.InvalidAttribute(null, null, null, "aaa", "4"),
                        Error.InvalidAttribute(null, null, null, "bbb", "4"),
                        Error.InvalidAttribute(null, null, null, "ccc", "4"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CheckTriggersAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "1"),
                    Error.MissingAttribute(null, null, null, "2")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CheckTriggersAttribute_NonExistingGroup()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingGroup",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingGroup(null, null, null, "11", "1"),
                    Error.NonExistingGroup(null, null, null, "21", "2"),
                    Error.NonExistingGroup(null, null, null, "23", "2")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CheckTriggersAttribute_NonExistingParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingParam",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingParam(null, null, null, "1", "1"),
                    Error.NonExistingParam(null, null, null, "1", "2"),
                    Error.NonExistingParam(null, null, null, "3", "2")
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
        public void QAction_CheckTriggersAttribute_DuplicateId()
        {
            // Create ErrorMessage
            var message = Error.DuplicateId(null, null, null, "0", "1");

            string description = "Duplicate value '0' in attribute 'QAction@triggers'. QAction ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void QAction_CheckTriggersAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "0");

            string description = "Empty attribute 'triggers' in QAction '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void QAction_CheckTriggersAttribute_InvalidAttribute()
        {
            // Create ErrorMessage
            var message = Error.InvalidAttribute(null, null, null, "0", "1");

            string description = "Invalid value '0' in attribute 'triggers'. QAction ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void QAction_CheckTriggersAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null, "0");

            string description = "Missing attribute 'triggers' in QAction '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void QAction_CheckTriggersAttribute_NonExistingGroup()
        {
            // Create ErrorMessage
            var message = Error.NonExistingGroup(null, null, null, "0", "1");

            string description = "Attribute 'triggers' references a non-existing 'Group' with ID '0'. QAction ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void QAction_CheckTriggersAttribute_NonExistingParam()
        {
            // Create ErrorMessage
            var message = Error.NonExistingParam(null, null, null, "0", "1");

            string description = "Attribute 'triggers' references a non-existing 'Param' with ID '0'. QAction ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckTriggersAttribute();

        [TestMethod]
        public void QAction_CheckTriggersAttribute_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CheckTriggersAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckTriggersAttribute);
    }
}
namespace ProtocolTests.Protocol.ExportRules.ExportRule.CheckTableAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ExportRules.ExportRule.CheckTableAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckTableAttribute();

        #region Valid Checks

        [TestMethod]
        public void ExportRule_CheckTableAttribute_Valid()
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
        public void ExportRule_CheckTableAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null)
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void ExportRule_CheckTableAttribute_InvalidAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "test")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void ExportRule_CheckTableAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null)
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void ExportRule_CheckTableAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void ExportRule_CheckTableAttribute_NonExistingIdNoParamsTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoParamsTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001")
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
        public void ExportRule_CheckTableAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null);

            string description = "Empty attribute 'ExportRule@table'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void ExportRule_CheckTableAttribute_InvalidAttribute()
        {
            // Create ErrorMessage
            var message = Error.InvalidAttribute(null, null, null, "0");

            string description = "Invalid value '0' in attribute 'ExportRule@table'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void ExportRule_CheckTableAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null);

            string description = "Missing attribute 'ExportRule@table'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void ExportRule_CheckTableAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "1000");

            string description = "Attribute 'ExportRule@table' references a non-existing 'Table' with PID '1000'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckTableAttribute();

        [TestMethod]
        public void ExportRule_CheckTableAttribute_CheckCategory() => Generic.CheckCategory(check, Category.ExportRule);

        [TestMethod]
        public void ExportRule_CheckTableAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckTableAttribute);
    }
}
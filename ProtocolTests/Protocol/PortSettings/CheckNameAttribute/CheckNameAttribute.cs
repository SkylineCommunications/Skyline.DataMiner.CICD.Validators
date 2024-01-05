namespace ProtocolTests.Protocol.PortSettings.CheckNameAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.PortSettings.CheckNameAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckNameAttribute();

        #region Valid Checks

        [TestMethod]
        public void PortSettings_CheckNameAttribute_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void PortSettings_CheckNameAttribute_ValidVirtualNoName()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidVirtualNoName",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void PortSettings_CheckNameAttribute_ValidVirtualNoPortSettings()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidVirtualNoPortSettings",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void PortSettings_CheckNameAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "0"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void PortSettings_CheckNameAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "0"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void PortSettings_CheckNameAttribute_MissingPortSettings()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingPortSettings",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "0"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void PortSettings_CheckNameAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "0", " AAA "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckNameAttribute();

        [TestMethod]
        public void PortSettings_CheckNameAttribute_EmptyAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyAttribute",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void PortSettings_CheckNameAttribute_MissingAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingAttribute",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void PortSettings_CheckNameAttribute_UntrimmedAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttribute",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void PortSettings_CheckNameAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "0");

            string description = "Empty attribute 'PortSettings@name' in Connection '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void PortSettings_CheckNameAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null, "0");

            string description = "Missing attribute 'PortSettings@name' in Connection '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void PortSettings_CheckNameAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "0", " myConnection ");

            string description = "Untrimmed attribute 'PortSettings@name' in Connection '0'. Current value ' myConnection '.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckNameAttribute();

        [TestMethod]
        public void PortSettings_CheckNameAttribute_CheckCategory() => Generic.CheckCategory(check, Category.PortSettings);

        [TestMethod]
        public void PortSettings_CheckNameAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckNameAttribute);
    }
}
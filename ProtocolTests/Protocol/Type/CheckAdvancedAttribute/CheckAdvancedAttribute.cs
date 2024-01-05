namespace ProtocolTests.Protocol.Type.CheckAdvancedAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Type.CheckAdvancedAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckAdvancedAttribute();

        #region Valid

        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_Valid_1Advanced()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_1Advanced",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_Valid_2Advanced()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_2Advanced",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_Valid_NoAdvanced()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_NoAdvanced",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid

        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_UnknownConnection()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnknownConnection",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnknownConnection(null, null, null, "AAA", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, " snmp; serial ").WithSubResults(
                        Error.UntrimmedValueInAttribute_Sub(null, null, null, " snmp"),
                        Error.UntrimmedValueInAttribute_Sub(null, null, null, " serial "))
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly IRoot codeFix = new CheckAdvancedAttribute();

        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_EmptyAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyAttribute",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_UntrimmedAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttribute",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null);

            string description = "Empty attribute 'Protocol/Type@advanced'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, " snmp;serial ");

            string description = "Untrimmed attribute 'Protocol/Type@advanced'. Current value ' snmp;serial '.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_UntrimmedAttribute_Sub()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedValueInAttribute_Sub(null, null, null, " snmp ");

            string description = "Untrimmed value ' snmp ' in attribute 'Protocol/Type@advanced'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_UnknownConnection()
        {
            // Create ErrorMessage
            var message = Error.UnknownConnection(null, null, null, "test", "1");

            string description = "Unknown connection type 'test' in Connection '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckAdvancedAttribute();

        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckAdvancedAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckAdvancedAttribute);
    }
}
namespace ProtocolTests.Protocol.TreeControls.TreeControl.ExtraDetails.LinkedDetails.CheckDiscreetColumnIdAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.ExtraDetails.LinkedDetails.CheckDiscreetColumnIdAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckDiscreetColumnIdAttribute();

        #region Valid Checks

        [TestMethod]
        public void TreeControl_CheckDiscreetColumnIdAttribute_Valid()
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
        public void TreeControl_CheckDiscreetColumnIdAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckDiscreetColumnIdAttribute_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, " A", "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckDiscreetColumnIdAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckDiscreetColumnIdAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1003"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void TreeControl_CheckDiscreetColumnIdAttribute_ReferencedColumnExpectingRTDisplay()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedColumnExpectingRTDisplay",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ReferencedColumnExpectingRTDisplay(null, null, null, "1002"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckDiscreetColumnIdAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "1", " 1002 "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckDiscreetColumnIdAttribute();

        [TestMethod]
        public void TreeControl_CheckDiscreetColumnIdAttribute_UntrimmedAttribute()
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
        public void TreeControl_CheckDiscreetColumnIdAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null, "0");

            string description = "Missing attribute 'LinkedDetails@discreetColumnId' in TreeControl '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckDiscreetColumnIdAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "0");

            string description = "Empty attribute 'LinkedDetails@discreetColumnId' in TreeControl '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckDiscreetColumnIdAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "0", " 1 ");

            string description = "Untrimmed attribute 'LinkedDetails@discreetColumnId' in TreeControl '0'. Current value ' 1 '.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckDiscreetColumnIdAttribute_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "aaa", "1");

            string description = "Invalid value 'aaa' in attribute 'LinkedDetails@discreetColumnId'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckDiscreetColumnIdAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "1005");

            string description = @"Attribute 'ExtraDetails/LinkedDetails@discreetColumnId' references a non-existing 'Column' with PID '1005'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckDiscreetColumnIdAttribute();

        [TestMethod]
        public void TreeControl_CheckDiscreetColumnIdAttribute_CheckCategory() => Generic.CheckCategory(check, Skyline.DataMiner.CICD.Validators.Common.Model.Category.TreeControl);

        [TestMethod]
        public void TreeControl_CheckDiscreetColumnIdAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckDiscreetColumnIdAttribute);
    }
}
namespace ProtocolTests.Protocol.TreeControls.TreeControl.Hierarchy.CheckPathAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.Hierarchy.CheckPathAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckPathAttribute();

        #region Valid Checks

        [TestMethod]
        public void TreeControl_CheckPathAttribute_Valid()
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
        public void TreeControl_CheckPathAttribute_DuplicateId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateId(null, null, null, "1000", "1"),
                    Error.DuplicateId(null, null, null, "2000", "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckPathAttribute_EmptyAttribute()
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
        public void TreeControl_CheckPathAttribute_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "A,B,C", "1").WithSubResults(
                        Error.InvalidValueInAttribute_Sub(null, null, null, "A"),
                        Error.InvalidValueInAttribute_Sub(null, null, null, "B"),
                        Error.InvalidValueInAttribute_Sub(null, null, null, "C")),
                    Error.InvalidValue(null, null, null, "1000;2000", "2").WithSubResults(
                        Error.InvalidValueInAttribute_Sub(null, null, null, "1000;2000"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckPathAttribute_NonExistingIdsInAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdsInAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingIdsInAttribute(null, null, null, "1").WithSubResults(
                        Error.NonExistingIdsInAttribute_Sub(null, null, null, "99", "1"),
                        Error.NonExistingIdsInAttribute_Sub(null, null, null, "999", "1"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void TreeControl_CheckPathAttribute_ReferencedParamExpectingRTDisplay()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedParamExpectingRTDisplay",
                ExpectedResults = new List<IValidationResult>
                {
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckPathAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "1", " 1000, 2000 ,  3000 ").WithSubResults(
                        Error.UntrimmedValueInAttribute_Sub(null, null, null, " 1000"),
                        Error.UntrimmedValueInAttribute_Sub(null, null, null, " 2000 "),
                        Error.UntrimmedValueInAttribute_Sub(null, null, null, "  3000 "))
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckPathAttribute();

        [TestMethod]
        public void TreeControl_CheckPathAttribute_UntrimmedAttribute()
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
        public void TreeControl_CheckPathAttribute_DuplicateId()
        {
            // Create ErrorMessage
            var message = Error.DuplicateId(null, null, null, "0", "1");

            string description = "Duplicate value '0' in attribute 'Hierarchy@path'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckPathAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "0");

            string description = "Empty attribute 'Hierarchy@path' in TreeControl '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckPathAttribute_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "A,B,C", "1");

            string description = "Invalid value 'A,B,C' in attribute 'Hierarchy@path'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckPathAttribute_InvalidValueInAttribute()
        {
            // Create ErrorMessage
            var message = Error.InvalidValueInAttribute_Sub(null, null, null, "A");

            string description = "Invalid value 'A' in attribute 'Hierarchy@path'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckPathAttribute_NonExistingIdsInAttribute()
        {
            // Create ErrorMessage
            var message = Error.NonExistingIdsInAttribute(null, null, null, "0");

            string description = "Attribute 'Hierarchy@path' references non-existing IDs. TreeControl ID '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckPathAttribute_NonExistingIdsInAttributeSub()
        {
            // Create ErrorMessage
            var message = Error.NonExistingIdsInAttribute_Sub(null, null, null, "0", "1");

            string description = "Attribute 'Hierarchy@path' references a non-existing 'Table' with PID '0'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckPathAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "0", " 1 , 2 ");

            string description = "Untrimmed attribute 'Hierarchy@path' in TreeControl '0'. Current value ' 1 , 2 '.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckPathAttribute_UntrimmedValueInAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedValueInAttribute_Sub(null, null, null, " 1 ");

            string description = "Untrimmed value ' 1 ' in attribute 'Hierarchy@path'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckPathAttribute();

        [TestMethod]
        public void TreeControl_CheckPathAttribute_CheckCategory() => Generic.CheckCategory(check, Category.TreeControl);

        [TestMethod]
        public void TreeControl_CheckPathAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckPathAttribute);
    }
}
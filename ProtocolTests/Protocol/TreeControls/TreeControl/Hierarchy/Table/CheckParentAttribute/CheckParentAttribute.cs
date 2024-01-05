namespace ProtocolTests.Protocol.TreeControls.TreeControl.Hierarchy.Table.CheckParentAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.Hierarchy.Table.CheckParentAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckParentAttribute();

        #region Valid Checks

        [TestMethod]
        public void TreeControl_CheckParentAttribute_Valid()
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
        public void TreeControl_CheckParentAttribute_EmptyAttribute()
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
        public void TreeControl_CheckParentAttribute_ExcessiveAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ExcessiveAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ExcessiveAttribute(null, null, null, "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckParentAttribute_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "A", "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckParentAttribute_MissingAttribute()
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
        public void TreeControl_CheckParentAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "99"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckParentAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "1", " 1000 "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckParentAttribute();

        [TestMethod]
        public void TreeControl_CheckParentAttribute_UntrimmedAttribute()
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
        public void TreeControl_CheckParentAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "0");

            string description = "Empty attribute 'Table@parent' in TreeControl '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckParentAttribute_ExcessiveAttribute()
        {
            // Create ErrorMessage
            var message = Error.ExcessiveAttribute(null, null, null, "0");

            string description = "Unsupported attribute 'Table@parent' in TreeControl '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckParentAttribute_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "0", "1");

            string description = "Invalid value '0' in attribute 'Table@parent'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckParentAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null, "0");

            string description = "Missing attribute 'Table@parent' in TreeControl '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckParentAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "1000");

            string description = @"Attribute 'Hierarchy/Table@parent' references a non-existing 'Table' with PID '1000'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckParentAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "0", "1");

            string description = "Untrimmed attribute 'Table@parent' in TreeControl '0'. Current value '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckParentAttribute();

        [TestMethod]
        public void TreeControl_CheckParentAttribute_CheckCategory() => Generic.CheckCategory(check, Category.TreeControl);

        [TestMethod]
        public void TreeControl_CheckParentAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckParentAttribute);
    }
}
namespace ProtocolTests.Protocol.TreeControls.TreeControl.ExtraTab.Tab.CheckParameterAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.ExtraTab.Tab.CheckParameterAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckParameterAttribute();

        #region Valid Checks

        [TestMethod]
        public void TreeControl_CheckParameterAttribute_Valid()
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
        public void TreeControl_CheckParameterAttribute_EmptyAttribute()
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
        public void TreeControl_CheckParameterAttribute_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    // Type="parameters" cases
                    Error.InvalidValue(null, null, null, "1002,aaa,1003,1009,bbb", "1").WithSubResults(
                            Error.InvalidValue(null, null, null, "aaa", "1"),
                            Error.NonExistingId(null, null, null, "1009"),
                            Error.InvalidValue(null, null, null, "bbb", "1")),
                    Error.InvalidValue(null, null, null, "aaa", "1"),
                    Error.InvalidValue(null, null, null, "1009,1010", "1").WithSubResults(
                            Error.NonExistingId(null, null, null, "1009"),
                            Error.NonExistingId(null, null, null, "1010")),
                    
                    // Type="relation" cases
                    Error.InvalidValue(null, null, null, "10001,10002", "1"),
                    Error.InvalidValue(null, null, null, "aaa", "1"),

                    // Type="summary" cases
                    Error.InvalidValue(null, null, null, "3000,4000", "1"),
                    Error.InvalidValue(null, null, null, "aaa", "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckParameterAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "1"),
                    Error.MissingAttribute(null, null, null, "1"),
                    Error.MissingAttribute(null, null, null, "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckParameterAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    // type="parameters" cases
                    Error.InvalidValue(null, null, null, "1010,1002,1003,1009", "1").WithSubResults(
                        Error.NonExistingId(null, null, null, "1010"),
                            Error.NonExistingId(null, null, null, "1009")),
                    Error.NonExistingId(null, null, null, "1009"),
                    
                    // type="relation" cases
                    Error.NonExistingId(null, null, null, "10009"),
                    
                    // type="summary" cases
                    Error.NonExistingId(null, null, null, "9000"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void TreeControl_CheckParameterAttribute_ReferencedParamExpectingRTDisplay()
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
        public void TreeControl_CheckParameterAttribute_UntrimmedAttribute()
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
        private readonly ICodeFix codeFix = new CheckParameterAttribute();

        [TestMethod]
        public void TreeControl_CheckParameterAttribute_UntrimmedAttribute()
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
        public void TreeControl_CheckParameterAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "0");

            string description = "Empty attribute 'Tab@parameter' in TreeControl '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckParameterAttribute_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "A", "1");

            string description = "Invalid value 'A' in attribute 'Tab@parameter'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckParameterAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null, "0");

            string description = "Missing attribute 'Tab@parameter' in TreeControl '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckParameterAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "0");

            string description = @"Attribute 'ExtraTabs/Tab@parameter' references a non-existing 'Column' with PID '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckParameterAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "0", " 1 ");

            string description = "Untrimmed attribute 'Tab@parameter' in TreeControl '0'. Current value ' 1 '.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckParameterAttribute();

        [TestMethod]
        public void TreeControl_CheckParameterAttribute_CheckCategory() => Generic.CheckCategory(check, Category.TreeControl);

        [TestMethod]
        public void TreeControl_CheckParameterAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckParameterAttribute);
    }
}
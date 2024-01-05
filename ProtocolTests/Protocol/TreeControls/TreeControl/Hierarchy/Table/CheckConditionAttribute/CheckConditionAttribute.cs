namespace ProtocolTests.Protocol.TreeControls.TreeControl.Hierarchy.Table.CheckConditionAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.Hierarchy.Table.CheckConditionAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckConditionAttribute();

        #region Valid Checks

        [TestMethod]
        public void TreeControl_CheckConditionAttribute_Valid()
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
        public void TreeControl_CheckConditionAttribute_EmptyAttribute()
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
        public void TreeControl_CheckConditionAttribute_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "AAA:BBB", "1").WithSubResults(
                        Error.InvalidValueInAttribute_Sub(null, null, null, "<columnPid>", "1", "AAA")),
                    Error.InvalidValue(null, null, null, "1001", "1").WithSubResults(
                        Error.MissingValueInAttribute_Sub(null, null, null, "<filterValue>", "1")),
                    Error.InvalidValue(null, null, null, "CCC", "1").WithSubResults(
                        Error.InvalidValueInAttribute_Sub(null, null, null, "<columnPid>", "1", "CCC"),
                        Error.MissingValueInAttribute_Sub(null, null, null, "<filterValue>", "1")),
                    Error.InvalidValue(null, null, null, "filter:fk=", "2").WithSubResults(
                        Error.MissingValueInAttribute_Sub(null, null, null, "<filterColumnPid>", "2")),
                    Error.InvalidValue(null, null, null, "filter:fk=AAA", "2").WithSubResults(
                        Error.InvalidValueInAttribute_Sub(null, null, null, "<filterColumnPid>", "2", "AAA"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckConditionAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1003"),
                    Error.NonExistingId(null, null, null, "1004"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void TreeControl_CheckConditionAttribute_ReferencedColumnExpectingRTDisplay()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedColumnExpectingRTDisplay",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ReferencedColumnExpectingRTDisplay(null, null, null, "1002", "1"),
                    Error.ReferencedColumnExpectingRTDisplay(null, null, null, "1002", "1"),

                    Error.ReferencedColumnExpectingRTDisplay(null, null, null, "1002", "1"),
                    Error.ReferencedColumnExpectingRTDisplay(null, null, null, "1002", "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckConditionAttribute_UntrimmedColumnPid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedColumnPid",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedColumnPid(null, null, null, " 1002 ", "1"),
                    Error.UntrimmedColumnPid(null, null, null, " 1002", "1"),
                    Error.UntrimmedColumnPid(null, null, null, "1002 ", "1"),

                    Error.UntrimmedColumnPid(null, null, null, " 1003 ", "2"),
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
        public void TreeControl_CheckConditionAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "0");

            string description = "Empty attribute 'Table@condition' in TreeControl '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckConditionAttribute_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "0", "1");

            string description = "Invalid value '0' in attribute 'Table@condition'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckConditionAttribute_InvalidValueInAttribute_Sub()
        {
            // Create ErrorMessage
            var message = Error.InvalidValueInAttribute_Sub(null, null, null, "<columnPid>", "1", "A");

            string description = "Invalid option '<columnPid>' in attribute 'Table@condition'. TreeControl ID '1'. Current Value 'A'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckConditionAttribute_MissingValueInAttribute_Sub()
        {
            // Create ErrorMessage
            var message = Error.MissingValueInAttribute_Sub(null, null, null, "<filterValue>", "1");

            string description = "Missing value '<filterValue>' in attribute 'Table@condition'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckConditionAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "1005");

            string description = @"Attribute 'Hierarchy/Table@condition' references a non-existing 'Column' with PID '1005'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckConditionAttribute_UntrimmedColumnPid()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedColumnPid(null, null, null, "0", "1");

            string description = "Untrimmed value '0' in attribute 'Table@condition' in TreeControl '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckConditionAttribute();

        [TestMethod]
        public void TreeControl_CheckConditionAttribute_CheckCategory() => Generic.CheckCategory(check, Category.TreeControl);

        [TestMethod]
        public void TreeControl_CheckConditionAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckConditionAttribute);
    }
}
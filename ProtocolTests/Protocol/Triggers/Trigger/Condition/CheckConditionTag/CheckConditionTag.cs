namespace ProtocolTests.Protocol.Triggers.Trigger.Condition.CheckConditionTag
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Triggers.Trigger.Condition.CheckConditionTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckConditionTag();

        #region Valid Checks

        [TestMethod]
        public void Trigger_CheckConditionTag_Valid()
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
        public void Trigger_CheckConditionTag_InvalidCondition()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidCondition",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidCondition(null, null, null, "", "Condition is empty.", "1"),
                    Error.InvalidCondition(null, null, null, "((id:12 + \"efg\") + 10) == \"defefgabc\"", "The addition operator ('+') must be used with operands of the same type.", "2")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Trigger_CheckConditionTag_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "10", "1")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Trigger_CheckConditionTag_ConditionCanBeSimplified()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ConditionCanBeSimplified",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ConditionCanBeSimplified(null, null, null, "id:12 == (\"test\")", "1")
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
        public void Trigger_CheckConditionTag_InvalidCondition()
        {
            // Create ErrorMessage
            var message = Error.InvalidCondition(null, null, null, "currentCondition", "reason", "100");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "5.5.1",
                Category = Category.Trigger,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invalid condition 'currentCondition'. Reason 'reason'. Trigger ID '100'.",
                HowToFix = "",
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Trigger_CheckConditionTag_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "5.5.2",
                Category = Category.Trigger,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Tag 'Trigger/Condition' references a non-existing 'Param' with PID '2'. Trigger ID '3'.",
                HowToFix = "",
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckConditionTag();

        [TestMethod]
        public void Trigger_CheckConditionTag_CheckCategory() => Generic.CheckCategory(check, Category.Trigger);

        [TestMethod]
        public void Trigger_CheckConditionTag_CheckId() => Generic.CheckId(check, CheckId.CheckConditionTag);
    }
}
namespace ProtocolTests.Protocol.Commands.Command.CheckAsciiAttribute
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Commands.Command.CheckAsciiAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckAsciiAttribute();

        #region Valid Checks

        [TestMethod]
        public void Command_CheckAsciiAttribute_Valid()
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
        public void Command_CheckAsciiAttribute_EmptyAttribute()
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
        public void Command_CheckAsciiAttribute_InvalidAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "test1", "1"),

                    Error.InvalidAttribute(null, null, null, "1;test2;2;test3;3", "2").WithSubResults(
                        Error.InvalidAttribute(null, null, null, "test2", "2"),
                        Error.InvalidAttribute(null, null, null, "test3", "2")),

                    Error.InvalidAttribute(null, null, null, "1;test4;2", "3").WithSubResults(
                        Error.InvalidAttribute(null, null, null, "test4", "3"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Command_CheckAsciiAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1"),

                    Error.InvalidAttribute(null, null, null, "1001;1002", "2").WithSubResults(
                        Error.NonExistingId(null, null, null, "1001", "2"),
                        Error.NonExistingId(null, null, null, "1002", "2")),

                    Error.InvalidAttribute(null, null, null, "1;1001;2;1002;3", "3").WithSubResults(
                        Error.NonExistingId(null, null, null, "1001", "3"),
                        Error.NonExistingId(null, null, null, "1002", "3"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Command_CheckAsciiAttribute_NonExistingIdNoParamsTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoParamsTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1"),

                    Error.InvalidAttribute(null, null, null, "1001;1002", "2").WithSubResults(
                        Error.NonExistingId(null, null, null, "1001", "2"),
                        Error.NonExistingId(null, null, null, "1002", "2"))
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
        public void Command_CheckAsciiAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "0");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "10.4.1",
                Category = Category.Command,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Empty attribute 'ascii' in Command '0'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Command_CheckAsciiAttribute_InvalidAttribute()
        {
            // Create ErrorMessage
            var message = Error.InvalidAttribute(null, null, null, "0", "1");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "10.4.2",
                Category = Category.Command,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Invalid value '0' in attribute 'ascii'. Command ID '1'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Command_CheckAsciiAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "0", "1");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "10.4.3",
                Category = Category.Command,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Attribute 'ascii' references a non-existing 'Param' with ID '0'. Command ID '1'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckAsciiAttribute();

        [TestMethod]
        public void Command_CheckAsciiAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Command);

        [TestMethod]
        public void Command_CheckAsciiAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckAsciiAttribute);
    }
}
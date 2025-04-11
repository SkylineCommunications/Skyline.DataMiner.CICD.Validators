namespace ProtocolTests.Protocol.Commands.Command.Content.Param.CheckParamTag
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Commands.Command.Content.Param.CheckParamTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckParamTag();

        #region Valid Checks

        [TestMethod]
        public void Command_CheckParamTag_Valid()
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
        public void Command_CheckParamTag_EmptyParamTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyParamTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyParamTag(null, null, null, "1"),
                    Error.EmptyParamTag(null, null, null, "2"),
                    Error.EmptyParamTag(null, null, null, "2")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Command_CheckParamTag_InvalidParamTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidParamTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidParamTag(null, null, null, "test1", "1"),
                    Error.InvalidParamTag(null, null, null, "test2", "2"),
                    Error.InvalidParamTag(null, null, null, "test3", "2")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Command_CheckParamTag_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1"),
                    Error.NonExistingId(null, null, null, "1001", "2"),
                    Error.NonExistingId(null, null, null, "1002", "2")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Command_CheckParamTag_NonExistingIdNoParamsTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoParamsTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1"),
                    Error.NonExistingId(null, null, null, "1001", "2"),
                    Error.NonExistingId(null, null, null, "1002", "2")
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
        public void Command_CheckParamTag_EmptyParamTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyParamTag(null, null, null, "0");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "10.3.2",
                Category = Category.Command,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Empty tag 'Content/Param' in Command '0'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Command_CheckParamTag_InvalidParamTag()
        {
            // Create ErrorMessage
            var message = Error.InvalidParamTag(null, null, null, "test", "1");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "10.3.3",
                Category = Category.Command,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Invalid value 'test' in tag 'Content/Param'. Command ID '1'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Command_CheckParamTag_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "0", "1");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "10.3.1",
                Category = Category.Command,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Tag 'Content/Param' references a non-existing 'Param' with ID '0'. Command ID '1'.",
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
        private readonly IRoot check = new CheckParamTag();

        [TestMethod]
        public void Command_CheckParamTag_CheckCategory() => Generic.CheckCategory(check, Category.Command);

        [TestMethod]
        public void Command_CheckParamTag_CheckId() => Generic.CheckId(check, CheckId.CheckParamTag);
    }
}
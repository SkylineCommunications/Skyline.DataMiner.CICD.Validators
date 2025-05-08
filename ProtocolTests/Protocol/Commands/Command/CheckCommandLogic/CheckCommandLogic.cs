namespace ProtocolTests.Protocol.Commands.Command.CheckCommandLogic
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Commands.Command.CheckCommandLogic;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckCommandLogic();

        #region Valid Checks

        [TestMethod]
        public void Command_CheckCommandLogic_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Command_CheckCommandLogic_ValidOnEach()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidOnEach.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }
        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Command_CheckCommandLogic_MissingCrcCommandAction()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingCrcCommandAction.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingCrcCommandAction(null, null, null, "1", "2"),
                    Error.MissingCrcCommandAction(null, null, null, "2", "2"),
                    Error.MissingCrcCommandAction(null, null, null, "3", "2"),
                    Error.MissingCrcCommandAction(null, null, null, "4", "2"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Command_CheckCommandLogic_MissingCrcCommandActionEachOverwrite()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingCrcCommandActionEachOverwrite.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingCrcCommandAction(null, null, null, "2", "2"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Command_CheckCommandLogic_MissingCrcCommandAction()
        {
            // Create ErrorMessage
            var message = Error.MissingCrcCommandAction(null, null, null, "1", "2");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "10.1.1",
                Category = Category.Command,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "No 'CRC' Action triggered before Command '1'. 'CRC' Param '2'.",
                HowToFix = "Make sure a CRC action is triggered before command.",
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckCommandLogic();

        [TestMethod]
        public void Command_CheckCommandLogic_CheckCategory() => Generic.CheckCategory(root, Category.Command);

        [TestMethod]
        public void Command_CheckCommandLogic_CheckId() => Generic.CheckId(root, CheckId.CheckCommandLogic);
    }
}
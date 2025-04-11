namespace ProtocolTests.Protocol.Commands.Command.Name.CheckNameTag
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Commands.Command.Name.CheckNameTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckNameTag();

        #region Valid Checks

        [TestMethod]
        public void Command_CheckNameTag_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Command_CheckNameTag_DuplicatedValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicatedValue.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicatedValue(null, null, null, "Name1", "1, 2").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Name1", "1"),
                        Error.DuplicatedValue(null, null, null, "Name1", "2"))
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
        public void Command_CheckParamTag_EmptyParamTag()
        {
            // Create ErrorMessage
            var message = Error.DuplicatedValue(null, null, null, "0", "1");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "10.2.1",
                Category = Category.Command,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Duplicated Command Name '0'. Command IDs '1'.",
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
        private readonly IRoot root = new CheckNameTag();

        [TestMethod]
        public void Command_CheckNameTag_CheckCategory() => Generic.CheckCategory(root, Category.Command);

        [TestMethod]
        public void Command_CheckNameTag_CheckId() => Generic.CheckId(root, CheckId.CheckNameTag);
    }
}
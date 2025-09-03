namespace ProtocolTests.Protocol.Actions.Action.Name.CheckNameTag
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Actions.Action.Name.CheckNameTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckNameTag();

        #region Valid Checks

        [TestMethod]
        public void Action_CheckNameTag_Valid()
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
        public void Action_CheckNameTag_DuplicatedValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicatedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicatedValue(null, null, null, "Name1", "1, 2").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Name1", "1"),
                        Error.DuplicatedValue(null, null, null, "Name1", "2"))
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
        public void Action_CheckNameTag_DuplicatedValue()
        {
            // Create ErrorMessage
            var message = Error.DuplicatedValue(null, null, null, "0", "1");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "6.1.1",
                Category = Category.Action,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Duplicated Action Name '0'. Action IDs '1'.",
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
        private readonly IRoot check = new CheckNameTag();

        [TestMethod]
        public void Action_CheckNameTag_CheckCategory() => Generic.CheckCategory(check, Category.Action);

        [TestMethod]
        public void Action_CheckNameTag_CheckId() => Generic.CheckId(check, CheckId.CheckNameTag);
    }
}
namespace ProtocolTests.Protocol.Chains.CheckChildNameAttributes
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Chains.CheckChildNameAttributes;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckChildNameAttributes();

        #region Valid Checks

        [TestMethod]
        public void Chain_CheckChildNameAttributes_Valid()
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
        public void Chain_CheckChildNameAttributes_DuplicatedValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicatedValue.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicatedValue(null, null, null, "Name1").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Name1"),
                        Error.DuplicatedValue(null, null, null, "Name1")),
                    Error.DuplicatedValue(null, null, null, "Name2").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Name2"),
                        Error.DuplicatedValue(null, null, null, "Name2")),
                    Error.DuplicatedValue(null, null, null, "Name3").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Name3"),
                        Error.DuplicatedValue(null, null, null, "Name3"))
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
        public void Action_CheckIdAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.DuplicatedValue(null, null, null, "2");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "15.1.1",
                Category = Category.Chain,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Duplicated Chain child Name '2'.",
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
        private readonly IRoot root = new CheckChildNameAttributes();

        [TestMethod]
        public void Chain_CheckChildNameAttributes_CheckCategory() => Generic.CheckCategory(root, Category.Chain);

        [TestMethod]
        public void Chain_CheckChildNameAttributes_CheckId() => Generic.CheckId(root, CheckId.CheckChildNameAttributes);
    }
}
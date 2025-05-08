namespace ProtocolTests.Protocol.CheckProtocolTag
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckProtocolTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckProtocolTag();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckProtocolTag_Valid()
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
        public void Protocol_CheckProtocolTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null),
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
        public void Protocol_CheckProtocolTag_MissingTag()
        {
            // Create ErrorMessage
            var message = Error.MissingTag(null, null, null);

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "1.1.1",
                Category = Category.Protocol,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = String.Empty,
                Description = "Missing tag 'Protocol'.",
                HowToFix = "Add Protocol root tag to the document.",
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckProtocolTag();

        [TestMethod]
        public void Protocol_CheckProtocolTag_CheckCategory() => Generic.CheckCategory(check, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckProtocolTag_CheckId() => Generic.CheckId(check, CheckId.CheckProtocolTag);
    }
}
namespace ProtocolTests.Protocol.CheckDuplicateTags
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckDuplicateTags;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckDuplicateTags();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckDuplicateTags_Valid()
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
        public void Protocol_CheckDuplicateTags_DuplicateRawTypeTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateRawTypeTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateRawTypeTag(null, null, null, "1"),
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
        public void Protocol_CheckDuplicateTags_DuplicateRawTypeTag()
        {
            // Create ErrorMessage
            var message = Error.DuplicateRawTypeTag(null, null, null, "paramId");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Duplicate 'Interprete/RawType' tag found. Param ID 'paramId'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckDuplicateTags();

        [TestMethod]
        public void Protocol_CheckDuplicateTags_CheckCategory() => Generic.CheckCategory(check, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckDuplicateTags_CheckId() => Generic.CheckId(check, CheckId.CheckDuplicateTags);
    }
}
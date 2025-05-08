namespace ProtocolTests.Protocol.Params.Param.Interprete.Length.CheckLengthTag
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Length.CheckLengthTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckLengthTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckLengthTag_Valid()
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
        public void Param_CheckLengthTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "300"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckLengthTag_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, Severity.Major, "abcd", "300"),
                    Error.InvalidValue(null, null, null, Severity.Critical, "5", "6024"),
                    Error.InvalidValue(null, null, null, Severity.Critical, "8", "6025"),
                    Error.InvalidValue(null, null, null, Severity.Critical, "7", "6026"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckLengthTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "1"),
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
        public void Param_CheckLengthTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "6024");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty tag 'Length' in Param '6024'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckLengthTag_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, Severity.Major, "tagValue", "6024");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invalid value 'tagValue' in tag 'Length'. Param ID '6024'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckLengthTag_MissingTag()
        {
            // Create ErrorMessage
            var message = Error.MissingTag(null, null, null, "6024");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Missing tag 'Length' in Param '6024'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckLengthTag();

        [TestMethod]
        public void Param_CheckLengthTag_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckLengthTag_CheckId() => Generic.CheckId(check, CheckId.CheckLengthTag);
    }
}
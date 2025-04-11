namespace ProtocolTests.Protocol.ExportRules.ExportRule.CheckWhereTagAttribute
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ExportRules.ExportRule.CheckWhereTagAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckWhereTagAttribute();

        #region Valid Checks

        [TestMethod]
        public void ExportRule_CheckWhereTagAttribute_Valid()
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
        public void ExportRule_CheckWhereTagAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void ExportRule_CheckWhereTagAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null),
                    Error.MissingAttribute(null, null, null),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void ExportRule_CheckWhereTagAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, " Protocol/Params/Param/Name "),
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
        public void ExportRule_CheckWhereTagAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null);

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty attribute 'ExportRule@whereTag'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void ExportRule_CheckWhereTagAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null);

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Missing attribute 'ExportRule@whereTag'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void ExportRule_CheckWhereTagAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "untrimmedValue");

            var expected = new ValidationResult
            {
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed attribute 'ExportRule@whereTag'. Current value 'untrimmedValue'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckWhereTagAttribute();

        [TestMethod]
        public void ExportRule_CheckWhereTagAttribute_CheckCategory() => Generic.CheckCategory(check, Category.ExportRule);

        [TestMethod]
        public void ExportRule_CheckWhereTagAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckWhereTagAttribute);
    }
}
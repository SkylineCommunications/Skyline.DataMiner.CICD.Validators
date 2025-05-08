namespace ProtocolTests.Protocol.Display.Pages.Page.Visibility.CheckOverridePidAttribute
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Display.Pages.Page.Visibility.CheckOverridePidAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckOverridePidAttribute();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckOverridePidAttribute_Valid()
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
        public void Protocol_CheckOverridePidAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "Empty"),
                    Error.EmptyAttribute(null, null, null, "Spaces"),
                    Error.EmptyAttribute(null, null, null, "Enters"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckOverridePidAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "Page1_Missing"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckOverridePidAttribute_NonExistingParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingParam",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingParam(null, null, null, "10 ", "Page1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by check on RTDisplay tag")]
        public void Protocol_CheckOverridePidAttribute_ReferencedParamExpectingRTDisplay()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedParamExpectingRTDisplay",
                ExpectedResults = new List<IValidationResult>
                {
                    ////Error.ReferencedParamExpectingRTDisplay(null, null, null, "10", "RTDisplay_False"),
                    ////Error.ReferencedParamExpectingRTDisplay(null, null, null, "11", "NoRTDisplay"),
                    ////Error.ReferencedParamExpectingRTDisplay(null, null, null, "12", "NoDisplay"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckOverridePidAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "Leading", " 10"),
                    Error.UntrimmedAttribute(null, null, null, "Trailing", "11  "),
                    Error.UntrimmedAttribute(null, null, null, "LeadingAndTrailing", "  12 "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckOverridePidAttribute();

        [TestMethod]
        public void Protocol_CheckOverridePidAttribute_UntrimmedAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttribute",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Protocol_CheckOverridePidAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "pageName");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "1.27.2",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty attribute 'Page.Visibility@overridePID' in Page 'pageName'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckOverridePidAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null, "pageName");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "1.27.1",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Missing attribute 'Page.Visibility@overridePID' in Page 'pageName'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckOverridePidAttribute_NonExistingParam()
        {
            // Create ErrorMessage
            var message = Error.NonExistingParam(null, null, null, "pid", "pageName");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "1.27.4",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Attribute 'Page.Visibility@overridePID' references a non-existing 'Param' with ID 'pid'. Page Name 'pageName'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckOverridePidAttribute_ReferencedParamExpectingRTDisplay()
        {
            // Create ErrorMessage
            var message = Error.ReferencedParamExpectingRTDisplay(null, null, null, "pid", "pageName");

            var expected = new ValidationResult
            {
                ErrorId = 5,
                FullId = "1.27.5",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "RTDisplay(true) expected on Param 'pid' used as page visibility condition. Page name 'pageName'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckOverridePidAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "pageName", "untrimmedValue");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "1.27.3",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed attribute 'Page.Visibility@overridePID' in Page 'pageName'. Current value 'untrimmedValue'.",
                HowToFix = "",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckOverridePidAttribute();

        [TestMethod]
        public void Protocol_CheckOverridePidAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckOverridePidAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckOverridePidAttribute);
    }
}
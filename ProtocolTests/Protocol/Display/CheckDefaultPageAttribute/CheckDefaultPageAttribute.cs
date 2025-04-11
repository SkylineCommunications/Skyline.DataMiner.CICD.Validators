namespace ProtocolTests.Protocol.Display.CheckDefaultPageAttribute
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Display.CheckDefaultPageAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckDefaultPageAttribute();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_Valid()
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
        public void Protocol_CheckDefaultPageAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_UnexistingPage()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnexistingPage.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnexistingPage(null, null, null, "ThisIsAnUnexistingPage"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, " General ")
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_UnsupportedPage()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedPage.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnsupportedPage(null, null, null, "popupPage"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_InvalidDefaultPage()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidDefaultPage.xml",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidDefaultPage(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckDefaultPageAttribute();

        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_UntrimmedAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttribute",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null);

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "1.21.1",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Missing attribute 'defaultPage'.",
                HowToFix = "Specify a default page using the defaultPage attribute.",
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null);

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "1.21.2",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Empty attribute 'defaultPage'.",
                HowToFix = "Specify a default page in the defaultPage attribute.",
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_UnexistingPage()
        {
            // Create ErrorMessage
            var message = Error.UnexistingPage(null, null, null, "ABC");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "1.21.3",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "The specified defaultPage 'ABC' does not exist.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "ABC");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "1.21.4",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Untrimmed attribute 'defaultPage'. Current value 'ABC'.",
                HowToFix = String.Empty,
                HasCodeFix = true
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_InvalidDefaultPage()
        {
            // Create ErrorMessage
            var message = Error.InvalidDefaultPage(null, null, null);

            var expected = new ValidationResult
            {
                ErrorId = 5,
                FullId = "1.21.5",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "The default page should be a page with name 'General'.",
                HowToFix = "Define a page with name 'General' and specify it as the default page.",
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_UnsupportedPage()
        {
            // Create ErrorMessage
            var message = Error.UnsupportedPage(null, null, null, "ABC");

            var expected = new ValidationResult
            {
                ErrorId = 6,
                FullId = "1.21.6",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Unsupported popup page 'ABC' in defaultPage attribute.",
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
        private readonly IRoot root = new CheckDefaultPageAttribute();

        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckDefaultPageAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckDefaultPageAttribute);
    }
}
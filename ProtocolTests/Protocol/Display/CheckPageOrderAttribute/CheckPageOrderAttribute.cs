namespace ProtocolTests.Protocol.Display.CheckPageOrderAttribute
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Display.CheckPageOrderAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckPageOrderAttribute();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_Valid()
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
        public void Protocol_CheckPageOrderAttribute_ValidWithDVE()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidWithDVE.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_ValidWithDVEs()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidWithDVEs.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_ValidWithoutWebPage()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidWithoutWebPage.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_ValidWithWebPage()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidWithWebPage.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_DuplicateEntries()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateEntries",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateEntries(null, null, null, "General"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_EmptyAttribute()
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

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_MissingPage()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingPage",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingPage(null, null, null, "Main").WithSubResults(
                        Error.MissingPage_Sub(null, null, null, "101", "Main"),
                        Error.MissingPage_Sub(null, null, null, "102", "Main"))
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_MissingWebPage()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingWebPage",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingWebPage(null, null, null),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "100"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay check")]
        public void Protocol_CheckPageOrderAttribute_ReferencedParamRTDisplayExpected()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedParamRTDisplayExpected",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ReferencedParamRTDisplayExpected(null, null, null, "100"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_UnexistingPage()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnexistingPage",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnexistingPage(null, null, null, "Main"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_UnsupportedPage()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedPage",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnsupportedPage(null, null, null, "Settings"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "General    "),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_WrongWebPagePosition()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "WrongWebPagePosition",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.WrongWebPagePosition(null, null, null, "Web Interface#http://[Polling Ip]/"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckPageOrderAttribute();

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_DuplicateEntries()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "DuplicateEntries",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_UnsupportedPage()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnsupportedPage",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_UntrimmedAttribute()
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
        public void Protocol_CheckPageOrderAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null);

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "1.22.1",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Missing attribute 'pageOrder'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null);

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "1.22.2",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Empty attribute 'pageOrder'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "ABC");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "1.22.3",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Untrimmed attribute 'pageOrder'. Current value 'ABC'.",
                HowToFix = String.Empty,
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_UnsupportedPage()
        {
            // Create ErrorMessage
            var message = Error.UnsupportedPage(null, null, null, "ABC");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "1.22.4",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Unsupported popup page 'ABC' in 'Protocol/Display@pageOrder' attribute.",
                HowToFix = String.Empty,
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_MissingPage()
        {
            // Create ErrorMessage
            var message = Error.MissingPage(null, null, null, "ABC");

            var expected = new ValidationResult
            {
                ErrorId = 5,
                FullId = "1.22.5",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Missing page 'ABC' on 'Protocol/Display@pageOrder' attribute.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_MissingWebPage()
        {
            // Create ErrorMessage
            var message = Error.MissingWebPage(null, null, null);

            var expected = new ValidationResult
            {
                ErrorId = 6,
                FullId = "1.22.6",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Missing WebInterface page.",
                HowToFix = "Make sure the webInterface page(s) are defined as last and preceded by a separator page.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_WrongWebPagePosition()
        {
            // Create ErrorMessage
            var message = Error.WrongWebPagePosition(null, null, null, "ABC");

            var expected = new ValidationResult
            {
                ErrorId = 7,
                FullId = "1.22.7",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Web page 'ABC' should be defined after all regular pages and the first web page should be preceded by a separator.",
                HowToFix = "Make sure the webInterface page(s) are defined as last and preceded by a separator page.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_UnexistingPage()
        {
            // Create ErrorMessage
            var message = Error.UnexistingPage(null, null, null, "ABC");

            var expected = new ValidationResult
            {
                ErrorId = 8,
                FullId = "1.22.8",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "The specified page 'ABC' does not exist.",
                HowToFix = String.Empty,
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_DuplicateEntries()
        {
            // Create ErrorMessage
            var message = Error.DuplicateEntries(null, null, null, "ABC");

            var expected = new ValidationResult
            {
                ErrorId = 9,
                FullId = "1.22.9",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Page 'ABC' has been added more than once to the pageOrder attribute.",
                HowToFix = String.Empty,
                HasCodeFix = true
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_MissingPage_Sub()
        {
            // Create ErrorMessage
            var message = Error.MissingPage_Sub(null, null, null, "1", "ABC");

            var expected = new ValidationResult
            {
                ErrorId = 10,
                FullId = "1.22.10",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Param with ID '1' is positioned on page 'ABC' which is not ordered via 'Protocol/Display@pageOrder' attribute.",
                HowToFix = String.Empty,
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckPageOrderAttribute();

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckPageOrderAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckPageOrderAttribute);
    }
}
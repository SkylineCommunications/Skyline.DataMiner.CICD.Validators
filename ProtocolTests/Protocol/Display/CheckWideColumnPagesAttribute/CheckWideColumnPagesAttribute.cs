namespace ProtocolTests.Protocol.Display.CheckWideColumnPagesAttribute
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Display.CheckWideColumnPagesAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckWideColumnPagesAttribute();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckWideColumnPagesAttribute_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckWideColumnPagesAttribute_ValidNoPage()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidNoPage",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckWideColumnPagesAttribute_ValidNoWidePage()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidNoWidePage",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        [Ignore("Disabled as long as the Display Editor automatically adds the empty attribute.")]
        public void Protocol_CheckWideColumnPagesAttribute_EmptyAttribute()
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
        public void Protocol_CheckWideColumnPagesAttribute_UnexistingPage()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnexistingPage",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnexistingPage(null, null, null, "unexistingPage"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckWideColumnPagesAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, " Wide Page 1;Wide Page 2 ; Wide Page 3 ")
                        .WithSubResults(
                            Error.UntrimmedAttribute(null, null, null, " Wide Page 1"),
                            Error.UntrimmedAttribute(null, null, null, "Wide Page 2 "),
                            Error.UntrimmedAttribute(null, null, null, " Wide Page 3 ")),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckWideColumnPagesAttribute();

        [TestMethod]
        [Ignore("Disabled as long as the Display Editor automatically adds the empty attribute.")]
        public void Protocol_CheckWideColumnPagesAttribute_EmptyAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyAttribute",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void Protocol_CheckWideColumnPagesAttribute_UntrimmedAttribute()
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
        public void Protocol_CheckWideColumnPagesAttribute_UnexistingPage()
        {
            // Create ErrorMessage
            var message = Error.UnexistingPage(null, null, null, "pageName");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "1.29.2",
                Category = Category.Protocol,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "The page 'pageName' specified in 'Protocol/Display@wideColumnPages' does not exist.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckWideColumnPagesAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "untrimmedValue");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "1.29.3",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed attribute 'wideColumnsPages'. Current value 'untrimmedValue'.",
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
        private readonly IRoot check = new CheckWideColumnPagesAttribute();

        [TestMethod]
        public void Protocol_CheckWideColumnPagesAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckWideColumnPagesAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckWideColumnPagesAttribute);
    }
}
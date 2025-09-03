namespace ProtocolTests.Protocol.Actions.Action.CheckIdAttribute
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Actions.Action.CheckIdAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckIdAttribute();

        #region Valid Checks

        [TestMethod]
        public void Action_CheckIdAttribute_Valid()
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
        public void Action_CheckIdAttribute_DuplicatedId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicatedId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicatedId(null, null, null, "101", "Duplicate_101_1, Duplicate_101_2").WithSubResults(
                        Error.DuplicatedId(null, null, null, "101", "Duplicate_101_1"),
                        Error.DuplicatedId(null, null, null, "101", "Duplicate_101_2")),

                    Error.DuplicatedId(null, null, null, "102", "Duplicate_102_1, Duplicate_102_2, Duplicate_102_3").WithSubResults(
                        Error.DuplicatedId(null, null, null, "102", "Duplicate_102_1"),
                        Error.DuplicatedId(null, null, null, "102", "Duplicate_102_2"),
                        Error.DuplicatedId(null, null, null, "102", "Duplicate_102_3"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckIdAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null),
                    Error.EmptyAttribute(null, null, null),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckIdAttribute_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "aaa", "String"),

                    Error.InvalidValue(null, null, null, "-2", "Number_Negative"),
                    Error.InvalidValue(null, null, null, "1.5", "Number_Double_1"),
                    Error.InvalidValue(null, null, null, "2,6", "Number_Double_2"),
                    Error.InvalidValue(null, null, null, "03", "Number_LeadingZero"),
                    Error.InvalidValue(null, null, null, "+4", "Number_LeadingPlusSign"),
                    Error.InvalidValue(null, null, null, "5x10^1", "Number_ScientificNotation"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckIdAttribute_MissingAttribute()
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

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckIdAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, " 101"),
                    Error.UntrimmedAttribute(null, null, null, "102 "),
                    Error.UntrimmedAttribute(null, null, null, " 103 "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckIdAttribute();

        [TestMethod]
        public void Action_CheckIdAttribute_UntrimmedAttribute()
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
        public void Action_CheckIdAttribute_DuplicatedId()
        {
            // Create ErrorMessage
            var message = Error.DuplicatedId(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 5,
                FullId = "6.2.5",
                Category = Category.Action,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = String.Empty,
                Description = "More than one Action with same ID '2'. Action Names '3'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Action_CheckIdAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null);

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "6.2.2",
                Category = Category.Action,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = String.Empty,
                Description = "Empty attribute 'Action@id'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Action_CheckIdAttribute_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "2", "MyName");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "6.2.4",
                Category = Category.Action,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = String.Empty,
                Description = "Invalid value '2' in attribute 'Action@id'. Action name 'MyName'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Action_CheckIdAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null);

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "6.2.1",
                Category = Category.Action,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = String.Empty,
                Description = "Missing attribute 'Action@id'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Action_CheckIdAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "2");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "6.2.3",
                Category = Category.Action,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Untrimmed attribute 'Action@id'. Current value '2'.",
                HowToFix = String.Empty,
                HasCodeFix = true
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckIdAttribute();

        [TestMethod]
        public void Action_CheckIdAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Action);

        [TestMethod]
        public void Action_CheckIdAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckIdAttribute);
    }
}
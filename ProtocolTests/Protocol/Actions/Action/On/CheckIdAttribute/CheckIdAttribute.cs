namespace ProtocolTests.Protocol.Actions.Action.On.CheckIdAttribute
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Actions.Action.On.CheckIdAttribute;

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
        public void Action_CheckIdAttribute_EmptyAttibute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttibute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttibute(null, null, null, "100"),
                    Error.EmptyAttibute(null, null, null, "101"),
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
                    Error.InvalidValue(null, null, null, "aaa", "100"),

                    Error.InvalidValue(null, null, null, "-2;1.5;2,6", "200").WithSubResults(
                        Error.InvalidValue(null, null, null, "-2", "200"),
                        Error.InvalidValue(null, null, null, "1.5", "200"),
                        Error.InvalidValue(null, null, null, "2,6", "200")),

                    Error.InvalidValue(null, null, null, "03;+4;5x10^1", "203").WithSubResults(
                        Error.InvalidValue(null, null, null, "03", "203"),
                        Error.InvalidValue(null, null, null, "+4", "203"),
                        Error.InvalidValue(null, null, null, "5x10^1", "203"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("TODO")]
        public void Action_CheckIdAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "actionType", "actionId"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckIdAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    // Command
                    Error.NonExistingId(null, null, null, "Command", "108", "101"),

                    Error.NonExistingId(null, null, null, "Command", "108", "102"),
                    Error.NonExistingId(null, null, null, "Command", "109", "102"),
                    
                    // Group
                    Error.NonExistingId(null, null, null, "Group", "208", "201"),

                    Error.NonExistingId(null, null, null, "Group", "208", "202"),
                    Error.NonExistingId(null, null, null, "Group", "209", "202"),
                    
                    // Pair
                    Error.NonExistingId(null, null, null, "Pair", "308", "301"),

                    Error.NonExistingId(null, null, null, "Pair", "308", "302"),
                    Error.NonExistingId(null, null, null, "Pair", "309", "302"),
                    
                    // Param
                    Error.NonExistingId(null, null, null, "Param", "408", "401"),

                    Error.NonExistingId(null, null, null, "Param", "408", "402"),
                    Error.NonExistingId(null, null, null, "Param", "409", "402"),
                    
                    // Response
                    Error.NonExistingId(null, null, null, "Response", "608", "601"),

                    Error.NonExistingId(null, null, null, "Response", "608", "602"),
                    Error.NonExistingId(null, null, null, "Response", "609", "602"),
                    
                    // Timer
                    Error.NonExistingId(null, null, null, "Timer", "708", "701"),

                    Error.NonExistingId(null, null, null, "Timer", "708", "702"),
                    Error.NonExistingId(null, null, null, "Timer", "709", "702"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Action_CheckIdAttribute_UntrimmedValueInAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedValueInAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedValueInAttribute(null, null, null, " 101", "1"),
                    Error.UntrimmedValueInAttribute(null, null, null, "102 ", "2"),
                    Error.UntrimmedValueInAttribute(null, null, null, " 103 ", "3"),

                    Error.UntrimmedValueInAttribute(null, null, null, "101 ; 102", "102").WithSubResults(
                        Error.UntrimmedValueInAttribute(null, null, null, "101 ", "102"),
                        Error.UntrimmedValueInAttribute(null, null, null, " 102", "102")),

                    Error.UntrimmedValueInAttribute(null, null, null, " 101;102 ; 103", "103").WithSubResults(
                        Error.UntrimmedValueInAttribute(null, null, null, " 101", "103"),
                        Error.UntrimmedValueInAttribute(null, null, null, "102 ", "103"),
                        Error.UntrimmedValueInAttribute(null, null, null, " 103", "103")),

                    Error.UntrimmedValueInAttribute(null, null, null, "101 ; 102;103 ; 104 ;105;106;107", "104").WithSubResults(
                        Error.UntrimmedValueInAttribute(null, null, null, "101 ", "104"),
                        Error.UntrimmedValueInAttribute(null, null, null, " 102", "104"),
                        Error.UntrimmedValueInAttribute(null, null, null, "103 ", "104"),
                        Error.UntrimmedValueInAttribute(null, null, null, " 104 ", "104"))
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
        public void Action_CheckIdAttribute_UntrimmedValueInAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedValueInAttribute",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Action_CheckIdAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttibute(null, null, null, "2");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "6.3.2",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Empty attribute 'On@id' in Action '2'.",
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
            var message = Error.InvalidValue(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "6.3.4",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Invalid value '2' in attribute 'On@id'. Action ID '3'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Action_CheckIdAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "2", "3", "4");

            var expected = new ValidationResult
            {
                ErrorId = 5,
                FullId = "6.3.5",
                Category = Category.Action,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Attribute 'On@id' references a non-existing '2' with ID '3'. Action ID '4'.",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Action_CheckIdAttribute_UntrimmedValueInAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedValueInAttribute(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "6.3.3",
                Category = Category.Action,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Untrimmed value '2' in attribute 'On@id'. Action ID '3'.",
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
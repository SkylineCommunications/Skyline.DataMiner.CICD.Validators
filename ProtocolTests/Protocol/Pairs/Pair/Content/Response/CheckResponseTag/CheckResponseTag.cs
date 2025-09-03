namespace ProtocolTests.Protocol.Pairs.Pair.Content.Response.CheckResponseTag
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Pairs.Pair.Content.Response.CheckResponseTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckResponseTag();

        #region Valid Checks

        [TestMethod]
        public void Pair_CheckResponseTag_Valid()
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
        public void Pair_CheckResponseTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "1"),
                    Error.EmptyTag(null, null, null, "2"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Pair_CheckResponseTag_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "aaa", "100"),

                    Error.InvalidValue(null, null, null, "-2", "200"),
                    Error.InvalidValue(null, null, null, "1.5", "200"),
                    Error.InvalidValue(null, null, null, "2,6", "200"),

                    Error.InvalidValue(null, null, null, "03", "203"),
                    Error.InvalidValue(null, null, null, "+4", "203"),
                    Error.InvalidValue(null, null, null, "5x10^1", "203"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Pair_CheckResponseTag_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "9", "1"),

                    Error.NonExistingId(null, null, null, "9", "2"),
                    Error.NonExistingId(null, null, null, "99", "2"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Pair_CheckResponseTag_NonExistingIdNoResponses()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoResponses",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "9", "1"),

                    Error.NonExistingId(null, null, null, "9", "2"),
                    Error.NonExistingId(null, null, null, "99", "2"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Pair_CheckResponseTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "1", " 101"),
                    Error.UntrimmedTag(null, null, null, "1", "102 "),

                    Error.UntrimmedTag(null, null, null, "3", " 103 "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckResponseTag();

        [TestMethod]
        public void Pair_CheckResponseTag_UntrimmedTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedTag",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Pair_CheckResponseTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "2");

            var expected = new ValidationResult()
            {
                ErrorId = 2,
                FullId = "9.5.2",
                Category = Category.Pair,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty tag 'Content/Response' in Pair '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Pair_CheckResponseTag_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 4,
                FullId = "9.5.4",
                Category = Category.Pair,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invalid value '2' in tag 'Content/Response'. Pair ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Pair_CheckResponseTag_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 5,
                FullId = "9.5.5",
                Category = Category.Pair,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Tag 'Content/Response' references a non-existing 'Response' with ID '2'. Pair ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Pair_CheckResponseTag_UntrimmedTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedTag(null, null, null, "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 3,
                FullId = "9.5.3",
                Category = Category.Pair,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed tag 'Content/Response' in Pair '2'. Current value '3'.",
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
        private readonly IRoot check = new CheckResponseTag();

        [TestMethod]
        public void Pair_CheckResponseTag_CheckCategory() => Generic.CheckCategory(check, Category.Pair);

        [TestMethod]
        public void Pair_CheckResponseTag_CheckId() => Generic.CheckId(check, CheckId.CheckResponseTag);
    }
}
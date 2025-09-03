namespace ProtocolTests.Protocol.Triggers.Trigger.Content.Id.CheckIdTag
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Triggers.Trigger.Content.Id.CheckIdTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckIdTag();

        #region Valid Checks

        [TestMethod]
        public void Trigger_CheckIdTag_Valid()
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
                    Error.EmptyTag(null, null, null, "100"),
                    Error.EmptyTag(null, null, null, "101"),
                    Error.EmptyTag(null, null, null, "101"),

                    Error.EmptyTag(null, null, null, "1000"),
                    Error.EmptyTag(null, null, null, "1001"),
                    Error.EmptyTag(null, null, null, "1001"),
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

                    Error.InvalidValue(null, null, null, "-2", "1000"),
                    Error.InvalidValue(null, null, null, "1.5", "1000"),
                    Error.InvalidValue(null, null, null, "2,6", "1000"),

                    Error.InvalidValue(null, null, null, "03", "1001"),
                    Error.InvalidValue(null, null, null, "+4", "1001"),
                    Error.InvalidValue(null, null, null, "5x10^1", "1001"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Pair_CheckResponseTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "100"),
                    Error.MissingTag(null, null, null, "101"),
                    Error.MissingTag(null, null, null, "102"),
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
                    Error.NonExistingId(null, null, null, "Trigger", "9", "100"),
                    Error.NonExistingId(null, null, null, "Trigger", "99", "101"),
                    Error.NonExistingId(null, null, null, "Trigger", "999", "101"),

                    Error.NonExistingId(null, null, null, "Action", "8", "1000"),
                    Error.NonExistingId(null, null, null, "Action", "88", "1001"),
                    Error.NonExistingId(null, null, null, "Action", "888", "1001"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Pair_CheckResponseTag_NonExistingIdNoActions()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIdNoActions",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "Trigger", "9", "100"),
                    Error.NonExistingId(null, null, null, "Trigger", "99", "101"),
                    Error.NonExistingId(null, null, null, "Trigger", "999", "101"),

                    Error.NonExistingId(null, null, null, "Action", "8", "1000"),
                    Error.NonExistingId(null, null, null, "Action", "88", "1001"),
                    Error.NonExistingId(null, null, null, "Action", "888", "1001"),
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
                    Error.UntrimmedTag(null, null, null, "100", " 1000"),
                    Error.UntrimmedTag(null, null, null, "100", "1001 "),
                    Error.UntrimmedTag(null, null, null, "101", " 1000 "),

                    Error.UntrimmedTag(null, null, null, "1000", " 1000"),
                    Error.UntrimmedTag(null, null, null, "1000", "1001 "),
                    Error.UntrimmedTag(null, null, null, "1001", " 1000 "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckIdTag();

        [TestMethod]
        public void Trigger_CheckIdTag_UntrimmedTag()
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
        public void Trigger_CheckIdTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "2");

            var expected = new ValidationResult()
            {
                ErrorId = 2,
                FullId = "5.10.2",
                Category = Category.Trigger,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty tag 'Content/Id' in Trigger '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Trigger_CheckIdTag_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 4,
                FullId = "5.10.4",
                Category = Category.Trigger,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invalid value '2' in tag 'Content/Id'. Trigger ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Trigger_CheckIdTag_MissingTag()
        {
            // Create ErrorMessage
            var message = Error.MissingTag(null, null, null, "2");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "5.10.1",
                Category = Category.Trigger,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Missing tag 'Content/Id' in Trigger '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Trigger_CheckIdTag_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "2", "3", "4");

            var expected = new ValidationResult()
            {
                ErrorId = 5,
                FullId = "5.10.5",
                Category = Category.Trigger,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Tag 'Content/Id' references a non-existing '2' with ID '3'. Trigger ID '4'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Trigger_CheckIdTag_UntrimmedTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedTag(null, null, null, "2", "3");

            var expected = new ValidationResult()
            {
                ErrorId = 3,
                FullId = "5.10.3",
                Category = Category.Trigger,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed tag 'Content/Id' in Trigger '2'. Current value '3'.",
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
        private readonly IRoot check = new CheckIdTag();

        [TestMethod]
        public void Trigger_CheckIdTag_CheckCategory() => Generic.CheckCategory(check, Category.Trigger);

        [TestMethod]
        public void Trigger_CheckIdTag_CheckId() => Generic.CheckId(check, CheckId.CheckIdTag);
    }
}
namespace ProtocolTests.Protocol.Params.Param.Display.Positions.Position.Row.CheckRowTag
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Positions.Position.Row.CheckRowTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckRowTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckRowTag_Valid()
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
        public void Param_CheckRowTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckRowTag_InvalidTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidTag(null, null, null, "abc", "1"),
                    Error.InvalidTag(null, null, null, "-1", "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckRowTag_MissingTag()
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

        [TestMethod]
        public void Param_CheckRowTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "1", " 1"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckRowTag();

        [TestMethod]
        public void Param_CheckRowTag_UntrimmedTag()
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
        public void Param_CheckRowTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "2");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "2.43.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty tag 'Position/Row' in Param '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckRowTag_InvalidTag()
        {
            // Create ErrorMessage
            var message = Error.InvalidTag(null, null, null, "abc", "100");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "2.43.3",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invalid value 'abc' in tag 'Position/Row'. Param ID '100'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckRowTag_MissingTag()
        {
            // Create ErrorMessage
            var message = Error.MissingTag(null, null, null, "2");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "2.43.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Missing tag 'Position/Row' in Param '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckRowTag_UntrimmedTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedTag(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "2.43.4",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed tag 'Position/Row' in Param '2'. Current value '3'.",
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
        private readonly IRoot check = new CheckRowTag();

        [TestMethod]
        public void Param_CheckRowTag_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckRowTag_CheckId() => Generic.CheckId(check, CheckId.CheckRowTag);
    }
}
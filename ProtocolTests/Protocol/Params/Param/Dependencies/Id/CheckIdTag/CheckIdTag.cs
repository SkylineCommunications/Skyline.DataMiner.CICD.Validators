namespace ProtocolTests.Protocol.Params.Param.Dependencies.Id.CheckIdTag
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Dependencies.Id.CheckIdTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckIdTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckIdTag_Valid()
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
        public void Param_CheckIdTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "10"),
                    Error.EmptyTag(null, null, null, "10"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckIdTag_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "11", "10"),
                    Error.NonExistingId(null, null, null, "12 ", "10"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by check on RTDisplay")]
        public void Param_CheckIdTag_RTDisplayExpected()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "10"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by check on RTDisplay")]
        public void Param_CheckIdTag_RTDisplayExpectedOnReferencedParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpectedOnReferencedParam",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpectedOnReferencedParam(null, null, null, "11", "10"),

                    Error.RTDisplayExpectedOnReferencedParam(null, null, null, "21", "20"),

                    Error.RTDisplayExpectedOnReferencedParam(null, null, null, "31", "30"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckIdTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "10", " 11"),
                    Error.UntrimmedTag(null, null, null, "10", "12 "),
                    Error.UntrimmedTag(null, null, null, "10", " 13 "),
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
        public void Param_CheckIdTag_EmptyTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyTag",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void Param_CheckIdTag_UntrimmedTag()
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
        public void Param_CheckIdTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "pid");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "2.67.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Missing tag 'Dependencies/Id' in Param 'pid'.",
                HowToFix = "",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckIdTag_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "referencedPid", "pid");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "2.67.3",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Attribute 'Dependencies/Id' references a non-existing 'Param' with ID 'referencedPid'. Param ID 'pid'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckIdTag_RTDisplayExpected()
        {
            // Create ErrorMessage
            var message = Error.RTDisplayExpected(null, null, null, "pid");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "2.67.4",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "RTDisplay(true) expected on Param 'pid' containing 'Dependencies/Id' tag(s).",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckIdTag_RTDisplayExpectedOnReferencedParam()
        {
            // Create ErrorMessage
            var message = Error.RTDisplayExpectedOnReferencedParam(null, null, null, "referencedPid", "referencingPid");

            var expected = new ValidationResult
            {
                ErrorId = 5,
                FullId = "2.67.5",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "RTDisplay(true) expected on Param 'referencedPid' referenced by a 'Dependencies/Id' tag. Param ID 'referencingPid'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckIdTag_UntrimmedTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedTag(null, null, null, "pid", "untrimmedValue");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "2.67.2",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed tag 'Dependencies/Id' in Param 'pid'. Current value 'untrimmedValue'.",
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
        public void Param_CheckIdTag_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckIdTag_CheckId() => Generic.CheckId(check, CheckId.CheckIdTag);
    }
}
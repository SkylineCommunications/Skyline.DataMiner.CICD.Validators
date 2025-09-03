namespace ProtocolTests.Protocol.Params.Param.Measurement.Discreets.Discreet.CheckDependencyValuesAttribute
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.Discreet.CheckDependencyValuesAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckDependencyValuesAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckDependencyValuesAttribute_Valid()
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
        public void Param_CheckDependencyValuesAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "100"),
                    Error.EmptyAttribute(null, null, null, "100"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckDependencyValuesAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1002", "999"),
                    Error.NonExistingId(null, null, null, "1003", "999"),

                    Error.NonExistingId(null, null, null, "1002", "999"),
                    Error.NonExistingId(null, null, null, "1003", "999"),
                    Error.NonExistingId(null, null, null, "1004", "999"),
                    Error.NonExistingId(null, null, null, "1005", "999"),
                    Error.NonExistingId(null, null, null, "1006", "999"),
                    Error.NonExistingId(null, null, null, "AAA", "999"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Param_CheckDependencyValuesAttribute_ReferencedParamExpectingRTDisplay()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedParamExpectingRTDisplay",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ReferencedParamExpectingRTDisplay(null, null, null, "1002", "999"),
                    Error.ReferencedParamExpectingRTDisplay(null, null, null, "1003", "999"),

                    Error.ReferencedParamExpectingRTDisplay(null, null, null, "1002", "999"),
                    Error.ReferencedParamExpectingRTDisplay(null, null, null, "1003", "999"),
                    Error.ReferencedParamExpectingRTDisplay(null, null, null, "1004", "999"),
                    Error.ReferencedParamExpectingRTDisplay(null, null, null, "1004", "999"),
                    Error.ReferencedParamExpectingRTDisplay(null, null, null, "1005", "999"),
                    Error.ReferencedParamExpectingRTDisplay(null, null, null, "1005", "999"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckDependencyValuesAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "100", " Leading"),
                    Error.UntrimmedAttribute(null, null, null, "100", "Trailing "),
                    Error.UntrimmedAttribute(null, null, null, "100", " LeadingAndTrailing "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckDependencyValuesAttribute();

        [TestMethod]
        public void Param_CheckDependencyValuesAttribute_UntrimmedAttribute()
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
        public void Param_CheckDependencyValuesAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "2");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "2.59.1",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty attribute 'Discreet@dependencyValues' in Param '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckDependencyValuesAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "2.59.3",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Attribute 'Discreet@dependencyValues' references a non-existing 'Param' with ID '2'. Param ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckDependencyValuesAttribute_ReferencedParamExpectingRTDisplay()
        {
            // Create ErrorMessage
            var message = Error.ReferencedParamExpectingRTDisplay(null, null, null, "1002", "999");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "2.59.4",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "RTDisplay(true) expected on Param '1002' referenced in 'Discreet@dependencyValues' attribute. Param ID '999'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckDependencyValuesAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "2.59.2",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed attribute 'Discreet@dependencyValues' in Param '2'. Current value '3'.",
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
        private readonly IRoot check = new CheckDependencyValuesAttribute();

        [TestMethod]
        public void Param_CheckDependencyValuesAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckDependencyValuesAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckDependencyValuesAttribute);
    }
}
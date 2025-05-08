namespace ProtocolTests.Protocol.Params.Param.Measurement.Discreets.CheckDependencyId
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.CheckDependencyId;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckDependencyId();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckDependencyId_Valid()
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
        public void Param_CheckDependencyId_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "1000"),
                    Error.EmptyAttribute(null, null, null, "1001"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckDependencyId_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "aaa", "1000"),
                    Error.InvalidValue(null, null, null, "-10", "1001"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckDependencyId_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "100", "1000"),
                    Error.NonExistingId(null, null, null, "200", "2000"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Param_CheckDependencyId_ReferencedParamRTDisplayExpected()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedParamRTDisplayExpected",
                ExpectedResults = new List<IValidationResult>
                {

                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckDependencyId_ReferencedParamWrongType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedParamWrongType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ReferencedParamWrongType(null, null, null, "array", "100"),
                    Error.ReferencedParamWrongType(null, null, null, "bus", "101"),
                    Error.ReferencedParamWrongType(null, null, null, "crc", "102"),
                    Error.ReferencedParamWrongType(null, null, null, "dataminer info", "103"),
                    Error.ReferencedParamWrongType(null, null, null, "discreet info", "104"),
                    Error.ReferencedParamWrongType(null, null, null, "dummy", "105"),
                    Error.ReferencedParamWrongType(null, null, null, "elementdmaid", "106"),
                    Error.ReferencedParamWrongType(null, null, null, "elementid", "107"),
                    Error.ReferencedParamWrongType(null, null, null, "elementname", "108"),
                    Error.ReferencedParamWrongType(null, null, null, "fixed", "109"),
                    Error.ReferencedParamWrongType(null, null, null, "group", "110"),
                    Error.ReferencedParamWrongType(null, null, null, "header", "111"),
                    Error.ReferencedParamWrongType(null, null, null, "ip", "112"),
                    Error.ReferencedParamWrongType(null, null, null, "length", "113"),
                    Error.ReferencedParamWrongType(null, null, null, "pollingip", "114"),
                    //Error.ReferencedParamWrongType(null, null, null, "read", "115"),
                    //Error.ReferencedParamWrongType(null, null, null, "read bit", "116"),
                    Error.ReferencedParamWrongType(null, null, null, "response", "117"),
                    Error.ReferencedParamWrongType(null, null, null, "trailer", "118"),
                    Error.ReferencedParamWrongType(null, null, null, "write", "119"),
                    Error.ReferencedParamWrongType(null, null, null, "write bit", "120"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckDependencyId_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "1000", " 100"),
                    Error.UntrimmedAttribute(null, null, null, "2000", "200 "),
                    Error.UntrimmedAttribute(null, null, null, "2001", " 200 "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckDependencyId();

        [TestMethod]
        public void Param_CheckDependencyId_UntrimmedAttribute()
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
        public void Param_CheckDependencyId_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "100");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "2.54.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty attribute 'Discreets@dependencyId' in Param '100'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckDependencyId_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "aaa", "100");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "2.54.3",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invalid value 'aaa' in attribute 'Discreets@dependencyId'. Param ID '100'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckDependencyId_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "100", "1000");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "2.54.4",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Attribute 'Discreets@dependencyId' references a non-existing 'Param' with ID '100'. Param ID '1000'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckDependencyId_ReferencedParamRTDisplayExpected()
        {
            // Create ErrorMessage
            var message = Error.ReferencedParamRTDisplayExpected(null, null, null, "100", "1000");

            var expected = new ValidationResult
            {
                ErrorId = 6,
                FullId = "2.54.6",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "RTDisplay(true) expected on Param '100' referenced by a 'Discreets@dependencyId' attribute. Param ID '1000'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckDependencyId_ReferencedParamWrongType()
        {
            // Create ErrorMessage
            var message = Error.ReferencedParamWrongType(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 5,
                FullId = "2.54.5",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invalid Param Type '2' on Param referenced by a 'Discreets@dependencyId' attribute. Param ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckDependencyId_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "100", " untrimmed ");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "2.54.2",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed attribute 'Discreets@dependencyId' in Param '100'. Current value ' untrimmed '.",
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
        private readonly IRoot check = new CheckDependencyId();

        [TestMethod]
        public void Param_CheckDependencyId_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckDependencyId_CheckId() => Generic.CheckId(check, CheckId.CheckDependencyId);
    }
}
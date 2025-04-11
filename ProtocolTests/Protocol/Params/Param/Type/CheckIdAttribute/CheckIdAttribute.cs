namespace ProtocolTests.Protocol.Params.Param.Type.CheckIdAttribute
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Type.CheckIdAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckIdAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckIdAttribute_Valid()
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
        public void Param_CheckIdAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "1"),
                    Error.EmptyAttribute(null, null, null, "2"),
                    Error.EmptyAttribute(null, null, null, "3"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "read bit", "11"),
                    Error.MissingAttribute(null, null, null, "write bit", "21"),
                    Error.MissingAttribute(null, null, null, "response", "100"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_NonExistingColumn()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingColumn",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingColumn(null, null, null, "1002", "1000"),
                    Error.NonExistingColumn(null, null, null, "1004", "1000"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_NonExistingParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingParam",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingParam(null, null, null, "10", "11"),
                    Error.NonExistingParam(null, null, null, "20", "21"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_NonExistingResponse()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingResponse",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingResponse(null, null, null, "1", "100"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "11", " 10"),
                    Error.UntrimmedAttribute(null, null, null, "21", "20 "),
                    Error.UntrimmedAttribute(null, null, null, "100", " 1 "),
                    Error.UntrimmedAttribute(null, null, null, "1000", " 1001;1002 "),
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
        public void Param_CheckIdAttribute_UntrimmedAttribute()
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
        public void Param_CheckIdAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "2");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "2.62.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty attribute 'Type@id' in Param '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_NonExistingColumn()
        {
            // Create ErrorMessage
            var message = Error.NonExistingColumn(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 5,
                FullId = "2.62.5",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Attribute 'Type@id' references a non-existing 'Column' with PID '2'. Param ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_NonExistingParam()
        {
            // Create ErrorMessage
            var message = Error.NonExistingParam(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "2.62.3",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Attribute 'Type@id' references a non-existing 'Param' with ID '2'. Param ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_NonExistingResponse()
        {
            // Create ErrorMessage
            var message = Error.NonExistingResponse(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "2.62.4",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Attribute 'Type@id' references a non-existing 'Response' with ID '2'. Param ID '3'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "2.62.2",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed attribute 'Type@id' in Param '2'. Current value '3'.",
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
        private readonly IRoot check = new CheckIdAttribute();

        [TestMethod]
        public void Param_CheckIdAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckIdAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckIdAttribute);
    }
}
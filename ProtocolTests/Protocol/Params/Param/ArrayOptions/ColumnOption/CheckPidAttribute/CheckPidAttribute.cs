namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckPidAttribute
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckPidAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckPidAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckPidAttribute_Valid()
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
        public void Param_CheckPidAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "1100"),
                    Error.EmptyAttribute(null, null, null, "1200"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckPidAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "1100"),
                    Error.MissingAttribute(null, null, null, "1200"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckPidAttribute_NonExistingParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingParam",
                ExpectedResults = new List<IValidationResult>
                {
                    // Syntax 2
                    Error.NonExistingParam(null, null, null, "1101", "1100"),
                    
                    // View Table
                    Error.NonExistingParam(null, null, null, "10001", "10000"),
                    Error.NonExistingParam(null, null, null, "10002", "10000"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckPidAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    // Syntax 2
                    Error.UntrimmedAttribute(null, null, null, "1100", " 1101"),
                    Error.UntrimmedAttribute(null, null, null, "1100", "1102 "),
                    
                    // Syntax 3
                    Error.UntrimmedAttribute(null, null, null, "1200", " 1201 "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckPidAttribute();

        [TestMethod]
        public void Param_CheckPidAttribute_UntrimmedAttribute()
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
        public void Param_CheckPidAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "tablePid");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "2.63.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty attribute 'ColumnOption@pid' in Param 'tablePid'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckPidAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null, "tablePid");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "2.63.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Missing attribute 'ColumnOption@pid' in table 'tablePid'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckPidAttribute_NonExistingParam()
        {
            // Create ErrorMessage
            var message = Error.NonExistingParam(null, null, null, "columnPid", "tablePid");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "2.63.4",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Attribute 'ColumnOption@pid' references a non-existing 'column' with PID 'columnPid'. Table PID 'tablePid'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckPidAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "tablePid", "untrimmedValue");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "2.63.3",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed attribute 'ColumnOption@pid' in Table 'tablePid'. Current value 'untrimmedValue'.",
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
        private readonly IRoot check = new CheckPidAttribute();

        [TestMethod]
        public void Param_CheckPidAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckPidAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckPidAttribute);
    }
}
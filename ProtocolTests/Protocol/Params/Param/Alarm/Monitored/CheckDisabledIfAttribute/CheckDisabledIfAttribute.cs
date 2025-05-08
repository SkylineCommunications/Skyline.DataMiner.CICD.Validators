namespace ProtocolTests.Protocol.Params.Param.Alarm.Monitored.CheckDisabledIfAttribute
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.Monitored.CheckDisabledIfAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckDisabledIfAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckDisabledIfAttribute_Valid()
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
        public void Param_CheckDisabledIfAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckDisabledIfAttribute_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "aaa", "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckDisabledIfAttribute_NonExistingId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingId(null, null, null, "1001", "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Param_CheckDisabledIfAttribute_ReferencedParamRTDisplayExpected()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedParamRTDisplayExpected",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ReferencedParamRTDisplayExpected(null, null, null, "1001", "1"),
                    Error.ReferencedParamRTDisplayExpected(null, null, null, "1002", "2"),
                    Error.ReferencedParamRTDisplayExpected(null, null, null, "1003", "3"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckDisabledIfAttribute_ReferencedParamWrongType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ReferencedParamWrongType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ReferencedParamWrongType(null, null, null, "write", "1001"),
                    Error.ReferencedParamWrongType(null, null, null, "write bit", "1002"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckDisabledIfAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "1", " 1001,value1"),
                    Error.UntrimmedAttribute(null, null, null, "2", "1002,0 "),
                    Error.UntrimmedAttribute(null, null, null, "3", " 1003,0 "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckDisabledIfAttribute();

        [TestMethod]
        public void Param_CheckDisabledIfAttribute_UntrimmedAttribute()
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
        public void Param_CheckDisabledIfAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "pid");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty attribute 'Monitored@disabledIf' in Param 'pid'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckDisabledIfAttribute_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "attributeValue", "pid");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invalid value 'attributeValue' in attribute 'Monitored@disabledIf'. Param ID 'pid'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckDisabledIfAttribute_NonExistingId()
        {
            // Create ErrorMessage
            var message = Error.NonExistingId(null, null, null, "referencedPid", "pid");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Attribute 'Monitored@disabledIf' references a non-existing 'Param' with ID 'referencedPid'. Param ID 'pid'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckDisabledIfAttribute_ReferencedParamRTDisplayExpected()
        {
            // Create ErrorMessage
            var message = Error.ReferencedParamRTDisplayExpected(null, null, null, "referencedPid", "referencingPid");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "RTDisplay(true) expected on Param 'referencedPid' referenced by a 'Monitored@disabledIf' attribute. Param ID 'referencingPid'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckDisabledIfAttribute_ReferencedParamWrongType()
        {
            // Create ErrorMessage
            var message = Error.ReferencedParamWrongType(null, null, null, "referencedParamType", "referencedPid");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invalid Param Type 'referencedParamType' on Param referenced by a 'Monitored@disabledIf' attribute. Param ID 'referencedPid'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckDisabledIfAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "pid", "untrimmedValue");

            var expected = new ValidationResult
            {
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed attribute 'Monitored@disabledIf' in Param 'pid'. Current value 'untrimmedValue'.",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckDisabledIfAttribute();

        [TestMethod]
        public void Param_CheckDisabledIfAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckDisabledIfAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckDisabledIfAttribute);
    }
}
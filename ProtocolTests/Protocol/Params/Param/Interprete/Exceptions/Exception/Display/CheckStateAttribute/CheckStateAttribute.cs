namespace ProtocolTests.Protocol.Params.Param.Interprete.Exceptions.Exception.Display.CheckStateAttribute
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Exceptions.Exception.Display.CheckStateAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckStateAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckStateAttribute_Valid()
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
        public void Param_CheckStateAttribute_UnrecommendedEnabledValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedEnabledValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedEnabledValue(null, null, null, "100"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckStateAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "100"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckStateAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "100"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckStateAttribute_InvalidAttributeValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAttributeValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttributeValue(null, null, null, "true", "100"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckStateAttribute_UntrimmedAttributeValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttributeValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttributeValue(null, null, null, "100", "disabled "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckStateAttribute();

        [TestMethod]
        public void Param_CheckStateAttribute_UnrecommendedEnabledValue()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedEnabledValue",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void Param_CheckStateAttribute_EmptyAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyAttribute",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void Param_CheckStateAttribute_MissingAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingAttribute",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void Param_CheckStateAttribute_InvalidAttributeValue()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "InvalidAttributeValue",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void Param_CheckStateAttribute_UntrimmedAttributeValue()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttributeValue",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckStateAttribute_UnrecommendedEnabledValue()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedEnabledValue(null, null, null, "paramId");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Warning,
                Certainty = Certainty.Uncertain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Exception with state 'enabled'. Param 'paramId'.",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckStateAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "paramId");

            var expected = new ValidationResult
            {
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty attribute 'state' in Param 'paramId'.",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckStateAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null, "paramId");

            var expected = new ValidationResult
            {
                Severity = Severity.Warning,
                Certainty = Certainty.Uncertain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Missing attribute 'state' in Param 'paramId'.",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckStateAttribute_InvalidAttributeValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidAttributeValue(null, null, null, null, "paramId");

            var expected = new ValidationResult
            {
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invalid value '' in attribute 'Display@state'. Param ID 'paramId'.",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckStateAttribute_UntrimmedAttributeValue()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttributeValue(null, null, null, "paramId", null);

            var expected = new ValidationResult
            {
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed attribute 'Display@state' in Param 'paramId'. Current value ''.",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckStateAttribute();

        [TestMethod]
        public void Param_CheckStateAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckStateAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckStateAttribute);
    }
}
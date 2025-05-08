namespace ProtocolTests.Protocol.Params.Param.SNMP.OID.CheckOptionsAttribute
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.SNMP.OID.CheckOptionsAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckOptionsAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckOptionsAttribute_Valid()
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
        public void Param_CheckOptionsAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "1000"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingInstanceOption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingInstanceOption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingInstanceOption(null, null, null, "1000"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "1000", " instance;partialSNMP:3 "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckOptionsAttribute();

        [TestMethod]
        public void Param_CheckOptionsAttribute_EmptyAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyAttribute",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingInstanceOption()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingInstanceOption",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_UntrimmedAttribute()
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
        public void Param_CheckOptionsAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "paramId");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty attribute 'SNMP/OID@options' in Param 'paramId'.",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_MissingInstanceOption()
        {
            // Create ErrorMessage
            var message = Error.MissingInstanceOption(null, null, null, "paramId");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Missing value 'instance' in attribute 'SNMP/OID@options'. Table PID 'paramId'.",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckOptionsAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "paramId", "untrimmedValue");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed attribute 'SNMP/OID@options' in Param 'paramId'. Current value 'untrimmedValue'.",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckOptionsAttribute();

        [TestMethod]
        public void Param_CheckOptionsAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckOptionsAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckOptionsAttribute);
    }
}
namespace ProtocolTests.Protocol.Params.Param.Type.CheckVirtualAttribute
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Type.CheckVirtualAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckVirtualAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckVirtualAttribute_Valid()
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
        public void Param_CheckVirtualAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "900"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Param_CheckVirtualAttribute_RTDisplayExpected()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "100"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckVirtualAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "200", " destination "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckVirtualAttribute();

        [TestMethod]
        public void Param_CheckVirtualAttribute_EmptyAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyAttribute",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void Param_CheckVirtualAttribute_UntrimmedAttribute()
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
        public void Param_CheckVirtualAttribute_EmptyAttribute()
        {
            // Create ErrorMessage
            var message = Error.EmptyAttribute(null, null, null, "2");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "2.58.1",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty attribute 'Type@virtual' in Param '2'.",
                HowToFix = "",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckVirtualAttribute_RTDisplayExpected()
        {
            // Create ErrorMessage
            var message = Error.RTDisplayExpected(null, null, null, "2");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "2.58.3",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "RTDisplay(true) expected on parameters used as virtual source. Param ID '2'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckVirtualAttribute_UntrimmedAttribute()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedAttribute(null, null, null, "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "2.58.2",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed attribute 'Type@virtual' in Param '2'. Current value '3'.",
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
        private readonly IRoot check = new CheckVirtualAttribute();

        [TestMethod]
        public void Param_CheckVirtualAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckVirtualAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckVirtualAttribute);
    }
}
namespace ProtocolTests.Protocol.Params.Param.Interprete.DefaultValue.CheckDefaultValueTag
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.DefaultValue.CheckDefaultValueTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckDefaultValueTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckDefaultValueTag_Valid()
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
        public void Param_CheckDefaultValueTag_NotYetSupportedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NotYetSupportedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    // Table Syntax 1 (not yet supported by the TryGetTable helper)
                    Error.NotYetSupportedTag(null, null, null, "1002"),
                    Error.NotYetSupportedTag(null, null, null, "1003"),
                    
                    // Table Syntax 2
                    Error.NotYetSupportedTag(null, null, null, "1102"),
                    Error.NotYetSupportedTag(null, null, null, "1103"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckDefaultValueTag_UnsupportedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    // Various Param Types
                    Error.UnsupportedTag(null, null, null, "1"),
                    Error.UnsupportedTag(null, null, null, "2"),
                    Error.UnsupportedTag(null, null, null, "3"),
                    Error.UnsupportedTag(null, null, null, "4"),
                    Error.UnsupportedTag(null, null, null, "5"),
                    Error.UnsupportedTag(null, null, null, "6"),
                    Error.UnsupportedTag(null, null, null, "7"),
                    Error.UnsupportedTag(null, null, null, "8"),
                    Error.UnsupportedTag(null, null, null, "9"),
                    Error.UnsupportedTag(null, null, null, "10"),
                    Error.UnsupportedTag(null, null, null, "11"),
                    Error.UnsupportedTag(null, null, null, "12"),
                    Error.UnsupportedTag(null, null, null, "13"),
                    Error.UnsupportedTag(null, null, null, "14"),
                    Error.UnsupportedTag(null, null, null, "15"),
                    Error.UnsupportedTag(null, null, null, "16"),
                    Error.UnsupportedTag(null, null, null, "17"),
                    Error.UnsupportedTag(null, null, null, "18"),

                    // String Write Params
                    Error.UnsupportedTag(null, null, null, "151"),
                    Error.UnsupportedTag(null, null, null, "152"),
                    
                    // Double Write Params
                    Error.UnsupportedTag(null, null, null, "251"),
                    Error.UnsupportedTag(null, null, null, "252"),

                    // Table Params
                    Error.UnsupportedTag(null, null, null, "1000"),
                    Error.UnsupportedTag(null, null, null, "1100"),
                    Error.UnsupportedTag(null, null, null, "1200"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckDefaultValueTag_ValueIncompatibleWithInterpreteType()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ValueIncompatibleWithInterpreteType",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ValueIncompatibleWithInterpreteType(null, null, null, "abc", "double", "100"),
                }
            };

            Generic.Validate(check, data);
        }


        #endregion
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckDefaultValueTag_NotYetSupportedTag()
        {
            // Create ErrorMessage
            var message = Error.NotYetSupportedTag(null, null, null, "columnPid");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "2.68.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unsupported tag 'DefaultValue' in Column 'columnPid'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckDefaultValueTag_UnsupportedTag()
        {
            // Create ErrorMessage
            var message = Error.UnsupportedTag(null, null, null, "paramId");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "2.68.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unsupported tag 'DefaultValue' in Param 'paramId'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckDefaultValueTag();

        [TestMethod]
        public void Param_CheckDefaultValueTag_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckDefaultValueTag_CheckId() => Generic.CheckId(check, CheckId.CheckDefaultValueTag);
    }
}
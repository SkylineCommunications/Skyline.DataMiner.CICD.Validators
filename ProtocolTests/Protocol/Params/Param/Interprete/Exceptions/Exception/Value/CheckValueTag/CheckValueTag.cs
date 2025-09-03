namespace ProtocolTests.Protocol.Params.Param.Interprete.Exceptions.Exception.Value.CheckValueTag
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Exceptions.Exception.Value.CheckValueTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckValueTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckValueTag_Valid()
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
        public void Param_CheckValueTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "1"),

                    Error.MissingTag(null, null, null, "2"),
                    Error.MissingTag(null, null, null, "2"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckValueTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    // TODO: double check if untrimmed value actually work or get trimmed.
                    Error.UntrimmedTag(null, null, null, "1", " abc "),
                    Error.UntrimmedTag(null, null, null, "100", " 123 "),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckValueTag_ValueIncompatibleWithInterpreteType()
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
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckValueTag();

        [TestMethod]
        public void Param_CheckDisplayTag_UntrimmedTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedTag",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckValueTag_MissingTag()
        {
            // Create ErrorMessage
            var message = Error.MissingTag(null, null, null, "paramId");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "2.71.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Missing tag 'Exception/Value' in Param 'paramId'.",
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
        private readonly IRoot check = new CheckValueTag();

        [TestMethod]
        public void Param_CheckValueTag_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckValueTag_CheckId() => Generic.CheckId(check, CheckId.CheckValueTag);
    }
}
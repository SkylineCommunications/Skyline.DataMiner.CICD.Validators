namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.NamingFormat.CheckNamingFormatTag
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.NamingFormat.CheckNamingFormatTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckNamingFormatTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckNamingFormatTag_Valid()
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
        public void Param_CheckNamingFormatTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "100"),    // Empty
                    Error.EmptyTag(null, null, null, "200"),    // Empty CDATA tag
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckNamingFormatTag_MissingDynamicPart()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingDynamicPart",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingDynamicPart(null, null, null, "100"),
                    Error.MissingDynamicPart(null, null, null, "200"),
                    Error.MissingDynamicPart(null, null, null, "300"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckNamingFormatTag_NonExistingParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingParam",
                ExpectedResults = new List<IValidationResult>
                {
                    // Table Syntax 1
                    Error.NonExistingParam(null, null, null, "1009", "1000"),
                    
                    // Table Syntax 1
                    Error.NonExistingParam(null, null, null, "1109", "1100"),
                    
                    // View Table
                    Error.NonExistingParam(null, null, null, "10008", "10000"),
                    Error.NonExistingParam(null, null, null, "10009", "10000"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckNamingFormatTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    // Table Syntax 1
                    Error.UntrimmedTag(null, null, null, "1000", "  ,HardCodedPart1,1002,HardCodedPart2"),
                    
                    // Table Syntax 1
                    Error.UntrimmedTag(null, null, null, "1100", ",1101,:,1102  "),

                    // View Table
                    Error.UntrimmedTag(null, null, null, "10000", ",10001,:,10002 "),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckNamingFormatTag();

        [TestMethod]
        public void Param_CheckNamingFormatTag_UntrimmedTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedTag",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckNamingFormatTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "tablePid");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "2.65.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty tag 'ArrayOptions/NamingFormat' in Table 'tablePid'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckNamingFormatTag_NonExistingParam()
        {
            // Create ErrorMessage
            var message = Error.NonExistingParam(null, null, null, "referencedPid", "tablePid");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "2.65.3",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Tag 'ArrayOptions/NamingFormat' references a non-existing 'Param' with ID 'referencedPid'. Table PID 'tablePid'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckNamingFormatTag_UntrimmedTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedTag(null, null, null, "tablePid", "untrimmedValue");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "2.65.2",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed tag 'ArrayOptions/NamingFormat' in Table 'tablePid'. Current value 'untrimmedValue'.",
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
        private readonly IRoot check = new CheckNamingFormatTag();

        [TestMethod]
        public void Param_CheckNamingFormatTag_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckNamingFormatTag_CheckId() => Generic.CheckId(check, CheckId.CheckNamingFormatTag);
    }
}
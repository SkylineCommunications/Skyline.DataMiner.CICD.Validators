namespace ProtocolTests.Protocol.Params.Param.Display.Positions.CheckPositionsTag
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Positions.CheckPositionsTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckPositionsTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckPositionsTag_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckPositionsTag_Valid_DVE()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_DVE",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckPositionsTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "1"),
                    Error.EmptyTag(null, null, null, "2"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Param_CheckPositionsTag_RTDisplayExpected()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, ""),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckPositionsTag();

        [TestMethod]
        public void Param_CheckPositionsTag_EmptyTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyTag",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckPositionsTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "2");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "2.57.1",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Missing tag 'Display/Positions' in Param '2'.",
                HowToFix = "",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckPositionsTag_RTDisplayExpected()
        {
            // Create ErrorMessage
            var message = Error.RTDisplayExpected(null, null, null, "2");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "2.57.2",
                Category = Category.Param,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "RTDisplay(true) expected on Param '2' which is positioned.",
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
        private readonly IRoot check = new CheckPositionsTag();

        [TestMethod]
        public void Param_CheckPositionsTag_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckPositionsTag_CheckId() => Generic.CheckId(check, CheckId.CheckPositionsTag);
    }
}
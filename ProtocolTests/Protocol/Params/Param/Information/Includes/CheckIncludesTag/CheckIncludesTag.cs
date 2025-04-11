namespace ProtocolTests.Protocol.Params.Param.Information.Includes.CheckIncludesTag
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Information.Includes.CheckIncludesTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckIncludesTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckIncludesTag_Valid()
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
        public void Param_CheckIncludesTag_ObsoleteTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ObsoleteTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ObsoleteTag(null, null, null, "1"),
                    Error.ObsoleteTag(null, null, null, "2"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckIncludesTag();

        [TestMethod]
        public void Param_CheckIncludesTag_ObsoleteTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "ObsoleteTag",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckIncludesTag_ObsoleteTag()
        {
            // Create ErrorMessage
            var message = Error.ObsoleteTag(null, null, null, "pid");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "2.66.1",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Obsolete tag 'Information/Includes'. Param ID 'pid'.",
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
        private readonly IRoot check = new CheckIncludesTag();

        [TestMethod]
        public void Param_CheckIncludesTag_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckIncludesTag_CheckId() => Generic.CheckId(check, CheckId.CheckIncludesTag);
    }
}
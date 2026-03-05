namespace ProtocolTests.Protocol.Params.Param.Alarm.Info.CheckInfoTag
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Alarm.Info.CheckInfoTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckInfoTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckInfoTag_Valid()
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
        public void Param_CheckInfoTag_UnrecommendedInfoTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedInfoTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedInfoTag(null, null, null, "10"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckInfoTag();

        [TestMethod]
        public void Param_CheckInfoTag_UnrecommendedInfoTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedInfoTag",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckInfoTag_UnrecommendedInfoTag()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedInfoTag(null, null, null, "paramId");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unrecommended tag 'Alarm/Info' for Param with ID 'paramId'.",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckInfoTag();

        [TestMethod]
        public void Param_CheckInfoTag_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckInfoTag_CheckId() => Generic.CheckId(check, CheckId.CheckInfoTag);
    }
}
namespace SLDisValidatorUnitTests.Protocol.Params.Param.Alarm.CheckAlarmTag
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;
    using SLDisValidator2.Tests.Protocol.Params.Param.Alarm.CheckAlarmTag;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckAlarmTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckAlarmTag_Valid()
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
        public void Param_CheckAlarmTag_MissingDefaultThreshold()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingDefaultThreshold",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingDefaultThreshold(null, null, null, "101"),
                    Error.MissingDefaultThreshold(null, null, null, "102"),
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
        public void Param_CheckAlarmTag_MissingDefaultThreshold()
        {
            // Create ErrorMessage
            var message = Error.MissingDefaultThreshold(null, null, null, "2");
                        
            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "2.5.1",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "Missing default thresholds on some monitored parameters.",
                Description = "Missing default thresholds on monitored parameter. Param ID '2'.",
                HowToFix = "",
                ExampleCode = "",
                Details = "When possible, default thresholds should be provided on monitored parameter as starting point to make things easier for a user when configuring alarm templates.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckAlarmTag();

        [TestMethod]
        public void Param_CheckAlarmTag_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckAlarmTag_CheckId() => Generic.CheckId(check, CheckId.CheckAlarmTag);
    }
}
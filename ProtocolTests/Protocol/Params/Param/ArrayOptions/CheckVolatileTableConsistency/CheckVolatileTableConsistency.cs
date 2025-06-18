namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.CheckVolatileTableConsistency
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckVolatileTableConsistency;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckVolatileTableConsistency();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckVolatileTableConsistency_Valid()
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
        public void Param_CheckVolatileTableConsistency_InvalidVolatileTableUsage()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidVolatileTableUsage",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidVolatileTableUsage(null, null, null, "1000", "saved column"),
                    Error.InvalidVolatileTableUsage(null, null, null, "1000", "trended column"),
                    Error.InvalidVolatileTableUsage(null, null, null, "1000", "alarmed column"),
                    Error.InvalidVolatileTableUsage(null, null, null, "1000", "DVE customization"),
                    Error.InvalidVolatileTableUsage(null, null, null, "1000", "DCF usage"),
                    Error.InvalidVolatileTableUsage(null, null, null, "1000", "foreign key"),
                    Error.InvalidVolatileTableUsage(null, null, null, "1000", "DVE element option"),
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
        public void Param_CheckVolatileTableConsistency_InvalidVolatileTableUsage()
        {
            // Create ErrorMessage
            var message = Error.InvalidVolatileTableUsage(null, null, null, "tableId", "incompatibleFeature");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.Breaking,
                GroupDescription = "",
                Description = "Table 'tableId' is marked as volatile, but it uses features incompatible with volatility: incompatibleFeature.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckVolatileTableConsistency();

        [TestMethod]
        public void Param_CheckVolatileTableConsistency_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckVolatileTableConsistency_CheckId() => Generic.CheckId(check, CheckId.CheckVolatileTableConsistency);
    }
}
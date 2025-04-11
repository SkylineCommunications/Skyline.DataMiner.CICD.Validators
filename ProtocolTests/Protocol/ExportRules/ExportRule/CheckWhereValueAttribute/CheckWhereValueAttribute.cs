namespace ProtocolTests.Protocol.ExportRules.ExportRule.CheckWhereValueAttribute
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ExportRules.ExportRule.CheckWhereValueAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckWhereValueAttribute();

        #region Valid Checks

        [TestMethod]
        public void ExportRule_CheckWhereValueAttribute_Valid()
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
        public void ExportRule_CheckWhereValueAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null),
                    Error.MissingAttribute(null, null, null),
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
        public void ExportRule_CheckWhereValueAttribute_MissingAttribute()
        {
            // Create ErrorMessage
            var message = Error.MissingAttribute(null, null, null);

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Missing attribute 'ExportRule@whereValue'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckWhereValueAttribute();

        [TestMethod]
        public void ExportRule_CheckWhereValueAttribute_CheckCategory() => Generic.CheckCategory(check, Category.ExportRule);

        [TestMethod]
        public void ExportRule_CheckWhereValueAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckWhereValueAttribute);
    }
}
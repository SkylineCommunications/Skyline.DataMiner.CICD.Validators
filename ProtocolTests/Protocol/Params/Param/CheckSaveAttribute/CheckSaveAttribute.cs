namespace ProtocolTests.Protocol.Params.Param.CheckSaveAttribute
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.CheckSaveAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckSaveAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckSaveAttribute_Valid()
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
        public void Param_CheckSaveAttribute_ValidUnsavedReadParamInResponse()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidUnsavedReadParamInResponse",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckSaveAttribute_UnrecommendedSavedReadParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedSavedReadParam",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedSavedReadParam(null, null, null, "1001"),
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
        public void Param_CheckSaveAttribute_UndesiredSavedReadParam()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedSavedReadParam(null, null, null, "1001");

            var expected = new ValidationResult
            {
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unrecommended use of 'save' attribute. Param ID '1001'.",
                Details = "Having a parameter being both saved and polled from the data-source seems inconsistent. Indeed:" + Environment.NewLine + "- A saved read parameter is typically used for configurations on the DataMiner element side so that user configuration can persist across restarts." + Environment.NewLine + "- A polled parameter will typically never need to be saved as we rely on the fact that the newer value will be polled again shortly after an element restart, no matter if such a parameter is a data parameter or a configuration parameter on the data-source side.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckSaveAttribute();

        [TestMethod]
        public void Param_CheckSaveAttribute_CheckCategory() => Generic.CheckCategory(check, Category.Param);

        [TestMethod]
        public void Param_CheckSaveAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckSaveAttribute);
    }
}
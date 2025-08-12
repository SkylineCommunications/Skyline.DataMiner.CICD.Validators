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
        public void Param_CheckSaveAttribute_UndesiredSavedReadParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedSavedReadParamInResponse",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UndesiredSavedReadParam(null, null, null, "1001"),
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
            var message = Error.UndesiredSavedReadParam(null, null, null, "1001");

            var expected = new ValidationResult
            {
                ErrorId = ErrorIds.UndesiredSavedReadParam,
                FullId = "2.77.1",
                Category = Category.Param,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unrecommended use of 'save' attribute. Param ID '1001'.",
                HowToFix = "Depending on the use-case, you should either remove the save option or include another parameter within your response." + Environment.NewLine + "-Is your parameter corresponding to a user configuration on the DataMiner element side -> don't use it in your response." + Environment.NewLine + "- Is your parameter corresponding to data or configuration retrieved from your data-source -> no need to save it." + Environment.NewLine + "" + Environment.NewLine + "There might be some use-cases where you need to use a dataminer element configuration parameter as a filter/validation on whether a data-source response should be accepted or not. In such cases,  you will indeed want to save such configuration parameter but then, you should copy its value to a fixed (and non-saved) parameter and include it within your response so that the user configuration does not get overwritten by the data-source response.",
                ExampleCode = "",
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
namespace ProtocolTests.Protocol.QActions.QAction.CheckDllImportAttribute
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckDllImportAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckDllImportAttribute();

        #region Valid Checks

        [TestMethod]
        public void QAction_CheckDllImportAttribute_Valid()
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
        public void QAction_CheckDllImportAttribute_ValidNoDllImportTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidNoDllImportTag",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CheckDllImportAttribute_ValidNoQAction()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidNoQAction",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void QAction_CheckDllImportAttribute_DeprecatedDll()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DeprecatedDll",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DeprecatedDll(null, null, null, "SLDatabase.dll", "10"),
                    Error.DeprecatedDll(test: null, null, null, "MySql.Data.dll", "10"),
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
        public void QAction_CheckDllImportAttribute_DeprecatedDll()
        {
            // Create ErrorMessage
            string dllImportValue = "SLDatabase.dll";
            string qactionId = "1";
            var message = Error.DeprecatedDll(null, null, null, dllImportValue, qactionId);
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = $"Deprecated DLL '{dllImportValue}' in attribute 'dllImport'. QAction '{qactionId}'.",
                Details = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckDllImportAttribute();

        [TestMethod]
        public void QAction_CheckDllImportAttribute_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CheckDllImportAttribute_CheckId() => Generic.CheckId(check, CheckId.CheckDllImportAttribute);
    }
}
namespace ProtocolTests.Protocol.QActions.QAction.CheckDeprecatedDllReferences
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckDeprecatedDllReferences;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckDeprecatedDllReferences();

        #region Valid Checks

        [TestMethod]
        public void QAction_CheckDeprecatedDllReferences_XML_Valid()
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
        public void QAction_CheckDeprecatedDllReferences_XML_Valid_NoDllImport()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidNoDllImportAttribute",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CheckDeprecatedDllReferences_XML_Valid_NoQAction()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidNoQAction",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        // The valid solution contains a NuGet reference to MySql.Data, which should be allowed, only direct usage of the MySql.Data.dll should be flagged.
        [TestMethod]
        public void QAction_CheckDeprecatedDllReferences_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                IsSolution = true,
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void QAction_CheckDeprecatedDllReferences_DeprecatedDll_XmlBased()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DeprecatedDll",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DeprecatedDll(null, null, null, "SLDatabase.dll", "10"),
                    Error.DeprecatedDll(null, null, null, "MySql.Data.dll", "10"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CheckDeprecatedDllReferences_DeprecatedDllReferences()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DeprecatedDllReferences",
                IsSolution = true,
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DeprecatedDll(null, null, null, "SLDatabase.dll", "1"),
                    Error.DeprecatedDll(null, null, null, "SLDatabase.dll", "2"),
                    Error.DeprecatedDll(null, null, null, "MySql.Data.dll", "2"),
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
        public void QAction_CheckDeprecatedDllReferences_DeprecatedDll()
        {
            string packageName = "test.dll";
            string qactionId = "1";

            // Create ErrorMessage
            var message = Error.DeprecatedDll(null, null, null, packageName, qactionId);
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = $"Deprecated DLL '{packageName}' referenced. QAction '{qactionId}'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckDeprecatedDllReferences();

        [TestMethod]
        public void QAction_CheckDeprecatedDllReferences_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CheckDeprecatedDllReferences_CheckId() => Generic.CheckId(check, CheckId.CheckDeprecatedDllReferences);
    }
}
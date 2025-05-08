namespace ProtocolTests.Protocol.QActions.QAction.CheckDataMinerDependency
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckDataMinerDependency;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckDataMinerDependency();

        #region Valid Checks

        [TestMethod]
        [Ignore("Check can only be done on a solution")]
        public void QAction_CheckDataMinerDependency_Valid_XmlBased()
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
        public void QAction_CheckDataMinerDependency_Valid()
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
        [Ignore("Check can only be done on a solution")]
        public void QAction_CheckDataMinerDependency_MismatchDevPack_Xml()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MismatchDevPack",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CheckDataMinerDependency_MismatchDevPack()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MismatchDevPack",
                IsSolution = true,
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MismatchDevPack(null, null, null, "Skyline.DataMiner.Dev.Protocol", "10.4.7", "10.2.0.0 - 11517", "1"),

                    Error.MismatchDevPack(null, null, null, "Skyline.DataMiner.Dev.Protocol", "10.4.7", "10.2.0.0 - 11517", "2"),
                    Error.MismatchDevPack(null, null, null, "Skyline.DataMiner.Files.SLMediationSnippets", "10.4.7", "10.2.0.0 - 11517", "2"),
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
        public void QAction_CheckDataMinerDependency_MismatchDevPack()
        {
            // Create ErrorMessage
            var message = Error.MismatchDevPack(null, null, null, "packageName", "packageVersion", "minimumRequiredVersion", "qactionId");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Package 'packageName' version 'packageVersion' has a higher version than the version specified in the MinimumRequiredVersion tag 'minimumRequiredVersion'. QAction ID 'qactionId'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckDataMinerDependency();

        [TestMethod]
        public void QAction_CheckDataMinerDependency_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CheckDataMinerDependency_CheckId() => Generic.CheckId(check, CheckId.CheckDataMinerDependency);
    }
}
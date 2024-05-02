namespace ProtocolTests.Protocol.QActions.CheckAssemblies
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.CheckAssemblies;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckAssemblies();

        #region Valid Checks

        [TestMethod]
        public void QAction_CheckAssemblies_Valid_XmlBased()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>(),
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CheckAssemblies_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                IsSolution = true,
                ExpectedResults = new List<IValidationResult>(),
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        [Ignore("Isn't really relevant and causes other checks to fail")]
        public void QAction_CheckAssemblies_UnconsolidatedPackageReference_XmlBased()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnconsolidatedPackageReference",
                ExpectedResults = new List<IValidationResult>(),
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CheckAssemblies_UnconsolidatedPackageReference()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnconsolidatedPackageReference",
                IsSolution = true,
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnconsolidatedPackageReference(null, null, null, "Skyline.DataMiner.Core.DataMinerSystem.Protocol").WithSubResults(
                        Error.UnconsolidatedPackageReference_Sub(null, null, null, "1", "Skyline.DataMiner.Core.DataMinerSystem.Protocol", "1.1.1.8"),
                        Error.UnconsolidatedPackageReference_Sub(null, null, null, "2", "Skyline.DataMiner.Core.DataMinerSystem.Protocol", "1.1.1.9")),
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
        public void QAction_CheckAssemblies_UnconsolidatedPackageReference()
        {
            // Create ErrorMessage
            var message = Error.UnconsolidatedPackageReference(null, null, null, "packageId");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Package 'packageId' has multiple versions across different QActions.",
                Details = "When 2 or more QActions are using a different version of a NuGet, you can have MissingMethodExceptions." + Environment.NewLine + "This is why you need to update NuGet packages across a solution.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CheckAssemblies_UnconsolidatedPackageReference_Sub()
        {
            // Create ErrorMessage
            var message = Error.UnconsolidatedPackageReference_Sub(null, null, null, "qactionId", "packageId", "packageVersion");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "QAction 'qactionId' has package 'packageId' with version 'packageVersion'.",
                Details = "When 2 or more QActions are using a different version of a NuGet, you can have MissingMethodExceptions." + Environment.NewLine + "This is why you need to update NuGet packages across a solution.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckAssemblies();

        [TestMethod]
        public void QAction_CheckAssemblies_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CheckAssemblies_CheckId() => Generic.CheckId(check, CheckId.CheckAssemblies);
    }
}
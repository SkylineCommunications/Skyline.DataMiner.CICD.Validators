namespace ProtocolTests.Protocol.QActions.QAction.CSharpCheckUnrecommendedPropertySet
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpCheckUnrecommendedPropertySet;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpCheckUnrecommendedPropertySet();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedPropertySet_Valid()
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
        public void QAction_CSharpCheckUnrecommendedPropertySet_UnrecommendedCultureInfoDefaultThreadCurrentCulture()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedCultureInfoDefaultThreadCurrentCulture",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedCultureInfoDefaultThreadCurrentCulture(null, null, null, "10"),
                    Error.UnrecommendedCultureInfoDefaultThreadCurrentCulture(null, null, null, "20"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedPropertySet_UnrecommendedThreadCurrentThreadCurrentCulture()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedThreadCurrentThreadCurrentCulture",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedThreadCurrentThreadCurrentCulture(null, null, null, "10"),
                    Error.UnrecommendedThreadCurrentThreadCurrentCulture(null, null, null, "20"),
                    Error.UnrecommendedThreadCurrentThreadCurrentCulture(null, null, null, "30"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedPropertySet_UnrecommendedThreadCurrentThreadCurrentUICulture()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedThreadCurrentThreadCurrentUICulture",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedThreadCurrentThreadCurrentUICulture(null, null, null, "10"),
                    Error.UnrecommendedThreadCurrentThreadCurrentUICulture(null, null, null, "20"),
                    Error.UnrecommendedThreadCurrentThreadCurrentUICulture(null, null, null, "30"),
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
        public void QAction_CSharpCheckUnrecommendedPropertySet_UnrecommendedCultureInfoDefaultThreadCurrentCulture()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedCultureInfoDefaultThreadCurrentCulture(null, null, null, "qactionId");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Setting property 'CultureInfo.DefaultThreadCurrentCulture' is unrecommended. QAction ID 'qactionId'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedPropertySet_UnrecommendedThreadCurrentThreadCurrentCulture()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedThreadCurrentThreadCurrentCulture(null, null, null, "qactionId");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Setting property 'Thread.CurrentThread.CurrentCulture' is unrecommended. QAction ID 'qactionId'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedPropertySet_UnrecommendedThreadCurrentThreadCurrentUICulture()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedThreadCurrentThreadCurrentUICulture(null, null, null, "qactionId");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Setting property 'Thread.CurrentThread.CurrentUICulture' is unrecommended. QAction ID 'qactionId'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CSharpCheckUnrecommendedPropertySet();

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedPropertySet_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedPropertySet_CheckId() => Generic.CheckId(check, CheckId.CSharpCheckUnrecommendedPropertySet);
    }
}
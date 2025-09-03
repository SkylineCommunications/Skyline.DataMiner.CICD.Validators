namespace ProtocolTests.Protocol.QActions.QAction.CSharpCheckUnrecommendedFinalizer
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpCheckUnrecommendedFinalizer;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpCheckUnrecommendedFinalizer();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedFinalizer_Valid()
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
        public void QAction_CSharpCheckUnrecommendedFinalizer_UnrecommendedFinalizer()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedFinalizer",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedFinalizer(null, null, null, "QAction", "1"),
                    Error.UnrecommendedFinalizer(null, null, null, "RandomClass", "2"),
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
        public void QAction_CSharpCheckUnrecommendedFinalizer_UnrecommendedFinalizer()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedFinalizer(null, null, null, "finalizerName", "qactionId");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Finalizer 'finalizerName' is unrecommended. QAction ID 'qactionId'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CSharpCheckUnrecommendedFinalizer();

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedFinalizer_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedFinalizer_CheckId() => Generic.CheckId(check, CheckId.CSharpCheckUnrecommendedFinalizer);
    }
}

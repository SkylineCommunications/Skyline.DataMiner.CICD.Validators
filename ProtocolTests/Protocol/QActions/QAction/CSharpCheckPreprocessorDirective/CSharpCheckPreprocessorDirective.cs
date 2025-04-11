namespace ProtocolTests.Protocol.QActions.QAction.CSharpCheckPreprocessorDirective
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpCheckPreprocessorDirective;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpCheckPreprocessorDirective();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpCheckPreprocessorDirective_Valid()
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
        public void QAction_CSharpCheckPreprocessorDirective_ObsoleteDcfV1()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ObsoleteDcfV1",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ObsoleteDcfV1(null, null, null, "1"),
                    Error.ObsoleteDcfV1(null, null, null, "1"),
                    Error.ObsoleteDcfV1(null, null, null, "1"),

                    Error.ObsoleteDcfV1(null, null, null, "100"),   // Uncommented
                    Error.ObsoleteDcfV1(null, null, null, "101"),   // Simple Comment

                    Error.ObsoleteDcfV1(null, null, null, "102"),   // MultiLine Comment
                    Error.ObsoleteDcfV1(null, null, null, "102"),   // MultiLine Comment
                    Error.ObsoleteDcfV1(null, null, null, "102"),   // MultiLine Comment
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
        public void QAction_CSharpCheckPreprocessorDirective_ObsoleteDcfV1()
        {
            // Create ErrorMessage
            var message = Error.ObsoleteDcfV1(null, null, null, "1");

            var expected = new ValidationResult()
            {
                ErrorId = 1,
                FullId = "3.13.1",
                Category = Category.QAction,
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                Description = "Obsolete preprocessor directive 'DCFv1' used in QAction. QAction ID '1'.",
                HowToFix = "",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CSharpCheckPreprocessorDirective();

        [TestMethod]
        public void QAction_CSharpCheckPreprocessorDirective_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpCheckPreprocessorDirective_CheckId() => Generic.CheckId(check, CheckId.CSharpCheckPreprocessorDirective);
    }
}
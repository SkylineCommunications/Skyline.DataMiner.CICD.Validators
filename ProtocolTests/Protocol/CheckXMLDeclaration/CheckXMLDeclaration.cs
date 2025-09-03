namespace ProtocolTests.Protocol.CheckXMLDeclaration
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckXMLDeclaration;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckXMLDeclaration();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckXMLDeclaration_ValidUtf8Declaration()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidUtf8Declaration.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckXMLDeclaration_ValidNoDeclaration()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidNoDeclaration.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckXMLDeclaration_ValidNoDeclaration2()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Protocol_CheckXMLDeclaration_ValidDeclarationWithoutEncoding()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidDeclarationWithoutEncoding.xml",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Protocol_CheckXMLDeclaration_InvalidDeclaration()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidDeclaration",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidDeclaration(null, null, null, "iso-8859-1", "UTF-8"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckXMLDeclaration();

        [TestMethod]
        public void Protocol_CheckXMLDeclaration_InvalidDeclaration()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "InvalidDeclaration",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Protocol_CheckXMLDeclaration_InvalidDeclaration()
        {
            // Create ErrorMessage
            var message = Error.InvalidDeclaration(null, null, null, "1", "2");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "1.18.1",
                Category = Category.Protocol,
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Invalid XML encoding '1'. Possible values '2'.",
                HowToFix = "Remove the XML declaration if not set to UTF-8.",
                HasCodeFix = true
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckXMLDeclaration();

        [TestMethod]
        public void Protocol_CheckXMLDeclaration_CheckCategory() => Generic.CheckCategory(root, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckXMLDeclaration_CheckId() => Generic.CheckId(root, CheckId.CheckXMLDeclaration);
    }
}
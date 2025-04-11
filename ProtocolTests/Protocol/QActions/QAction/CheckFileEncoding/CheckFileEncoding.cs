namespace ProtocolTests.Protocol.QActions.QAction.CheckFileEncoding
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckFileEncoding;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckFileEncoding();

        #region Valid Checks

        [TestMethod]
        [Ignore("Isn't really relevant and causes other checks to fail")]
        public void QAction_CheckFileEncoding_Valid_Xml()
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
        public void QAction_CheckFileEncoding_Valid()
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
        [Ignore("Isn't really relevant and causes other checks to fail")]
        public void QAction_CheckFileEncoding_InvalidFileEncoding_Xml()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidFileEncoding",
                ExpectedResults = new List<IValidationResult>
                {
                    //Error.InvalidFileEncoding(null, null, null, "invalidFileEncoding", "qactionId"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void QAction_CheckFileEncoding_InvalidFileEncoding()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidFileEncoding",
                IsSolution = true,
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidFileEncoding(null, null, null, "Unicode", "QAction_1.cs", "1"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckFileEncoding();

        [TestMethod]
        [Ignore("Unable to test as actual files are being modified, breaking the test")]
        public void QAction_CheckFileEncoding_InvalidFileEncoding()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "InvalidFileEncoding",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void QAction_CheckFileEncoding_InvalidFileEncoding()
        {
            // Create ErrorMessage
            var message = Error.InvalidFileEncoding(null, null, null, "Unicode", "QAction_1.cs", "1");
                        
            var expected = new ValidationResult
            {
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invalid file encoding 'Unicode' detected in file 'QAction_1.cs'. QAction ID '1'.",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckFileEncoding();

        [TestMethod]
        public void QAction_CheckFileEncoding_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CheckFileEncoding_CheckId() => Generic.CheckId(check, CheckId.CheckFileEncoding);
    }
}
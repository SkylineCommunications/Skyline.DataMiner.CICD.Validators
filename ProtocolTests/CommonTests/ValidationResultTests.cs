namespace ProtocolTests.CommonTests
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;

    [TestClass]
    public class ValidationResultTests
    {
        [TestMethod]
        public void ValidationResult_BubbleUp()
        {
            // Arrange
            var subResultCritical = new ValidationResult { Severity = Severity.Critical };
            var subResultCritical2 = new ValidationResult { Severity = Severity.Critical };
            var subResultMajor = new ValidationResult { Severity = Severity.Major };

            var result = new ValidationResult
            {
                Severity = Severity.BubbleUp,
                SubResults = new List<IValidationResult>
                {
                    subResultMajor,
                    subResultCritical,
                    subResultCritical2
                }
            };

            // Act
            var bubbleUpResult = (ISeverityBubbleUpResult)result;
            bubbleUpResult.DoSeverityBubbleUp();

            // Assert
            Assert.AreEqual(Severity.Critical, result.Severity);
        }

        [TestMethod]
        public void ValidationResult_BubbleUp_NoHigherSub()
        {
            // Arrange
            var subResultWarning = new ValidationResult { Severity = Severity.Warning };
            var subResultMinor = new ValidationResult { Severity = Severity.Minor };

            var result = new ValidationResult
            {
                Severity = Severity.Major,
                SubResults = new List<IValidationResult>
                {
                    subResultWarning,
                    subResultMinor
                }
            };

            // Act
            var bubbleUpResult = (ISeverityBubbleUpResult)result;
            bubbleUpResult.DoSeverityBubbleUp();

            // Assert
            Assert.AreEqual(Severity.Major, result.Severity);
        }

        [TestMethod]
        public void ValidationResult_BubbleUp_HigherSub()
        {
            // Arrange
            var subResultCritical = new ValidationResult { Severity = Severity.Critical };
            var subResultMinor = new ValidationResult { Severity = Severity.Minor };

            var result = new ValidationResult
            {
                Severity = Severity.Major,
                SubResults = new List<IValidationResult>
                {
                    subResultCritical,
                    subResultMinor
                }
            };

            // Act
            var bubbleUpResult = (ISeverityBubbleUpResult)result;
            bubbleUpResult.DoSeverityBubbleUp();

            // Assert
            Assert.AreEqual(Severity.Critical, result.Severity);
        }
    }
}
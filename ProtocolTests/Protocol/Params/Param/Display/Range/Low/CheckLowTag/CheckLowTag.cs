namespace ProtocolTests.Protocol.Params.Param.Display.Range.Low.CheckLowTag
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Range.Low.CheckLowTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckLowTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckLowTag_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckLowTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "1"),
                    Error.EmptyTag(null, null, null, "2"),
                    Error.EmptyTag(null, null, null, "3"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckLowTag_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "abc", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckLowTag_LogarithmicLowerOrEqualToZero()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "LogarithmicLowerOrEqualToZero",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.LogarithmicLowerOrEqualToZero(null, null, null, "0", "1"),
                    Error.LogarithmicLowerOrEqualToZero(null, null, null, "-1", "2"),
                    Error.LogarithmicLowerOrEqualToZero(null, null, null, "-10.5", "3"),
                    Error.LogarithmicLowerOrEqualToZero(null, null, null, "", "4"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckLowTag_UntrimmedValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedValue(null, null, null, "1", "   18"),
                    Error.UntrimmedValue(null, null, null, "2", "18 "),
                    Error.UntrimmedValue(null, null, null, "3", " 18 "),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckLowTag_WriteDifferentThanRead()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "WriteDifferentThanRead",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.WriteDifferentThanRead(null, null, null, "9", "8", "2"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckLowTag();

        [TestMethod]
        public void Param_CheckLowTag_UntrimmedValue()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedValue",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckLowTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckLowTag_Valid()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckLowTag_UpdatedLowRange()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedLowRange",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedLowRange(null, null, "60", "1", "80"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckLowTag_AddedLowRange()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "AddedLowRange",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.AddedLowRange(null, null, "10", "1"),
                    ErrorCompare.AddedLowRange(null, null, "10", "2"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckLowTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "2");

            var expected = new ValidationResult
            {
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty tag 'Range/Low' in Param '2'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckLowTag_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "1", "2");

            var expected = new ValidationResult
            {
                Severity = Severity.Minor,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Invalid value '1' in tag 'Range/Low'. Param ID '2'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckLowTag_UntrimmedValue()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedValue(null, null, null, "1", "2");

            var expected = new ValidationResult
            {
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Untrimmed tag 'Range/Low' in Param '1'. Current value '2'.",
                HasCodeFix = true,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckLowTag_LogarithmicLowerOrEqualToZero()
        {
            // Create ErrorMessage
            var message = Error.LogarithmicLowerOrEqualToZero(null, null, null, "rangeLow", "paramId");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Range/Low 'rangeLow' should be bigger than zero due to Trending@logarithmic 'true'. Param ID 'paramId'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckLowTag();

        [TestMethod]
        public void Param_CheckLowTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckLowTag_CheckId() => Generic.CheckId(root, CheckId.CheckLowTag);
    }
}
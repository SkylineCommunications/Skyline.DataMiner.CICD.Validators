namespace ProtocolTests.Protocol.Compliancies.MinimumRequiredVersion.CheckMinimumRequiredVersionTag
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
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Compliancies.MinimumRequiredVersion.CheckMinimumRequiredVersionTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckMinimumRequiredVersionTag();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_Valid()
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
        public void Protocol_CheckMinimumRequiredVersionTag_ValidNoTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_NoTag",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_MinVersionTooLow()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MinVersionTooLow",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MinVersionTooLow(null, null, null, "10.0.0.0", "10.1.3.0 - 9963").WithSubResults
                    (
                        Error.MinVersionTooLow_Sub(null, null, null, "10.1.3.0 - 9963", "Chain Grouping Name").WithSubResults
                        (
                            Error.MinVersionFeatureUsedInItemWithId_Sub(null, null, null, "Chain", "Name", "MyChain")
                        ),
                        Error.MinVersionTooLow_Sub(null, null, null, "10.1.3.0 - 9963", "Chain Default Selection Field").WithSubResults
                        (
                            Error.MinVersionFeatureUsedInItem_Sub(null, null, null, "Chain")
                        )
                    )
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, " 10.0.0.0 - 4685 ")
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix check = new CheckMinimumRequiredVersionTag();

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_MinVersionTooLow()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MinVersionTooLow",
            };

            Generic.Fix(check, data);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_UntrimmedTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedTag",
            };

            Generic.Fix(check, data);
        }
    }

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckMinimumRequiredVersionTag();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_Valid()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_ValidDecrease1()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "ValidDecrease1",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_ValidDecrease2()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "ValidDecrease2",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_ValidDecrease3()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "ValidDecrease3",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_ValidDecrease4()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "ValidDecrease4",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_ValidDecrease5()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "ValidDecrease5",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_ValidIncrease()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "ValidIncrease",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_ValidRemoval()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "ValidRemoval",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(compare, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_MinVersionIncreased()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "MinVersionIncreased",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.MinVersionIncreased(null, null, "101.102.103.104 - 10005", "199.102.103.104 - 10005"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_MinVersionIncreased2()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "MinVersionIncreased2",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.MinVersionIncreased(null, null, "101.102.103.104 - 10005", "101.199.103.104 - 10005"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_MinVersionIncreased3()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "MinVersionIncreased3",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.MinVersionIncreased(null, null, "101.102.103.104 - 10005", "101.102.199.104 - 10005"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_MinVersionIncreased4()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "MinVersionIncreased4",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.MinVersionIncreased(null, null, "101.102.103.104 - 10005", "101.102.103.199 - 10005"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_MinVersionIncreased5()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "MinVersionIncreased5",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.MinVersionIncreased(null, null, "101.102.103.104 - 10005", "101.102.103.104 - 10099"),
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
        public void Protocol_CheckMinimumRequiredVersionTag_MinVersionFeatureUsedInItem_Sub()
        {
            // Create ErrorMessage
            var message = Error.MinVersionFeatureUsedInItem_Sub(null, null, null, "Protocol");

            var expected = new ValidationResult
            {
                ErrorId = 4,
                FullId = "1.25.4",
                Category = Category.Protocol,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = String.Empty,
                Description = "Feature used in 'Protocol'.",
                HowToFix = String.Empty,
                ExampleCode = String.Empty,
                Details = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_MinVersionFeatureUsedInItemWithId_Sub()
        {
            // Create ErrorMessage
            var message = Error.MinVersionFeatureUsedInItemWithId_Sub(null, null, null, "Param", "ID", "3");

            var expected = new ValidationResult
            {
                ErrorId = 3,
                FullId = "1.25.3",
                Category = Category.Protocol,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = String.Empty,
                Description = "Feature used in 'Param' with 'ID' '3'.",
                HowToFix = String.Empty,
                ExampleCode = String.Empty,
                Details = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_MinVersionTooLow()
        {
            // Create ErrorMessage
            var message = Error.MinVersionTooLow(null, null, null, "9.5.0.0 - 1234", "10.0.0.0 - 1234");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "1.25.1",
                Category = Category.Protocol,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = String.Empty,
                Description = "Minimum required version '9.5.0.0 - 1234' too low. Expected value '10.0.0.0 - 1234'.",
                HowToFix = String.Empty,
                ExampleCode = String.Empty,
                Details =
                    $"Indicates the minimum DataMiner ver­sion that the driver is compatible with. {Environment.NewLine}" +
                    "If the DMS software version is less recent than the indicated version, the protocol will not be useable.",
                HasCodeFix = true
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_MinVersionTooLow_Sub()
        {
            // Create ErrorMessage
            var message = Error.MinVersionTooLow_Sub(null, null, null, "10.0.0.0 - 1234", "Test Feature");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "1.25.2",
                Category = Category.Protocol,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = String.Empty,
                Description = "'10.0.0.0 - 1234' : 'Test Feature'",
                HowToFix = String.Empty,
                ExampleCode = String.Empty,
                Details = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_UntrimmedTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedTag(null, null, null, " 10.0.0.0 - 1234 ");

            var expected = new ValidationResult
            {
                ErrorId = 5,
                FullId = "1.25.5",
                Category = Category.Protocol,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = String.Empty,
                Description = "Untrimmed tag 'MinimumRequiredVersion'. Current value ' 10.0.0.0 - 1234 '.",
                HowToFix = String.Empty,
                ExampleCode = String.Empty,
                Details = String.Empty,
                HasCodeFix = true
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckMinimumRequiredVersionTag();

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_CheckCategory() => Generic.CheckCategory(check, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckMinimumRequiredVersionTag_CheckId() => Generic.CheckId(check, CheckId.CheckMinimumRequiredVersionTag);
    }
}
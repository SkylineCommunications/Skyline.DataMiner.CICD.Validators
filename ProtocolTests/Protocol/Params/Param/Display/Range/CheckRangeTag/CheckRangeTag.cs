namespace ProtocolTests.Protocol.Params.Param.Display.Range.CheckRangeTag
{
    using System.Collections.Generic;

    using FluentAssertions;
    using FluentAssertions.Equivalency;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Range.CheckRangeTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckRangeTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckRangeTag_Valid()
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
        public void Param_CheckRangeTag_EmptyTag()
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
        public void Param_CheckRangeTag_LowShouldBeSmallerThanHigh()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "LowShouldBeSmallerThanHigh",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.LowShouldBeSmallerThanHigh(null, null, null, rangeLow: "1", rangeHigh: "1", paramId: "100"),
                    Error.LowShouldBeSmallerThanHigh(null, null, null, rangeLow: "1", rangeHigh: "0", paramId: "101"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckRangeTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "number", "2"),
                    Error.MissingTag(null, null, null, "analog", "4"),
                    Error.MissingTag(null, null, null, "progress", "5"),
                    Error.MissingTag(null, null, null, "number", "102"),
                    Error.MissingTag(null, null, null, "analog", "104"),
                    Error.MissingTag(null, null, null, "progress", "105"),
                    Error.MissingTag(null, null, null, "number", "202"),
                    Error.MissingTag(null, null, null, "number", "252"),
                    Error.MissingTag(null, null, null, "analog", "204"),
                    Error.MissingTag(null, null, null, "analog", "254"),
                    Error.MissingTag(null, null, null, "progress", "205"),
                    Error.MissingTag(null, null, null, "progress", "255"),
                    Error.MissingTag(null, null, null, "number", "1102"),
                    Error.MissingTag(null, null, null, "number", "1152"),
                    Error.MissingTag(null, null, null, "analog", "1104"),
                    Error.MissingTag(null, null, null, "analog", "1154"),
                    Error.MissingTag(null, null, null, "progress", "1105"),
                    Error.MissingTag(null, null, null, "progress", "1155"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckRangeTag_MissingTagTable()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTagTable",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "number", "1002"),
                    Error.MissingTag(null, null, null, "analog", "1004"),
                    Error.MissingTag(null, null, null, "progress", "1005"),
                    Error.MissingTag(null, null, null, "number", "1010"),
                    Error.MissingTag(null, null, null, "number", "1011"),
                    Error.MissingTag(null, null, null, "number", "1012"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckRangeTag_UnsupportedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnsupportedTag(null, null, null, "discreet", "3"),
                    Error.UnsupportedTag(null, null, null, "button", "6"),
                    Error.UnsupportedTag(null, null, null, "chart", "7"),
                    Error.UnsupportedTag(null, null, null, "digital threshold", "8"),
                    Error.UnsupportedTag(null, null, null, "togglebutton", "9"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckRangeTag_UnsupportedTagTable()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedTagTable",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnsupportedTag(null, null, null, "table", "1000"),
                    Error.UnsupportedTag(null, null, null, "primary key", "1001"),
                    Error.UnsupportedTag(null, null, null, "discreet", "1003"),
                    Error.UnsupportedTag(null, null, null, "button", "1006"),
                    Error.UnsupportedTag(null, null, null, "chart", "1007"),
                    Error.UnsupportedTag(null, null, null, "digital threshold", "1008"),
                    Error.UnsupportedTag(null, null, null, "togglebutton", "1009"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckRangeTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "2");

            var expected = new ValidationResult
            {
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Empty tag 'Display/Range' in Param '2'.",
                HowToFix = "Either add 'Range/Low' and/or 'Range/High' tag(s), either remove the empty 'Display/Range' tag.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, ExcludePropertiesForErrorMessagesIncludingExtra);

            EquivalencyAssertionOptions<ValidationResult> ExcludePropertiesForErrorMessagesIncludingExtra(EquivalencyAssertionOptions<ValidationResult> options)
            {
                Generic.ExcludePropertiesForErrorMessages(options);

                options.Including(x => x.HowToFix);

                return options;
            }
        }

        [TestMethod]
        public void Param_CheckRangeTag_LowShouldBeSmallerThanHigh()
        {
            // Create ErrorMessage
            var message = Error.LowShouldBeSmallerThanHigh(null, null, null, "10", "1", "2");

            var expected = new ValidationResult
            {
                Severity = Severity.Major,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Range/Low '10' should be smaller than Range/High '1'. Param ID '2'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckRangeTag_MissingTag()
        {
            // Create ErrorMessage
            var message = Error.MissingTag(null, null, null, "number", "2");

            var expected = new ValidationResult
            {
                Severity = Severity.Minor,
                Certainty = Certainty.Uncertain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "Missing tag 'Display/Range' in some parameters.",
                Description = "Missing 'Display/Range' tag for 'number' Param with ID '2'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Param_CheckRangeTag_UnsupportedTag()
        {
            // Create ErrorMessage
            var message = Error.UnsupportedTag(null, null, null, "button", "2");

            var expected = new ValidationResult
            {
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Unsupported 'Display/Range' tag for 'button' Param with ID '2'.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckRangeTag();

        [TestMethod]
        public void Param_CheckRangeTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckRangeTag_CheckId() => Generic.CheckId(root, CheckId.CheckRangeTag);
    }
}
namespace ProtocolTests.Protocol.Params.Param.Name.CheckNameTag
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Name.CheckNameTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckNameTag();

        #region Valid

        [TestMethod]
        public void Param_CheckNameTag_Valid()
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

        #region Invalid

        [TestMethod]
        public void Param_CheckNameTag_DuplicatedValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicatedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicatedValue(null, null, null, "Duplicate_2Read", "100, 101").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Duplicate_2Read", "100"),
                        Error.DuplicatedValue(null, null, null, "Duplicate_2Read", "101")),

                    Error.DuplicatedValue(null, null, null, "Duplicate_2Write", "200, 201").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Duplicate_2Write", "200"),
                        Error.DuplicatedValue(null, null, null, "Duplicate_2Write", "201")),

                    Error.DuplicatedValue(null, null, null, "Duplicate_2Read_1Write", "300, 350, 301").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Duplicate_2Read_1Write", "300"),
                        Error.DuplicatedValue(null, null, null, "Duplicate_2Read_1Write", "350"),
                        Error.DuplicatedValue(null, null, null, "Duplicate_2Read_1Write", "301")),

                    Error.DuplicatedValue(null, null, null, "Duplicate_1Read_1Table", "400, 401").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Duplicate_1Read_1Table", "400"),
                        Error.DuplicatedValue(null, null, null, "Duplicate_1Read_1Table", "401")),

                    Error.DuplicatedValue(null, null, null, "Duplicate_1Write_1Table", "500, 501").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Duplicate_1Write_1Table", "500"),
                        Error.DuplicatedValue(null, null, null, "Duplicate_1Write_1Table", "501")),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckNameTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "1"),
                    Error.EmptyTag(null, null, null, "2"),
                    // DuplicatedValue not really necessary here as EmptyTag is already a critical error
                    //Error.DuplicatedValue(null, null, null, "", "1, 2").WithSubResults(new List<IValidationResult>
                    //{
                    //    Error.DuplicatedValue(null, null, null, "", "1"),
                    //    Error.DuplicatedValue(null, null, null, "", "2"),
                    //}),

                    Error.EmptyTag(null, null, null, "10"),
                    Error.EmptyTag(null, null, null, "11"),
                    // DuplicatedValue not really necessary here as EmptyTag is already a critical error
                    //Error.DuplicatedValue(null, null, null, "  ", "10, 11").WithSubResults(new List<IValidationResult>
                    //{
                    //    Error.DuplicatedValue(null, null, null, "  ", "10"),
                    //    Error.DuplicatedValue(null, null, null, "  ", "11"),
                    //}),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Currently there aren't any invalid characters.")]
        public void Param_CheckNameTag_InvalidChars()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidChars",
                ExpectedResults = new List<IValidationResult>
                {
                    // There are currently no invalid char
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckNameTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "1"),
                    Error.MissingTag(null, null, null, "2"),
                    // DuplicatedValue not really necessary here as MissingTag is already a critical error
                    //Error.DuplicatedValue(null, null, null, "", "1, 2").WithSubResults(new List<IValidationResult>
                    //{
                    //    Error.DuplicatedValue(null, null, null, "", "1"),
                    //    Error.DuplicatedValue(null, null, null, "", "2"),
                    //}),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckNameTag_RestrictedName()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RestrictedName",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RestrictedName(null, null, null, "1", "__StartingWithTwoUnderscores"),
                    Error.RestrictedName(null, null, null, "2", "__StartingWithTwoUnderscores"),
                    // DuplicatedValue not really necessary here as MissingTag is already a critical error
                    //Error.DuplicatedValue(null, null, null, "__StartingWithTwoUnderscores", "1, 2").WithSubResults(new List<IValidationResult>
                    //{
                    //    Error.DuplicatedValue(null, null, null, "__StartingWithTwoUnderscores", "1"),
                    //    Error.DuplicatedValue(null, null, null, "__StartingWithTwoUnderscores", "2"),
                    //}),

                    Error.RestrictedName(null, null, null, "10", "TotalNbrOfActiveCriticalAlarms"),
                    Error.RestrictedName(null, null, null, "11", "TotalNbrOfActiveCriticalAlarms"),
                    // DuplicatedValue not really necessary here as MissingTag is already a critical error
                    //Error.DuplicatedValue(null, null, null, "TotalNbrOfActiveCriticalAlarms", "10, 11").WithSubResults(new List<IValidationResult>
                    //{
                    //    Error.DuplicatedValue(null, null, null, "TotalNbrOfActiveCriticalAlarms", "10"),
                    //    Error.DuplicatedValue(null, null, null, "TotalNbrOfActiveCriticalAlarms", "11"),
                    //}),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Param_CheckNameTag_RTDisplayExpectedOnContextMenu()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpectedOnContextMenu",
                ExpectedResults = new List<IValidationResult>
                {
                    //Error.RTDisplayExpectedOnContextMenu(null, null, null, ""),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Param_CheckNameTag_RTDisplayExpectedOnQActionFeedback()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpectedOnQActionFeedback",
                ExpectedResults = new List<IValidationResult>
                {
                    //Error.RTDisplayExpectedOnQActionFeedback(null, null, null, ""),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckNameTag_UnrecommendedChars()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedChars",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedChars(null, null, null, "Name", "AAA[AAA", "["),
                    Error.UnrecommendedChars(null, null, null, "Name", "AAA[AAA", "["),
                    Error.DuplicatedValue(null, null, null, "AAA[AAA", "1, 2").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "AAA[AAA", "1"),
                        Error.DuplicatedValue(null, null, null, "AAA[AAA", "2")),

                    Error.UnrecommendedChars(null, null, null, "Name", "Poll_metaData_%[x]-_|_QATrigger", "% [ ] - |"),

                    Error.UnrecommendedChars(null, null, null, "Name", "test with Spaces in The middle", "[Whitespace]"),
                    Error.UnrecommendedChars(null, null, null, "Name", " test With leading space", "[Whitespace]"),
                    Error.UnrecommendedChars(null, null, null, "Name", "    test With leading spaces", "[Whitespace]"),
                    Error.UnrecommendedChars(null, null, null, "Name", "test With trailing space ", "[Whitespace]"),
                    Error.UnrecommendedChars(null, null, null, "Name", "test With trailing spaces   ", "[Whitespace]"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckNameTag_UnrecommendedStartChars()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedStartChars",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedStartChars(null, null, null, "Name", "1AAA", "1"),

                    Error.UnrecommendedStartChars(null, null, null, "Name", "10AAA", "1, 0"),
                    Error.UnrecommendedStartChars(null, null, null, "Name", "10AAA", "1, 0"),
                    Error.DuplicatedValue(null, null, null, "10AAA", "10, 11").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "10AAA", "10"),
                        Error.DuplicatedValue(null, null, null, "10AAA", "11"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Param_CheckNameTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "1", " ABC "),

                    Error.UntrimmedTag(null, null, null, "10", " XYZ "),
                    Error.UntrimmedTag(null, null, null, "11", " XYZ "),
                    Error.DuplicatedValue(null, null, null, "XYZ", "10, 11").WithSubResults(
                        Error.DuplicatedValue(null, null, null, " XYZ ", "10"),
                        Error.DuplicatedValue(null, null, null, " XYZ ", "11"))
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckNameTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckNameTag_Valid()
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
        public void Param_CheckNameTag_LoggerTableColumnNameChanged()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "LoggerTableColumnNameChanged",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.LoggerTableColumnNameChanged(null, null, "oldNameOfColumn", "101", "100", "renamedOldNameOfColumn"),
                    ErrorCompare.LoggerTableColumnNameChanged(null, null, "oldName_ofLastColumn", "104", "100", "oldNameofLastColumn"),
                    ErrorCompare.LoggerTableColumnNameChanged(null, null, "oldName_ofLastColumn", "203", "200", "oldName_ofLastColumn11"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckNameTag();

        [TestMethod]
        [Ignore("Currently there aren't any invalid characters, and so also no code fix.")]
        public void Param_CheckNameTag_InvalidChars()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "InvalidChars",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckNameTag_UnrecommendedChars()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedChars",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckNameTag_UntrimmedTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedTag",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void Param_CheckNameTag_UnrecommendedChars()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedChars(null, null, null, "1", "2", "3");

            var expected = new ValidationResult
            {
                ErrorId = 7,
                FullId = "2.2.7",
                Category = Category.Param,
                Severity = Severity.Warning,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "Unrecommended chars in some parameter names.",
                Description = "Unrecommended chars '3' in tag '1'. Current value '2'.",
                HowToFix = "",
                HasCodeFix = true,
                AutoFixWarnings = new List<(string, bool)>
                {
                    ("Double check the use of the Parameter class in QActions.", true),
                    ("Double check the use of the (Get/Set)ParameterByName methods in QActions.", false)
                }
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckNameTag();

        [TestMethod]
        public void Param_CheckNameTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckNameTag_CheckId() => Generic.CheckId(root, CheckId.CheckNameTag);
    }
}
namespace ProtocolTests.Protocol.Params.Param.Measurement.Discreets.Discreet.Display.CheckDisplayTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.Discreet.Display.CheckDisplayTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckDisplayTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckDisplayTag_Valid()
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
        public void Param_CheckDisplayTag_DuplicatedValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicatedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicatedValue(null, null, null, "My Discreet", "100").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "My Discreet", "100"),
                        Error.DuplicatedValue(null, null, null, "My Discreet", "100")),

                    Error.DuplicatedValue(null, null, null, "My Discreet", "101").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "My Discreet", "101"),
                        Error.DuplicatedValue(null, null, null, "My discreet", "101")),
                    Error.WrongCasing(null, null, null).WithSubResults(
                        Error.WrongCasing_Sub(null, null, null, "My discreet", "My Discreet", "101"))
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDisplayTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "100"),
                    Error.EmptyTag(null, null, null, "101"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDisplayTag_InvalidPagebuttonCaption()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidPagebuttonCaption",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidPagebuttonCaption(null, null, null, "AA...A", "AAA...", "1"),
                    Error.InvalidPagebuttonCaption(null, null, null, "BBB ...", "BBB...", "2"),
                    Error.InvalidPagebuttonCaption(null, null, null, " CCC ... ", "CCC...", "3"),
        }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDisplayTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "100"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDisplayTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "100", "AAA... "),
                    Error.UntrimmedTag(null, null, null, "200", " BBB "),
                    Error.UntrimmedTag(null, null, null, "300", " CCC"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDisplayTag_WrongCasing()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "WrongCasing",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.WrongCasing(null, null, null).WithSubResults(
                        Error.WrongCasing_Sub(null, null, null, "wrong casing", "Wrong Casing", "100"))
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly IRoot codeFix = new CheckDisplayTag();

        [TestMethod]
        public void Param_CheckDisplayTag_InvalidPagebuttonCaption()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "InvalidPagebuttonCaption",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckDisplayTag_UntrimmedTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedTag",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckDisplayTag_WrongCasing()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "WrongCasing",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckDisplayTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckDisplayTag_Valid()
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
        public void Param_CheckDisplayTag_UpdatedValue()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedValue(null, null, "1", "100", "Updated", "New_Updated"),
                    ErrorCompare.UpdatedValue(null, null, "2", "100", "Updated_2", "New_Updated_2"),
                    ErrorCompare.UpdatedValue(null, null, "3", "100", "ToEmpty", ""),

                    ErrorCompare.UpdatedValue(null, null, "aaa", "200", "Updated", "New_Updated"),
                    ErrorCompare.UpdatedValue(null, null, "bbb", "200", "Updated_2", "New_Updated_2"),
                    ErrorCompare.UpdatedValue(null, null, "ccc", "200", "ToEmpty", ""),
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
        public void Param_CheckDisplayTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "0");

            string description = "Empty tag 'Discreet/Display' in Param '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckDisplayTag_InvalidPagebuttonCaption()
        {
            // Create ErrorMessage
            var message = Error.InvalidPagebuttonCaption(null, null, null, "AAA", "AAA...", "2");

            string description = "Invalid pagebutton caption format 'AAA'. Suggested fix 'AAA...'. Param ID '2'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckDisplayTag_MissingTag()
        {
            // Create ErrorMessage
            var message = Error.MissingTag(null, null, null, "0");

            string description = "Missing tag 'Discreet/Display' in Param '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void Param_CheckDisplayTag_UntrimmedTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedTag(null, null, null, "0", "1");

            string description = "Untrimmed tag 'Discreet/Display' in Param '0'. Current value '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckDisplayTag();

        [TestMethod]
        public void Param_CheckDisplayTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckDisplayTag_CheckId() => Generic.CheckId(root, CheckId.CheckDisplayTag);
    }
}
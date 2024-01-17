namespace ProtocolTests.Protocol.TreeControls.TreeControl.HiddenColumns.CheckHiddenColumnsTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.HiddenColumns.CheckHiddenColumnsTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckHiddenColumnsTag();

        #region Valid Checks

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_Valid()
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
        public void TreeControl_CheckHiddenColumnsTag_DuplicateId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateId(null, null, null, "1002", "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "A", "1").WithSubResults(
                        Error.InvalidValueInTag_Sub(null, null, null, "A")),
                    Error.InvalidValue(null, null, null, "A,B,C", "2").WithSubResults(
                        Error.InvalidValueInTag_Sub(null, null, null, "A"),
                        Error.InvalidValueInTag_Sub(null, null, null, "B"),
                        Error.InvalidValueInTag_Sub(null, null, null, "C")),
                    Error.InvalidValue(null, null, null, "1001;1002", "3").WithSubResults(
                        Error.InvalidValueInTag_Sub(null, null, null, "1001;1002"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_IrrelevantColumn()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "IrrelevantColumn",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.IrrelevantColumn(null, null, null, "5001", "1"),
                    Error.IrrelevantColumn(null, null, null, "5002", "1"),

                    Error.IrrelevantColumn(null, null, null, "5001", "3"),
                    Error.IrrelevantColumn(null, null, null, "5002", "3"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_NonExistingIds()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIds",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingIds(null, null, null, "1").WithSubResults(
                        Error.NonExistingIds_Sub(null, null, null, "1003", "1"),
                        Error.NonExistingIds_Sub(null, null, null, "2005", "1"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "1", " 1002 ").WithSubResults(
                        Error.UntrimmedInTag_Sub(null, null, null, " 1002 ")),
                    Error.UntrimmedTag(null, null, null, "2", " 1001, 1002 ,  2001 ").WithSubResults(
                        Error.UntrimmedInTag_Sub(null, null, null, " 1001"),
                        Error.UntrimmedInTag_Sub(null, null, null, " 1002 "),
                        Error.UntrimmedInTag_Sub(null, null, null, "  2001 "))
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckHiddenColumnsTag();

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_UntrimmedTag()
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
        public void TreeControl_CheckHiddenColumnsTag_DuplicateId()
        {
            // Create ErrorMessage
            var message = Error.DuplicateId(null, null, null, "0", "1");

            string description = "Duplicate value '0' in tag 'HiddenColumns'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "0");

            string description = "Empty tag 'HiddenColumns' in TreeControl '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "A", "1");

            string description = "Invalid value 'A' in tag 'HiddenColumns'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_InvalidValueInTag()
        {
            // Create ErrorMessage
            var message = Error.InvalidValueInTag_Sub(null, null, null, "A");

            string description = "Invalid value 'A' in tag 'HiddenColumns'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_NonExistingIds()
        {
            // Create ErrorMessage
            var message = Error.NonExistingIds(null, null, null, "0");

            string description = "Tag 'HiddenColumns' references non-existing IDs. TreeControl ID '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_NonExistingIdsSub()
        {
            // Create ErrorMessage
            var message = Error.NonExistingIds_Sub(null, null, null, "0", "1");

            string description = "Tag 'HiddenColumns' references a non-existing 'Column' with PID '0'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_UntrimmedInTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedInTag_Sub(null, null, null, " 0 ");

            string description = "Untrimmed value ' 0 ' in tag 'HiddenColumns'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_UntrimmedTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedTag(null, null, null, "0", " 1 ");

            string description = "Untrimmed tag 'HiddenColumns' in TreeControl '0'. Current value ' 1 '.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckHiddenColumnsTag();

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_CheckCategory() => Generic.CheckCategory(check, Category.TreeControl);

        [TestMethod]
        public void TreeControl_CheckHiddenColumnsTag_CheckId() => Generic.CheckId(check, CheckId.CheckHiddenColumnsTag);
    }
}
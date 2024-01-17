namespace ProtocolTests.Protocol.TreeControls.TreeControl.OverrideDisplayColumns.CheckOverrideDisplayColumnsTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.OverrideDisplayColumns.CheckOverrideDisplayColumnsTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckOverrideDisplayColumnsTag();

        #region Valid Checks

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_Valid()
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
        public void TreeControl_CheckOverrideDisplayColumnsTag_DuplicateId()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateId",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateId(null, null, null, "1002", "1"),
                    Error.DuplicateId(null, null, null, "2002", "1"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_DuplicateOverrideDisplayColumn()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateOverrideDisplayColumn",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateOverrideDisplayColumn(null, null, null, "1000", "1").WithSubResults(
                        Error.DuplicateOverrideDisplayColumns_Sub(null, null, null, "1002"),
                        Error.DuplicateOverrideDisplayColumns_Sub(null, null, null, "1003"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_EmptyTag()
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
        public void TreeControl_CheckOverrideDisplayColumnsTag_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "A|C,B", "1").WithSubResults(
                        Error.InvalidValueInTag_Sub(null, null, null, "A"),
                        Error.InvalidValueInTag_Sub(null, null, null, "C"),
                        Error.InvalidValueInTag_Sub(null, null, null, "B"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_IrrelevantColumn()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "IrrelevantColumn",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.IrrelevantColumn(null, null, null, "5002", "1"),

                    Error.IrrelevantColumn(null, null, null, "5002", "3"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_NonExistingIds()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIds",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingIds(null, null, null, "1").WithSubResults(
                        Error.NonExistingIds_Sub(null, null, null, "1004", "1"),
                        Error.NonExistingIds_Sub(null, null, null, "1005", "1"),
                        Error.NonExistingIds_Sub(null, null, null, "2003", "1"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "1", " 1002 | 1003 ,2002 ").WithSubResults(
                        Error.UntrimmedValueInTag_Sub(null, null, null, " 1002 "),
                        Error.UntrimmedValueInTag_Sub(null, null, null, " 1003 "),
                        Error.UntrimmedValueInTag_Sub(null, null, null, "2002 ")),
                    Error.UntrimmedTag(null, null, null, "2", "1002 | 1003 ,2002").WithSubResults(
                        Error.UntrimmedValueInTag_Sub(null, null, null, "1002 "),
                        Error.UntrimmedValueInTag_Sub(null, null, null, " 1003 "))
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckOverrideDisplayColumnsTag();

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_UntrimmedTag()
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
        public void TreeControl_CheckOverrideDisplayColumnsTag_DuplicateId()
        {
            // Create ErrorMessage
            var message = Error.DuplicateId(null, null, null, "1001", "1");

            string description = "Duplicate value '1001' in tag 'OverrideDisplayColumns'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_DuplicateOverrideDisplayColumn()
        {
            // Create ErrorMessage
            var message = Error.DuplicateOverrideDisplayColumn(null, null, null, "0", "1");

            string description = "Duplicate OverrideDisplayColumns IDs for Table '0'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_DuplicateOverrideDisplayColumn_Subs()
        {
            // Create ErrorMessage
            var message = Error.DuplicateOverrideDisplayColumns_Sub(null, null, null, "0");

            string description = "Duplicate OverrideDisplayColumns ID '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "0");

            string description = "Empty tag 'OverrideDisplayColumns' in TreeControl '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "A|B,C", "1");

            string description = "Invalid value 'A|B,C' in tag 'OverrideDisplayColumns'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_InvalidValueInTag()
        {
            // Create ErrorMessage
            var message = Error.InvalidValueInTag_Sub(null, null, null, "A");

            string description = "Invalid value 'A' in tag 'OverrideDisplayColumns'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_NonExistingIds()
        {
            // Create ErrorMessage
            var message = Error.NonExistingIds(null, null, null, "0");

            string description = "Tag 'OverrideDisplayColumns' references non-existing IDs. TreeControl ID '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_NonExistingIdsSub()
        {
            // Create ErrorMessage
            var message = Error.NonExistingIds_Sub(null, null, null, "0", "1");

            string description = "Tag 'OverrideDisplayColumns' references a non-existing 'Column' with PID '0'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_UntrimmedTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedTag(null, null, null, "0", " 1 |2, 3 ");

            string description = "Untrimmed tag 'OverrideDisplayColumns' in TreeControl '0'. Current value ' 1 |2, 3 '.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_UntrimmedValueInTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedValueInTag_Sub(null, null, null, " 1 ");

            string description = "Untrimmed value ' 1 ' in tag 'OverrideDisplayColumns'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckOverrideDisplayColumnsTag();

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_CheckCategory() => Generic.CheckCategory(check, Category.TreeControl);

        [TestMethod]
        public void TreeControl_CheckOverrideDisplayColumnsTag_CheckId() => Generic.CheckId(check, CheckId.CheckOverrideDisplayColumnsTag);
    }
}
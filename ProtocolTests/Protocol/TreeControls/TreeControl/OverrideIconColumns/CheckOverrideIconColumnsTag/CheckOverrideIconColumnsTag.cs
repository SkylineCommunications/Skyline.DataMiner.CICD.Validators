namespace ProtocolTests.Protocol.TreeControls.TreeControl.OverrideIconColumns.CheckOverrideIconColumnsTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.TreeControls.TreeControl.OverrideIconColumns.CheckOverrideIconColumnsTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckOverrideIconColumnsTag();

        #region Valid Checks

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_Valid()
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
        public void TreeControl_CheckOverrideIconColumnsTag_DuplicateId()
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
        public void TreeControl_CheckOverrideIconColumnsTag_DuplicateOverrideIconColumns()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicateOverrideIconColumns",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicateOverrideIconColumns(null, null, null, "1000", "1").WithSubResults(
                        Error.DuplicateOverrideIconColumns_Sub(null, null, null, "1002"),
                        Error.DuplicateOverrideIconColumns_Sub(null, null, null, "1003")),
                    Error.DuplicateOverrideIconColumns(null, null, null, "2000", "1").WithSubResults(
                        Error.DuplicateOverrideIconColumns_Sub(null, null, null, "2002"),
                        Error.DuplicateOverrideIconColumns_Sub(null, null, null, "2003"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_EmptyTag()
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
        public void TreeControl_CheckOverrideIconColumnsTag_InvalidValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidValue(null, null, null, "A,B", "1").WithSubResults(
                        Error.InvalidValueInTag_Sub(null, null, null, "A"),
                        Error.InvalidValueInTag_Sub(null, null, null, "B"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_IrrelevantColumn()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "IrrelevantColumn",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.IrrelevantColumn(null, null, null, "5002", "1"),

                    Error.IrrelevantColumn(null, null, null, "5002", "3")
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_NonExistingIds()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingIds",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingIds(null, null, null, "1").WithSubResults(
                        Error.NonExistingIds_Sub(null, null, null, "1099", "1"),
                        Error.NonExistingIds_Sub(null, null, null, "2099", "1"))
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "1", " 1002, 2002 ").WithSubResults(
                        Error.UntrimmedValueInTag_Sub(null, null, null, " 1002"),
                        Error.UntrimmedValueInTag_Sub(null, null, null, " 2002 ")),
                    Error.UntrimmedTag(null, null, null, "2", "1002, 2002").WithSubResults(
                        Error.UntrimmedValueInTag_Sub(null, null, null, " 2002"))
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckOverrideIconColumnsTag();

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_UntrimmedTag()
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
        public void TreeControl_CheckOverrideIconColumnsTag_DuplicateId()
        {
            // Create ErrorMessage
            var message = Error.DuplicateId(null, null, null, "0", "1");

            string description = "Duplicate value '0' in tag 'OverrideIconColumns'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_DuplicateOverrideIconColumns()
        {
            // Create ErrorMessage
            var message = Error.DuplicateOverrideIconColumns(null, null, null, "0", "1");

            string description = "Duplicate OverrideIconColumns IDs for Table '0'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_DuplicateOverrideIconColumns_Subs()
        {
            // Create ErrorMessage
            var message = Error.DuplicateOverrideIconColumns_Sub(null, null, null, "0");

            string description = "Duplicate OverrideIconColumns ID '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_EmptyTag()
        {
            // Create ErrorMessage
            var message = Error.EmptyTag(null, null, null, "0");

            string description = "Empty tag 'OverrideIconColumns' in TreeControl '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_InvalidValue()
        {
            // Create ErrorMessage
            var message = Error.InvalidValue(null, null, null, "A,B", "1");

            string description = "Invalid value 'A,B' in tag 'OverrideIconColumns'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_InvalidValueInTag()
        {
            // Create ErrorMessage
            var message = Error.InvalidValueInTag_Sub(null, null, null, "A");

            string description = "Invalid value 'A' in tag 'OverrideIconColumns'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_NonExistingIds()
        {
            // Create ErrorMessage
            var message = Error.NonExistingIds(null, null, null, "0");

            string description = "Tag 'OverrideIconColumns' references non-existing IDs. TreeControl ID '0'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_NonExistingIdsSub()
        {
            // Create ErrorMessage
            var message = Error.NonExistingIds_Sub(null, null, null, "0", "1");

            string description = "Tag 'OverrideIconColumns' references a non-existing 'Column' with PID '0'. TreeControl ID '1'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_UntrimmedTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedTag(null, null, null, "0", " 1 ");

            string description = "Untrimmed tag 'OverrideIconColumns' in TreeControl '0'. Current value ' 1 '.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_UntrimmedValueInTag()
        {
            // Create ErrorMessage
            var message = Error.UntrimmedValueInTag_Sub(null, null, null, " 0 ");

            string description = "Untrimmed value ' 0 ' in tag 'OverrideIconColumns'.";

            // Assert
            Assert.AreEqual(description, message.Description);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CheckOverrideIconColumnsTag();

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_CheckCategory() => Generic.CheckCategory(check, Category.TreeControl);

        [TestMethod]
        public void TreeControl_CheckOverrideIconColumnsTag_CheckId() => Generic.CheckId(check, CheckId.CheckOverrideIconColumnsTag);
    }
}
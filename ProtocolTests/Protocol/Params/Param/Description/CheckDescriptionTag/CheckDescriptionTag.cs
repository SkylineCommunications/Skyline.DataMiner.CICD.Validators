namespace ProtocolTests.Protocol.Params.Param.Description.CheckDescriptionTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Description.CheckDescriptionTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckDescriptionTag();

        #region Valid
        [TestMethod]
        public void Param_CheckDescriptionTag_Valid()
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

        #region Invalid
        [TestMethod]
        public void Param_CheckDescriptionTag_DuplicatedValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicatedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicatedValue(null, null, null, "Duplicate Read", "100, 101").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Duplicate Read", "100"),
                        Error.DuplicatedValue(null, null, null, "Duplicate Read", "101")),

                    Error.DuplicatedValue(null, null, null, "Duplicate Write", "110, 111").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Duplicate Write", "110"),
                        Error.DuplicatedValue(null, null, null, "Duplicate Write", "111")),

                    Error.DuplicatedValue(null, null, null, "Duplicate Read-Write", "120, 220, 121, 221").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Duplicate Read-Write", "120"),
                        Error.DuplicatedValue(null, null, null, "Duplicate Read-Write", "220"),

                        Error.DuplicatedValue(null, null, null, "Duplicate Read-Write", "121"),
                        Error.DuplicatedValue(null, null, null, "Duplicate Read-Write", "221")),

                    Error.DuplicatedValue(null, null, null, "Duplicate_Different_Casing", "1000, 1001").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Duplicate_Different_Casing", "1000"),
                        Error.DuplicatedValue(null, null, null, "Duplicate_different_Casing", "1001")),

                    Error.WrongCasing(null, null, null).WithSubResults(
                        Error.WrongCasing_Sub(null, null, null, "Duplicate_different_Casing", "Duplicate_Different_Casing", "1001"))
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDescriptionTag_EmptyTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyTag(null, null, null, "100"),    // Empty
                    Error.EmptyTag(null, null, null, "101"),    // Spaces
                    
                    Error.EmptyTag(null, null, null, "200"),    // Exported
                    Error.EmptyTag(null, null, null, "201"),    // RTDisplay_True
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDescriptionTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "100"),  // Read
                    Error.MissingTag(null, null, null, "201"),  // Write

                    Error.MissingTag(null, null, null, "102"),  // ReadWrite
                    Error.MissingTag(null, null, null, "202"),  // ReadWrite

                    Error.MissingTag(null, null, null, "103"),  // Title Begin
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDescriptionTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "100", " Leading Space"),
                    Error.UntrimmedTag(null, null, null, "101", "Trailing Spaces  "),
                    Error.UntrimmedTag(null, null, null, "102", "  Untrimmed Spaces "),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckDescriptionTag_WrongCasing()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "WrongCasing",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.WrongCasing(null, null, null).WithSubResults(
                        Error.WrongCasing_Sub(null, null, null, "space separator", "Space Separator", "100"),
                        Error.WrongCasing_Sub(null, null, null, "underscore_Separator_1", "Underscore_Separator_1", "101"),

                        Error.WrongCasing_Sub(null, null, null, "in Starting_With_Preposition", "In Starting_With_Preposition", "200"),
                        Error.WrongCasing_Sub(null, null, null, "Ending_With_Preposition in", "Ending_With_Preposition In", "201"))
                }
            };

            Generic.Validate(test, data);
        }
        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckDescriptionTag();

        [TestMethod]
        public void Param_CheckDescriptionTag_UntrimmedTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedTag",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckDescriptionTag_WrongCasing()
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
        private readonly ICompare compare = new CheckDescriptionTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckDescriptionTag_Valid()
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
        public void Param_CheckDescriptionTag_RemovedItem()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedItem",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedItem(null, null, "RTDisplay True", "1"),
                    ErrorCompare.RemovedItem(null, null, "Trending True", "100"),
                    ErrorCompare.RemovedItem(null, null, "Monitored True", "200"),

                    ErrorCompare.RemovedItem(null, null, "Export True", "300"),
                    ErrorCompare.RemovedItem(null, null, "Export 1000", "301"),
                    ErrorCompare.RemovedItem(null, null, "Export 1000 And 2000", "302"),

                    ErrorCompare.RemovedItem(null, null, "My Table", "1000"),
                    ErrorCompare.RemovedItem(null, null, "Column1 RTDisplay True (My Table)", "1001"),
                    ErrorCompare.RemovedItem(null, null, "Column2 RTDisplay Default1 (My Table)", "1002"),
                    ErrorCompare.RemovedItem(null, null, "Column3 RTDisplay Default2 (My Table)", "1003"),
                    ErrorCompare.RemovedItem(null, null, "Column4 RTDisplay False (My Table)", "1004"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckDescriptionTag_UpdatedValue()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedValue(null, null, "1", "RTDisplay True", "Changed RTDisplay True"),
                    ErrorCompare.UpdatedValue(null, null, "100", "Trending True", "Changed Trending True"),
                    ErrorCompare.UpdatedValue(null, null, "200", "Monitored True", "Changed Monitored True"),

                    ErrorCompare.UpdatedValue(null, null, "300", "Export True", "Changed Export True"),
                    ErrorCompare.UpdatedValue(null, null, "301", "Export 1000", "Changed Export 1000"),
                    ErrorCompare.UpdatedValue(null, null, "302", "Export 1000 And 2000", "Changed Export 1000 And 2000"),

                    ErrorCompare.UpdatedValue(null, null, "1000", "My Table", "Changed My Table"),
                    ErrorCompare.UpdatedValue(null, null, "1001", "Column1 RTDisplay True (My Table)", "Changed Column1 RTDisplay True (My Table)"),
                    ErrorCompare.UpdatedValue(null, null, "1002", "Column2 RTDisplay Default1 (My Table)", "Changed Column2 RTDisplay Default1 (My Table)"),
                    ErrorCompare.UpdatedValue(null, null, "1003", "Column3 RTDisplay Default2 (My Table)", "Changed Column3 RTDisplay Default2 (My Table)"),
                    ErrorCompare.UpdatedValue(null, null, "1004", "Column4 RTDisplay False (My Table)", "Changed Column4 RTDisplay False (My Table)"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckDescriptionTag();

        [TestMethod]
        public void Param_CheckDescriptionTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckDescriptionTag_CheckId() => Generic.CheckId(root, CheckId.CheckDescriptionTag);
    }
}
namespace ProtocolTests.Protocol.Params.Param.Display.Positions.Position.Page.CheckPageTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Positions.Position.Page.CheckPageTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckPageTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckPageTag_Valid()
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
        public void Param_CheckRangeTag_DuplicatedValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicatedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicatedValue(null, null, null, "My Page", "100").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "My Page", "100"),
                        Error.DuplicatedValue(null, null, null, "My Page", "100"))
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckRangeTag_EmptyTag()
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
        public void Param_CheckRangeTag_MissingTag()
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
        public void Param_CheckRangeTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "100", " Leading Space"),
                    Error.UntrimmedTag(null, null, null, "100", "Trailing Spaces   "),
                    Error.UntrimmedTag(null, null, null, "100", " Leading and Trailing Spaces   "),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckRangeTag_WrongCasing()
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
        private readonly ICodeFix codeFix = new CheckPageTag();

        [TestMethod]
        public void Param_CheckPageTag_UntrimmedTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedTag",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckPageTag_WrongCasing()
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
        private readonly ICompare compare = new CheckPageTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckPageTag_Valid()
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
        public void Param_CheckPageTag_RemovedFromPage()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedFromPage",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedFromPage(null, null, "1", "Page2"),
                    ErrorCompare.RemovedFromPage(null, null, "2", "Page1"),
                    ErrorCompare.RemovedFromPage(null, null, "3", "Page With Spaces"),
                    ErrorCompare.RemovedFromPage(null, null, "3", "Page4"),
                    ErrorCompare.RemovedFromPage(null, null, "4", "Page With Spaces"),
                    ErrorCompare.RemovedFromPage(null, null, "5", "Page With Spaces"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckPageTag();

        [TestMethod]
        public void Param_CheckPageTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckPageTag_CheckId() => Generic.CheckId(root, CheckId.CheckPageTag);
    }
}
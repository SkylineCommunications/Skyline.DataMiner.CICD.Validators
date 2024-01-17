namespace ProtocolTests.Protocol.Params.Param.Display.Positions.Position.Column.CheckColumnTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Positions.Position.Column.CheckColumnTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckColumnTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckColumnTag_Valid()
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
        public void Param_CheckColumnTag_MissingTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingTag(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckColumnTag_EmptyTag()
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

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckColumnTag_InvalidTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidTag(null, null, null, "abc", "1"),
                    Error.InvalidTag(null, null, null, "-1", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckColumnTag_UntrimmedTag()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedTag(null, null, null, "1", " 1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckColumnTag_UnrecommendedValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedValue(null, null, null, "Page2", "1, 2").WithSubResults(
                        Error.UnrecommendedValue(null, null, null, "Page2", "1"),
                        Error.UnrecommendedValue(null, null, null, "Page2", "2")),
                    Error.UnrecommendedValue(null, null, null, "Page3", "10").WithSubResults(
                        Error.UnrecommendedValue(null, null, null, "Page3", "10"))
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckColumnTag();

        [TestMethod]
        public void Param_CheckColumnTag_UntrimmedTag()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedTag",
            };

            Generic.Fix(codeFix, data);
        }

    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckColumnTag();

        [TestMethod]
        public void Param_CheckColumnTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckColumnTag_CheckId() => Generic.CheckId(root, CheckId.CheckColumnTag);
    }
}
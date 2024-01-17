namespace ProtocolTests.Protocol.Params.Param.Interprete.Exceptions.Exception.Display.CheckDisplayTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Exceptions.Exception.Display.CheckDisplayTag;

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
                    Error.DuplicatedValue(null, null, null, "My Exception", "100").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "My Exception", "100"),
                        Error.DuplicatedValue(null, null, null, "My Exception", "100"))
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
        public void Param_CheckDisplayTag_UnrecommendedNADisplayValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedNADisplayValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedNADisplayValue(null, null, null, "not applicable", "10", "N/A"),
                    Error.UnrecommendedNADisplayValue(null, null, null, "NA", "10", "N/A"),
                    Error.UnrecommendedNADisplayValue(null, null, null, "n/a", "10", "N/A"),
                    Error.UnrecommendedNADisplayValue(null, null, null, "n_a", "10", "N/A"),
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
                    Error.UntrimmedTag(null, null, null, "100", " Leading Space"),
                    Error.UntrimmedTag(null, null, null, "100", "Trailing Spaces   "),
                    Error.UntrimmedTag(null, null, null, "100", " Leading and Trailing Spaces   "),
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
        private readonly ICodeFix codeFix = new CheckDisplayTag();

        [TestMethod]
        public void Param_CheckDisplayTag_UnrecommendedNADisplayValue()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedNADisplayValue",
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
    public class Attribute
    {
        private readonly IRoot root = new CheckDisplayTag();

        [TestMethod]
        public void Param_CheckDisplayTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckDisplayTag_CheckId() => Generic.CheckId(root, CheckId.CheckDisplayTag);
    }
}
namespace ProtocolTests.Protocol.Params.Param.CheckTrendingAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.CheckTrendingAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckTrendingAttribute();

        #region Valid
        [TestMethod]
        public void Param_CheckTrendingAttribute_Valid()
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
        public void Param_CheckTrendingAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "100"),    // Empty
                    Error.EmptyAttribute(null, null, null, "101"),    // Spaces
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        [Ignore("Covered by RTDisplay unit test")]
        public void Param_CheckTrendingAttribute_RTDisplayExpected()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "RTDisplayExpected",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.RTDisplayExpected(null, null, null, "100"),
                    Error.RTDisplayExpected(null, null, null, "101"),
                    Error.RTDisplayExpected(null, null, null, "102"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckTrendingAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "100", " true"),

                    Error.UntrimmedAttribute(null, null, null, "200", "false "),
                    Error.UntrimmedAttribute(null, null, null, "201", " false "),
                }
            };

            Generic.Validate(test, data);
        }
        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckTrendingAttribute();

        [TestMethod]
        public void Param_CheckTrendingAttribute_UntrimmedAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttribute",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckTrendingAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckTrendingAttribute_Valid()
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
        public void Param_CheckTrendingAttribute_DisabledTrending()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "DisabledTrending",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.DisabledTrending(null, null, "1"),
                    ErrorCompare.DisabledTrending(null, null, "2"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckTrendingAttribute();

        [TestMethod]
        public void Param_CheckTrendingAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckTrendingAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckTrendingAttribute);
    }
}
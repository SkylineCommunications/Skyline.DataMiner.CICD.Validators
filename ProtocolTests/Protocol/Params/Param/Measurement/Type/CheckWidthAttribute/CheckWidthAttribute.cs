namespace ProtocolTests.Protocol.Params.Param.Measurement.Type.CheckWidthAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Type.CheckWidthAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckWidthAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckWidthAttribute_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion Valid Checks

        #region Invalid Checks

        [TestMethod]
        public void Param_CheckWidthAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckWidthAttribute_EmptyWidth()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyWidth",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyWidth(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckWidthAttribute_UntrimmedWidth()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedWidth",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedWidth(null, null, null, " 110 ", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckWidthAttribute_InvalidWidth()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidWidth",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidWidth(null, null, null, "z", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckWidthAttribute_UnrecommendedWidth()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedWidth",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedWidth(null, null, null, "50", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckWidthAttribute_InconsistentWidth()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InconsistentWidth",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InconsistentWidth(null, null, null, "General", "1, 2", "110, 140", true).WithSubResults(
                        Error.InconsistentWidth(null, null, null, "General", "1", "110", false),
                        Error.InconsistentWidth(null, null, null, "General", "2", "140", false))
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckWidthAttribute_UnsupportedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnsupportedAttribute(null, null, null, "Discreet", "1"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion Invalid Checks
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckWidthAttribute();

        [TestMethod]
        public void Param_CheckWidthAttribute_MissingAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingAttribute",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckWidthAttribute_EmptyWidth()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "EmptyWidth",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckWidthAttribute_UntrimmedWidth()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedWidth",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckWidthAttribute_InvalidWidth()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "InvalidWidth",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckWidthAttribute_UnrecommendedWidth()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UnrecommendedWidth",
            };

            Generic.Fix(codeFix, data);
        }

        [TestMethod]
        public void Param_CheckWidthAttribute_InconsistentWidth()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "InconsistentWidth",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckWidthAttribute();

        [TestMethod]
        public void Param_CheckWidthAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckWidthAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckWidthAttribute);
    }
}
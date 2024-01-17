namespace ProtocolTests.Protocol.Params.Param.SNMP.OID.CheckIdAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.SNMP.OID.CheckIdAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckIdAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckIdAttribute_Valid()
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
        public void Param_CheckIdAttribute_EmptyAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EmptyAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EmptyAttribute(null, null, null, "10"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_InvalidAttributeValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAttributeValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttributeValue(null, null, null, "10", " 011 "),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_NonExistingParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "NonExistingParam",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.NonExistingParam(null, null, null, " 11 ", "10"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_UnsupportedParam()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnsupportedParam",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnsupportedParam(null, null, null, " 100 ", "10"),
                    Error.UnsupportedParam(null, null, null, " 101 ", "11"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckIdAttribute_UntrimmedAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UntrimmedAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UntrimmedAttribute(null, null, null, "10", " 11 "),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckIdAttribute();

        [TestMethod]
        public void Param_CheckIdAttribute_UntrimmedAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "UntrimmedAttribute",
            };

            Generic.Fix(codeFix, data);
        }

    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckIdAttribute();

        [TestMethod]
        public void Param_CheckIdAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckIdAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckIdAttribute);
    }
}
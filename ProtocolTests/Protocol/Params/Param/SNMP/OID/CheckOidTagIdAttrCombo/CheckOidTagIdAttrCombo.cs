namespace ProtocolTests.Protocol.Params.Param.SNMP.OID.CheckOidTagIdAttrCombo
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.SNMP.OID.CheckOidTagIdAttrCombo;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckOidTagIdAttrCombo();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckOidTagIdAttrCombo_Valid()
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
        public void Param_CheckOidTagIdAttrCombo_ExcessiveAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "ExcessiveAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.ExcessiveAttribute(null, null, null, "1000"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckOidTagIdAttrCombo_InvalidCombo()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidCombo",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidCombo(null, null, null, "1.3.6.1.2", " 1 ", "10"),
                    Error.InvalidCombo(null, null, null, "1.3.6.1.2.*", "", "11"),
                    Error.InvalidCombo(null, null, null, "1.3.6.1.1", " 1 ", "2001"),
                    Error.InvalidCombo(null, null, null, "1.3.6.1.2.*", "", "2002"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckOidTagIdAttrCombo();

        [TestMethod]
        public void Param_CheckOidTagIdAttrCombo_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckOidTagIdAttrCombo_CheckId() => Generic.CheckId(root, CheckId.CheckOidTagIdAttrCombo);
    }
}
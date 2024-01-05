namespace ProtocolTests.Protocol.Params.Param.Measurement.Type.CheckLinkAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Type.CheckLinkAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckLinkAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckLinkAttribute_Valid()
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
        public void Param_CheckLinkAttribute_InvalidAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "InvalidAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.InvalidAttribute(null, null, null, "10000"),
                    Error.InvalidAttribute(null, null, null, "10001"),
                    Error.InvalidAttribute(null, null, null, "10002"),
                }
            };

            Generic.Validate(test, data);
        }

        [TestMethod]
        public void Param_CheckLinkAttribute_MissingAttribute()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "MissingAttribute",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.MissingAttribute(null, null, null, "10000"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion Invalid Checks
    }

    [TestClass]
    public class CodeFix
    {
        private readonly ICodeFix codeFix = new CheckLinkAttribute();

        [TestMethod]
        public void Param_CheckLinkAttribute_MissingAttribute()
        {
            Generic.FixData data = new Generic.FixData
            {
                FileNameBase = "MissingAttribute",
            };

            Generic.Fix(codeFix, data);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckLinkAttribute();

        [TestMethod]
        public void Param_CheckLinkAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckLinkAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckLinkAttribute);
    }
}
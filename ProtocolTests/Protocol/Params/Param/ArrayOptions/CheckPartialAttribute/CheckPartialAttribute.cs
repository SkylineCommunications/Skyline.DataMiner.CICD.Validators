namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.CheckPartialAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckPartialAttribute;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckPartialAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckPartialAttribute_Valid()
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
        public void Param_CheckPartialAttribute_EnabledPartial()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "EnabledPartial",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.EnabledPartial(null, null, "1"),
                    ErrorCompare.EnabledPartial(null, null, "2"),
                    ErrorCompare.EnabledPartial(null, null, "3"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckPartialAttribute();

        [TestMethod]
        public void Param_CheckPartialAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckPartialAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckPartialAttribute);
    }
}
namespace ProtocolTests.Protocol.Params.Param.CheckHistorySetAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.CheckHistorySetAttribute;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckHistorySetAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckHistorySetAttribute_Valid()
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
        public void Param_CheckHistorySetAttribute_EnabledHistorySet()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "EnabledHistorySet",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.EnabledHistorySet(null, null, "1"),
                    ErrorCompare.EnabledHistorySet(null, null, "2"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckHistorySetAttribute();

        [TestMethod]
        public void Param_CheckHistorySetAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckHistorySetAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckHistorySetAttribute);
    }
}
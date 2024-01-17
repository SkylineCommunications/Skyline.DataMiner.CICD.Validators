namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckColumnOptionTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckColumnOptionTag;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckColumnOptionTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckColumnOptionTag_Valid()
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
        public void Param_CheckColumnOptionTag_RemovedColumnOptionTag()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedColumnOptionTag",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedColumnOptionTag(null, null, "1004", "1000"),
                    ErrorCompare.RemovedColumnOptionTag(null, null, "2002", "2000"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckColumnOptionTag();

        [TestMethod]
        public void Param_CheckColumnOptionTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckColumnOptionTag_CheckId() => Generic.CheckId(root, CheckId.CheckColumnOptionTag);
    }
}
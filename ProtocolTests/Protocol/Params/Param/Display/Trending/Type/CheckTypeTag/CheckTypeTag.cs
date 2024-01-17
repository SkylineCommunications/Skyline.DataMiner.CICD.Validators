namespace ProtocolTests.Protocol.Params.Param.Display.Trending.Type.CheckTypeTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Display.Trending.Type.CheckTypeTag;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckTypeTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckTypeTag_Valid()
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
        public void Param_CheckTypeTag_UpdatedTrendType()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedTrendType",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedTrendType(null, null, "Average", "1", "Max"),
                    ErrorCompare.UpdatedTrendType(null, null, "Average", "2", "Min"),
                    ErrorCompare.UpdatedTrendType(null, null, "Average", "3", "Sum"),
                    ErrorCompare.UpdatedTrendType(null, null, "Sum", "4", "Average"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckTypeTag();

        [TestMethod]
        public void Param_CheckTypeTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckTypeTag_CheckId() => Generic.CheckId(root, CheckId.CheckTypeTag);
    }
}
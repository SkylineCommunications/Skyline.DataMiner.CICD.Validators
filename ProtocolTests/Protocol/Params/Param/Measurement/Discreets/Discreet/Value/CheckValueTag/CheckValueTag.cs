namespace ProtocolTests.Protocol.Params.Param.Measurement.Discreets.Discreet.Value.CheckValueTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Discreets.Discreet.Value.CheckValueTag;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckValueTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckValueTag_Valid()
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
        public void Param_CheckValueTag_UpdatedValue()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedValue(null, null, "SomeDisplay2", "1", "SomeValue1", "SomeValue3"),
                    ErrorCompare.UpdatedValue(null, null, "SomeDisplay2", "2", "2", "3"),
                    ErrorCompare.UpdatedValue(null, null, "SomeDisplay1", "3", "1", "3"),
                    ErrorCompare.UpdatedValue(null, null, "SomeDisplay3", "3", "3", "1"),
                    ErrorCompare.UpdatedValue(null, null, "SomeDisplay2", "6", "1", "5"),
                    ErrorCompare.UpdatedValue(null, null, "SomeDisplay1", "7", "2", "7"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckValueTag_RemovedItem()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedItem",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedItem(null, null, "SomeValue2", "1"),
                    ErrorCompare.RemovedItem(null, null, "2", "2"),
                    ErrorCompare.RemovedItem(null, null, "1", "3"),
                    ErrorCompare.RemovedItem(null, null, "2", "4"),
                    ErrorCompare.RemovedItem(null, null, "3", "5"),
                    ErrorCompare.UpdatedValue(null, null, "SomeDisplay2", "5", "2", "4"),
                    ErrorCompare.UpdatedValue(null, null, "SomeDisplay1", "5", "1", "5"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckValueTag();

        [TestMethod]
        public void Param_CheckValueTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckValueTag_CheckId() => Generic.CheckId(root, CheckId.CheckValueTag);
    }
}
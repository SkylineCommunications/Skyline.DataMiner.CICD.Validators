namespace ProtocolTests.Protocol.Params.Param.Interprete.Type.CheckTypeTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Type.CheckTypeTag;

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
        public void Param_CheckTypeTag_UpdatedValue()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedValue(null, null, "1", "double", "string"),
                    ErrorCompare.UpdatedValue(null, null, "2", "string", "high nibble"),
                    ErrorCompare.UpdatedValue(null, null, "3", "high nibble", "double"),
                    ErrorCompare.UpdatedValue(null, null, "4", "high nibble", "string"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_RemovedTag()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedTag(null, null, "1"),
                    ErrorCompare.RemovedTag(null, null, "2"),
                    ErrorCompare.RemovedTag(null, null, "3"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckTypeTag_AddedTag()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "AddedTag",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.AddedTag(null, null, "double", "1"),
                    ErrorCompare.AddedTag(null, null, "string", "2"),
                    ErrorCompare.AddedTag(null, null, "high nibble", "3"),
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
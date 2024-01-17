namespace ProtocolTests.Protocol.Params.Param.Interprete.Others.CheckOthersTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Interprete.Others.CheckOthersTag;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckOthersTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckOthersTag_Valid()
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
        public void Param_CheckOthersTag_UpdateOtherId()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdateOtherId",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdateOtherId(null, null, "10", "11", "1", "1"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckOthersTag_UpdateOtherDisplay()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdateOtherDisplay",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdateOtherDisplay(null, null, "Display1", "Display2", "1", "1"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckOthersTag_DeletedValue()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "DeletedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.DeletedValue(null, null, "1", "1"),
                    ErrorCompare.DeletedValue(null, null, "1", "2"),
                    ErrorCompare.DeletedValue(null, null, "6", "101"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckOthersTag_AddedOthers()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "AddedOthers",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.AddedOthers(null, null, "1", "1"),
                    ErrorCompare.AddedOthers(null, null, "1", "2"),
                    ErrorCompare.AddedOthers(null, null, "6", "101"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckOthersTag();

        [TestMethod]
        public void Param_CheckOthersTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckOthersTag_CheckId() => Generic.CheckId(root, CheckId.CheckOthersTag);
    }
}
namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.CheckDisplayColumnAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckDisplayColumnAttribute;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckDisplayColumnAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckDisplayColumnAttribute_Valid()
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
        public void Param_CheckDisplayColumnAttribute_DisplayColumnRemoved()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "DisplayColumnRemoved",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.DisplayColumnRemoved(null, null, "1", "100"),
                    ErrorCompare.DisplayColumnRemoved(null, null, "2", "200"),
                    ErrorCompare.DisplayColumnRemoved(null, null, "1", "300"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckDisplayColumnAttribute_DisplayColumnAdded()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "DisplayColumnAdded",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.DisplayColumnAdded(null, null, "1", "100"),
                    ErrorCompare.DisplayColumnAdded(null, null, "2", "200"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckDisplayColumnAttribute_DisplayColumnContentChanged()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "DisplayColumnContentChanged",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.DisplayColumnContentChanged(null, null, "1", "100", "2"),
                    ErrorCompare.DisplayColumnContentChanged(null, null, "2", "200", "1"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckDisplayColumnAttribute();

        [TestMethod]
        public void Param_CheckDisplayColumnAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckDisplayColumnAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckDisplayColumnAttribute);
    }
}
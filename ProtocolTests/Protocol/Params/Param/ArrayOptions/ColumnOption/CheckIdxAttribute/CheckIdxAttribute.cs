namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckIdxAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.ColumnOption.CheckIdxAttribute;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckIdxAttribute();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckIdxAttribute_Valid()
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
        public void Param_CheckIdxAttribute_UpdatedIdxValue()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedIdxValue",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedIdxValue(null, null, "102", "2", "1", "100"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckIdxAttribute_UpdatedIdxValue_Parent()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "UpdatedIdxValue_Parent",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.UpdatedIdxValue_Parent(null, null, "100").WithSubResults(
                        ErrorCompare.UpdatedIdxValue(null, null, "101", "0", "4", "100"),
                        ErrorCompare.UpdatedIdxValue(null, null, "105", "4", "0", "100")),

                    ErrorCompare.UpdatedIdxValue_Parent(null, null, "200").WithSubResults(
                        //ErrorCompare.UpdatedIdxValue(null, null, "202", "1", "3", "200"), // Ignoring displayKey
                        ErrorCompare.UpdatedIdxValue(null, null, "203", "1", "2", "200"),
                        ErrorCompare.UpdatedIdxValue(null, null, "204", "2", "1", "200"))
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckIdxAttribute();

        [TestMethod]
        public void Param_CheckIdxAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckIdxAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckIdxAttribute);
    }
}
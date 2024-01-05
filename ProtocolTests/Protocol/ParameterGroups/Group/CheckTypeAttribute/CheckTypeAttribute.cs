namespace ProtocolTests.Protocol.ParameterGroups.Group.CheckTypeAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ParameterGroups.Group.CheckTypeAttribute;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckTypeAttribute();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckTypeAttribute_Valid()
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
        public void Protocol_CheckTypeAttribute_DcfParameterGroupTypeChanged()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "DcfParameterGroupTypeChanged",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.DcfParameterGroupTypeChanged(null, null, "1", "In", "Inout"),
                    ErrorCompare.DcfParameterGroupTypeChanged(null, null, "2", "Out", "In"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckTypeAttribute();

        [TestMethod]
        public void Protocol_CheckTypeAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckTypeAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckTypeAttribute);
    }
}
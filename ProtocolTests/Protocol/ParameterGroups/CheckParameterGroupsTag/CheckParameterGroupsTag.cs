namespace SLDisValidatorUnitTests.Protocol.ParameterGroups.CheckParameterGroupsTag
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SLDisValidator2.Interfaces;
    using SLDisValidator2.Tests.Protocol.ParameterGroups.CheckParameterGroupsTag;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckParameterGroupsTag();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckParameterGroupsTag_Valid()
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
        public void Protocol_CheckParameterGroupsTag_DcfAdded()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "DcfAdded",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.DcfAdded(null, null),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckParameterGroupsTag();

        [TestMethod]
        public void Protocol_CheckParameterGroupsTag_CheckCategory() => Generic.CheckCategory(root, Category.ParameterGroup);

        [TestMethod]
        public void Protocol_CheckParameterGroupsTag_CheckId() => Generic.CheckId(root, CheckId.CheckParameterGroupsTag);
    }
}
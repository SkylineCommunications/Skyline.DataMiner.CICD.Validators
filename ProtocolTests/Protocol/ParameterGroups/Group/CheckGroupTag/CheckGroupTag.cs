namespace ProtocolTests.Protocol.ParameterGroups.Group.CheckGroupTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.ParameterGroups.Group.CheckGroupTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckGroupTag();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckGroupTag_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(test, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Protocol_CheckGroupTag_IncompatibleParamReferences()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "IncompatibleParamReferences",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.IncompatibleParamReferences(null, null, null, "1"),
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckGroupTag();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckGroupTag_Valid()
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
        public void Protocol_CheckGroupTag_DcfParameterGroupRemoved()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "DcfParameterGroupRemoved",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.DcfParameterGroupRemoved(null, null, "1"),
                    ErrorCompare.DcfParameterGroupRemoved(null, null, "3"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckGroupTag();

        [TestMethod]
        public void Protocol_CheckGroupTag_CheckCategory() => Generic.CheckCategory(root, Category.ParameterGroup);

        [TestMethod]
        public void Protocol_CheckGroupTag_CheckId() => Generic.CheckId(root, CheckId.CheckGroupTag);
    }
}
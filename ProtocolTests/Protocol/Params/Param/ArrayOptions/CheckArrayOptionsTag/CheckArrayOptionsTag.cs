namespace ProtocolTests.Protocol.Params.Param.ArrayOptions.CheckArrayOptionsTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.ArrayOptions.CheckArrayOptionsTag;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckArrayOptionsTag();

        #region Valid Checks

        [TestMethod]
        public void Param_CheckArrayOptionsTag_Valid()
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
        public void Param_CheckArrayOptionsTag_DisplayColumnChangedToNaming()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "DisplayColumnChangedToNaming",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.DisplayColumnChangedToNaming(null, null, "0", "100", "naming=/103"),
                    ErrorCompare.DisplayColumnChangedToNaming(null, null, "1", "200", "naming=*103,105"),
                    ErrorCompare.DisplayColumnChangedToNaming(null, null, "1", "300", "naming=*103,105"),
                }
            };

            Generic.Compare(compare, data);
        }

        [TestMethod]
        public void Param_CheckArrayOptionsTag_DisplayColumnChangeToNamingFormat()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "DisplayColumnChangeToNamingFormat",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.DisplayColumnChangeToNamingFormat(null, null, "0", "100", ",string,1512,discreet,1514"),
                    ErrorCompare.DisplayColumnChangeToNamingFormat(null, null, "1", "200", ";Hello World;1512;Hello World,1514"),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckArrayOptionsTag();

        [TestMethod]
        public void Param_CheckArrayOptionsTag_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckArrayOptionsTag_CheckId() => Generic.CheckId(root, CheckId.CheckArrayOptionsTag);
    }
}
namespace ProtocolTests.Protocol.DVEs.DVEProtocols.DVEProtocol.ElementPrefix.CheckElementPrefixTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.DVEs.DVEProtocols.DVEProtocol.ElementPrefix.CheckElementPrefixTag;

    [TestClass]
    public class Compare
    {
        private readonly ICompare test = new CheckElementPrefixTag();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckElementPrefixTag_Valid()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Valid,
                FileNameBase = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Compare(test, data);
        }

        #endregion Valid Checks

        #region Invalid Checks

        [TestMethod]
        public void Protocol_CheckElementPrefixTag_AddedElementPrefix()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "AddedElementPrefix",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.AddedElementPrefix(null, null, "MotherChild2", "400"),
                    ErrorCompare.AddedElementPrefix(null, null, "MotherChild3", "500"),
                }
            };

            Generic.Compare(test, data);
        }

        [TestMethod]
        public void Protocol_CheckElementPrefixTag_RemovedElementPrefix()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "RemovedElementPrefix",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.RemovedElementPrefix(null, null, "Mother - Child2", "200"),
                    ErrorCompare.RemovedElementPrefix(null, null, "MotherChild3", "500"),
                }
            };

            Generic.Compare(test, data);
        }

        #endregion Invalid Checks
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckElementPrefixTag();

        [TestMethod]
        public void Protocol_CheckElementPrefixTag_CheckCategory() => Generic.CheckCategory(root, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckElementPrefixTag_CheckId() => Generic.CheckId(root, CheckId.CheckElementPrefixTag);
    }
}
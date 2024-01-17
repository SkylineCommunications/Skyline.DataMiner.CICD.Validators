namespace ProtocolTests.Protocol.Type.CheckDatabaseOptionsAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Type.CheckDatabaseOptionsAttribute;

    [TestClass]
    public class Compare
    {
        private readonly ICompare compare = new CheckDatabaseOptionsAttribute();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckDatabaseOptionsAttribute_Valid()
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
        public void Protocol_CheckDatabaseOptionsAttribute_EnabledPartitionedTrending()
        {
            Generic.CompareData data = new Generic.CompareData
            {
                TestType = Generic.TestType.Invalid,
                FileNameBase = "EnabledPartitionedTrending",
                ExpectedResults = new List<IValidationResult>
                {
                    ErrorCompare.EnabledPartitionedTrending(null, null),
                }
            };

            Generic.Compare(compare, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckDatabaseOptionsAttribute();

        [TestMethod]
        public void Protocol_CheckDatabaseOptionsAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckDatabaseOptionsAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckDatabaseOptionsAttribute);
    }
}
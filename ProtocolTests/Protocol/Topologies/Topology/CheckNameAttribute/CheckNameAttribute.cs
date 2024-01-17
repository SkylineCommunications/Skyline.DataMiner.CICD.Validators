namespace ProtocolTests.Protocol.Topologies.Topology.CheckNameAttribute
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Topologies.Topology.CheckNameAttribute;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckNameAttribute();

        #region Valid Checks

        [TestMethod]
        public void Topology_CheckNameAttribute_Valid()
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
        public void Topology_CheckNameAttribute_DuplicatedValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicatedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicatedValue(null, null, null, "Name1").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Name1"),
                        Error.DuplicatedValue(null, null, null, "Name1"))
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckNameAttribute();

        [TestMethod]
        public void Topology_CheckNameAttribute_CheckCategory() => Generic.CheckCategory(root, Category.Topology);

        [TestMethod]
        public void Topology_CheckNameAttribute_CheckId() => Generic.CheckId(root, CheckId.CheckNameAttribute);
    }
}
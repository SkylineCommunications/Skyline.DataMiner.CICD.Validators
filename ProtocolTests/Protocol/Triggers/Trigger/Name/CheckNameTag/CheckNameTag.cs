namespace ProtocolTests.Protocol.Triggers.Trigger.Name.CheckNameTag
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Triggers.Trigger.Name.CheckNameTag;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckNameTag();

        #region Valid Checks

        [TestMethod]
        public void Trigger_CheckNameTag_Valid()
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
        public void Trigger_CheckNameTag_DuplicatedValue()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "DuplicatedValue",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.DuplicatedValue(null, null, null, "Name1", "1, 2").WithSubResults(
                        Error.DuplicatedValue(null, null, null, "Name1", "1"),
                        Error.DuplicatedValue(null, null, null, "Name1", "2"))
                }
            };

            Generic.Validate(test, data);
        }

        #endregion
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckNameTag();

        [TestMethod]
        public void Trigger_CheckNameTag_CheckCategory() => Generic.CheckCategory(root, Category.Trigger);

        [TestMethod]
        public void Trigger_CheckNameTag_CheckId() => Generic.CheckId(root, CheckId.CheckNameTag);
    }
}
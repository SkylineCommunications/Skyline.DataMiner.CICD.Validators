namespace SLDisDmFeatureCheckUnitTests.Features
{
    using System.Linq;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Data;

    using SLDisDmFeatureCheck.Common;
    using SLDisDmFeatureCheck.Common.Results;
    using SLDisDmFeatureCheck.Features;

    [TestClass]
    public class ExportRule_whereAttributeTests
    {
        private static ExportRule_whereAttribute check;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            check = new ExportRule_whereAttribute();
        }

        [TestMethod]
        public void CheckIsUsed()
        {
            const string code = "<Protocol><ExportRules><ExportRule whereAttribute='' /></ExportRules></Protocol>";
            var input = new ProtocolInputData(code);

            FeatureCheckContext context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);
            var expected = context.Model.Protocol.ExportRules.Select(rule => new FeatureCheckResultItem(rule));

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }
    }
}
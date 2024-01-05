namespace Protocol.FeaturesTests.Features._10._4
{
    using System.Linq;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Features;

    [TestClass]
    public class ExportRule_whereAttributeTests
    {
        private static ExportRuleWhereAttribute check;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            check = new ExportRuleWhereAttribute();
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
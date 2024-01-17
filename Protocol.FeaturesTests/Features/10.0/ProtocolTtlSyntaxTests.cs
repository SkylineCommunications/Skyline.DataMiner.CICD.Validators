namespace Protocol.FeaturesTests.Features._10._0
{
    using System.Linq;

    using Common.Testing;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Features;

    [TestClass]
    public class ProtocolTtlSyntaxTests
    {
        private static ProtocolTtlSyntax check;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            check = new ProtocolTtlSyntax();
        }

        [TestMethod]
        public void CheckIsUsed()
        {
            const string fileName = "ProtocolTtlSyntaxTests.xml";
            var input = ProtocolTestsHelper.GetProtocolInputData(fileName);

            FeatureCheckContext context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);
            var expected = context.Model.Protocol.Params.Select(x => new FeatureCheckResultItem(x));

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }
    }
}
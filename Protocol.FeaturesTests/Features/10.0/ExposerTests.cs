namespace Protocol.FeaturesTests.Features._10._0
{
    using System.Linq;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Features;

    [TestClass]
    public class ExposerTests
    {
        private static Exposer check;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            check = new Exposer();
        }

        [TestMethod]
        public void CheckIsUsed()
        {
            const string code = "<Protocol><Topologies><Topology><Cell><Exposer /></Cell></Topology></Topologies></Protocol>";

            var input = new ProtocolInputData(code);
            var context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);
            var expected = context.Model.Protocol.Topologies.Select(x => new FeatureCheckResultItem(x));

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }
    }
}
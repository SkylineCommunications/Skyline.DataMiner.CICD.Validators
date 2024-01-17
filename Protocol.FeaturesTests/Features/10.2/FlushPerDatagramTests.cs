namespace Protocol.FeaturesTests.Features._10._2
{
    using System.Linq;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Features;

    [TestClass]
    public class FlushPerDatagramTests
    {
        private static FlushPerDatagram check;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            check = new FlushPerDatagram();
        }

        [TestMethod]
        public void CheckIsUsed_PortSettings()
        {
            const string code = "<Protocol><PortSettings><FlushPerDatagram>true</FlushPerDatagram></PortSettings></Protocol>";
            var input = new ProtocolInputData(code);

            FeatureCheckContext context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);
            var expected = new FeatureCheckResultItem(context.Model.Protocol.PortSettings);

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void CheckIsUsed_Ports()
        {
            const string code = "<Protocol><Ports><PortSettings><FlushPerDatagram>true</FlushPerDatagram></PortSettings></Ports></Protocol>";
            var input = new ProtocolInputData(code);

            FeatureCheckContext context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);
            var expected = context.Model.Protocol.Ports.Select(x => new FeatureCheckResultItem(x));

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }
    }
}
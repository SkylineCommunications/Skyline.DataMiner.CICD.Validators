namespace Protocol.FeaturesTests.Features._10._1
{
    using System.Linq;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Features;

    [TestClass]
    public class InternalLicensesTests
    {
        private static InternalLicenses check;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            check = new InternalLicenses();
        }

        [TestMethod]
        public void CheckIsUsed()
        {
            const string code = "<Protocol><InternalLicenses><InternalLicense type='solution' /></InternalLicenses></Protocol>";
            var input = new ProtocolInputData(code);

            FeatureCheckContext context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);
            var expected = context.Model.Protocol.InternalLicenses.Select(x => new FeatureCheckResultItem(x));

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }
    }
}
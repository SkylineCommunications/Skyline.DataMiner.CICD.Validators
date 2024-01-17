namespace Protocol.FeaturesTests.Features._9._5
{
    using System.Linq;

    using Common.Testing;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Features;

    [TestClass]
    public class ConditionalShowHidePageTests
    {
        private static ConditionalShowHidePage check;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            check = new ConditionalShowHidePage();
        }

        [TestMethod]
        public void CheckIsUsed()
        {
            const string fileName = "ConditionalShowHidePageTests.xml";
            var input = ProtocolTestsHelper.GetProtocolInputData(fileName);

            FeatureCheckContext context = new FeatureCheckContext(input, false);

            var result = check.CheckIfUsed(context);
            var expected = context.Model.Protocol.Display.Pages.Select(x => new FeatureCheckResultItem(x));

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }
    }
}
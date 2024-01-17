namespace Protocol.FeaturesTests.Features._10._0
{
    using System.Linq;

    using Common.Testing;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Features;

    [TestClass]
    public class GetPkIdByTableIdTests
    {
        private static GetPkIdByTableId check;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            check = new GetPkIdByTableId();
        }

        [TestMethod]
        public void CheckIsUsed_CSharp()
        {
            const string fileName = "GetPkIdByTableIdTests.xml";
            var input = ProtocolTestsHelper.GetProtocolInputData(fileName);

            FeatureCheckContext context = new FeatureCheckContext(input, false);

            IFeatureCheckResult result = check.CheckIfUsed(context);

            Assert.IsTrue(result.IsUsed);
            Assert.IsTrue(result.FeatureItems.All(x => x is CSharpFeatureCheckResultItem));

            var ids = result.FeatureItems.Select(item => ((IQActionsQAction)item.Node).Id.Value);
            var expectedIds = context.Model.Protocol.QActions.Select(qaction => qaction.Id.Value);
            ids.Should().BeEquivalentTo(expectedIds);
        }
    }
}
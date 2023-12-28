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
    public class SkipInDiagramTests
    {
        private static SkipInDiagram check;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            check = new SkipInDiagram();
        }

        [TestMethod]
        public void CheckIsUsed()
        {
            const string code = "<Protocol><Chains><Chain><Field options='skipInDiagram' /></Chain></Chains></Protocol>";
            var input = new ProtocolInputData(code);

            FeatureCheckContext context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);
            var expected = context.Model.Protocol.Chains.Select(x => new FeatureCheckResultItem(x));

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }
    }
}
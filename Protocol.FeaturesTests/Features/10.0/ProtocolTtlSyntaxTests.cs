namespace SLDisDmFeatureCheckUnitTests.Features
{
    using System.Linq;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SLDisDmFeatureCheck.Common;
    using SLDisDmFeatureCheck.Common.Results;
    using SLDisDmFeatureCheck.Features;
    using SLDisUnitTestsShared;

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
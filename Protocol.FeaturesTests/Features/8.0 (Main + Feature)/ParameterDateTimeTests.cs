namespace Protocol.FeaturesTests.Features._8._0__Main___Feature_
{
    using System.Linq;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Features;

    [TestClass]
    public class ParameterDateTimeTests
    {
        private static ParameterDateTime check;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            check = new ParameterDateTime();
        }

        [TestMethod]
        public void CheckIsUsed()
        {
            const string code = "<Protocol><Params><Param><Measurement><Type options='datetime'/></Measurement></Param></Params></Protocol>";
            var input = new ProtocolInputData(code);

            FeatureCheckContext context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);
            var expected = context.Model.Protocol.Params.Select(x => new FeatureCheckResultItem(x));

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }
    }
}
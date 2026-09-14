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
            const string code = "<Protocol><Params><Param><Measurement><Type options='datetime'/></Measurement><Interprete><Decimals>8</Decimals></Interprete><Display><Decimals>8</Decimals></Display></Param></Params></Protocol>";
            var input = new ProtocolInputData(code);

            FeatureCheckContext context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);
            var expected = context.Model.Protocol.Params.Select(x => new FeatureCheckResultItem(x));

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void CheckIsUsed_DateTimeInTableWithDisabledHeaderSum()
        {
            const string code = "<Protocol><Params><Param id='1000'><ArrayOptions><ColumnOption idx='0' pid='1001' options=';disableHeaderSum'/></ArrayOptions></Param><Param id='1001'><Measurement><Type options='datetime'/></Measurement><Interprete><Decimals>8</Decimals></Interprete><Display><Decimals>8</Decimals></Display></Param></Params></Protocol>";
            var input = new ProtocolInputData(code);

            FeatureCheckContext context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);
            var expected = context.Model.Protocol.Params.Where(x => x.Id?.Value == 1001).Select(x => new FeatureCheckResultItem(x));

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void CheckIsUsed_DateTimeInTableWithDisabledHeaderSumInMultiOptionValue()
        {
            const string code = "<Protocol><Params><Param id='1000'><ArrayOptions><ColumnOption idx='0' pid='1001' options=',foo,disableHeaderSum'/></ArrayOptions></Param><Param id='1001'><Measurement><Type options='datetime'/></Measurement><Interprete><Decimals>8</Decimals></Interprete><Display><Decimals>8</Decimals></Display></Param></Params></Protocol>";
            var input = new ProtocolInputData(code);

            FeatureCheckContext context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);
            var expected = context.Model.Protocol.Params.Where(x => x.Id?.Value == 1001).Select(x => new FeatureCheckResultItem(x));

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void CheckIsNotUsed_Time()
        {
            const string code = "<Protocol><Params><Param><Measurement><Type options='time'/></Measurement><Interprete><Decimals>8</Decimals></Interprete><Display><Decimals>8</Decimals></Display></Param></Params></Protocol>";
            var input = new ProtocolInputData(code);

            FeatureCheckContext context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);

            Assert.IsFalse(result.IsUsed);
            result.FeatureItems.Should().BeEmpty();
        }

        [TestMethod]
        public void CheckIsNotUsed_WithoutRequiredDecimals()
        {
            const string code = "<Protocol><Params><Param><Measurement><Type options='date'/></Measurement><Interprete><Decimals>8</Decimals></Interprete><Display><Decimals>7</Decimals></Display></Param></Params></Protocol>";
            var input = new ProtocolInputData(code);

            FeatureCheckContext context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);

            Assert.IsFalse(result.IsUsed);
            result.FeatureItems.Should().BeEmpty();
        }

        [TestMethod]
        public void CheckIsNotUsed_DateTimeInTableWithoutDisabledHeaderSum()
        {
            const string code = "<Protocol><Params><Param id='1000'><ArrayOptions><ColumnOption idx='0' pid='1001' options=';enableHeaderSum'/></ArrayOptions></Param><Param id='1001'><Measurement><Type options='datetime'/></Measurement><Interprete><Decimals>8</Decimals></Interprete><Display><Decimals>8</Decimals></Display></Param></Params></Protocol>";
            var input = new ProtocolInputData(code);

            FeatureCheckContext context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);

            Assert.IsFalse(result.IsUsed);
            result.FeatureItems.Should().BeEmpty();
        }
    }

}
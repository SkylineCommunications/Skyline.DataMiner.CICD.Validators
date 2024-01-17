namespace Protocol.FeaturesTests.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Common.Testing;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Features;

    //[TestClass]
    public class ExampleTests
    {
        private static ParameterDateTime check;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            check = new ParameterDateTime();
        }

        [TestMethod]
        public void CheckReleaseNotes() /* Optional */
        {
            List<uint> expected = new List<uint> { 6046 };
            check.ReleaseNotes.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void CheckDescription() /* Optional */
        {
            string expected = "The parameter will be displayed as a DateTime. The value represents a decimal number indicating the total number of days that have passed since midnight 1899-12-30. The Interprete/Decimals tag of this parameter needs to be set to 8 to avoid rounding errors.";
            check.Description.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void CheckTitle() /* Optional */
        {
            string expected = "Parameter DateTime";
            check.Title.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void CheckIsUsed()
        {
            const string code = "<Protocol><Params><Param><Measurement><Type options='datetime'/></Measurement></Param></Params></Protocol>";
            var input = new ProtocolInputData(code);

            FeatureCheckContext context = new FeatureCheckContext(input, false);

            var result = check.CheckIfUsed(context);
            var expected = context.Model.Protocol.Params.Select(x => new FeatureCheckResultItem(x));

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void CheckIsUsed_File()
        {
            const string fileName = "FileName.xml";
            var input = ProtocolTestsHelper.GetProtocolInputData(fileName);

            FeatureCheckContext context = new FeatureCheckContext(input, false);

            var result = check.CheckIfUsed(context);
            var expected = context.Model.Protocol.Params.Select(x => new FeatureCheckResultItem(x));

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void CheckIsUsed_CSharp()
        {
            const string fileName = "FileName.xml";
            var input = ProtocolTestsHelper.GetProtocolInputData(fileName);

            FeatureCheckContext context = new FeatureCheckContext(input, false);

            IFeatureCheckResult result = check.CheckIfUsed(context);

            Assert.IsTrue(result.IsUsed);
            var ids = result.FeatureItems.Select(item => ((IQActionsQAction)item).Id.Value);
            var expectedIds = context.Model.Protocol.QActions.Select(qaction => qaction.Id.Value);
            ids.Should().BeEquivalentTo(expectedIds);
        }
    }
}
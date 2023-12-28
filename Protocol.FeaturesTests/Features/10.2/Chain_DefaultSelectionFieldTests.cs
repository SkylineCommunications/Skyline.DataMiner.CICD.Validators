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
    public class Chain_DefaultSelectionFieldTests
    {
        private static Chain_DefaultSelectionField check;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            check = new Chain_DefaultSelectionField();
        }

        [TestMethod]
        public void CheckIsUsed()
        {
            const string code = "<Protocol><Chains><Chain defaultSelectionField='MyFieldName'></Chain></Chains></Protocol>";
            var input = new ProtocolInputData(code);

            FeatureCheckContext context = new FeatureCheckContext(input);

            var result = check.CheckIfUsed(context);
            var expected = context.Model.Protocol.Chains.Select(x => new FeatureCheckResultItem(x));

            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }
    }
}
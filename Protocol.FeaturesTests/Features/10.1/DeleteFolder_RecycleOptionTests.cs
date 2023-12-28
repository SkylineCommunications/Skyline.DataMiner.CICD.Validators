namespace SLDisDmFeatureCheckUnitTests.Features
{
    using System.Linq;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SLDisDmFeatureCheck.Common;
    using SLDisDmFeatureCheck.Common.Interfaces;
    using SLDisDmFeatureCheck.Common.Results;
    using SLDisDmFeatureCheck.Features;
    
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using SLDisUnitTestsShared;

    [TestClass]
    public class DeleteFolder_RecycleOptionTests
    {
        private static DeleteFolder_RecycleOption check;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            check = new DeleteFolder_RecycleOption();
        }
        
        [TestMethod]
        public void CheckIsUsed_CSharp()
        {
            const string fileName = "DeleteFolder_RecycleOptionTests.xml";
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
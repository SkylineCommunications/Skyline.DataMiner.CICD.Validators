#if NET // For some reason, this test is not working in .NET Framework...
namespace Protocol.FeaturesTests.Features._10._2
{
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Testing;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Features;

    [TestClass]
    public class DotNetFrameworkTests
    {
        private static DotNetFramework check;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            check = new DotNetFramework();
        }

        [TestMethod]
        public async Task CheckIsUsed_Solution_Framework462()
        {
            // Arrange
            var input = await ProtocolTestsHelper.GetProtocolInputDataFromSolutionAsync(@"Test Files\Solutions\SolutionFramework462\SolutionFramework462.sln");

            FeatureCheckContext context = new FeatureCheckContext(input);

            // Act
            var result = check.CheckIfUsed(context);

            // Assert
            Assert.IsFalse(result.IsUsed);
            result.FeatureItems.Should().BeEmpty();
        }

        [TestMethod]
        public async Task CheckIsUsed_Solution_Framework462_Legacy()
        {
            // Arrange
            var input = await ProtocolTestsHelper.GetProtocolInputDataFromSolutionAsync(@"Test Files\Solutions\SolutionFramework462_Legacy\SolutionFramework462.sln");

            FeatureCheckContext context = new FeatureCheckContext(input);

            // Act
            var result = check.CheckIfUsed(context);

            // Assert
            Assert.IsFalse(result.IsUsed);
            result.FeatureItems.Should().BeEmpty();
        }

        [TestMethod]
        public async Task CheckIsUsed_Solution_OtherFramework()
        {
            // Arrange
            var input = await ProtocolTestsHelper.GetProtocolInputDataFromSolutionAsync(@"Test Files\Solutions\SolutionFrameworkOther\SolutionFrameworkOther.sln");
            var expected = input.Model.Protocol.QActions.Select(qAction => new FeatureCheckResultItem(qAction)).ToList();

            FeatureCheckContext context = new FeatureCheckContext(input);

            // Act
            var result = check.CheckIfUsed(context);

            // Assert
            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task CheckIsUsed_Solution_OtherFramework_Legacy()
        {
            // Arrange
            var input = await ProtocolTestsHelper.GetProtocolInputDataFromSolutionAsync(@"Test Files\Solutions\SolutionFrameworkOther_Legacy\SolutionFrameworkOther.sln");
            var expected = input.Model.Protocol.QActions.Select(qAction => new FeatureCheckResultItem(qAction)).ToList();

            FeatureCheckContext context = new FeatureCheckContext(input);

            // Act
            var result = check.CheckIfUsed(context);

            // Assert
            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }
    }
}
#endif
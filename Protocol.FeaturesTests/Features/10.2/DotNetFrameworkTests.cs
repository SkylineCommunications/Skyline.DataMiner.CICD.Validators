namespace Protocol.FeaturesTests.Features._10._2
{
    using System.Linq;

    using Common.Testing;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Results;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Features;

#if NET8_0 // Times out when running in .NET Framework test runner. No clue why, but not worth investigating right now.
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
        public void CheckIsUsed_Solution_Framework462()
        {
            // Arrange
            var input = ProtocolTestsHelper.GetProtocolInputDataFromSolution(@"Test Files\Solutions\SolutionFramework462\SolutionFramework462.sln");

            FeatureCheckContext context = new FeatureCheckContext(input);

            // Act
            var result = check.CheckIfUsed(context);

            // Assert
            Assert.IsFalse(result.IsUsed);
            result.FeatureItems.Should().BeEmpty();
        }

        [TestMethod]
        public void CheckIsUsed_Solution_Framework462_Legacy()
        {
            // Arrange
            var input = ProtocolTestsHelper.GetProtocolInputDataFromSolution(@"Test Files\Solutions\SolutionFramework462_Legacy\SolutionFramework462.sln");

            FeatureCheckContext context = new FeatureCheckContext(input);

            // Act
            var result = check.CheckIfUsed(context);

            // Assert
            Assert.IsFalse(result.IsUsed);
            result.FeatureItems.Should().BeEmpty();
        }

        [TestMethod]
        public void CheckIsUsed_Solution_OtherFramework()
        {
            // Arrange
            var input = ProtocolTestsHelper.GetProtocolInputDataFromSolution(@"Test Files\Solutions\SolutionFrameworkOther\SolutionFrameworkOther.sln");
            var expected = input.Model.Protocol.QActions.Select(qAction => new FeatureCheckResultItem(qAction)).ToList();

            FeatureCheckContext context = new FeatureCheckContext(input);

            // Act
            var result = check.CheckIfUsed(context);

            // Assert
            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void CheckIsUsed_Solution_OtherFramework_Legacy()
        {
            // Arrange
            var input = ProtocolTestsHelper.GetProtocolInputDataFromSolution(@"Test Files\Solutions\SolutionFrameworkOther_Legacy\SolutionFrameworkOther.sln");
            var expected = input.Model.Protocol.QActions.Select(qAction => new FeatureCheckResultItem(qAction)).ToList();

            FeatureCheckContext context = new FeatureCheckContext(input);

            // Act
            var result = check.CheckIfUsed(context);

            // Assert
            Assert.IsTrue(result.IsUsed);
            result.FeatureItems.Should().BeEquivalentTo(expected);
        }
    }
#endif
}
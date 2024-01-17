namespace ProtocolTests.CommonTests
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [TestClass]
    public class TestCollectorTests
    {
        [TestMethod]
        public void GetAllValidateTests_Valid()
        {
            // Act
            TestCollection<IValidate> tests = TestCollector.GetAllValidateTests();

            // Assert
            tests.Should().NotBeNull();
            tests.Tests.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void GetAllCompareTests_Valid()
        {
            // Act
            TestCollection<ICompare> tests = TestCollector.GetAllCompareTests();

            // Assert
            tests.Should().NotBeNull();
            tests.Tests.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void GetAllTests_NoDuplicates()
        {
            IReadOnlyList<(IValidate Test, TestAttribute Attribute)> tests = TestCollector.GetAllValidateTests().Tests;

            HashSet<(Category, uint)> testKeys = new HashSet<(Category, uint)>();

            foreach ((IValidate test, TestAttribute attribute) in tests)
            {
                string testName = test.GetType().FullName;

                if (testKeys.Add((attribute.Category, attribute.CheckId)))
                {
                    continue;
                }

                string testId = String.Join(".", (int)attribute.Category, attribute.CheckId);
                Assert.Fail($"Duplicate validator test: {attribute.Category}/{testName} (ID {testId})");
            }
        }
    }
}
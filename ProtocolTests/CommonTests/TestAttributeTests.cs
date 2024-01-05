namespace ProtocolTests.CommonTests
{
    using System;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Enums;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckProtocolTag;

    using CheckId = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckProtocolTag.CheckId;


    [TestClass]
    public class TestAttributeTests
    {
        #region GetAttribute

        [TestMethod]
        public void GetAttribute_CheckProtocolTag_Valid()
        {
            IValidate test = new CheckProtocolTag();

            TestAttribute attr = TestAttribute.GetAttribute(test);

            attr.Should().BeAssignableTo(typeof(TestAttribute));
            attr.Category.Should().BeEquivalentTo(Category.Protocol);
            attr.CheckId.Should().Be(CheckId.CheckProtocolTag);
        }

        [TestMethod]
        public void GetAttribute_CheckProtocolTag_ThrowsArgumentNullException()
        {
            Type type = null;
            Assert.ThrowsException<ArgumentNullException>(() => TestAttribute.GetAttribute(type));
        }

        [TestMethod]
        public void GetAttribute_RandomClass_Null()
        {
            TestAttribute attr = TestAttribute.GetAttribute(typeof(TestAttributeTests));

            attr.Should().BeNull();
        }

        #endregion

        #region Constructor

        [TestMethod]
        public void Constructor_Valid()
        {
            TestAttribute attr = new TestAttribute(5, Category.Protocol);

            Assert.AreEqual((uint)5, attr.CheckId);
            Assert.AreEqual(Category.Protocol, attr.Category);
            Assert.AreEqual(TestOrder.Mid, attr.TestOrder);
        }

        [TestMethod]
        public void Constructor_Valid2()
        {
            TestAttribute attr = new TestAttribute(5, Category.Param, TestOrder.Post1);

            Assert.AreEqual((uint)5, attr.CheckId);
            Assert.AreEqual(Category.Param, attr.Category);
            Assert.AreEqual(TestOrder.Post1, attr.TestOrder);
        }

        #endregion
    }
}
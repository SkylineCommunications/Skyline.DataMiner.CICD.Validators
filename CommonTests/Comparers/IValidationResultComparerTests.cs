namespace CommonTests.Comparers
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Comparers;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    [TestClass]
    public class IValidationResultComparerTests
    {
        private static readonly IValidationResultComparer _comparer = new IValidationResultComparer();

        #region Equal

        [TestMethod]
        public void IValidationResultComparer_Equal_Equal()
        {
            var (result1, result2) = GetResults();

            var equal = _comparer.Equals(result1, result2);

            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void IValidationResultComparer_Equal_NonEqualSeverity()
        {
            var (result1, result2) = GetResults(withDifferentSeverity: true);

            var equal = _comparer.Equals(result1, result2);

            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void IValidationResultComparer_Equal_NonEqualCategory()
        {
            var (result1, result2) = GetResults(withDifferentCategory: true);

            var equal = _comparer.Equals(result1, result2);

            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void IValidationResultComparer_Equal_NonEqualFullId()
        {
            var (result1, result2) = GetResults(withDifferentFullId: true);

            var equal = _comparer.Equals(result1, result2);

            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void IValidationResultComparer_Equal_NonEqualDescription()
        {
            var (result1, result2) = GetResults(withDifferentDescription: true);

            var equal = _comparer.Equals(result1, result2);

            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void IValidationResultComparer_Equal_NonEqualNode()
        {
            var (result1, result2) = GetResults(withDifferentNode: true);

            var equal = _comparer.Equals(result1, result2);

            Assert.IsFalse(equal);
        }

        #endregion

        #region HashCode

        [TestMethod]
        public void IValidationResultComparer_GetHashCode_Equal()
        {
            var (result1, result2) = GetResults();

            Assert.AreEqual(_comparer.GetHashCode(result1), _comparer.GetHashCode(result2));
        }

        [TestMethod]
        public void IValidationResultComparer_GetHashCode_NonEqualSeverity()
        {
            var (result1, result2) = GetResults(withDifferentSeverity: true);

            Assert.AreNotEqual(_comparer.GetHashCode(result1), _comparer.GetHashCode(result2));
        }

        [TestMethod]
        public void IValidationResultComparer_GetHashCode_NonEqualCategory()
        {
            var (result1, result2) = GetResults(withDifferentCategory: true);

            Assert.AreNotEqual(_comparer.GetHashCode(result1), _comparer.GetHashCode(result2));
        }

        [TestMethod]
        public void IValidationResultComparer_GetHashCode_NonEqualFullId()
        {
            var (result1, result2) = GetResults(withDifferentFullId: true);

            Assert.AreNotEqual(_comparer.GetHashCode(result1), _comparer.GetHashCode(result2));
        }

        [TestMethod]
        public void IValidationResultComparer_GetHashCode_NonEqualDescription()
        {
            var (result1, result2) = GetResults(withDifferentDescription: true);

            Assert.AreNotEqual(_comparer.GetHashCode(result1), _comparer.GetHashCode(result2));
        }

        [TestMethod]
        public void IValidationResultComparer_GetHashCode_NonEqualNode()
        {
            var (result1, result2) = GetResults(withDifferentNode: true);

            Assert.AreNotEqual(_comparer.GetHashCode(result1), _comparer.GetHashCode(result2));
        }

        #endregion

        private (IValidationResult, IValidationResult) GetResults(
            bool withDifferentSeverity = false,
            bool withDifferentCategory = false,
            bool withDifferentFullId = false,
            bool withDifferentDescription = false,
            bool withDifferentNode = false)
        {
            var xml = "<Protocol><Params><Param id=\"1\"><Name>Param1</Name></Param><Param id=\"2\"><Name>Param1</Name></Param></Params></Protocol>";

            // parse two times to make sure we have different objects
            var model1 = new ProtocolModel(xml);
            var param1 = model1.Protocol.Params[0];

            xml = Environment.NewLine + xml; // to simulate shifted positions
            var model2 = new ProtocolModel(xml);
            var param2_1 = model2.Protocol.Params[0];
            var param2_2 = model2.Protocol.Params[1];

            var result1 = new TestValidationResult
            {
                Severity = Severity.Critical,
                Category = Category.Param,
                FullId = "1.1.1",
                Description = "Test description",
                ReferenceNode = param1,
                PositionNode = param1.Name,
            };

            var result2 = new TestValidationResult
            {
                Severity = withDifferentSeverity ? Severity.Major : Severity.Critical,
                Category = withDifferentCategory ? Category.Timer : Category.Param,
                FullId = withDifferentFullId ? "1.1.2" : "1.1.1",
                Description = withDifferentDescription ? "Other description" : "Test description",
                ReferenceNode = withDifferentNode ? param2_2 : param2_1,
                PositionNode = withDifferentNode ? param2_2.Name : param2_1.Name,
            };

            return (result1, result2);
        }

        private class TestValidationResult : IValidationResult
        {
            public List<IValidationResult> SubResults { get; set; }
            public uint CheckId { get; set; }
            public uint ErrorId { get; set; }
            public string FullId { get; set; }
            public Category Category { get; set; }
            public Severity Severity { get; set; }
            public Certainty Certainty { get; set; }
            public Source Source { get; set; }
            public FixImpact FixImpact { get; set; }
            public string GroupDescription { get; set; }
            public string Description { get; set; }
            public string HowToFix { get; set; }
            [Obsolete("Has been moved to the dedicated markdown file about the error. Will be removed in the next major change update.")]
            public string ExampleCode { get; set; }
            [Obsolete("Has been moved to the dedicated markdown file about the error. Will be removed in the next major change update.")]
            public string Details { get; set; }
            public IReadable ReferenceNode { get; set; }
            public bool HasCodeFix { get; set; }
            public int Position { get; set; }
            public IReadable PositionNode { get; set; }
            public List<(string Message, bool AutoFixPopup)> AutoFixWarnings { get; set; }
            public (int TablePid, string Name)? DveExport { get; set; }
            public int Line { get; set; }
            public string DescriptionFormat { get; set; }
            public object[] DescriptionParameters { get; set; }
        }
    }
}
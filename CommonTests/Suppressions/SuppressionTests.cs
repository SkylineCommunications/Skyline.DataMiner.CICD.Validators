namespace CommonTests.Suppressions
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Parsers.Common.Xml;
    using Skyline.DataMiner.CICD.Validators.Common.Suppressions;

    [TestClass]
    public class SuppressionTests
    {
        [TestMethod]
        public void Suppression_GetAllSuppressions_Normal()
        {
            string text = @"
<a>
    <!-- SuppressValidator 1.2.3 my reason -->
    <b></b>
    <!-- /SuppressValidator 1.2.3 -->
</a>
";

            var document = new Parser(text).Document;
            var suppressions = CommentSuppression.GetAllSuppressions(document).ToList();

            Assert.AreEqual(1, suppressions.Count);

            var suppression = suppressions[0];
            Assert.AreEqual(document, suppression.Document);
            Assert.AreEqual(SuppressionType.Normal, suppression.Type);
            Assert.AreEqual("1.2.3", suppression.Code);
            Assert.AreEqual("my reason", (suppression.StartToken as NormalValidatorSuppressionToken).Reason);

            Assert.AreEqual(text.IndexOf("<!-- SuppressValidator"), suppression.Start);
            Assert.AreEqual(text.IndexOf("<!-- /SuppressValidator"), suppression.End);
        }

        [TestMethod]
        public void Suppression_GetAllSuppressions_CloseInside()
        {
            string text = @"
<a>
    <!-- SuppressValidator 1.2.3 my reason -->
    <b>
        <!-- /SuppressValidator 1.2.3 -->
    </b>
</a>
";

            var document = new Parser(text).Document;
            var suppressions = CommentSuppression.GetAllSuppressions(document).ToList();

            Assert.AreEqual(1, suppressions.Count);

            var suppression = suppressions[0];
            Assert.AreEqual(document, suppression.Document);
            Assert.AreEqual(SuppressionType.Normal, suppression.Type);
            Assert.AreEqual("1.2.3", suppression.Code);
            Assert.AreEqual("my reason", (suppression.StartToken as NormalValidatorSuppressionToken).Reason);

            Assert.AreEqual(text.IndexOf("<!-- SuppressValidator"), suppression.Start);
            Assert.AreEqual(text.IndexOf("<!-- /SuppressValidator"), suppression.End);
        }

        [TestMethod]
        public void Suppression_GetAllSuppressions_NoOpen()
        {
            string text = @"
<a>
    <b></b>
    <!-- /SuppressValidator 1.2.3 -->
</a>
";

            var document = new Parser(text).Document;
            var suppressions = CommentSuppression.GetAllSuppressions(document).ToList();

            Assert.AreEqual(0, suppressions.Count);
        }

        [TestMethod]
        public void Suppression_GetAllSuppressions_NoClose()
        {
            string text = @"
<a>
    <!-- SuppressValidator 1.2.3 my reason -->
    <b></b>
    <c></c>
</a>
";

            var document = new Parser(text).Document;
            var suppressions = CommentSuppression.GetAllSuppressions(document).ToList();

            Assert.AreEqual(1, suppressions.Count);

            var suppression = suppressions[0];
            Assert.AreEqual(document, suppression.Document);
            Assert.AreEqual(SuppressionType.Normal, suppression.Type);
            Assert.AreEqual("1.2.3", suppression.Code);
            Assert.AreEqual("my reason", (suppression.StartToken as NormalValidatorSuppressionToken).Reason);

            Assert.AreEqual(text.IndexOf("<!-- SuppressValidator"), suppression.Start);
            Assert.AreEqual(text.IndexOf("</b>") + 4, suppression.End);
        }

        [TestMethod]
        public void Suppression_GetAllSuppressions_NoCloseWithOpen()
        {
            string text = @"
<a>
    <!-- SuppressValidator 1.2.3 my reason -->
    <b></b>
    <c></c>
    <!-- SuppressValidator 1.2.3 my reason 2 -->
    <d></d>
</a>
";

            var document = new Parser(text).Document;
            var suppressions = CommentSuppression.GetAllSuppressions(document).ToList();

            Assert.AreEqual(2, suppressions.Count);

            var suppression1 = suppressions[0];
            Assert.AreEqual(document, suppression1.Document);
            Assert.AreEqual(SuppressionType.Normal, suppression1.Type);
            Assert.AreEqual("1.2.3", suppression1.Code);
            Assert.AreEqual("my reason 2", (suppression1.StartToken as NormalValidatorSuppressionToken).Reason);
            Assert.AreEqual(text.IndexOf("<!-- SuppressValidator 1.2.3 my reason 2 -->"), suppression1.Start);
            Assert.AreEqual(text.IndexOf("</d>") + 4, suppression1.End);

            var suppression2 = suppressions[1];
            Assert.AreEqual(document, suppression2.Document);
            Assert.AreEqual(SuppressionType.Normal, suppression2.Type);
            Assert.AreEqual("1.2.3", suppression2.Code);
            Assert.AreEqual("my reason", (suppression2.StartToken as NormalValidatorSuppressionToken).Reason);
            Assert.AreEqual(text.IndexOf("<!-- SuppressValidator 1.2.3 my reason -->"), suppression2.Start);
            Assert.AreEqual(text.IndexOf("</b>") + 4, suppression2.End);
        }

        [TestMethod]
        public void Suppression_GetAllSuppressions_Nested()
        {
            string text = @"
<a>
    <!-- SuppressValidator 1.2.3 my reason -->
    <b>
        <!-- SuppressValidator 1.2.3 my reason 2 -->
        <c></c>
        <!-- /SuppressValidator 1.2.3 -->
    </b>
    <!-- /SuppressValidator 1.2.3 -->
</a>
";

            var document = new Parser(text).Document;
            var suppressions = CommentSuppression.GetAllSuppressions(document).ToList();

            Assert.AreEqual(2, suppressions.Count);

            var suppression1 = suppressions[0];
            Assert.AreEqual(document, suppression1.Document);
            Assert.AreEqual(SuppressionType.Normal, suppression1.Type);
            Assert.AreEqual("1.2.3", suppression1.Code);
            Assert.AreEqual("my reason 2", (suppression1.StartToken as NormalValidatorSuppressionToken).Reason);
            Assert.AreEqual(text.IndexOf("<!-- SuppressValidator 1.2.3 my reason 2 -->"), suppression1.Start);
            Assert.AreEqual(text.IndexOf("</c>") + 14, suppression1.End);

            var suppression2 = suppressions[1];
            Assert.AreEqual(document, suppression2.Document);
            Assert.AreEqual(SuppressionType.Normal, suppression2.Type);
            Assert.AreEqual("1.2.3", suppression2.Code);
            Assert.AreEqual("my reason", (suppression2.StartToken as NormalValidatorSuppressionToken).Reason);
            Assert.AreEqual(text.IndexOf("<!-- SuppressValidator 1.2.3 my reason -->"), suppression2.Start);
            Assert.AreEqual(text.IndexOf("</b>") + 10, suppression2.End);
        }

        [TestMethod]
        public void Suppression_GetAllPostpones_Normal()
        {
            string text = @"
<a>
    <!-- PostponeValidator 1.2.3 DCP12345 my reason -->
    <b></b>
    <!-- /PostponeValidator 1.2.3 -->
</a>
";

            var document = new Parser(text).Document;
            var suppressions = CommentSuppression.GetAllSuppressions(document).ToList();

            Assert.AreEqual(1, suppressions.Count);

            var suppression = suppressions[0];
            Assert.AreEqual(document, suppression.Document);
            Assert.AreEqual(SuppressionType.Postpone, suppression.Type);
            Assert.AreEqual("1.2.3", suppression.Code);
            Assert.AreEqual("my reason", (suppression.StartToken as PostponeValidatorSuppressionToken).Reason);
            Assert.AreEqual(12345, (suppression.StartToken as PostponeValidatorSuppressionToken).TaskId);

            Assert.AreEqual(text.IndexOf("<!-- PostponeValidator"), suppression.Start);
            Assert.AreEqual(text.IndexOf("<!-- /PostponeValidator"), suppression.End);
        }

        [TestMethod]
        public void Suppression_GetAllMixed_Normal()
        {
            string text = @"
<a>
    <!-- SuppressValidator 1.2.3 my reason -->
    <b></b>
    <!-- /SuppressValidator 1.2.3 -->
    <!-- PostponeValidator 1.2.4 DCP12345 my reason2 -->
    <c></c>
    <!-- /PostponeValidator 1.2.4 -->
</a>
";

            var document = new Parser(text).Document;
            var suppressions = CommentSuppression.GetAllSuppressions(document).ToList();

            Assert.AreEqual(2, suppressions.Count);

            var suppression1 = suppressions[0];
            Assert.AreEqual(document, suppression1.Document);
            Assert.AreEqual(SuppressionType.Normal, suppression1.Type);
            Assert.AreEqual("1.2.3", suppression1.Code);
            Assert.AreEqual("my reason", (suppression1.StartToken as NormalValidatorSuppressionToken).Reason);
            Assert.AreEqual(text.IndexOf("<!-- SuppressValidator"), suppression1.Start);
            Assert.AreEqual(text.IndexOf("<!-- /SuppressValidator"), suppression1.End);

            var suppression2 = suppressions[1];
            Assert.AreEqual(document, suppression2.Document);
            Assert.AreEqual(SuppressionType.Postpone, suppression2.Type);
            Assert.AreEqual("1.2.4", suppression2.Code);
            Assert.AreEqual("my reason2", (suppression2.StartToken as PostponeValidatorSuppressionToken).Reason);
            Assert.AreEqual(12345, (suppression2.StartToken as PostponeValidatorSuppressionToken).TaskId);
            Assert.AreEqual(text.IndexOf("<!-- PostponeValidator"), suppression2.Start);
            Assert.AreEqual(text.IndexOf("<!-- /PostponeValidator"), suppression2.End);
        }
    }
}

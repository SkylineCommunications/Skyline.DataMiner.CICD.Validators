namespace CommonTests.Suppressions
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Parsers.Common.Xml;
    using Skyline.DataMiner.CICD.Validators.Common.Suppressions;

    [TestClass]
    public class SuppressionTokenTests
    {
        [TestMethod]
        public void SuppressionToken_TryParse_OpenSuppress()
        {
            string text = @"<!-- SuppressValidator 1.2.3 my reason -->";

            var document = XmlDocument.Parse(text);
            var comment = document.Children[0] as XmlComment;
            bool result = SuppressionToken.DetectToken(document, comment, out var token);

            Assert.IsTrue(result);

            if (token is NormalValidatorSuppressionToken t1)
            {
                Assert.AreEqual(document, t1.Document);
                Assert.AreEqual(SuppressionType.Normal, t1.Type);
                Assert.AreEqual(false, t1.IsClose);
                Assert.AreEqual("1.2.3", t1.Code);
                Assert.AreEqual("my reason", t1.Reason);
            }
            else
            {
                Assert.Fail("token is not of type " + nameof(NormalValidatorSuppressionToken));
            }
        }

        [TestMethod]
        public void SuppressionToken_TryParse_OpenPostpone()
        {
            string text = @"<!-- PostponeValidator 1.2.3 DCP12345 my reason -->";

            var document = XmlDocument.Parse(text);
            var comment = document.Children[0] as XmlComment;
            bool result = SuppressionToken.DetectToken(document, comment, out var token);

            Assert.IsTrue(result);

            if (token is PostponeValidatorSuppressionToken t1)
            {
                Assert.AreEqual(document, t1.Document);
                Assert.AreEqual(SuppressionType.Postpone, t1.Type);
                Assert.AreEqual(false, t1.IsClose);
                Assert.AreEqual("1.2.3", t1.Code);
                Assert.AreEqual(12345, t1.TaskId);
                Assert.AreEqual("my reason", t1.Reason);
            }
            else
            {
                Assert.Fail("token is not of type " + nameof(PostponeValidatorSuppressionToken));
            }
        }

        [TestMethod]
        public void SuppressionToken_TryParse_MultilineReason()
        {
            string text = "<!-- SuppressValidator 1.2.3 line 1\r\nline 2 -->";

            var document = XmlDocument.Parse(text);
            var comment = document.Children[0] as XmlComment;
            bool result = SuppressionToken.DetectToken(document, comment, out var token);

            Assert.IsTrue(result);
            Assert.AreEqual(SuppressionType.Normal, token.Type);
            Assert.AreEqual(false, token.IsClose);
            Assert.AreEqual("1.2.3", token.Code);

            Assert.IsInstanceOfType(token, typeof(NormalValidatorSuppressionToken));
            Assert.AreEqual("line 1\r\nline 2", ((NormalValidatorSuppressionToken)token).Reason);
        }

        [TestMethod]
        public void SuppressionToken_TryParse_Close1()
        {
            string text = @"<!-- /SuppressValidator 1.2.3 -->";

            var document = XmlDocument.Parse(text);
            var comment = document.Children[0] as XmlComment;
            bool result = SuppressionToken.DetectToken(document, comment, out var token);

            Assert.IsTrue(result);
            Assert.AreEqual(SuppressionType.Normal, token.Type);
            Assert.AreEqual(true, token.IsClose);
            Assert.AreEqual("1.2.3", token.Code);
        }

        [TestMethod]
        public void SuppressionToken_TryParse_Close2()
        {
            string text = @"<!-- \SuppressValidator 1.2.3 -->";

            var document = XmlDocument.Parse(text);
            var comment = document.Children[0] as XmlComment;
            bool result = SuppressionToken.DetectToken(document, comment, out var token);

            Assert.IsTrue(result);
            Assert.AreEqual(SuppressionType.Normal, token.Type);
            Assert.AreEqual(true, token.IsClose);
            Assert.AreEqual("1.2.3", token.Code);
        }

        [TestMethod]
        public void SuppressionToken_TryParse_Unsuccessfull1()
        {
            string text = @"<!-- something else -->";

            var document = XmlDocument.Parse(text);
            var comment = document.Children[0] as XmlComment;
            bool result = SuppressionToken.DetectToken(document, comment, out var token);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SuppressionToken_TryParse_Unsuccessfull2()
        {
            string text = @"<!-- \\\\SuppressValidator 1.2.3 -->";

            var document = XmlDocument.Parse(text);
            var comment = document.Children[0] as XmlComment;
            bool result = SuppressionToken.DetectToken(document, comment, out var token);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SuppressionToken_GetAllTokens()
        {
            string text = @"
<!-- SuppressValidator 1.2.3 my reason -->
<a></a>
<!-- /SuppressValidator 1.2.3 -->
";

            var document = XmlDocument.Parse(text);
            var tokens = SuppressionToken.GetAllTokens(document).OrderBy(t => t.Position).ToList();

            Assert.AreEqual(2, tokens.Count);

            var tokenOpen = tokens[0];
            if (tokenOpen is NormalValidatorSuppressionToken t1)
            {
                Assert.AreEqual(document, t1.Document);
                Assert.AreEqual(SuppressionType.Normal, t1.Type);
                Assert.AreEqual(false, t1.IsClose);
                Assert.AreEqual("1.2.3", t1.Code);
                Assert.AreEqual("my reason", t1.Reason);
            }
            else
            {
                Assert.Fail("token is not of type " + nameof(NormalValidatorSuppressionToken));
            }

            var tokenClose = tokens[1];
            if (tokenClose is NormalValidatorSuppressionToken t2)
            {
                Assert.AreEqual(document, t2.Document);
                Assert.AreEqual(SuppressionType.Normal, t2.Type);
                Assert.AreEqual(true, t2.IsClose);
                Assert.AreEqual("1.2.3", t2.Code);
                Assert.AreEqual("", t2.Reason);
            }
            else
            {
                Assert.Fail("token is not of type " + nameof(NormalValidatorSuppressionToken));
            }
        }
    }
}
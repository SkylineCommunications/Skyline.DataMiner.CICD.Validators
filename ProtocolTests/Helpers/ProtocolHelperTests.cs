namespace ProtocolTests.Helpers
{
    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Protocol.Tests;

    [TestClass]
    public class ProtocolHelperTests
    {
        [TestMethod]
        [DoNotParallelize]
        [DataRow("ABC<ABC", "ABC_ABC")]
        [DataRow("ABC>ABC", "ABC_ABC")]
        [DataRow("ABC:ABC", "ABC_ABC")]
        [DataRow("ABC\"ABC", "ABC_ABC")]
        [DataRow("ABC/ABC", "ABC_ABC")]
        [DataRow("ABC\\ABC", "ABC_ABC")]
        [DataRow("ABC|ABC", "ABC_ABC")]
        [DataRow("ABC?ABC", "ABC_ABC")]
        [DataRow("ABC*ABC", "ABC_ABC")]
        [DataRow("ABC;ABC", "ABC_ABC")]
        [DataRow("ABC°ABC", "ABC_ABC")]
        [DataRow("ABCABC", "ABCABC")]
        public void CheckReplaceProtocolNameInvalidChars_DefaultChar(string text, string newText)
        {
            string protocolName = ProtocolHelper.ReplaceProtocolNameInvalidChars(text);

            protocolName.Should().BeEquivalentTo(newText);
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow("ABC<ABC", "ABC-ABC")]
        [DataRow("ABC>ABC", "ABC-ABC")]
        [DataRow("ABC:ABC", "ABC-ABC")]
        [DataRow("ABC\"ABC", "ABC-ABC")]
        [DataRow("ABC/ABC", "ABC-ABC")]
        [DataRow("ABC\\ABC", "ABC-ABC")]
        [DataRow("ABC|ABC", "ABC-ABC")]
        [DataRow("ABC?ABC", "ABC-ABC")]
        [DataRow("ABC*ABC", "ABC-ABC")]
        [DataRow("ABC;ABC", "ABC-ABC")]
        [DataRow("ABC°ABC", "ABC-ABC")]
        [DataRow("ABCABC", "ABCABC")]
        public void CheckReplaceProtocolNameInvalidChars_CustomChar(string text, string newText)
        {
            string protocolName = ProtocolHelper.ReplaceProtocolNameInvalidChars(text, "-");

            protocolName.Should().BeEquivalentTo(newText);
        }
    }
}
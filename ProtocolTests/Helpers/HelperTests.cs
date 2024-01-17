namespace ProtocolTests.Helpers
{
    using System.Linq;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Protocol.Tests;

    [TestClass]
    public class HelperTests
    {
        [TestMethod]
        [DoNotParallelize]
        [DataRow("ABC:ABC")]
        [DataRow("ABC-ABC")]
        [DataRow("ABCABC")]
        public void CheckParamNameInvalidChars(string text)
        {
            // ToArray() is required to make the fluentAssertions thread safe when having multiple DataRows
            char[] invalidCharacters = Helper.CheckInvalidChars(text, ParamHelper.RestrictedParamNameChars).ToArray();

            invalidCharacters.Should().BeEmpty();
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow("ABC<ABC", '<')]
        [DataRow("ABC>ABC", '>')]
        [DataRow("ABC:ABC", ':')]
        [DataRow("ABC\"ABC", '"')]
        [DataRow("ABC/ABC", '/')]
        [DataRow("ABC\\ABC", '\\')]
        [DataRow("ABC|ABC", '|')]
        [DataRow("ABC?ABC", '?')]
        [DataRow("ABC*ABC", '*')]
        [DataRow("ABC;ABC", ';')]
        [DataRow("ABC°ABC", '°')]
        [DataRow("ABCABC", null)]
        public void CheckInvalidChars_ProtocolName(string text, char? invalidChar)
        {
            // ToArray() is required to make the fluentAssertions thread safe when having multiple DataRows
            char[] invalidCharacters = Helper.CheckInvalidChars(text, ProtocolHelper.RestrictedProtocolNameChars).ToArray();

            if (invalidChar == null)
            {
                invalidCharacters.Should().BeEmpty();
            }
            else
            {
                invalidCharacters.Should().ContainSingle().Which.Should().BeEquivalentTo(invalidChar);
            }
        }
    }
}
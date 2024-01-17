namespace ProtocolTests.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Protocol.Tests;

    [TestClass]
    public class ParamHelperTests
    {
        [TestMethod]
        [DoNotParallelize]
        [DataRow("ABC<ABC", new[] { '<' })]
        [DataRow("ABC>ABC", new[] { '>' })]
        [DataRow("ABC:ABC", new[] { ':' })]
        [DataRow("ABC-ABC", new[] { '-' })]
        [DataRow("ABC\"ABC", new[] { '"' })]
        [DataRow("ABC/ABC", new[] { '/' })]
        [DataRow("ABC\\ABC", new[] { '\\' })]
        [DataRow("ABC|ABC", new[] { '|' })]
        [DataRow("ABC?ABC", new[] { '?' })]
        [DataRow("ABC*ABC", new[] { '*' })]
        [DataRow("ABC;ABC", new[] { ';' })]
        [DataRow("ABC°ABC", new[] { '°' })]
        [DataRow("A<B>C*ABC", new[] { '<', '>', '*' })]
        [DataRow("ABCABC", null)]
        [DataRow("ABC_ABC", null)]
        public void CheckParamNameUnrecommendedChars(string text, IReadOnlyList<char> invalidChars)
        {
            // ToArray() is required to make the fluentAssertions thread safe when having multiple DataRows
            object[] unrecommendedCharacters = ParamHelper.GetParamNameUnrecommendedChars(text).ToArray();

            if (invalidChars == null)
            {
                unrecommendedCharacters.Should().BeEmpty();
            }
            else
            {
                unrecommendedCharacters.Should().BeEquivalentTo(invalidChars);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow("ABC ABC", "[Whitespace]")]
        public void CheckParamNameUnrecommendedChars_SpecialCases(string text, string invalidText)
        {
            // ToArray() is required to make the fluentAssertions thread safe when having multiple DataRows
            object[] unrecommendedCharacters = ParamHelper.GetParamNameUnrecommendedChars(text).ToArray();

            unrecommendedCharacters.Should().ContainSingle().Which.Should().BeEquivalentTo(invalidText);
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow("ABC<ABC", "ABC_ABC")]
        [DataRow("ABC>ABC", "ABC_ABC")]
        [DataRow("ABC:ABC", "ABC_ABC")]
        [DataRow("ABC-ABC", "ABC_ABC")]
        [DataRow("ABCABC", "ABCABC")]
        [DataRow("ABC ABC", "ABC ABC")]
        [DataRow("ABC_ABC", "ABC_ABC")]
        public void CheckReplaceParamNameInvalidChars(string text, string newText)
        {
            string paramName = ParamHelper.ReplaceParamNameInvalidChars(text);

            paramName.Should().BeEquivalentTo(newText);
        }
    }
}
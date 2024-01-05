namespace ProtocolTests.Helpers
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests;

    [TestClass]
    public class TitleCasingTests
    {
        private static IEnumerable<object[]> ReusableTestData => new List<object[]>
        {
            new []{ "",   ""},

            new []{ "A",          "A"},
            new []{ "Oneword",    "Oneword"},
            new []{ "oneword",    "Oneword"},

            // For single letters, we allow all casing
            new []{ "i",          "i"},     // ex: 1080i (Interlaced)
            new []{ "I",          "I"},     // ex: Item I
            new []{ "p",          "p"},     // ex: 1080p (Progressive)
            new []{ "P",          "P"},     // ex: Item P

            // Prepositions with <=3 chars
            new []{ "Preposition in Between",     "Preposition in Between"},
            new []{ "Preposition In Between",     "Preposition in Between"},
            new []{ "In Leading Preposition",     "In Leading Preposition"},
            new []{ "in Leading Preposition",     "In Leading Preposition"},
            new []{ "Trailing Preposition In",    "Trailing Preposition In"},
            new []{ "Trailing Preposition in",    "Trailing Preposition In"},

            // Prepositions with >3 chars
            new []{ "Preposition From Between",   "Preposition From Between"},
            new []{ "Preposition from Between",   "Preposition From Between"},
            new []{ "From Leading Preposition",   "From Leading Preposition"},
            new []{ "from Leading Preposition",   "From Leading Preposition"},
            new []{ "Trailing Preposition From",  "Trailing Preposition From"},
            new []{ "Trailing Preposition from",  "Trailing Preposition From"},

            // Verbs
            new []{ "Verb is in the Middle",  "Verb Is in the Middle"},

            // Coordinating Conjunctions
            new []{ "Something and Something Else",   "Something and Something Else"},
            new []{"Something And Something Else",    "Something and Something Else"},

            // Word Separators
            new []{ "Dash-Separator",         "Dash-Separator"},
            new []{ "dash-separator",         "Dash-Separator"},
            new []{ "Slash-Separator",        "Slash-Separator"},
            new []{ "slash-separator",        "Slash-Separator"},
            new []{ "Underscore_Separator",   "Underscore_Separator"},
            new []{ "underscore_separator",   "Underscore_Separator"},
            new []{ "Numerical2Separator",    "Numerical2Separator"},
            new []{ "numerical2separator",    "Numerical2Separator"},
            new []{ "Between (Parentheses)",  "Between (Parentheses)"},
            new []{ "Between (parentheses)",  "Between (Parentheses)"},

            // XML Reserved Chars
            new []{ "&quot;",     "&quot;"},
            new []{ "&quot;",     "&quot;"},
            new []{ "&lt;",       "&lt;"},
            new []{ "&gt;",       "&gt;"},
            new []{ "&amp;",      "&amp;"},

            new []{ "&amp;Word&amp;",     "&amp;Word&amp;"},
            new []{ "&amp;word&amp;",     "&amp;Word&amp;"},
            new []{ "&quot;Word&quot;",   "&quot;Word&quot;"},
            new []{ "&quot;word&quot;",   "&quot;Word&quot;"},
            new []{ "One &lt; Two",   "One &lt; Two"},
            new []{ "one &lt; two",   "One &lt; Two"},
            new []{ "Two &gt; One",   "Two &gt; One"},
            new []{ "two &gt; one",   "Two &gt; One"},
            new []{ "Something &amp; Something Else",     "Something &amp; Something Else"},
            new []{ "something &amp; something else",     "Something &amp; Something Else"},

            // Abbreviations
            new []{ "HTTP", "HTTP" },
            new []{ "Http", "HTTP" },

            // Abbreviations in their plural form
            new []{ "VLANs", "VLANs" },
            new []{ "Aaa VLANs", "Aaa VLANs" },
            new []{ "VLANs Bbb", "VLANs Bbb" },
            new []{ "Column 1 (VLANs)", "Column 1 (VLANs)" },
            new []{ "PIDs", "PIDs" },

            // Units with ignoreInDescription=false
            new []{ "aA",     "aA"},
            new []{ "Aa",     "aA"},
            new []{ "am/d",     "am/d"},
            new []{ "Am/D",     "am/d"},
            new []{ "barG",     "barG"},
            new []{ "Barg",     "barG"},
            new []{ "ft/s",     "ft/s"},
            new []{ "Ft/S",     "ft/s"},

            // For small (< 4 chars) units & words with specific casing, we allow both specific casing and all upper case.
            // Otherwise, we end up with too many conflicts between words with specific casing and abbreviations.
            new []{ "cH",     "cH"},
            new []{ "CH",     "CH"},

            // Words with specific casing
            new []{ "CM", "CM" },
            new []{ "Cm", "CM" },
            new []{ "Dataminer", "DataMiner" },
            new []{ "DataMiner", "DataMiner" },
            new []{ "QAction", "QAction" },
            new []{ "QActions", "QActions" },
            new []{ "DiSEqC", "DiSEqC" },
            new []{ "Diseqc", "DiSEqC" },
            new []{ "DISEQC", "DiSEqC" },
            new []{ "My IP", "My IP" },
            new []{ "My Ip", "My IP" },
            new []{ "My IPv4", "My IPv4" },
            new []{ "My Ipv6", "My IPv6" },
            new []{ "My Ipv6 Smth", "My IPv6 Smth" },
            new []{ "1080psf/25", "1080psf/25" },
            new []{ "1080 25psf", "1080 25psf" },
            new []{ "PsF Test", "PsF Test" },
            new []{ "Always On-Air", "Always On-Air" },

            // Units with 2 possible casing
            new []{ "mbar",     "mbar"},    // Milli-bar
            new []{ "Mbar",     "Mbar"},    // Mega-bar

            new []{ "peV",     "peV"},    // pico-electronvolt
            new []{ "PeV",     "PeV"},    // peta-electronvolt

            new []{ "CC Errors",    "CC Errors"},   // Continuity Counter Errors
            new []{ "cC",    "cC"},                 // centi-coulomb unit
            
            // Words with 2 possible casing: min (minutes) vs Min (Minimum)
            new []{ "min",     "min"},
            new []{ "Min",     "Min"},
            new []{ "Aaa min",     "Aaa min"},
            new []{ "Aaa Min",     "Aaa Min"},
            new []{ "min Aaa",     "min Aaa"},
            new []{ "Min Aaa",     "Min Aaa"},
            new []{ "Aaa min Bbb",     "Aaa min Bbb"},
            new []{ "Aaa Min Bbb",     "Aaa Min Bbb"},

            new []{ "bar",     "bar"},  // bar (pressure unit)
            new []{ "Bar",     "Bar"},  // Bar (Bar Tender, Metal Bar, etc)

            new []{ "Voice over IP",     "Voice over IP" },     // over : Known principles such as "Voice over IP", "IP over CDLC", etc
            new []{ "Come Over Here",     "Come Over Here"},    // Over : preposition of >3 chars

            // Words followed by brackets should be considered same as a last word
            new []{ "Turn on Turn On",                                  "Turn on Turn On"},
            new []{ "Turn on Turn On (Turn On)",                        "Turn on Turn On (Turn On)"},
            new []{ "Turn on (Turn On) Turn On (Turn On) (Turn On)",    "Turn On (Turn On) Turn On (Turn On) (Turn On)"},

            // Words preceded by brackets should be considered same as a first word
            new []{ "On Air on Air",                        "On Air on Air"},
            new []{ "On Air on Air (On Air)",               "On Air on Air (On Air)"},
            new []{ "(On Air) On Air on Air (On Air)",      "(On Air) On Air on Air (On Air)"},

            // Words with non-ASCII chars
            new []{ "500 µs",    "500 µs"},         // Micro Symbol µ (hex C2B5)
            new []{ "500µs",    "500µs"},           // Micro Symbol µ (hex C2B5)
            //new []{ "µ",    "µ"},                 // Micro Symbol µ (hex C2B5)
            //new []{ "\u00B5",    "µ"},            // Micro Symbol µ (hex C2B5)
            //new []{ "" + (char)0xC2B5,    "µ"},   // Micro Symbol µ (hex C2B5)

            new []{ "500 μs",    "500 Μs"},         // Greek letter μ (hex CEBC)
            new []{ "500μs",    "500Μs"},           // Greek letter μ (hex CEBC)
            //new []{ "μ",    "Μ"},                 // Greek letter μ (hex CEBC)
            //new []{ "\u03BC",    "Μ"},            // Greek letter μ (hex CEBC)
            //new []{ "" + (char)0xCEBC,    "Μ"},   // Greek letter μ (hex CEBC)
        };

        [TestMethod]
        [DoNotParallelize]
        [DynamicData(nameof(ReusableTestData))]
        public void ToTitleCase_Valid(string inputValue, string expectedOutput)
        {
            TitleCasing titleCasing = new TitleCasing(new ValidatorSettings());
            string output = titleCasing.ToTitleCase(inputValue);

            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow("On Air on Air", "On Air on Air")]
        [DataRow("On Air on Air (On Air)", "On Air on Air (On Air)")]
        [DataRow("(On Air) On Air on Air (On Air)", "(On Air) On Air on Air (On Air)")]
        public void ToTitleCase_Valid_ForQuickDebugging(string inputValue, string expectedOutput)
        {
            TitleCasing titleCasing = new TitleCasing(new ValidatorSettings());
            string output = titleCasing.ToTitleCase(inputValue);

            Assert.AreEqual(expectedOutput, output);
        }
    }
}
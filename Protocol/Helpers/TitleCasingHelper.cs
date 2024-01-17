namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    using Skyline.DataMiner.CICD.Validators.Common.Model;

    internal class TitleCasing
    {
        private readonly FixedCasing fixedCasing;

        // lowerCaseWords are not combined with FixedCasing.LoadFixedCaseWords because
        // - FixedCasing.LoadFixedCaseWords are fixed in all case
        // - lowerCaseWords are lower case except if first of last word.
        private static readonly string[] LowerCaseWords =
        {
            // Articles
            "a", "an", "the",

            // Coordinating Conjunctions
            "and", "but", "for", "nor", "or", "so", "yet",
                    
            // Prepositions (<2 Letters)
            "as", "at", "by", "in", "of", "on", "to", "up",

            // Prepositions (3 Letters)
            "for", "off", "via",

            // Prepositions (>3 Letters) -> should be capitalized
            //"amid", "atop", "down", "from", "into", "like", "near", "next",
            //"onto", "over", "than", "till", "with",
                    
            // Prepositions (>4 Letters) -> should be capitalized
            // ...
        };

        private static readonly string[] XmlEscapedChars =
        {
            "&quot;",
            "&apos;",
            "&lt;",
            "&gt;",
            "&amp;",
        };

        // This field contains words that have multiple valid casing
        private static readonly Dictionary<string, string[]> MultipleFixedCasingWords = new Dictionary<string, string[]>
        {
            { "AC", new string[] { "AC", "aC"}},        // AC3 (Audio Compression) vs aC (attocoulomb unit)
            { "bar", new string[] { "bar", "Bar"}},     // bar (pressure unit) vs Bar (Bar tender, metal bar, etc)
            { "CC", new string[] { "CC", "cC"}},        // CC Errors (Continuity Counter Errors) vs cC (centicoulomb unit)
            { "ECM", new string[] { "ECM", "eCM"}},     // ECM (Stream PID Type) vs eCM (Embedded Cable Modem)
            { "ES", new string[] { "ES", "Es"}},        // ES (Abbreviation) vs Es (exasecond unit)
            { "min", new string[] { "min", "Min"}},     // min (minutes) vs Min (Minimum)
            { "TS", new string[] { "TS", "Ts"}},        // TS (Transport Stream) vs Ts (terasecond unit)
            { "D", new string[] { "D", "d"}},           // D (As in listing A, B, C, D...) vs d (day unit)
            { "I", new string[] { "I", "i"}},           // I (As in listing A, B, C, D...) vs i (interlaced (resolution 1080i))
            { "P", new string[] { "P", "p"}},           // P (As in listing A, B, C, D...) vs p (progressive scan (resolution 1080p))
            { "per", new string[] { "per", "Per"}},     // per (preposition) vs Per (adverb)
            { "over", new string[] { "Over", "over"}},  // Over (preposition of >6 letters) vs over (many principles such as "Voice over IP", "IP over CDLC", etc)
            { "psf", new string[] { "psf", "PsF"}},     // Progressive Segmented Frames ('PsF' used when referring to the principle, 'psf' used in image formats '1080psf/25'
        };

        private static ConcurrentDictionary<string, string> _toTitleCaseCache = new ConcurrentDictionary<string, string>();

        public TitleCasing(ValidatorSettings settings)
        {
            fixedCasing = new FixedCasing(settings);
        }

        /// <summary>
        /// Rules were defined based on info from "https://capitalizemytitle.com/#capitalizationrules"
        /// - Should be capitalized
        ///     - First and Last word
        ///     - Important words ()
        /// - Should not be capitalized
        ///     - Articles (a, an, the)
        ///     - Coordinating Conjunctions (and, but, for, nor, or, so, yet)
        ///     - Prepositions (at, by, to...) with less than 4 chars
        /// </summary>
        /// <param name="value">String that needs to be checked.</param>
        /// <param name="expectedValue">Expected value.</param>
        /// <param name="exceptions">Exception when return false.</param>
        /// <returns></returns>
        public bool IsTitleCase(string value, out string expectedValue, string[] exceptions = null)
        {
            expectedValue = ToTitleCase(value, exceptions);
            return String.Equals(value, expectedValue);
        }

        /// <summary>
        /// Rules were defined based on info from "https://capitalizemytitle.com/#capitalizationrules"
        /// - Should be capitalized
        ///     - First and Last word
        ///     - Important words ()
        /// - Should not be capitalized
        ///     - Articles (a, an, the)
        ///     - Coordinating Conjunctions (and, but, for, nor, or, so, yet)
        ///     - Prepositions (at, by, to...) with less than 4 chars
        /// </summary>
        /// <param name="inputValue">String that needs to be title cased.</param>
        /// <param name="exceptions">Exception when return false.</param>
        /// <returns></returns>
        public string ToTitleCase(string inputValue, string[] exceptions = null)
        {
            string result = _toTitleCaseCache.GetOrAdd(inputValue,
                (input) =>
                {
                    List<string> resultParts = new List<string>();

                    int startIndex = 0;
                    for (int i = 0; i < input.Length; i++)
                    {
                        bool isLastChar;
                        int wordLength;

                        // Identify position & length of next word
                        if (i == startIndex /*Making sure this is only checked once for the same word*/
                            && StartsWithFixedCaseWord(input.Substring(startIndex), out wordLength))
                        {
                            // Make sure fixedCaseWords are considered as one word even if they contain a separator
                            i += wordLength;
                            isLastChar = i >= input.Length - 1;

                            if (!isLastChar && i >= input.Length)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            // 'Normal' words
                            isLastChar = i == input.Length - 1;
                            if (!isLastChar && Char.IsLetter(input[i]))
                            {
                                // Move on as we are in the middle of a word
                                continue;
                            }

                            wordLength = i - startIndex;
                            if (isLastChar && Char.IsLetter(input[i]))
                            {
                                wordLength++;
                            }
                        }

                        // Process with word
                        if (wordLength > 0 && (i != startIndex || isLastChar))
                        {
                            // Check if last word before a bracket
                            string remainingPart = input.Substring(startIndex);

                            int previousCharPos = startIndex - 1;
                            while (previousCharPos > 0 && Char.IsWhiteSpace(input[previousCharPos]))
                            {
                                previousCharPos--;
                            }

                            bool isPrecededByBracket = previousCharPos >= 0
                                && new char[] { '(', ')', '[', ']', '{', '}' }.Contains(input[previousCharPos]);

                            int nextCharPos = wordLength;
                            while (nextCharPos < remainingPart.Length && Char.IsWhiteSpace(remainingPart[nextCharPos]))
                            {
                                nextCharPos++;
                            }

                            bool isFollowedByBracket = nextCharPos < remainingPart.Length
                                && new char[] { '(', ')', '[', ']', '{', '}' }.Contains(remainingPart[nextCharPos]);

                            // Process with word
                            string word = remainingPart.Substring(0, wordLength);
                            string resultWord = ToTitleCaseSingleWord(word, out bool capitalizeIfFirstOrLastWord);

                            bool isFirstOrLastWord = startIndex == 0 || isPrecededByBracket || isLastChar || isFollowedByBracket;
                            if (isFirstOrLastWord && capitalizeIfFirstOrLastWord)
                            {
                                if (resultWord[0] == 181 && !RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework"))
                                {
                                    // When micro is the first character and we're not in the .NET Framework environment, don't change it.
                                    // In .NET it doesn't differentiate between micro and the Greek letter mu.
                                }
                                else
                                {

                                    resultWord = Char.ToUpperInvariant(resultWord[0]) + resultWord.Substring(1);
                                }
                            }

                            resultParts.Add(resultWord);
                        }

                        // Add next separator char if needed
                        if (i >= input.Length)
                        {
                            break;
                        }

                        if (!(isLastChar && Char.IsLetter(input[i])))
                        {
                            // Word Separator Char
                            resultParts.Add(Convert.ToString(input[i]));
                            startIndex = i + 1;
                        }
                    }

                    string innerResult = String.Join(null, resultParts);

                    // XML Escaped Chars (&amp; &gt; etc)
                    foreach (string xmlEscapedChar in XmlEscapedChars)
                    {
                        innerResult = Regex.Replace(innerResult, xmlEscapedChar, xmlEscapedChar, RegexOptions.IgnoreCase);
                    }

                    return innerResult;
                });

            // Extra exception provided by the API user
            if (exceptions != null)
            {
                foreach (string exception in exceptions)
                {
                    result = Regex.Replace(result, exception, exception, RegexOptions.IgnoreCase);
                }
            }

            return result;
        }

        /// <summary>
        /// Rules were defined based on info from "https://capitalizemytitle.com/#capitalizationrules"
        /// - Should be capitalized
        ///     - First and Last word
        ///     - Important words ()
        /// - Should not be capitalized
        ///     - Articles (a, an, the)
        ///     - Coordinating Conjunctions (and, but, for, nor, or, so, yet)
        ///     - Prepositions (at, by, to...) with less than 4 chars
        /// </summary>
        /// <param name="word">Word to be title cased.</param>
        /// <param name="capitalizeIfFirstOrLastWord">Capitalize if first or last word.</param>
        /// <returns></returns>
        private string ToTitleCaseSingleWord(string word, out bool capitalizeIfFirstOrLastWord)
        {
            string wordUpperCased = word.ToUpperInvariant();

            // Single letter words => we allow both casing
            // Examples: 'Item I' vs '1080i/25' (Interlaced)
            if (word.Length == 1)
            {
                capitalizeIfFirstOrLastWord = false;
                return word;
            }

            // Small upper case words
            // For small words, we expect a lot of conflicts between upper case abbreviation and fixedCasingWords so we are more flexible here
            // For bigger words, the same logic is executed but after the different fixed casing words checks
            if (word.Length < 4 && IsUpperCase(word, wordUpperCased))
            {
                capitalizeIfFirstOrLastWord = false;
                return word;
            }

            // Words with multiple possible known casing
            foreach (KeyValuePair<string, string[]> kvp in MultipleFixedCasingWords)
            {
                if (!String.Equals(word, kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                capitalizeIfFirstOrLastWord = false;

                foreach (string value in kvp.Value)
                {
                    if (String.Equals(word, value))
                    {
                        return value;
                    }
                }

                return kvp.Key;
            }

            // Words with correct fixed casing
            if (fixedCasing.FixedCaseWords.Contains(word))
            {
                capitalizeIfFirstOrLastWord = false;
                return word;
            }

            // Words with incorrect fixed casing
            foreach (string fixedCaseWord in fixedCasing.FixedCaseWords)
            {
                if (String.Equals(word, fixedCaseWord, StringComparison.OrdinalIgnoreCase))
                {
                    capitalizeIfFirstOrLastWord = false;
                    return fixedCaseWord;
                }
            }

            // Big upper case words
            if (IsUpperCase(word, wordUpperCased))
            {
                capitalizeIfFirstOrLastWord = false;
                return word;
            }

            // Lower case words
            string workLowerCased = word.ToLowerInvariant();
            if (LowerCaseWords.Contains(workLowerCased))
            {
                capitalizeIfFirstOrLastWord = true;
                return workLowerCased;
            }

            // All other words
            if (word[0] == 181 && !RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework"))
            {
                word = word[0] + workLowerCased.Substring(1);

                // When micro is the first character and we're not in the .NET Framework environment, don't change it.
                // In .NET it doesn't differentiate between micro and the Greek letter mu.
            }
            else
            {
                word = Char.ToUpperInvariant(word[0]) + workLowerCased.Substring(1);
            }

            capitalizeIfFirstOrLastWord = true;
            return word;
        }

        private bool StartsWithFixedCaseWord(string inputValue, out int fixedCaseWordLength)
        {
            fixedCaseWordLength = 0;
            foreach (string fixedCaseWord in fixedCasing.FixedCaseWords)
            {
                if (inputValue.Length > fixedCaseWord.Length && Char.IsLetter(inputValue[fixedCaseWord.Length]))
                {
                    // Avoid considering it a fixed case word when in the middle of a word
                    // ex: any word with an 'l' in it since 'L' is the unit for Liter
                    continue;
                }

                if (fixedCaseWord.Length > fixedCaseWordLength &&
                    inputValue.StartsWith(fixedCaseWord, StringComparison.OrdinalIgnoreCase))
                {
                    fixedCaseWordLength = fixedCaseWord.Length;
                    // We don't yet return here as in case of multiple matches we want to find the one with biggest length
                }
            }

            return fixedCaseWordLength > 0;
        }

        private static bool IsUpperCase(string word, string wordUpperCased)
        {
            // Abbreviations (HTTP, VLAN, PID, etc)
            if (word == wordUpperCased)
            {
                return true;
            }

            // Abbreviations plural form (VLANs, PIDs, etc)
            if (word[word.Length - 1] == 's' &&
                word.Substring(0, word.Length - 1) == wordUpperCased.Substring(0, word.Length - 1))
            {
                return true;
            }

            return false;
        }

        private sealed class FixedCasing
        {
            public FixedCasing(ValidatorSettings settings)
            {
                LoadFixedCaseWords(settings);
            }

            public string[] FixedCaseWords { get; private set; }

            private void LoadFixedCaseWords(ValidatorSettings settings)
            {
                List<string> words = new List<string>
                {
                    // Add fixed case words that are not in the list of units here
                    "ACK",      // Acknowledgment
                    "ACL",      // Access Control List
                    "ADSL",     // AAsymmetric Digital Subscriber Line
                    "AES",      // Advanced Encryption Standard
                    "ANSI",     // American National Standards Institute
                    "API",      // Application Programming Interface
                    "ARP",      // Address Resolution Protocol
                    "ATM",      // Asynchronous Transfer Mode
                    "BGP",      // Border Gateway Protocol
                    "BSS",      // Basic Service Set
                    "DataMiner",    // Best application ever everyone should know about !!!
                    "CHAP",     // Challenge-Handshake Authentication Protocol (PPP)
                    "cHDLC",    // Cisco HDLC
                    "CIDR",     // Classless Inter-Domain Routing
                    "CIR",      // Committed Information Rate (Frame Relay)
                    "CLI",      // Command Line Interpreter
                    "CM",       // Cable Modem
                    "CoS",      // Class of Service
                    "CPE",      // Customer Premises Equipment
                    "CPU",      // Central Processing Unit
                    "CRC",          // Cyclic Redundancy Check
                    "CRC_16-CCITT", // Cyclic Redundancy Check (X.25, HDLC)
                    "CRT",          // Cathode Ray Tube
                    "CSMA/CA",      // Carrier sense multiple access / collision avoidance
                    "CSMA/CD",      // Carrier sense multiple access / collision detection
                    "CSU/DSU",      // Channel service unit / data service unit
                    "CMOS",     // Complementary metal-oxide semiconductor
                    "DAM",      // Digital asset management
                    "DC",       // Down Converter
                    "DCE",      // Data Communications Equipment
                    "DDC",      // Digital Down Converter
                    "DHCP",     // Dynamic Host Configuration Protocol
                    "DHCPv4",   // IPv4 version of the Dynamic Host Configuration Protocol
                    "DHCPv6",   // IPv6 version of the Dynamic Host Configuration Protocol
                    "DiSEqC",   // Digital Satellite Equipment Control
                    "DIMM",     // Dual In-line Memory Module
                    "DSN",      // Domain Name System
                    "DoS",      // Denial of Service
                    "DRAM",     // Dynamic random-access memory
                    "DSL",      // Digital subscriber line
                    "DSLAM",    // Digital subscriber line access multiplexer
                    "DTE",      // Data Terminal Equipment
                    "DMI",      // Desktop Management Interface
                    "DUC",      // Digital Up Converter
                    //"eCM",      // Embedded Cable Modem -> Added to multipleFixedCasingWords
                    "EHA",      // Ethernet Hardware Address (MAC address)
                    "EIA",      // Electronics Industry Alliance
                    "EIGRP",    // Enhanced Interior Gateway Routing Protocol
                    "eMTA",     // Embedded Multimedia Terminal Adapter
                    "EOF",      // End Of Frame (HDLC, etc.)
                    "EPM",      // Experience & Performance Management
                    "ePS",      // Embedded Portal Services
                    "EPoc",     // Ethernet passive optical network Protocol Over Coax
                    "eRouter",  // Embedded Router
                    "eSAFE",    // Embedded Service/Application Functional Entity
                    "ESS",      // Extended service set (WiFi group)
                    "FCC",      // Federal Communications Commission (US)
                    "FCS",      // Frame check sequence (Ethernet)
                    "FDDI",     // Fiber Distributed Data Interface
                    "FTTdp",    // Fiber To The Distribution Point
                    "FTP",      // File Transfer Protocol
                    "GbE",      // GigaBit Ethernet
                    "GBIC",     // GigaBit interface converter
                    "GEPOF",    // GigaBit Ethernet (over) Plastic Optical Fiber
                    "HDLC",     // High-level Data Link Control
                    "HTTP",     // HyperText Transfer Protocol
                    "HTTPS",    // HyperText Transfer Protocol Secure
                    "IaaS",     // Infrastructure as a Service
                    "IANA",     // Internet Assigned Number Authority
                    "ICaaS",    // Integration Capability as a Service
                    "ICMP",     // Internet Control Message Protocol
                    "ICMPv6",   // Internet Control Message Protocol Version 6
                    "IDF",      // Intermediate distribution frame
                    "IDS",      // Intrusion Detection System
                    "IEEE",     // Institute for Electrical and Electronic Engineers
                    "IETF",     // Internet Engineering Task Force
                    "IGMP",     // Internet Group Management Protocol
                    "IGMPv2",   // Internet Group Management Protocol Version 2
                    "IGMPv3",   // Internet Group Management Protocol Version 3
                    "IMAP",     // Internet Message Access Protocol
                    "IMAPS",    // Internet Message Access Protocol over SSL/TLS
                    "IoT",      // Internet of Things
                    "IP",       // Internet Protocol
                    "IPv4",     // Internet Protocol Version 4
                    "IPv6",     // Internet Protocol Version 6
                    "iPad",
                    "iPhone",
                    "IPS",      // Intrusion Prevention System
                    "IPv4",
                    "IPv6",
                    "IS-IS",    // Intermediate System to Intermediate System (routing protocol)
                    "ISDN",     // Integrated Services Digital Network
                    "ISP",      // Internet Service Provider
                    "iSCSI",    // Internet Small Computer System Interface
                    "ITU-T",    // International Telecommunications Union
                    "LACP",     // Link Aggregation Control Protocol
                    "LAN",      // Local Area Network
                    "LAPB",     // Link Access Procedure, Balanced (x.25)
                    "LAPF",     // Link-access procedure for frame relay
                    "LLC",      // Logical link control
                    "MAC",      // Media access control
                    "MAM",      // Media access management (related to Digital asset management)
                    //"MAN",      // Metropolitan area network
                    "MDF",      // Main distribution frame
                    "MediaConvert",     // 
                    "MediaTailor",      // 
                    "MIB",      // Management information base (SNMP)
                    "MoCA",     // Multimedia over Coax Alliance
                    "MLD",      // Multicast Listener Discovery
                    "MPLS",     // MultiProtocol Label Switching
                    "MTU",      // Maximum Transmission Unit
                    "NAC",      // Network access control
                    "NAT",      // Network Address Translation
                    "NBMA",     // Non-Broadcast Multiple Access (e.g. Frame Relay ATM)
                    "NIC",      // Network Interface Card
                    "NRZ",      // Non-return-to-zero
                    "NRZI",     // Non-return to zero inverted
                    "NVRAM",    // Non-volatile RAM
                    "Off-Air",
                    "On-Air",
                    "OpenConfig",
                    "OSI",      // Open System Interconnect (joint ISO and ITU standard)
                    "OSPF",     // Open Shortest Path First (routing protocol)
                    "OUI",      // Organization Unique Identifier
                    "PaaS",     // Platform as a Service
                    "PAP",      // Password authentication protocol
                    "PAT",      // Port address translation
                    "PC",       // Personal computer (host)
                    "PIM",      // Personal information manager / Privileged Identity Management
                    "PCM",      // Pulse-code modulation
                    "PDU",      // Protocol data unit (such as segment, packet, frame, etc.)
                    "PLP",      // Physical Layer Pipe
                    "PoE",      // Power over Ethernet
                    "PoP",      // Point of Presence
                    "POP3",     // Post Office Protocol, version 3
                    "PPP",      // Point-to-point Protocol
                    "PPTP",     // Point-to-Point Tunneling Protocol
                    "PTT",      // Public Telephone and Telegraph
                    "PVST",     // Per-VLAN Spanning Tree
                    "QAction",
                    "QActions",
                    "QoE",      // Quality of Experience
                    "QoL",      // Quality of Live
                    "QoS",      // Quality of Service
                    "RAM",      // Random Access Memory
                    "RARP",     // Reverse ARP
                    "RIMM",     // Rambus In-line Memory Module
                    "RFC",      // Request for Comments
                    "RIP",      // Routing Information Protocol
                    "RLL",      // Run-Length Limited
                    "ROM",      // Read-Only Memory
                    "RSTP",     // Rapid Spanning Tree Protocol
                    "RTP",      // Real-time Transport Protocol
                    "RxMER",    // Receive Modulation Error Ration
                    "SaaS",     // Software as a Service
                    "SDLC",     // Synchronous Data Link Control
                    "SDN",      // Software Defined Networking
                    "SFD",      // Start-of-frame delimiter (Ethernet, HDLC, etc.)
                    "SFP",      // Small form-factor pluggable
                    "SIMM",     // Single In-line Memory Module
                    "SLARP",    // Serial Line ARP (Address Resolution Protocol)
                    "SMTP",     // Simple Mail Transfer Protocol
                    "SNA",      // Systems Network Architecture (IBM)
                    "SNAP",     // SubNet Access Protocol
                    "SNMP",     // Simple Network Management Protocol
                    "SNMPv1",   // Simple Network Management Protocol version 1
                    "SNMPv2",   // Simple Network Management Protocol version 2
                    "SNMPv2c",  // Simple Network Management Protocol community-based version 2
                    "SNMPv3",   // Simple Network Management Protocol version 3
                    "SRAM",     // Static random access memory
                    "SSH",      // Secure shell
                    "SSID",     // Service set identifier (WiFi)
                    "T2-MI",    // T2 Modulator Interface
                    "TCP/IP",   // Transmission Control Protocol/Internet Protocol
                    "TDM",      // Time-division multiplexing
                    "TFTP",     // Trivial File Transfer Protocol
                    "TIA",      // Telecommunications Industry Alliance
                    "ToD",      // Time of Day
                    "TOFU",     // Trust On First Use
                    "ToS",      // Type of Service
                    "UC",       // Up Converter
                    "UDP",      // User Datagram Protocol
                    "USB",      // Universal Serial Bus
                    "UTP",      // Unshielded twisted pair
                    "VC",       // Virtual circuit
                    "VLAN",     // Virtual local area network
                    "vCloud",       // 
                    "vCloudPoint",  // 
                    "vMatrix",      // 
                    "VMware",   // 
                    "VoD",      // Video on Demand
                    "VoIP",     // Voice over IP
                    "VPN",      // Virtual private network
                    "W3C",      // World Wide Web Consortium
                    "WAN",      // Wide-area network
                    "WEP",      // Wired Equivalent Privacy
                    "WLC",      // Wireless LAN Controller
                    "WPA",      // Wi-Fi Protected Access
                    "YCbCr",    // Color spaces
                    "ICtCp",    // Color representation
                };

                words.AddRange(settings.UnitList.Units.Where(u => !u.IgnoreInDescription).Select(u => u.Value));

                FixedCaseWords = words.ToArray();
            }
        }
    }
}
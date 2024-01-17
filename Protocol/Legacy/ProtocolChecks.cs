namespace Skyline.DataMiner.CICD.Validators.Protocol.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Xml;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    internal partial class ProtocolChecks
    {
        #region fields and properties
        /// <summary>
        /// Generic line number, used for lineNum estimates in case a ProtocolChecks test fails.
        /// </summary>
        public string LineNum { get; set; } = "-1";

        /// <summary>
        /// List of all duplicated parameters.
        /// Key = Duplicate parameter Id.
        /// Value = real Parameter Id.
        /// </summary>
        private readonly Dictionary<int, int> DuplicateParameterDictionary = new Dictionary<int, int>();

        /// <summary>
        /// The groups dictionary.
        /// </summary>
        private readonly Dictionary<int, XmlNode> GroupsDictionary = new Dictionary<int, XmlNode>();

        /// <summary>
        /// List of all parameter id's in the protocol.
        /// </summary>
        private readonly HashSet<string> ParameterIdSet = new HashSet<string>();

        /// <summary>
        /// Parameter Info for all parameters.
        /// Key = Parameter Id.
        /// Value = ParameterInfo.
        /// </summary>
        private readonly Dictionary<int, ParameterInfo> ParameterInfoDictionary = new Dictionary<int, ParameterInfo>();
        #endregion

        /// <summary>
        /// Checks the content of the attributes.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckAttributesContent(XmlDocument xDoc) // M
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();

            // Add xmlNameSpaceManager
            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            // Re factored into separate methods to make code more readable
            CheckActionAttributes(xDoc, resultMsg, xmlNsm);
            CheckChainAttributes(xDoc, resultMsg, xmlNsm);
            CheckPairAttributes(xDoc, resultMsg, xmlNsm);
            CheckParamAttributes(xDoc, resultMsg, xmlNsm);
            CheckQActionAttributes(xDoc, resultMsg, xmlNsm);
            CheckRelationAttributes(xDoc, resultMsg, xmlNsm);
            CheckResponseAttributes(xDoc, resultMsg, xmlNsm);

            return resultMsg;
        }

        /// <summary>
        /// Checks the copy action.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckCopyAction(XmlDocument xDoc)
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();

            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            XmlNodeList xnlCopyActions = xDoc.SelectNodes("/slc:Protocol/slc:Actions/slc:Action[slc:Type='copy']|.//slc:Actions/slc:Action[slc:Type='Copy']", xmlNsm);
            if (xnlCopyActions == null)
            {
                return resultMsg;
            }

            foreach (XmlNode xnCopyAction in xnlCopyActions)
            {
                LineNum = xnCopyAction.Attributes?["QA_LNx"].InnerXml;
                string typeIdRawValue = xnCopyAction.SelectSingleNode("./slc:Type", xmlNsm)?.Attributes?["id"]?.InnerXml;

                if (String.IsNullOrEmpty(typeIdRawValue))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 3103,
                        DescriptionFormat = "Type id attribute empty or not present in Copy Action",
                        DescriptionParameters = new object[] { typeIdRawValue },
                        TestName = "CheckCopyAction",
                        Severity = Severity.Minor
                    });
                }
                else
                {
                    if (!Int32.TryParse(typeIdRawValue, out int typeId)
                        || !ParameterInfoDictionary.TryGetValue(typeId, out ParameterInfo paramInfo)
                        || paramInfo?.Element == null)
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 3103,
                            DescriptionFormat = "Attribute 'Action/Type@id' references a non-existing Param with ID '" + typeIdRawValue + "'.",
                            DescriptionParameters = new object[] { typeIdRawValue },
                            TestName = "CheckCopyAction",
                            Severity = Severity.Major
                        });

                        continue;
                    }

                    XmlNode xnFromParam = paramInfo.Element;
                    string rawType = xnFromParam.SelectSingleNode(".//slc:Interprete/slc:RawType", xmlNsm)?.InnerXml;
                    string lengthType = xnFromParam.SelectSingleNode(".//slc:Interprete/slc:LengthType", xmlNsm)?.InnerXml;

                    // Fixed and unsigned number
                    if (String.Equals(rawType, "unsigned number", StringComparison.OrdinalIgnoreCase) && String.Equals(lengthType, "fixed", StringComparison.OrdinalIgnoreCase))
                    {
                        // Get value
                        XmlNode xnValue = xnFromParam.SelectSingleNode(".//slc:Interprete/slc:Value", xmlNsm);
                        if (xnValue != null)
                        {
                            LineNum = xnValue.Attributes?["QA_LNx"].InnerXml;
                            string sValue = xnValue.InnerXml;
                            const string RegexUnsignedNumber = "(0[xX][0-9a-fA-F])+";
                            if (!Regex.IsMatch(sValue, RegexUnsignedNumber))
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 3102,
                                    DescriptionFormat = "Unsigned Number value '{0}' is not written in format 0xFF.",
                                    DescriptionParameters = new object[] { sValue },
                                    TestName = "CheckCopyAction",
                                    Severity = Severity.Minor
                                });
                            }

                            if (sValue.Contains("0x00"))
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 3101,
                                    DescriptionFormat =
                                        "Copy from Param with ID {0} may fail because the value contains 0x00.",
                                    DescriptionParameters = new object[] { typeIdRawValue },
                                    TestName = "CheckCopyAction",
                                    Severity = Severity.Minor
                                });
                            }
                        }
                    }
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Checks the DVE column option.
        /// Check if DVE exported table has exactly one columnoption with options=;element.
        /// </summary>
        /// <param name="xDoc">The procotol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckDveColumnOptionElement(XmlDocument xDoc) // M
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();

            // Get exported table parameter ID's
            Dictionary<string, string> exportedTables = GetDveTables(xDoc);

            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            XmlNodeList xnlParam = xDoc.SelectNodes("slc:Protocol/slc:Params/slc:Param", xmlNsm);

            foreach (XmlNode xnParam in xnlParam)
            {
                string parameterId = xnParam.Attributes?.GetNamedItem("id")?.InnerXml;
                LineNum = xnParam.Attributes?.GetNamedItem("QA_LNx")?.InnerXml;

                XmlNodeList xnlColumnOption = xnParam.SelectNodes("./slc:ArrayOptions/slc:ColumnOption", xmlNsm);

                // Count number of columns with element option
                int elementCounter = 0;

                foreach (XmlNode columnOption in xnlColumnOption)
                {
                    string sOptions = columnOption.Attributes?["options"]?.InnerXml;
                    if (String.IsNullOrWhiteSpace(sOptions)) { continue; }

                    string[] asOptions = sOptions.Split(sOptions[0]);
                    foreach (string option in asOptions)
                    {
                        if (option == "element")
                        {
                            elementCounter++;
                        }
                    }
                }

                // If element option is not present, generate error if param id is in exportedTable list
                if (elementCounter == 0 && exportedTables.Keys.Count != 0 && exportedTables.Keys.Contains(parameterId))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 2001,
                        DescriptionFormat = "There is no column with element option in exported table {0}",
                        DescriptionParameters = new object[] { parameterId },
                        TestName = "CheckDVEColumnOptionElement",
                        Severity = Severity.Major
                    });
                }

                // If exactly one element option is present and param is not in exported tables list, generate an error.
                if (elementCounter == 1 && !exportedTables.Keys.Contains(parameterId))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 2003,
                        DescriptionFormat = "There is no export table defined for table {0}",
                        DescriptionParameters = new object[] { parameterId },
                        TestName = "CheckDVEColumnOptionElement",
                        Severity = Severity.Major
                    });
                }

                if (elementCounter > 1)
                {
                    // Generate error for multiple columns with element option
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 2002,
                        DescriptionFormat = "There are multiple columns with element option in exported table {0}",
                        DescriptionParameters = new object[] { parameterId },
                        TestName = "CheckDVEColumnOptionElement",
                        Severity = Severity.Major
                    });

                    // Generate error if table param is not in exportedTables list.
                    if (!exportedTables.Keys.Contains(parameterId))
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 2003,
                            DescriptionFormat = "There is no export table defined for table {0}",
                            DescriptionParameters = new object[] { parameterId },
                            TestName = "CheckDVEColumnOptionElement",
                            Severity = Severity.Major
                        });
                    }
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Checks on group Settings.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckGroupSettings(XmlDocument xDoc)
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();

            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            string protocolType = xDoc.SelectSingleNode("/slc:Protocol/slc:Type", xmlNsm)?.InnerXml;
            bool bVirtual = String.Equals(protocolType, "virtual", StringComparison.OrdinalIgnoreCase);

            bool bMultithreaded = false;
            XmlNodeList xnlTimers = xDoc.SelectNodes("./slc:Timers/slc:Timer", xmlNsm);
            foreach (XmlNode xnTimer in xnlTimers)
            {
                string sOptions = xnTimer?.Attributes?["options"]?.InnerXml;
                if (String.IsNullOrEmpty(sOptions)) { continue; }

                if (sOptions.StartsWith("ip:", StringComparison.InvariantCulture) || sOptions.Contains(";ip:"))
                {
                    bMultithreaded = true;
                }
            }

            XmlNodeList xnlGroups = xDoc.SelectNodes("/slc:Protocol/slc:Groups/slc:Group", xmlNsm);
            foreach (XmlNode xnGroup in xnlGroups)
            {
                XmlNode xnContent = xnGroup.SelectSingleNode("./slc:Content", xmlNsm);
                if (xnContent == null)
                {
                    if (!bVirtual && !bMultithreaded)
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 1901,
                            DescriptionFormat =
                                "Group with missing Content tag. This should only be used in virtual protocols or when using a multithreaded timer.",
                            DescriptionParameters = null,
                            TestName = "CheckGroupSettings",
                            Severity = Severity.Minor
                        });
                    }
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Checks the Interprete Measurement.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckInterpreteMeasurement(XmlDocument xDoc)
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();

            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            foreach (ParameterInfo pi in ParameterInfoDictionary.Values)
            {
                XmlNode xnRawType = pi.Element.SelectSingleNode("./slc:Interprete/slc:RawType", xmlNsm);
                if (xnRawType == null) { continue; }

                string rawType = xnRawType.InnerXml;
                string type = pi.IntType;
                string measurementType = pi.MeasType;

                // 3 Main Types, String, Number or Table
                string simpleType = null;
                switch (measurementType)
                {
                    case "string":
                    case "pagebutton":
                        simpleType = "string";
                        break;

                    case "number":
                    case "analog":
                    case "chart":
                    case "digital threshold":
                    case "progress":
                    case "table":
                    case "matrix":
                        // Matrix now handled in Validator2
                        simpleType = null;
                        break;
                    case "button":
                    case "discreet":
                    case "togglebutton":
                        {
                            bool allNumbers = true;
                            XmlNode xnDiscreets = pi.Element.SelectSingleNode("./slc:Measurement/slc:Discreets", xmlNsm);
                            if (xnDiscreets != null)
                            {
                                // Check if Discreets Tag has dependencyId
                                XmlAttribute xaDependencyId = xnDiscreets.Attributes?["dependencyId"];

                                if (xaDependencyId == null)
                                {
                                    foreach (XmlNode innerDiscreet in xnDiscreets.SelectNodes("./slc:Discreet/slc:Value", xmlNsm))
                                    {
                                        if (!Double.TryParse(innerDiscreet.InnerXml, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                                        {
                                            allNumbers = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // Has dependencyId => String
                                    allNumbers = false;
                                }
                            }

                            if (allNumbers)
                            {
                                simpleType = "double";
                            }
                            else
                            {
                                simpleType = "string";
                            }
                        }
                        break;
                }

                if (simpleType == null) { continue; }

                if (simpleType != type)
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(pi.LineNum),
                        ErrorId = 5001,
                        DescriptionFormat = "Verify Measurement - Interprete Combination for {0} : {1}",
                        DescriptionParameters = new object[] { measurementType, type },
                        TestName = "CheckInterpreteMeasurement",
                        Severity = Severity.Minor
                    });
                }

                if (measurementType != "table" && measurementType != "matrix" &&
                    (rawType == "other" && type != "string") || (rawType == "numeric text" && type != "double"))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(pi.LineNum),
                        ErrorId = 5002,
                        DescriptionFormat = "Verify Interprete RawType - Type Combination: {0} - {1}",
                        DescriptionParameters = new object[] { rawType, type },
                        TestName = "CheckInterpreteMeasurement",
                        Severity = Severity.Minor
                    });
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Checks the port settings.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckPortSettings(XmlDocument xDoc) // SR
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();

            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            XmlNodeList nlType = xDoc.SelectNodes("slc:Protocol/slc:Type", xmlNsm);
            foreach (XmlNode xnType in nlType)
            {
                LineNum = xnType.Attributes?["QA_LNx"].InnerXml;

                // Check relative timers exists and is true
                if (xnType.Attributes?["relativeTimers"] != null)
                {
                    if (!xnType.Attributes["relativeTimers"].InnerXml.ToLower().Contains("true"))
                    {
                        // Relative timers exists but is not true
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 1802,
                            DescriptionFormat = "Main Type tag attribute relativeTimers is not set to 'true'.",
                            DescriptionParameters = null,
                            TestName = "CheckPortSettings",
                            Severity = Severity.Minor
                        });
                    }
                }
                else
                {
                    // Relative timers does not exist
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 1803,
                        DescriptionFormat = "Main Type tag attribute relativeTimers must exist and be set to 'true'.",
                        DescriptionParameters = null,
                        TestName = "CheckPortSettings",
                        Severity = Severity.Minor
                    });
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Checks the positions.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckPositions(XmlDocument xDoc) // M
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();
            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            Dictionary<Position, List<string>> dictUniquePositions = new Dictionary<Position, List<string>>();
            foreach (ParameterInfo pi in ParameterInfoDictionary.Values)
            {
                LineNum = pi.LineNum;

                if (pi.Positions != null && pi.RTDisplay)
                {
                    foreach (Position po in pi.Positions)
                    {
                        if (!po.IsValid()) { continue; }

                        if (!dictUniquePositions.ContainsKey(po))
                        {
                            dictUniquePositions.Add(po, new List<string> { Convert.ToString(pi.Pid) });
                        }
                        else
                        {
                            dictUniquePositions[po].Add(Convert.ToString(pi.Pid));
                        }
                    }
                }
            }

            // Check duplicate positions
            foreach (Position position in dictUniquePositions.Keys)
            {
                List<string> pids = dictUniquePositions[position];

                if (pids.Count >= 2)
                {
                    bool error = true;
                    if (pids.Count == 2)
                    {
                        ParameterInfo pi1 = ParameterInfoDictionary[Convert.ToInt32(pids[0])];
                        ParameterInfo pi2 = ParameterInfoDictionary[Convert.ToInt32(pids[1])];
                        if (pi1.Description == pi2.Description)
                        {
                            // Descriptions are the same, check if types are allowed combinations
                            // Allowed combinations:
                            //  read - write
                            //  array - write
                            //  read bit - write bit
                            //  read bit - write

                            string type1 = pi1.Type;
                            if (type1.StartsWith("write", StringComparison.InvariantCulture))
                            {
                                type1 = "write";
                            }
                            else
                            {
                                type1 = "read";
                            }

                            string type2 = pi2.Type;
                            if (type2.StartsWith("write", StringComparison.InvariantCulture))
                            {
                                type2 = "write";
                            }
                            else
                            {
                                type2 = "read";
                            }

                            if (type1 != type2)
                            {
                                // Types are read/write pair
                                error = false;
                            }
                        }
                        // Else error: different param descriptions on same position
                    }

                    // If more than two parameters or different descriptions, or illegal combination, generate an error
                    if (error)
                    {
                        foreach (string spid in pids)
                        {
                            int ipid = Convert.ToInt32(spid);
                            ParameterInfo pi = ParameterInfoDictionary[ipid];

                            // Types are the same, these cannot be on the same position. Generate Error
                            resultMsg.Add(new ValidationResult
                            {
                                Line = Convert.ToInt32(pi.LineNum),
                                ErrorId = 2201,
                                DescriptionFormat = "There are multiple parameters on position {0}. Parameter {1} with description {2}",
                                DescriptionParameters = new object[] { position.ToString(), pi.Pid, pi.Description },
                                TestName = "CheckPositions",
                                Severity = Severity.Major
                            });
                        }
                    }
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Checks the protocol name.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckProtocolNames(XmlDocument xDoc) // M
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();

            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            string name = null;
            // Main protocol name
            XmlNode xnProtocolName = xDoc.SelectSingleNode("./slc:Protocol/slc:Name", xmlNsm);
            if (xnProtocolName != null)
            {
                name = xnProtocolName.InnerXml;
            }

            // Exported protocols
            List<string> exportProtocolNames = new List<string>();

            // Get exported protocols from type tag
            XmlNode xnTypeOptions = xDoc.SelectSingleNode("./slc:Protocol/slc:Type/@options", xmlNsm);
            if (xnTypeOptions != null)
            {
                string options = xnTypeOptions.InnerXml;
                string[] alloptions = options.Split(';');
                foreach (string option in alloptions)
                {
                    if (option.StartsWith("exportProtocol", StringComparison.InvariantCulture))
                    {
                        string[] s = option.Split(':');
                        string exportprotocolname = s[1];
                        exportProtocolNames.Add(exportprotocolname);
                    }
                }
            }

            // Get exported protocols from DVEs/DveProtocols/DveProtocol tags
            XmlNodeList xnlDveProtocol = xDoc.SelectNodes("slc:Protocol/slc:DVEs/slc:DVEProtocols/slc:DVEProtocol", xmlNsm);
            foreach (XmlNode dveProtocol in xnlDveProtocol)
            {
                LineNum = dveProtocol.Attributes?["QA_LNx"]?.InnerXml;
                string dveName = dveProtocol.Attributes?["name"]?.InnerXml;
                exportProtocolNames.Add(dveName);
            }

            foreach (string exportProtocolName in exportProtocolNames)
            {
                List<char> badChars = CheckBadCharacters(exportProtocolName);
                if (badChars.Count > 0)
                {
                    string chars = String.Join(", ", badChars);
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 4902,
                        DescriptionFormat = "Exported protocol name {0} contains illegal characters {1}.",
                        DescriptionParameters = new object[] { exportProtocolName, chars },
                        TestName = "CheckProtocolNames",
                        Severity = Severity.Major
                    });
                }

                if (!exportProtocolName.StartsWith(name + " - ", StringComparison.InvariantCulture) || exportProtocolName.Trim() == (name + " -") || exportProtocolName.Trim() == (name + "-"))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 4903,
                        DescriptionFormat = "Exported protocol name \"{0}\" has incorrect format. Expected format is \"[Mother Protocol Name] - [Name]\"",
                        DescriptionParameters = new object[] { exportProtocolName },
                        TestName = "CheckAttributesContent",
                        Severity = Severity.Major
                    });
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Parameters with RawType double should have fixed LengthType, or be changed to numeric text.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckRawTypeDouble(XmlDocument xDoc) // M
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();

            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            XmlNodeList xnlInterprete = xDoc.SelectNodes("slc:Protocol/slc:Params/slc:Param/slc:Interprete", xmlNsm);
            foreach (XmlNode xnInterprete in xnlInterprete)
            {
                LineNum = xnInterprete.Attributes?["QA_LNx"].InnerXml;
                string lengthType = String.Empty;
                XmlNode xnRawType = xnInterprete.SelectSingleNode("./slc:RawType", xmlNsm);
                if (xnRawType == null) { continue; }

                LineNum = xnRawType.Attributes?["QA_LNx"].InnerXml;
                string rawType = xnRawType.InnerXml;

                if (!String.Equals(rawType, "double", StringComparison.OrdinalIgnoreCase)) { continue; }

                XmlNode xnLengthType = xnInterprete.SelectSingleNode("./slc:LengthType", xmlNsm);
                if (xnLengthType != null)
                {
                    LineNum = xnLengthType.Attributes?["QA_LNx"].InnerXml;
                    lengthType = xnLengthType.InnerXml;
                }

                if (String.Equals(lengthType, "next param", StringComparison.OrdinalIgnoreCase))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 2601,
                        DescriptionFormat = "RawType double is used with LengthType {0}. Change LengthType to fixed or RawType to numeric text",
                        DescriptionParameters = new object[] { lengthType },
                        TestName = "CheckSNMPRawTypeDouble",
                        Severity = Severity.Major
                    });
                }
                else if (String.Equals(lengthType, "fixed", StringComparison.OrdinalIgnoreCase))
                {
                    XmlNode xnLength = xnInterprete.SelectSingleNode("./slc:Length", xmlNsm);
                    if (xnLength == null)
                    {
                        //// Covered by 2.74.1
                        ////LineNum = xnInterprete.Attributes?["QA_LNx"].InnerXml;
                        ////resultMsg.Add(new ValidationResult
                        ////{
                        ////    Line = Convert.ToInt32(LineNum),
                        ////    ErrorId = 2603,
                        ////    DescriptionFormat = "RawType double has no length definition.",
                        ////    DescriptionParameters = null,
                        ////    TestName = "CheckSNMPRawTypeDouble",
                        ////    Severity = Severity.Major
                        ////});
                    }
                    else
                    {
                        LineNum = xnLength.Attributes?["QA_LNx"].InnerXml;
                        string sLength = xnLength.InnerXml;
                        if (sLength != "4" && sLength != "8")
                        {
                            resultMsg.Add(new ValidationResult
                            {
                                Line = Convert.ToInt32(LineNum),
                                ErrorId = 2602,
                                DescriptionFormat = "RawType double has length {0}. Length should be 4 or 8.",
                                DescriptionParameters = new object[] { sLength },
                                TestName = "CheckSNMPRawTypeDouble",
                                Severity = Severity.Major
                            });
                        }
                    }
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Checks the recursive page buttons.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckRecursivePageButtons(XmlDocument xDoc)
        {
            // Check that pageButtons are not used within a pageButton page. This crashes IE and is against design agreements.
            List<IValidationResult> resultMsg = new List<IValidationResult>();
            List<string> pbPageNames = new List<string>();

            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            // Find all pageButton pages and add to list
            // .NET framework uses XPath 1.0. upper-case/lower-case are not supported, so need to use translate to perform case-insensitive check.
            XmlNodeList xnlPageButtons = xDoc.SelectNodes("/slc:Protocol/slc:Params/slc:Param/slc:Measurement[translate(slc:Type,'PAGEBUTON','pagebuton')=\"pagebutton\"]", xmlNsm);

            foreach (XmlNode pageButton in xnlPageButtons)
            {
                LineNum = pageButton.Attributes?["QA_LNx"].InnerXml;
                XmlNodeList pageButtonValues = pageButton.SelectNodes("./slc:Discreets/slc:Discreet/slc:Value", xmlNsm);
                foreach (XmlNode pageButtonValue in pageButtonValues)
                {
                    if (pageButtonValue == null) { continue; }

                    LineNum = pageButtonValue.Attributes?["QA_LNx"].InnerXml;
                    string pbPageName = pageButtonValue.InnerXml;
                    if (!pbPageNames.Contains(pbPageName))
                    {
                        pbPageNames.Add(pbPageName);
                    }
                }
            }

            // Select all pageButton parameters
            XmlNodeList pageButtonParameters = xDoc.SelectNodes("/slc:Protocol/slc:Params/slc:Param[translate(slc:Measurement/slc:Type,'PAGEBUTON','pagebuton')=\"pagebutton\"]", xmlNsm);
            foreach (XmlNode pageButtonParameter in pageButtonParameters)
            {
                LineNum = pageButtonParameter.Attributes?["QA_LNx"]?.InnerXml;
                string id = pageButtonParameter.Attributes?["id"]?.InnerXml;

                // Get parameter position pages
                XmlNodeList xnlPages = pageButtonParameter.SelectNodes("./slc:Display/slc:Positions/slc:Position/slc:Page", xmlNsm);
                foreach (XmlNode xnPage in xnlPages)
                {
                    if (xnPage == null) { continue; }

                    LineNum = xnPage.Attributes?["QA_LNx"].InnerXml;
                    string page = xnPage.InnerXml;
                    if (pbPageNames.Contains(page))
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 3401,
                            DescriptionFormat = "PageButton Parameter {0} is included on pageButton page {1}.",
                            DescriptionParameters = new object[] { id, page },
                            TestName = "CheckRecursivePageButtons",
                            Severity = Severity.Minor
                        });
                    }
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Checks the content of the response.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckResponseContent(XmlDocument xDoc) // M
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();
            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            XmlNodeList xnlResponses = xDoc.SelectNodes("slc:Protocol/slc:Responses/slc:Response", xmlNsm);
            if (xnlResponses != null)
            {
                foreach (XmlNode xnResponse in xnlResponses)
                {
                    LineNum = xnResponse.Attributes?["QA_LNx"].InnerXml;
                    string responseId = xnResponse.Attributes?["id"].InnerXml;

                    // List of parameterInfo for all pids in response, using the same order (may contain duplicates)
                    List<ParameterInfo> responseParams = new List<ParameterInfo>();
                    XmlNodeList xnlRParams = xnResponse.SelectNodes("./slc:Content/slc:Param", xmlNsm);
                    foreach (XmlNode xnRParam in xnlRParams)
                    {
                        string pid = xnRParam?.InnerXml;

                        // ValV2 Fixed after issue found during QA of DCP97933
                        if (!String.IsNullOrEmpty(pid))
                        {
                            var paramId = Convert.ToInt32(pid);
                            if (!ParameterInfoDictionary.ContainsKey(paramId))
                            {
                                continue;
                            }

                            responseParams.Add(ParameterInfoDictionary[paramId]);
                        }
                    }

                    // Check if all parameters have fixed length
                    bool allfixed = responseParams.All(a => a.LengthType == "fixed");

                    // AllFixed = OK, no further tests needed
                    if (!allfixed)
                    {
                        bool prevNextParam = false;
                        bool prevTrailer = false;
                        string prevNpLtId = String.Empty;
                        string prevNpId = String.Empty;
                        string lengthParam = String.Empty;
                        for (int i = 0; i < responseParams.Count; i++)
                        {
                            ParameterInfo pi = responseParams[i];
                            bool b_last = i == responseParams.Count - 2;
                            bool last = i == responseParams.Count - 1;
                            if (pi.Type == "length")
                            {
                                lengthParam = Convert.ToString(pi.Pid);
                            }

                            if (pi.LengthType == "fixed")
                            {
                                if (prevNextParam)
                                {
                                    // Parameter should be fixed
                                    if (prevNpLtId == String.Empty)
                                    {
                                        if (pi.Type != "fixed" && pi.Type != "trailer")
                                        {
                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 4701,
                                                DescriptionFormat =
                                                    "Next Param {0}. in response {1} is not followed by a fixed parameter or trailer.",
                                                DescriptionParameters = new object[] { prevNpId, responseId },
                                                TestName = "CheckResponseContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                        else if (pi.Type == "fixed" || pi.Type == "trailer")
                                        {
                                            // Response part is closed by fixed param. Clear variables, response OK up to here
                                            prevNextParam = false;
                                            prevNpLtId = String.Empty;
                                            prevNpId = String.Empty;
                                        }
                                    }
                                    else // There is a lenghttype pid, loop until a fixed parameter is found
                                    {
                                        if (pi.Type == "fixed" || pi.Type == "trailer")
                                        {
                                            // Check if lengthType parameter matches
                                            if (Convert.ToString(pi.Pid) == prevNpLtId)
                                            {
                                                // Matching parameter found, clear variables, response OK up to here
                                                prevNextParam = false;
                                                prevNpLtId = String.Empty;
                                                prevNpId = String.Empty;
                                            }
                                            else // No match, throw error
                                            {
                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 4702,
                                                    DescriptionFormat =
                                                        "ID of fixed or trailer parameter {0} does not match lengthType id {1} in preceding Next Parameter ({2}) in response {3}.",
                                                    DescriptionParameters = new object[] { pi.Pid, prevNpLtId, prevNpId, responseId },
                                                    TestName = "CheckResponseContent",
                                                    Severity = Severity.Major
                                                });
                                            }
                                        }
                                        else // Not a fixed parameter
                                        {
                                            if (last) // Response is not closed by fixed parameter, throw error
                                            {
                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 4703,
                                                    DescriptionFormat = "Next Param {0} with lengthType id {1} followed by fixed length param is not followed by a fixed or trailer parameter in response {2}.",
                                                    DescriptionParameters = new object[] { prevNpId, prevNpLtId, responseId },
                                                    TestName = "CheckResponseContent",
                                                    Severity = Severity.Major
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                            else if (pi.LengthType == "next param" || pi.LengthType == "last next param")
                            {
                                if (prevNextParam)
                                {
                                    resultMsg.Add(new ValidationResult
                                    {
                                        Line = Convert.ToInt32(LineNum),
                                        ErrorId = 4704,
                                        DescriptionFormat = "Next Param {0} in response {1} is followed by another next param {2} without fixed separator.",
                                        DescriptionParameters = new object[] { prevNpId, responseId, pi.Pid },
                                        TestName = "CheckResponseContent",
                                        Severity = Severity.Major
                                    });
                                }

                                if (!last)
                                {
                                    prevNextParam = true;
                                    prevNpId = Convert.ToString(pi.Pid);
                                    if (pi.LengthTypeId != String.Empty)
                                    {
                                        prevNpLtId = pi.LengthTypeId;
                                    }
                                }
                            }

                            if (b_last && pi.Type == "trailer")
                            {
                                prevTrailer = true;
                            }

                            if (last)
                            {
                                bool check = pi.Type == "trailer" || lengthParam != String.Empty || prevTrailer && pi.Type == "crc";
                                if (!check)
                                {
                                    resultMsg.Add(new ValidationResult
                                    {
                                        Line = Convert.ToInt32(LineNum),
                                        ErrorId = 4705,
                                        DescriptionFormat = "Response {0} has no length parameter and is not closed by a trailer parameter. This will cause the communication to wait for timeout.",
                                        DescriptionParameters = new object[] { responseId },
                                        TestName = "CheckResponseContent",
                                        Severity = Severity.Minor
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Checks the table column exports.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckTableColumnExports(XmlDocument xDoc) // M
        {
            // TODO: Check on viewTables
            List<IValidationResult> resultMsg = new List<IValidationResult>();

            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            var allTables = GetAllTables(xDoc);
            foreach (XmlNode xnTable in allTables)
            {
                if (xnTable == null) { continue; }

                string tableId = xnTable.Attributes?["id"].InnerXml;
                LineNum = xnTable.Attributes?["QA_LNx"].InnerXml;

                XmlAttribute xaExports = xnTable.Attributes?["export"];
                if (xaExports == null) { continue; }

                string sExports = xaExports.InnerXml;
                string[] sTableExports = sExports.Split(';');
                for (int i = 0; i < sTableExports.Length; i++)
                {
                    if (sTableExports[i] == "true")
                    {
                        sTableExports[i] = "-1";
                    }
                }

                HashSet<string> tableExports = new HashSet<string>(sTableExports);
                tableExports.Remove("false");

                Dictionary<string, string> columns = GetColumnPids(xDoc, tableId);
                foreach (string column in columns.Keys)
                {
                    int columnPid = Convert.ToInt32(column);
                    if (!ParameterInfoDictionary.TryGetValue(columnPid, out ParameterInfo pi))
                    {
                        pi = ParameterInfoDictionary.Values.FirstOrDefault(x => x.DuplicateAs == columnPid);
                    }

                    if (pi == null)
                    {
                        // Invalid Column Pid (New validator should throw error for that)
                        continue;
                    }

                    LineNum = pi.LineNum;
                    HashSet<string> columnExports = new HashSet<string>();
                    foreach (Position cpo in pi.Positions)
                    {
                        if (cpo.Export != 0)
                        {
                            columnExports.Add(cpo.Export.ToString());
                        }
                    }

                    if (!tableExports.SetEquals(columnExports))
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 4801,
                            DescriptionFormat = "Table Column {0} has different exports from table {1}.",
                            DescriptionParameters = new object[] { column, tableId },
                            TestName = "CheckTableColumnExports",
                            Severity = Severity.Major
                        });
                    }
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Check dynamic table columns parameter are set correctly, including DVE tables.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckTableColumnParams(XmlDocument xDoc) // M
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();

            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            // Concatenate both lists
            var allTables = GetAllTables(xDoc);

            // Gather data for DVE check
            Dictionary<string, string> exportedRootTables = GetDveTables(xDoc);
            List<string> allowedTables = new List<string>(exportedRootTables.Keys);

            // Check for tables with relations to exported tables
            XmlNodeList xnlRelations = xDoc.SelectNodes("slc:Protocol/slc:Relations/slc:Relation", xmlNsm);
            foreach (XmlNode xnRel in xnlRelations)
            {
                LineNum = xnRel.Attributes?["QA_LNx"].InnerXml;
                XmlNode xnPath = xnRel.Attributes?["path"];
                if (xnPath == null) { continue; }

                string path = xnPath.InnerXml;
                string[] relTables = path.Split(';');
                bool exportfound = false;
                foreach (string s in relTables)
                {
                    if (exportfound)
                    {
                        allowedTables.Add(s);
                    }

                    if (exportedRootTables.ContainsKey(s))
                    {
                        exportfound = true;
                    }
                }
            }

            foreach (XmlNode xnParam in allTables)
            {
                LineNum = xnParam.Attributes?.GetNamedItem("QA_LNx").InnerXml;
                string sTablePid = xnParam.Attributes?.GetNamedItem("id")?.InnerXml;

                Dictionary<string, string> pidsToCheck = GetColumnPids(xDoc, sTablePid, resultMsg);

                // Check if all columns in measurement are in the table.
                HashSet<int> measPids = GetTableMeasurementPids(xnParam, false);
                foreach (int i in measPids)
                {
                    if (!pidsToCheck.Keys.Contains(i.ToString()))
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 1705,
                            DescriptionFormat = "Parameter {0} is included in the table measurement but not in the table definition.",
                            DescriptionParameters = new object[] { i },
                            TestName = "CheckTableColumnParams",
                            Severity = Severity.Major
                        });
                    }
                }

                // Check if measurement type is table
                string measType = xnParam.SelectSingleNode("./slc:Measurement/slc:Type", xmlNsm)?.InnerXml;
                LineNum = xnParam.SelectSingleNode("./slc:Measurement/slc:Type", xmlNsm)?.Attributes?["QA_LNx"].InnerXml;
                if (measType != null && !String.Equals(measType, "table", StringComparison.OrdinalIgnoreCase) && !String.Equals(measType, "matrix", StringComparison.OrdinalIgnoreCase))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 1706,
                        DescriptionFormat = "Measurement type for array is not table or matrix.",
                        DescriptionParameters = null,
                        TestName = "CheckTableColumnParams",
                        Severity = Severity.Major
                    });
                }

                // Find foreignKeys
                // Find tables with foreignKey to exported table that are not in a relation, in this case the table rows will be exported as standalone parameters.
                XmlNodeList xnlCOoptions = xnParam.SelectNodes("./slc:ArrayOptions/slc:ColumnOption/@options", xmlNsm);
                foreach (XmlNode options in xnlCOoptions)
                {
                    string sOptions = options?.InnerXml;
                    if (String.IsNullOrEmpty(sOptions)) { continue; }

                    string[] allOptions = sOptions.Split(new[] { sOptions[0] }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string option in allOptions)
                    {
                        if (option.StartsWith("foreignkey=", StringComparison.InvariantCultureIgnoreCase))
                        {
                            string fkTable = option.Substring("foreignkey=".Length);
                            if (exportedRootTables.Keys.Contains(fkTable))
                            {
                                allowedTables.Add(sTablePid);
                            }
                        }
                    }
                }

                // Check on table columns
                foreach (string sPidToCheck in pidsToCheck.Keys)
                {
                    if (Int32.TryParse(sPidToCheck, out int iPidToCheck))
                    {
                        ParameterInfo piTable = ParameterInfoDictionary[Convert.ToInt32(sTablePid)];

                        // In case sPidToCheck relates to a parameter made by the duplicateAs attribute (meant to be used in case of viewTables)
                        int iRealPidToCheck = GetRealPid(iPidToCheck);
                        if (!ParameterIdSet.Contains(Convert.ToString(iRealPidToCheck)))
                        {
                            // Retrieve correct Line Number of ColumnOption. Current LineNum is from Measurement Type Tag.
                            if (!Int32.TryParse(xnParam.SelectSingleNode("./slc:ArrayOptions/slc:ColumnOption[@pid='" + iPidToCheck + "']", xmlNsm)?.Attributes?["QA_LNx"].InnerXml, out int iTempLineNum))
                            {
                                // For some reason ColumnOption tag can't be found. Assign Table LineNumber.
                                iTempLineNum = Convert.ToInt32(piTable.LineNum);
                            }

                            continue;
                        }

                        ParameterInfo pi = ParameterInfoDictionary[iRealPidToCheck];
                        bool columnPositionAllowed = allowedTables.Contains(sTablePid);
                        bool tableDisplayedInExport = piTable.Positions.Any(x => x.Export != 0);
                        bool validPosition = pi.Positions.Any(x => x.IsValid());

                        if (!validPosition)
                        {
                            continue;
                        }

                        if (columnPositionAllowed && !tableDisplayedInExport)
                        {
                            continue;
                        }

                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(pi.LineNum),
                            ErrorId = 1701,
                            DescriptionFormat = "Table column parameter {0} should not contain Positions tag unless exported as standalone parameter.",
                            DescriptionParameters = new object[] { pi.Pid },
                            TestName = "CheckTableColumnParams",
                            Severity = Severity.Major
                        });
                    }
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Checks the table index sequence.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckTableIndexSequence(XmlDocument xDoc) // M
        {
            // Check that all table column indexes are sequential

            List<IValidationResult> resultMsg = new List<IValidationResult>();

            XmlNameTable nt = xDoc.NameTable;
            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(nt);
            xmlNsm.AddNamespace("slc", _Uri);

            // Params with arrayOptions = table
            XmlNodeList xnlArrayOptions = xDoc.GetElementsByTagName("ArrayOptions");

            foreach (XmlNode xnArrayOptions in xnlArrayOptions)
            {
                LineNum = xnArrayOptions.Attributes?["QA_LNx"]?.InnerXml;
                XmlNodeList xnlColumnOptions = xnArrayOptions.SelectNodes("slc:ColumnOption", xmlNsm);

                List<int> typePids = new List<int>();
                List<int> idxList = new List<int>();
                List<string> types = new List<string>();
                int indexTypeIdx = -1;

                if (xnArrayOptions.Attributes?["index"] != null)
                {
                    // Get column parameter id's in Type Tag
                    string id = xnArrayOptions.ParentNode?.SelectSingleNode(".//slc:Type", xmlNsm)?.Attributes?["id"]?.InnerXml;
                    if (id != null)
                    {
                        string[] typeIds = id.Split(';');
                        foreach (string s in typeIds)
                        {
                            if (Int32.TryParse(s, out int i))
                            {
                                typePids.Add(i);
                            }
                        }
                    }

                    // Get column parameter id's , indexes and types in columnOptions
                    foreach (XmlNode columnOption in xnlColumnOptions)
                    {
                        string sIdx = columnOption.Attributes?["idx"]?.InnerXml;
                        string sPid = columnOption.Attributes?["pid"]?.InnerXml;
                        string type = columnOption.Attributes?["type"]?.InnerXml;

                        if (type != null)
                        {
                            types.Add(type);
                            if (type == "index" && indexTypeIdx == -1)
                            {
                                indexTypeIdx = Convert.ToInt32(sIdx);
                            }
                        }

                        if (Int32.TryParse(sIdx, out int iIdx) && Int32.TryParse(sPid, out int iPid))
                        {
                            idxList.Add(iIdx);
                        }
                    }

                    // Perform Checks
                    bool warn = false;
                    bool error = false;

                    int typeCounter = typePids.Count;
                    for (int i = 0; i < idxList.Count - 1; i++) // Avoid out of range exception on last item
                    {
                        if (idxList[i] > idxList[i + 1])
                        {
                            error = true;
                            break;
                        }
                    }

                    for (int i = 0; i < idxList.Count; i++)
                    {
                        if (idxList[i] != i + typeCounter)
                        {
                            if (typeCounter == 0)
                            {
                                error = true;
                                break;
                            }

                            if (idxList[i] != i)
                            {
                                typeCounter--;
                            }

                            warn = true;
                        }
                    }

                    if (error)
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 3501,
                            DescriptionFormat = "Indexes of table are not sequential.",
                            DescriptionParameters = null,
                            TestName = "CheckTableIndexSequence",
                            Severity = Severity.Major
                        });
                    }
                    else if (warn)
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 3502,
                            DescriptionFormat = "Parameter indexes in Type and ColumnOptions are not consecutive. This is unconventional.",
                            DescriptionParameters = null,
                            TestName = "CheckTableIndexSequence",
                            Severity = Severity.Minor
                        });
                    }
                }

                // Check for type="index": can occur only once
                int indexCount = types.Count(a => a == "index");
                if (indexCount > 1)
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 3503,
                        DescriptionFormat = "There is more than one ColumnOption with type=\"index\"",
                        DescriptionParameters = null,
                        TestName = "CheckTableIndexSequence",
                        Severity = Severity.Major
                    });
                }
                else if (indexCount == 1)
                {
                    // Check for type="index": only allowed on SNMP and WMI tables
                    XmlNode xnSnmp = xnArrayOptions.ParentNode?.SelectSingleNode("./slc:SNMP", xmlNsm);
                    if (xnSnmp == null)
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 3504,
                            DescriptionFormat = "ColumnOption with type=\"index\" is used on a non-SNMP table",
                            DescriptionParameters = null,
                            TestName = "CheckTableIndexSequence",
                            Severity = Severity.Minor
                        });
                    }

                    // Verify that index defined in ArrayOptions matches ColumnOption with index
                    // Get index column
                    string sIndex = xnArrayOptions?.Attributes?["index"]?.InnerXml;
                    if (sIndex != indexTypeIdx.ToString())
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 3505,
                            DescriptionFormat = "ColumnOption with type=\"index\" does not match table index idx.",
                            DescriptionParameters = null,
                            TestName = "CheckTableIndexSequence",
                            Severity = Severity.Minor
                        });
                    }
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Checks the timers.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckTimers(XmlDocument xDoc)
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();

            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            XmlNodeList xnlTimers = xDoc.SelectNodes("/slc:Protocol/slc:Timers/slc:Timer", xmlNsm);

            List<string> lsTimes = new List<string>();
            foreach (XmlNode xnTimer in xnlTimers)
            {
                LineNum = xnTimer.Attributes?["QA_LNx"].InnerXml;

                // Check duplicate speeds
                string speed = xnTimer.SelectSingleNode("./slc:Time", xmlNsm)?.InnerXml;
                if (!lsTimes.Contains(speed))
                {
                    lsTimes.Add(speed);
                }

                // Check that last group is a poll group.
                XmlNode xnLastGroup = xnTimer.SelectSingleNode(".//slc:Content/slc:Group[last()]", xmlNsm);
                string sType = String.Empty;
                bool emptyTimer = false;
                if (xnLastGroup == null)
                {
                    emptyTimer = true;
                }
                else
                {
                    string groupId = xnLastGroup.InnerXml;
                    if (!String.IsNullOrWhiteSpace(groupId) && Int32.TryParse(groupId, out int iGroupId))
                    {
                        if (GroupsDictionary.TryGetValue(iGroupId, out XmlNode xnGroup))
                        {
                            XmlNode xnType = xnGroup.SelectSingleNode(".//slc:Type", xmlNsm);
                            if (xnType != null)
                            {
                                sType = xnType.InnerXml;
                            }
                            else
                            {
                                XmlNode xnContent = xnGroup.SelectSingleNode("./slc:Content", xmlNsm);
                                if (xnContent != null)
                                {
                                    int paramCounter = 0;
                                    int pairCounter = 0;
                                    int sessionCounter = 0;
                                    XmlNodeList xnlContent = xnContent.ChildNodes;
                                    int count = 0;
                                    foreach (XmlNode xnContentChild in xnlContent)
                                    {
                                        if (xnContentChild.NodeType != XmlNodeType.Comment)
                                        {
                                            count++;
                                            switch (xnContentChild.Name)
                                            {
                                                case "Param":
                                                    paramCounter++;
                                                    break;

                                                case "Pair":
                                                    pairCounter++;
                                                    break;

                                                case "Session":
                                                    sessionCounter++;
                                                    break;
                                            }
                                        }
                                    }

                                    if (paramCounter == count)
                                    {
                                        sType = "_allParams";
                                    }
                                    else if (pairCounter == count)
                                    {
                                        sType = "_allPairs";
                                    }
                                    else if (sessionCounter == count)
                                    {
                                        sType = "_allSessions";
                                    }
                                }
                            }
                        }
                    }
                }

                List<string> pollTypes = new List<string> { "poll", "poll action", "poll trigger" };

                if (!pollTypes.Contains(sType.ToLower()) && sType != "_allParams" && sType != "_allPairs" && sType != "_allSessions" && !emptyTimer)
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 3202,
                        DescriptionFormat = "The last group in the timer is not a poll group.",
                        DescriptionParameters = null,
                        TestName = "CheckTimers",
                        Severity = Severity.Minor
                    });
                }

                // Check that non-threaded timers contain no empty groups
                string timerOptions = xnTimer.Attributes?["options"]?.InnerXml?.ToLower();

                bool threaded = !String.IsNullOrEmpty(timerOptions) && timerOptions.Contains("threadpool");

                if (!threaded)
                {
                    // Get groups
                    bool empty = false;
                    List<string> emptyIDs = new List<string>();
                    XmlNodeList xnlTGroups = xnTimer.SelectNodes("./slc:Content/slc:Group", xmlNsm);
                    foreach (XmlNode xnTGroup in xnlTGroups)
                    {
                        if (xnTGroup == null) { continue; }

                        string groupId = xnTGroup.InnerXml;

                        if (!Int32.TryParse(groupId, out int iGroupId))
                        {
                            continue;
                        }

                        if (!GroupsDictionary.TryGetValue(iGroupId, out XmlNode xnGroup))
                        {
                            continue;
                        }

                        if (xnGroup == null) { continue; }

                        XmlNode xnContent = xnGroup.SelectSingleNode("./slc:Content", xmlNsm);
                        if (xnContent == null || xnContent.ChildNodes.Count == 0)
                        {
                            empty = true;
                            emptyIDs.Add(groupId);
                        }
                        else
                        {
                            foreach (XmlNode xnContentChild in xnContent.ChildNodes)
                            {
                                if (xnContentChild.NodeType == XmlNodeType.Comment) { continue; }

                                if (xnContentChild.InnerXml == String.Empty)
                                {
                                    empty = true;
                                    emptyIDs.Add(groupId);
                                    break;
                                }
                            }
                        }
                    }

                    if (empty)
                    {
                        string groups = String.Join(", ", emptyIDs);
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 3203,
                            DescriptionFormat = "Timer contains empty Group(s) {0}.",
                            DescriptionParameters = new object[] { groups },
                            TestName = "CheckTimers",
                            Severity = Severity.Major
                        });
                    }
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Check that there is no alarming or trending on write parameters.
        /// Check that parameters with explicit trending = true have RTDisplay = true.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckTrendAlarm(XmlDocument xDoc) // M
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();
            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            // Concatenate both lists
            var allTables = GetAllTables(xDoc);

            List<string> columnids = new List<string>();
            foreach (XmlNode xnTable in allTables)
            {
                string id = xnTable.Attributes?["id"].InnerXml;
                Dictionary<string, string> columns = GetColumnPids(xDoc, id);
                foreach (string s in columns.Keys)
                {
                    columnids.Add(s);
                }
            }

            foreach (ParameterInfo pi in ParameterInfoDictionary.Values)
            {
                LineNum = pi.LineNum;
                if (pi.Trended && pi.Alarmed && pi.Type.Contains("write"))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(pi.LineNum),
                        ErrorId = 2405,
                        DescriptionFormat = "Write Parameter is trended and alarmed",
                        DescriptionParameters = null,
                        TestName = "CheckTrendAlarm",
                        Severity = Severity.Major
                    });
                }
                else if (pi.Trended && pi.Type.Contains("write"))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(pi.LineNum),
                        ErrorId = 2401,
                        DescriptionFormat = "Write Parameter is trended",
                        DescriptionParameters = null,
                        TestName = "CheckTrendAlarm",
                        Severity = Severity.Major
                    });
                }
                else if (pi.Alarmed && pi.Type.Contains("write"))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(pi.LineNum),
                        ErrorId = 2402,
                        DescriptionFormat = "Write Parameter is alarmed",
                        DescriptionParameters = null,
                        TestName = "CheckTrendAlarm",
                        Severity = Severity.Major
                    });
                }

                // Check positions
                bool anyPosition = pi.Positions.Any(x => x.IsValid());

                // Check if table column
                bool displayed = pi.RTDisplay && (anyPosition || columnids.Contains(Convert.ToString(pi.Pid)));
                if (pi.Trended && pi.Alarmed && !displayed && pi.Type.Contains("read"))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(pi.LineNum),
                        ErrorId = 2406,
                        DescriptionFormat = "Parameter {0} has trending=\"true\" and is alarmed but is not displayed on any page, which is inconsistent, please verify.",
                        DescriptionParameters = new object[] { pi.Pid },
                        TestName = "CheckTrendAlarm",
                        Severity = Severity.Minor
                    });
                }
                else if (pi.Trended && !displayed && pi.Type.Contains("read"))
                {
                    string value = pi.Element.Attributes?["trending"]?.Value;

                    if (String.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
                    {
                        resultMsg.Add(new ValidationResult()
                        {
                            Line = Convert.ToInt32(pi.LineNum),
                            ErrorId = 2403,
                            DescriptionFormat = "Parameter {0} has trending=\"true\" but is not displayed on any page, which is inconsistent, please verify.",
                            DescriptionParameters = new object[] { pi.Pid },
                            TestName = "CheckTrendAlarm",
                            Severity = Severity.Major
                        });
                    }
                }
                else if (pi.Alarmed && !displayed && pi.Type.Contains("read"))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(pi.LineNum),
                        ErrorId = 2404,
                        DescriptionFormat = "Parameter {0} is alarmed but is not displayed on any page, which is inconsistent, please verify.",
                        DescriptionParameters = new object[] { pi.Pid },
                        TestName = "CheckTrendAlarm",
                        Severity = Severity.Minor
                    });
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Checks the action attributes.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <param name="resultMsg">List of results.</param>
        /// <param name="xmlNsm">The namespace.</param>
        private void CheckActionAttributes(XmlDocument xDoc, List<IValidationResult> resultMsg, XmlNamespaceManager xmlNsm)
        {
            string[] operators = { "<", ">", "==", "<=", ">=", "!=", "&lt;", "&gt;", "&lt;=", "&gt;=" };

            // Action.On => done  in CheckResponsePairGroup
            // Action.Type options: semicolon separated.
            XmlNodeList xnlActionTypes = xDoc.SelectNodes("slc:Protocol/slc:Actions/slc:Action/slc:Type[@options]", xmlNsm);
            foreach (XmlNode xnActionType in xnlActionTypes)
            {
                LineNum = xnActionType.Attributes?["QA_LNx"].InnerText;
                XmlNode xnActionTypeOptions = xnActionType.Attributes?.GetNamedItem("options");
                if (xnActionTypeOptions == null) { continue; }

                string actiontype = xnActionType.InnerXml.ToLower();
                if (actiontype == "aggregate")
                {
                    string[] aggregateOptions = { "type", "groupby", "groupbytable", "equation", "equationvalue", "return", "result", "allowvalues", "ignorevalues", "threaded", "filter", "avoidzeroinresult", "join", "status", "defaultvalue", "defaultif", "weight", "deletehistory" };
                    string[] aggregateTypes = { "pct", "avg", "max", "min", "most", "range", "stddev", "count", "sum", "most count", "meandev", "dbmv", "db", "avg extended" };
                    string sActionTypeOptions = xnActionTypeOptions.InnerXml;
                    sActionTypeOptions = WebUtility.HtmlDecode(sActionTypeOptions);
                    string[] actionTypeOptions = sActionTypeOptions.Split(';');
                    string optionName = String.Empty;
                    foreach (string actionTypeOption in actionTypeOptions)
                    {
                        bool starter = false;
                        foreach (string option in aggregateOptions)
                        {
                            if (actionTypeOption.StartsWith(option, StringComparison.InvariantCultureIgnoreCase))
                            {
                                starter = true;
                                optionName = option;
                            }
                        }

                        if (!starter)
                        {
                            resultMsg.Add(new ValidationResult
                            {
                                Line = Convert.ToInt32(LineNum),
                                ErrorId = 2901,
                                DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                DescriptionParameters = new object[] { "Action Type Aggregation", actionTypeOption },
                                TestName = "CheckAttributesContent",
                                Severity = Severity.Minor
                            });
                        }
                        else
                        {
                            string[] optionContent = actionTypeOption.Split(':');
                            switch (optionName)
                            {
                                case "type":
                                    {
                                        // Check if the aggregation type is known
                                        string aggregateType = optionContent[1].ToLower();
                                        if (!aggregateTypes.Contains(aggregateType))
                                        {
                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2902,
                                                DescriptionFormat = "Unknown or malformed {0} type {1}.",
                                                DescriptionParameters = new object[] { actiontype, aggregateType },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Minor
                                            });
                                        }
                                    }
                                    break;

                                case "groupby":
                                    {
                                        // These are column indexes: should be a number or a comma separated id list of numbers.
                                        string groupby = optionContent[1].ToLower();
                                        string[] idxs = groupby.Split(',');

                                        // Check that values are numeric
                                        if (idxs.All(a => Int32.TryParse(a, out int id))) { continue; }

                                        resultMsg.Add(new ValidationResult
                                        {
                                            Line = Convert.ToInt32(LineNum),
                                            ErrorId = 2903,
                                            DescriptionFormat = "{0} option contains an invalid separator or character. This should be a single column index or a comma separated list of column indexes.",
                                            DescriptionParameters = new object[] { optionName },
                                            TestName = "CheckAttributesContent",
                                            Severity = Severity.Major
                                        });
                                    }
                                    break;

                                case "allowValues":
                                case "ignoreValues":
                                    {
                                        // These should be a single id or a comma separated id list.
                                        string groupby = optionContent[1].ToLower();
                                        string[] values = groupby.Split(',');
                                        foreach (string value in values)
                                        {
                                            if (!value.Contains('/')) { continue; }

                                            string[] columnTest = value.Split('/');
                                            if (IsInteger(columnTest[0])) { continue; }

                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2966,
                                                DescriptionFormat = "{0} option contains an invalid separator or character. This should be comma separated list of \"idx/value\" pairs.",
                                                DescriptionParameters = new object[] { optionName },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                    }
                                    break;

                                case "return":
                                    {
                                        // These should be a single id or a comma separated id list.
                                        string returnPids = optionContent[1].ToLower();
                                        string[] pids = returnPids.Split(',');
                                        foreach (string pid in pids)
                                        {
                                            if (IsInteger(pid)) // Check if value is single number
                                            {
                                                if (ParameterIdSet.Contains(pid)) { continue; }

                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 2952,
                                                    DescriptionFormat = "Parameter with ID {0} not found.",
                                                    DescriptionParameters = new object[] { pid },
                                                    TestName = "CheckAttributesContent",
                                                    Severity = Severity.Major
                                                });
                                            }
                                            else
                                            {
                                                // Create invalid id error
                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 2904,
                                                    DescriptionFormat = "{0} option contains an invalid character. This should be a single parameter ID.",
                                                    DescriptionParameters = new object[] { optionName },
                                                    TestName = "CheckAttributesContent",
                                                    Severity = Severity.Major
                                                });
                                            }
                                        }
                                    }
                                    break;

                                case "groupbyTable":
                                case "result":
                                case "status":
                                case "weight":
                                    {
                                        // These should contain a single id .
                                        string pid = optionContent[1].ToLower();
                                        if (IsInteger(pid)) // Check if value is single number
                                        {
                                            if (ParameterIdSet.Contains(pid)) { continue; }

                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2952,
                                                DescriptionFormat = "Parameter with ID {0} not found.",
                                                DescriptionParameters = new object[] { pid },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                        else
                                        {
                                            // Create invalid id error
                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2904,
                                                DescriptionFormat = "{0} option contains an invalid character. This should be a single parameter ID.",
                                                DescriptionParameters = new object[] { optionName },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                    }
                                    break;

                                case "equation":
                                    {
                                        // Format should be Operator - comma - pid
                                        string equation = optionContent[1].ToLower();
                                        string[] operatorPid = equation.Split(',');
                                        if (operatorPid.Length == 2)
                                        {
                                            string op = operatorPid[0];
                                            string pid = operatorPid[1];
                                            if (!operators.Contains(op))
                                            {
                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 2905,
                                                    DescriptionFormat = "Equation operator {0} is invalid.",
                                                    DescriptionParameters = new object[] { op },
                                                    TestName = "CheckAttributesContent",
                                                    Severity = Severity.Major
                                                });
                                            }

                                            if (Int32.TryParse(pid, out int id)) // Check if value is single number
                                            {
                                                if (ParameterIdSet.Contains(pid)) { continue; }

                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 2952,
                                                    DescriptionFormat = "Parameter with ID {0} not found.",
                                                    DescriptionParameters = new object[] { pid },
                                                    TestName = "CheckAttributesContent",
                                                    Severity = Severity.Major
                                                });
                                            }
                                            else
                                            {
                                                // Create invalid id error
                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 2906,
                                                    DescriptionFormat = "ID {0} in equation is not correctly formatted.",
                                                    DescriptionParameters = new object[] { pid },
                                                    TestName = "CheckAttributesContent",
                                                    Severity = Severity.Major
                                                });
                                            }
                                        }
                                        else
                                        {
                                            // Format is generally incorrect
                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2907,
                                                DescriptionFormat = "Equation is not correctly formatted. Format should be \"Operator - Comma - PID\".",
                                                DescriptionParameters = null,
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                    }
                                    break;

                                case "equationvalue":
                                    {
                                        string equationvalue = optionContent[1].ToLower();
                                        string[] equationvalues = equationvalue.Split(',');
                                        if (equationvalues.Length != 4 && equationvalues.Length != 3)
                                        {
                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2904,
                                                DescriptionFormat = "{0} action {1} contains {2} parameters. 3 or 4 Parameters are expected.",
                                                DescriptionParameters = new object[] { actiontype, optionName, equationvalues.Length },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                        else
                                        {
                                            string op = equationvalues[0];
                                            ////string compareto = equationvalues[1];
                                            string pid = equationvalues[2];
                                            ////string instance = equationvalues[3];

                                            if (!operators.Contains(op))
                                            {
                                                // Create invalid operator error
                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 2905,
                                                    DescriptionFormat = "Equation operator {0} is invalid.",
                                                    DescriptionParameters = new object[] { op },
                                                    TestName = "CheckAttributesContent",
                                                    Severity = Severity.Major
                                                });
                                            }

                                            if (!ParameterIdSet.Contains(pid))
                                            {
                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 2952,
                                                    DescriptionFormat = "Parameter with ID {0} not found.",
                                                    DescriptionParameters = new object[] { pid },
                                                    TestName = "CheckAttributesContent",
                                                    Severity = Severity.Major
                                                });
                                            }
                                        }
                                    }
                                    break;

                                case "JOIN":
                                    {
                                        // These should be a a comma separated id list.
                                        string groupby = optionContent[1].ToLower();

                                        string[] ids = groupby.Split(',');
                                        if (ids.All(a => IsInteger(a)))
                                        {
                                            foreach (string id in ids)
                                            {
                                                if (ParameterIdSet.Contains(id)) { continue; }

                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 2952,
                                                    DescriptionFormat = "Parameter with ID {0} not found.",
                                                    DescriptionParameters = new object[] { id },
                                                    TestName = "CheckAttributesContent",
                                                    Severity = Severity.Major
                                                });
                                            }
                                        }
                                        else
                                        {
                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2908,
                                                DescriptionFormat = "JOIN option contains an invalid separator or character. This should be a comma separated list of ID's.",
                                                DescriptionParameters = null,
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                    }
                                    break;

                                case "threaded":
                                case "avoidzeroinresult":
                                    {
                                        if (optionContent.Length > 1)
                                        {
                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2911,
                                                DescriptionFormat = "Option {0} does not require a value.",
                                                DescriptionParameters = new object[] { optionName },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                    }
                                    break;

                                case "defaultvalue":
                                    {
                                        string[] pidValue = optionContent[1].Split(',');
                                        string spid = pidValue[0];
                                        if (Int32.TryParse(spid, out int pid))
                                        {
                                            if (ParameterIdSet.Contains(spid)) { continue; }

                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2903,
                                                DescriptionFormat = "Aggregation action {0} contains non-existing parameter id '{1}'.",
                                                DescriptionParameters = new object[] { optionName, pid },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                        else
                                        {
                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2906,
                                                DescriptionFormat = "ID {0} in merge action {1} option is not correctly formatted.",
                                                DescriptionParameters = new object[] { pid, optionName },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                    }
                                    break;

                                case "defaultif":
                                    {
                                        string[] idxValue = optionContent[1].Split(',');
                                        string idx = idxValue[0];
                                        if (IsInteger(idx)) { continue; }

                                        resultMsg.Add(new ValidationResult
                                        {
                                            Line = Convert.ToInt32(LineNum),
                                            ErrorId = 2906,
                                            DescriptionFormat = "index {0} in merge action {1} option is not correctly formatted.",
                                            DescriptionParameters = new object[] { idx, optionName },
                                            TestName = "CheckAttributesContent",
                                            Severity = Severity.Major
                                        });
                                    }
                                    break;
                            }
                        }
                    }
                }
                else if (actiontype == "merge")
                {
                    string[] mergeOptions = { "remoteelements", "trigger", "destination", "destinationfindpk", "resolve", "type", "mergeresult", "deletehistory", "defaultvalue", "defaultif", "limitresult" };
                    List<string> mergeTypes = new List<string> { "pct", "avg", "max", "min", "most", "range", "stddev", "count", "sum", "most count", "meandev", "dbmv", "db", "avg extended" };
                    string sActionTypeOptions = xnActionTypeOptions.InnerXml;
                    sActionTypeOptions = WebUtility.HtmlDecode(sActionTypeOptions);
                    string[] actionTypeOptions = sActionTypeOptions.Split(';');
                    string optionName = String.Empty;
                    foreach (string actionTypeOption in actionTypeOptions)
                    {
                        bool starter = false;
                        foreach (string option in mergeOptions)
                        {
                            if (actionTypeOption.StartsWith(option, StringComparison.InvariantCultureIgnoreCase))
                            {
                                starter = true;
                                optionName = option;
                            }
                        }

                        if (!starter)
                        {
                            resultMsg.Add(new ValidationResult
                            {
                                Line = Convert.ToInt32(LineNum),
                                ErrorId = 2901,
                                DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                DescriptionParameters = new object[] { "Action Type Merge", actionTypeOption },
                                TestName = "CheckAttributesContent",
                                Severity = Severity.Minor
                            });
                        }
                        else
                        {
                            string type = String.Empty;
                            string[] optionContent = actionTypeOption.Split(new[] { ':' }, 2);
                            switch (optionName)
                            {
                                case "type":
                                    {
                                        // Check if the aggregation type is known
                                        type = optionContent[1].ToLower();
                                        if (mergeTypes.Contains(type)) { continue; }

                                        resultMsg.Add(new ValidationResult
                                        {
                                            Line = Convert.ToInt32(LineNum),
                                            ErrorId = 2902,
                                            DescriptionFormat = "Unknown or malformed {0} type {1}.",
                                            DescriptionParameters = new object[] { actiontype, type },
                                            TestName = "CheckAttributesContent",
                                            Severity = Severity.Minor
                                        });
                                    }
                                    break;

                                case "destination":
                                    {
                                        if (type == "avg extended") // 4 column pids expected
                                        {
                                            string content = optionContent[1];
                                            if (content.Contains(','))
                                            {
                                                string[] ids = content.Split(',');
                                                if (ids.Length == 4)
                                                {
                                                    foreach (string id in ids)
                                                    {
                                                        string pid = id;

                                                        // Check for 1:200 format
                                                        if (id.Contains(':'))
                                                        {
                                                            pid = id.Split(':')[1];
                                                        }

                                                        if (ParameterIdSet.Contains(pid)) { continue; }

                                                        resultMsg.Add(new ValidationResult
                                                        {
                                                            Line = Convert.ToInt32(LineNum),
                                                            ErrorId = 2903,
                                                            DescriptionFormat = "Aggregation action destination contains non-existing parameter id '{0}'.",
                                                            DescriptionParameters = new object[] { pid },
                                                            TestName = "CheckAttributesContent",
                                                            Severity = Severity.Major
                                                        });
                                                    }
                                                }
                                                else
                                                {
                                                    resultMsg.Add(new ValidationResult
                                                    {
                                                        Line = Convert.ToInt32(LineNum),
                                                        ErrorId = 2904,
                                                        DescriptionFormat = "Aggregation action destination contains {0} parameters for avg extended aggregation type. Exactly 4 Parameters are expected.",
                                                        DescriptionParameters = new object[] { ids.Length },
                                                        TestName = "CheckAttributesContent",
                                                        Severity = Severity.Major
                                                    });
                                                }
                                            }
                                            else // Single pid expected
                                            {
                                                string pid = content;

                                                // Check for 1:200 format
                                                if (content.Contains(':'))
                                                {
                                                    pid = content.Split(':')[1];
                                                }

                                                if (ParameterIdSet.Contains(pid)) { continue; }

                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 2903,
                                                    DescriptionFormat = "Merge action {0} contains non-existing parameter id '{1}'.",
                                                    DescriptionParameters = new object[] { optionName, pid },
                                                    TestName = "CheckAttributesContent",
                                                    Severity = Severity.Major
                                                });
                                            }
                                        }
                                    }
                                    break;

                                case "limitresult": // Single pid required
                                case "remoteelements":
                                    {
                                        if (IsInteger(optionContent[1]))
                                        {
                                            if (ParameterIdSet.Contains(optionContent[1])) { continue; }

                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2903,
                                                DescriptionFormat = "Aggregation action {0} contains non-existing parameter id '{1}'.",
                                                DescriptionParameters = new object[] { optionName, optionContent[1] },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                        else
                                        {
                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2906,
                                                DescriptionFormat = "ID {0} in merge action {1} option is not correctly formatted.",
                                                DescriptionParameters = new object[] { optionContent[1], optionName },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                    }
                                    break;

                                case "destinationfindpk": // One or more pid's, comma separated
                                    {
                                        string findpkContent = optionContent[1];
                                        string[] findpkPids = findpkContent.Split(',');
                                        foreach (string findpkPid in findpkPids)
                                        {
                                            if (Int32.TryParse(optionContent[1], out int pid))
                                            {
                                                if (ParameterIdSet.Contains(findpkPid)) { continue; }

                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 2903,
                                                    DescriptionFormat = "Aggregation action {0} contains non-existing parameter id '{1}'.",
                                                    DescriptionParameters = new object[] { optionName, pid },
                                                    TestName = "CheckAttributesContent",
                                                    Severity = Severity.Major
                                                });
                                            }
                                            else
                                            {
                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 2906,
                                                    DescriptionFormat = "ID {0} in merge action {1} option is not correctly formatted.",
                                                    DescriptionParameters = new object[] { optionContent[1], optionName },
                                                    TestName = "CheckAttributesContent",
                                                    Severity = Severity.Major
                                                });
                                            }
                                        }
                                    }
                                    break;

                                case "defaultvalue":
                                    {
                                        string[] pidValue = optionContent[1].Split(',');
                                        string spid = pidValue[0];
                                        if (Int32.TryParse(spid, out int pid))
                                        {
                                            if (ParameterIdSet.Contains(spid)) { continue; }

                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2903,
                                                DescriptionFormat = "Aggregation action {0} contains non-existing parameter id '{1}'.",
                                                DescriptionParameters = new object[] { optionName, pid },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                        else
                                        {
                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2906,
                                                DescriptionFormat = "ID {0} in merge action {1} option is not correctly formatted.",
                                                DescriptionParameters = new object[] { spid, optionName },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                    }
                                    break;

                                case "defaultif":
                                    {
                                        string[] idxValue = optionContent[1].Split(',');
                                        string sidx = idxValue[0];
                                        if (!Int32.TryParse(sidx, out int idx))
                                        {
                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2906,
                                                DescriptionFormat = "index {0} in merge action {1} option is not correctly formatted.",
                                                DescriptionParameters = new object[] { sidx, optionName },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the chain attributes.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <param name="resultMsg">List of results.</param>
        /// <param name="xmlNsm">The namespace.</param>
        private void CheckChainAttributes(XmlDocument xDoc, List<IValidationResult> resultMsg, XmlNamespaceManager xmlNsm)
        {
            // Chain options: ??

            // Chain.Field options: options for CPE environment, under construction
            XmlNodeList xnlFields = xDoc.SelectNodes("slc:Protocol/slc:Chains/slc:Chain/slc:Field[@options]", xmlNsm);
            foreach (XmlNode xnField in xnlFields)
            {
                LineNum = xnField.Attributes?["QA_LNx"].InnerXml;
                XmlNode xnFieldOptions = xnField.Attributes?["options"];
                if (xnFieldOptions == null) { continue; }

                string sFieldOptions = xnFieldOptions.InnerXml.ToLower();
                if (sFieldOptions == String.Empty) { continue; }

                string[] asFieldOptions = sFieldOptions.Split(';');
                foreach (string sOption in asFieldOptions)
                {
                    string option = sOption.ToLower();
                    string starter = sOption.Split(':', '=')[0];
                    switch (starter)
                    {
                        // No details needed
                        case "displayinfilter":
                        case "hidediagramalarmcolors":
                        case "ignoreemptyfiltervalues":
                        case "noloadonfilter":
                        case "readonly":
                        case "showcpechilds":
                        case "showsiblings":
                        case "showbubbleupandinstancealarmlevel":
                        case "showtree":
                        case "tilelist":
                            break;

                        case "chain":
                        case "chainfilter":
                        case "details":
                        case "detailtabs":
                        case "displayinfiltercombo":
                        case "filter":
                        case "filtercombo":
                        case "maxdiagrampid":
                        case "statustabs":
                        case "taborder":
                        case "tabs":
                        case "topologychains":
                        case "diagramsort":
                            {
                                string[] sOptionSplit = option.Split(':');
                                if (sOptionSplit.Length < 2)
                                {
                                    resultMsg.Add(new ValidationResult
                                    {
                                        Line = Convert.ToInt32(LineNum),
                                        ErrorId = 2901,
                                        DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                        DescriptionParameters = new object[] { "Field", sOption },
                                        TestName = "CheckAttributesContent",
                                        Severity = Severity.Minor
                                    });
                                }
                            }
                            break;

                        case "filtermode":
                            {
                                string[] sOptionSplit = option.Split('=');
                                if (sOptionSplit[1] != "edit" && sOptionSplit[1] != "combo")
                                {
                                    resultMsg.Add(new ValidationResult
                                    {
                                        Line = Convert.ToInt32(LineNum),
                                        ErrorId = 2901,
                                        DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                        DescriptionParameters = new object[] { "Field", sOption },
                                        TestName = "CheckAttributesContent",
                                        Severity = Severity.Minor
                                    });
                                }
                            }
                            break;

                        case "fixedposition": // Can be empty or contain value with =
                            {
                                string[] sOptionSplit = option.Split('=');
                                if (sOptionSplit.Length > 2)
                                {
                                    resultMsg.Add(new ValidationResult
                                    {
                                        Line = Convert.ToInt32(LineNum),
                                        ErrorId = 2901,
                                        DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                        DescriptionParameters = new object[] { "Field", sOption },
                                        TestName = "CheckAttributesContent",
                                        Severity = Severity.Minor
                                    });
                                }
                            }
                            break;

                        default:
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 2901,
                                    DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                    DescriptionParameters = new object[] { "Field", sOption },
                                    TestName = "CheckAttributesContent",
                                    Severity = Severity.Minor
                                });
                            }
                            break;
                    }
                }
                break;
            }
        }

        /// <summary>
        /// Checks the pair attributes.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <param name="resultMsg">List of results.</param>
        /// <param name="xmlNsm">The namespace.</param>
        private void CheckPairAttributes(XmlDocument xDoc, List<IValidationResult> resultMsg, XmlNamespaceManager xmlNsm)
        {
            // Pair options: separator is semicolon, no leading character separator.
            XmlNodeList xnlPair = xDoc.SelectNodes("slc:Protocol/slc:Pairs/slc:Pair[@options]", xmlNsm);
            foreach (XmlNode xnPair in xnlPair)
            {
                LineNum = xnPair.Attributes?["QA_LNx"].InnerXml;
                string pairOptions = xnPair.Attributes?["options"]?.InnerXml;
                if (!String.IsNullOrEmpty(pairOptions))
                {
                    // This is a semicolon separated list
                    string[] asPairOptions = pairOptions.Split(';');

                    // Count connections used in protocol - used when checking connections, retrieved outside loop for performance
                    XmlNodeList xnPortSettings = xDoc.SelectNodes("slc:Protocol/slc:Ports/slc:PortSettings", xmlNsm);
                    int connectionsCount = xnPortSettings.Count;
                    if (xDoc.SelectSingleNode("slc:Protocol/slc:PortSettings", xmlNsm) != null)
                    {
                        connectionsCount++;
                    }

                    foreach (string pairOption in asPairOptions)
                    {
                        string[] asOptionContent = pairOption.Split(':');
                        string optionName = asOptionContent[0].ToLower();
                        switch (optionName)
                        {
                            case "onebyte":
                            case "commbreak":
                            case "receiveinterval":
                            case "retries":
                                {
                                    // A single numeric value is needed
                                    if (Int32.TryParse(asOptionContent[1], out _)) { continue; }

                                    // Create invalid id error
                                    resultMsg.Add(new ValidationResult
                                    {
                                        Line = Convert.ToInt32(LineNum),
                                        ErrorId = 2909,
                                        DescriptionFormat = "Option {0} value is not correctly formatted. A single integer value is expected.",
                                        DescriptionParameters = new object[] { optionName },
                                        TestName = "CheckAttributesContent",
                                        Severity = Severity.Major
                                    });
                                }
                                break;

                            case "connection":
                                {
                                    // A single numeric value is needed
                                    if (!Int32.TryParse(asOptionContent[1], out int connectionId)) // Check if value is single number
                                    {
                                        resultMsg.Add(new ValidationResult
                                        {
                                            Line = Convert.ToInt32(LineNum),
                                            ErrorId = 2909,
                                            DescriptionFormat = "Option {0} value is not correctly formatted. A single integer value is expected.",
                                            DescriptionParameters = new object[] { optionName },
                                            TestName = "CheckAttributesContent",
                                            Severity = Severity.Major
                                        });
                                    }

                                    if (connectionId >= connectionsCount)
                                    {
                                        resultMsg.Add(new ValidationResult
                                        {
                                            Line = Convert.ToInt32(LineNum),
                                            ErrorId = 2910,
                                            DescriptionFormat = "Connection number does not match number of ports in PortSettings.",
                                            DescriptionParameters = null,
                                            TestName = "CheckAtributesContent",
                                            Severity = Severity.Major
                                        });
                                    }
                                }
                                break;

                            case "ignoretimeout":
                                {
                                    // No further details needed
                                    if (asOptionContent.Length > 1) // MichielV: added 16/01/2014. Caused index out of bounds error without this check.
                                    {
                                        resultMsg.Add(new ValidationResult
                                        {
                                            Line = Convert.ToInt32(LineNum),
                                            ErrorId = 2911,
                                            DescriptionFormat = "Option {0} does not require a value.",
                                            DescriptionParameters = new object[] { optionName },
                                            TestName = "CheckAttributesContent",
                                            Severity = Severity.Major
                                        });
                                    }
                                }
                                break;

                            default:
                                {
                                    resultMsg.Add(new ValidationResult
                                    {
                                        Line = Convert.ToInt32(LineNum),
                                        ErrorId = 2901,
                                        DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                        DescriptionParameters = new object[] { "Pair", optionName },
                                        TestName = "CheckAttributesContent",
                                        Severity = Severity.Minor
                                    });
                                }
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the parameter attributes.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <param name="resultMsg">List of results.</param>
        /// <param name="xmlNsm">The namespace.</param>
        private void CheckParamAttributes(XmlDocument xDoc, List<IValidationResult> resultMsg, XmlNamespaceManager xmlNsm)
        {
            #region Param@options

            // Param options
            XmlNodeList xnlParam = xDoc.SelectNodes("slc:Protocol/slc:Params/slc:Param[@options]", xmlNsm);
            foreach (XmlNode xnParam in xnlParam)
            {
                LineNum = xnParam.Attributes?["QA_LNx"].InnerXml;
                XmlNode xnParamOptions = xnParam.Attributes?["options"];
                if (xnParamOptions == null) { continue; }

                string sParamOptions = xnParamOptions.InnerXml.ToLower();
                string[] paramOptions = { String.Empty, "snmpset", "snmpsetandgetwithwait", "snmpsetwithwait" };
                if (!paramOptions.Contains(sParamOptions))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 2901,
                        DescriptionFormat = "Unknown or malformed {0} option {1}.",
                        DescriptionParameters = new object[] { "Param", sParamOptions },
                        TestName = "CheckAttributesContent",
                        Severity = Severity.Minor
                    });
                }
            }

            #endregion Param@options

            #region Param.Alarm

            // Param.Alarm options: separator is semicolon. Used with linked tables
            XmlNodeList xnlParamAlarm = xDoc.SelectNodes("slc:Protocol/slc:Params/slc:Param/slc:Alarm[@*]", xmlNsm);
            foreach (XmlNode xnParamAlarm in xnlParamAlarm)
            {
                LineNum = xnParamAlarm.Attributes?["QA_LNx"].InnerXml;
                XmlNode xnParamAlarmType = xnParamAlarm.Attributes?["type"];
                if (xnParamAlarmType != null)
                {
                    string sType = xnParamAlarmType.InnerXml.ToLower();
                    string nominalId = String.Empty;
                    string multiplier = String.Empty;

                    // The type attribute may contain additional normalization settings
                    string[] typeSplit = sType.Split(':');
                    string type = typeSplit[0];
                    if (type != "nominal" && type != "absolute" && type != "relative")
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 2912,
                            DescriptionFormat = "Unknown alarm type '{0}'.",
                            DescriptionParameters = new object[] { type },
                            TestName = "CheckAttributesContent",
                            Severity = Severity.Major
                        });
                    }

                    if (typeSplit.Length == 2)
                    {
                        // Should be single pid or 2 comma separated pids
                        string sPids = typeSplit[1];
                        if (!Int32.TryParse(sPids, out int iPid))
                        {
                            string[] pids = typeSplit[1].Split(',');
                            if (pids.Length == 2)
                            {
                                nominalId = pids[0];
                                multiplier = pids[1];
                            }

                            if (!Int32.TryParse(nominalId, out iPid) || !Int32.TryParse(multiplier, out iPid))
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 2913,
                                    DescriptionFormat = "{0} attribute is not correctly formatted. Expected format is 'valuepid' or 'valuepid,multiplierpid'.",
                                    DescriptionParameters = new object[] { "Alarm Type" },
                                    TestName = "CheckAttributesContent",
                                    Severity = Severity.Major
                                });
                            }
                            else if (!ParameterIdSet.Contains(nominalId))
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 2952,
                                    DescriptionFormat = "Parameter with ID {0} not found.",
                                    DescriptionParameters = new object[] { nominalId },
                                    TestName = "CheckAttributesContent",
                                    Severity = Severity.Major
                                });
                            }
                        }
                    }
                }

                XmlNode xnActiveTime = xnParamAlarm.Attributes?["activeTime"];
                if (xnActiveTime != null)
                {
                    string sActiveTime = xnActiveTime.InnerXml;

                    if (!Int32.TryParse(sActiveTime, out int time))
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 2915,
                            DescriptionFormat = "Alarm activeTime value is not correctly formatted. Expected integer value.",
                            DescriptionParameters = null,
                            TestName = "CheckAttributesContent",
                            Severity = Severity.Major
                        });
                    }
                }

                XmlNode xnOptions = xnParamAlarm.Attributes?["options"];
                if (xnOptions != null)
                {
                    string[] asOptions = xnOptions.InnerXml.Split(';');
                    int properties = 0;
                    foreach (string sOption in asOptions)
                    {
                        char[] colon = { ':' };
                        string[] asOptionContent = sOption.Split(colon, 2);   // MVT 25/06/2014, limited to two results to handle colons in the option content.
                        string optionName = asOptionContent[0].ToLower();

                        switch (optionName)
                        {
                            case "threshold": // Two parameter id's must be specified, comma separated
                                {
                                    string[] tresholdParams = asOptionContent[1].Split(',');
                                    if (tresholdParams.All(a => Int32.TryParse(a, out int pid)))
                                    {
                                        foreach (string pId in tresholdParams)
                                        {
                                            if (ParameterIdSet.Contains(pId))
                                            {
                                                continue;   // OK
                                            }

                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2952,
                                                DescriptionFormat = "Parameter with ID {0} not found.",
                                                DescriptionParameters = new object[] { pId },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                    }
                                    else
                                    {
                                        resultMsg.Add(new ValidationResult
                                        {
                                            Line = Convert.ToInt32(LineNum),
                                            ErrorId = 2916,
                                            DescriptionFormat = "Option {0} is not correctly formatted. Expected format is 2 comma separated Parameter ID's.",
                                            DescriptionParameters = new object[] { "treshold" },
                                            TestName = "CheckAttributesContent",
                                            Severity = Severity.Major
                                        });
                                    }
                                }
                                break;

                            case "propertynames": // Property labels: one or more names separated by a comma
                                {
                                    // This is free text, no check. labels are counted to use in "properties" test.
                                    string[] names = asOptionContent[1].Split(',');
                                    properties = names.Length;
                                }
                                break;

                            case "properties":    // Format of the added properties, first character is separator(pipe as default)
                                {
                                    char splitter = asOptionContent[1][0];
                                    if (splitter != '|')    // Pipe character is the default, show a warning if another character is used.
                                    {
                                        resultMsg.Add(new ValidationResult
                                        {
                                            Line = Convert.ToInt32(LineNum),
                                            ErrorId = 2917,
                                            DescriptionFormat = "Alarm properties are split by first character '{0}'. Use of pipe character '|' is recommended.",
                                            DescriptionParameters = new object[] { splitter },
                                            TestName = "CheckAttributesContent",
                                            Severity = Severity.Minor
                                        });
                                    }

                                    string sOContent = asOptionContent[1].TrimStart(splitter);
                                    string[] asProperties = sOContent.Split(splitter);
                                    if (asProperties.Length > properties)
                                    {
                                        resultMsg.Add(new ValidationResult
                                        {
                                            Line = Convert.ToInt32(LineNum),
                                            ErrorId = 2961,
                                            DescriptionFormat = "There are more alarm properties than propertyNames.",
                                            DescriptionParameters = null,
                                            TestName = "CheckAttributesContent",
                                            Severity = Severity.Minor
                                        });
                                    }
                                    else if (asProperties.Length < properties)
                                    {
                                        resultMsg.Add(new ValidationResult
                                        {
                                            Line = Convert.ToInt32(LineNum),
                                            ErrorId = 2962,
                                            DescriptionFormat = "There are more propertyNames than alarm properties.",
                                            DescriptionParameters = null,
                                            TestName = "CheckAttributesContent",
                                            Severity = Severity.Minor
                                        });
                                    }
                                }
                                break;

                            default:
                                {
                                    resultMsg.Add(new ValidationResult
                                    {
                                        Line = Convert.ToInt32(LineNum),
                                        ErrorId = 2901,
                                        DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                        DescriptionParameters = new object[] { "Alarm", optionName },
                                        TestName = "CheckAttributesContent",
                                        Severity = Severity.Minor
                                    });
                                }
                                break;
                        }
                    }
                }
            }

            #endregion Param.Alarm

            #region Param.ArrayOptions

            // Param.ArrayOptions SNMP Index: semicolon separated

            // Param.ArrayOptions options: first character = separator
            XmlNodeList xnlParamArrayOptions = xDoc.SelectNodes("slc:Protocol/slc:Params/slc:Param/slc:ArrayOptions[@options]", xmlNsm);
            foreach (XmlNode xnArrayOptions in xnlParamArrayOptions)
            {
                LineNum = xnArrayOptions.Attributes?["QA_LNx"].InnerXml;

                string optionContent = xnArrayOptions.Attributes?["options"]?.InnerXml;
                if (String.IsNullOrWhiteSpace(optionContent))
                {
                    continue;
                }

                char[] splitter = { ';' };
                string trimmed = optionContent.Trim(splitter);

                string[] asOptions = trimmed.Split(splitter);
                foreach (string option in asOptions)
                {
                    string[] asOption = option.Split('=');
                    string optionName = asOption[0].ToLower();
                    string optionDetails = asOption.Length == 2 ? asOption[1] : String.Empty;

                    optionName = optionName.Split(':')[0];  // Quick dirty fix since some options use "=" and some ":" as separator.

                    switch (optionName)
                    {
                        case "autoadd":
                        case "customdatabasename":
                        case "database":
                        case "databasename":
                        case "databasenameprotocol":
                        case "directview":              // Detailed check to be implemented
                        case "interrupttrend":
                        case "naming":                  // Detailed check to be implemented
                        case "onlyfiltereddirectview":  // Possible check: only use if direct view is already present.
                        case "pkcaseinsensitive": // Not in documentation anymore
                        case "preserve state":
                        case "propertytable":
                        case "processingorder":
                        case "querytablepid":
                        case "resolvedpk": // Not in documentation anymore
                        case "savecolumns": // Not in documentation anymore
                        case "sizehint":
                        case "volatile":
                        case "view":            // Detailed check to be implemented
                            break;

                        case "discreetdestination":
                            if (!optionDetails.EndsWith(".xml", StringComparison.InvariantCulture))
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 2966,
                                    DescriptionFormat = "DiscreetDestination is not an xml file.",
                                    DescriptionParameters = null,
                                    TestName = "CheckAttributesContent",
                                    Severity = Severity.Major
                                });
                            }
                            break;

                        case "filterchange":
                            {
                                string[] filterpairs = optionDetails.Split(',');
                                foreach (string pair in filterpairs)
                                {
                                    string[] pids = pair.Split('-');
                                    if (pids.Length == 2)
                                    {
                                        string sLocalPid = pids[0];
                                        if (!ParameterIdSet.Contains(sLocalPid))
                                        {
                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2968,
                                                DescriptionFormat = "Local pid {0} in filterchange pair not found.",
                                                DescriptionParameters = new object[] { sLocalPid },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }

                                        string foreingPid = pids[1];
                                        if (!IsInteger(foreingPid))
                                        {
                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2969,
                                                DescriptionFormat = "Foreign pid {0} in filterchange pair is not a valid integer number.",
                                                DescriptionParameters = new object[] { foreingPid },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                    }
                                    else
                                    {
                                        resultMsg.Add(new ValidationResult
                                        {
                                            Line = Convert.ToInt32(LineNum),
                                            ErrorId = 2967,
                                            DescriptionFormat = "Filterchange pair {0} is not correctly formatted.",
                                            DescriptionParameters = new object[] { pair },
                                            TestName = "CheckAttributesContent",
                                            Severity = Severity.Major
                                        });
                                    }
                                }
                            }
                            break;

                        default:
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 2901,
                                    DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                    DescriptionParameters = new object[] { "ArrayOptions", optionName },
                                    TestName = "CheckAttributesContent",
                                    Severity = Severity.Minor
                                });
                            }
                            break;
                    }
                }
            }

            // Param.ArrayOptions.ColumnOptions
            // Value: comma separated
            // Options: first character separated
            XmlNodeList xnlColumnOption = xDoc.SelectNodes(".//slc:ColumnOption[@*]", xmlNsm);  // Select all ColumnOptions with attributes
            foreach (XmlNode xnColumnOption in xnlColumnOption)
            {
                LineNum = xnColumnOption.Attributes?["QA_LNx"].InnerXml;

                // Check type
                XmlNode xnType = xnColumnOption.Attributes?["type"];
                if (xnType != null)
                {
                    string sColumnOptionType = xnType.InnerXml.ToLower();

                    // Check if option is allowed
                    string[] columnOptionTypes = { "concatenation", "state", "autoincrement", "index", "custom", "retrieved", "snmp", "displaykey", "viewtablekey" };
                    if (!columnOptionTypes.Contains(sColumnOptionType))
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 2920,
                            DescriptionFormat = "Unknown ColumnOption Type '{0}'",
                            DescriptionParameters = new object[] { sColumnOptionType },
                            TestName = "CheckAttributesContent",
                            Severity = Severity.Major
                        });
                    }

                    // Check on retrieved columns in SNMP table (DCP 16252)
                    if (sColumnOptionType == "retrieved")
                    {
                        XmlNode param = xnColumnOption.SelectSingleNode("ancestor::slc:Param", xmlNsm);
                        string id = param?.SelectSingleNode("./slc:Type/@id", xmlNsm)?.InnerXml;
                        if (!String.IsNullOrEmpty(id))
                        {
                            XmlNode xnSnmp = param.SelectSingleNode("./slc:SNMP", xmlNsm);
                            if (xnSnmp != null)
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 2963,
                                    DescriptionFormat = "Retrieved ColumnOption in SNMP table. All columnOptions should be explicitly defined with SNMP type.",
                                    DescriptionParameters = new object[] { sColumnOptionType },
                                    TestName = "CheckAttributesContent",
                                    Severity = Severity.Major
                                });
                            }
                        }
                    }
                }

                XmlNode xnValue = xnColumnOption.Attributes?["value"];

                // Number of comma separated list of numbers.
                string sValues = xnValue?.InnerXml;
                if (!String.IsNullOrWhiteSpace(sValues))
                {
                    string[] asValues = sValues.Split(',');
                    foreach (string value in asValues)
                    {
                        if (!Int32.TryParse(value, out int iValue))
                        {
                            resultMsg.Add(new ValidationResult
                            {
                                Line = Convert.ToInt32(LineNum),
                                ErrorId = 2921,
                                DescriptionFormat = "ColumnOption attribute value not correctly formatted. Expected int or comma separated list of int values.",
                                DescriptionParameters = null,
                                TestName = "CheckAttributesContent",
                                Severity = Severity.Major
                            });
                        }
                    }
                }

                XmlNode xnOptions = xnColumnOption.Attributes?["options"];
                if (xnOptions != null)
                {
                    string sOptions = xnOptions.InnerXml;
                    if (sOptions != String.Empty)
                    {
                        char[] splitter = { sOptions[0] };
                        if (splitter[0] != ';')
                        {
                            resultMsg.Add(new ValidationResult
                            {
                                Line = Convert.ToInt32(LineNum),
                                ErrorId = 2922,
                                DescriptionFormat = "ColumnOption options are separated by first character '{0}'. Using semicolon ';' is recommended.",
                                DescriptionParameters = new object[] { splitter[0] },
                                TestName = "CheckAttributesContent",
                                Severity = Severity.Minor
                            });
                        }

                        string[] asOptions = sOptions.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string option in asOptions)
                        {
                            string[] asOptionContent = option.Split('=', ':');
                            string optionName = asOptionContent[0].ToLower();
                            switch (optionName)
                            {
                                case "cpedummycolumn":
                                case "delete":
                                case "delta":
                                case "displayicon":
                                case "displayelementalarm":
                                case "displayservicealarm":
                                case "displayviewalarm":
                                case "disableheaderavg":
                                case "disableheadermax":
                                case "disableheadermin":
                                case "disableheadersum":
                                case "disableheatmap":
                                case "disablehistogram":
                                case "dynamicdata":
                                case "enableheaderavg":
                                case "enableheadermax":
                                case "enableheadermin":
                                case "enableheadersum":
                                case "enableheatmap":
                                case "enablehistogram":
                                case "element":
                                case "foreignkey":
                                case "groupby":
                                case "hidden":
                                case "hidekpi":
                                case "hidekpiwhennotinitialized": // Not in documentation anymore
                                case "indexcolumn":
                                case "kpihidewrite":
                                case "kpishowdisplaykey": // Not in documentation anymore
                                case "linkelement":
                                case "rowtextcoloring":
                                case "save":
                                case "selectionsetvar":
                                case "selectionsetcardvar":
                                case "selectionsetpagevar":
                                case "selectionsetworkspacevar":
                                case "separator": // Not in documentation anymore
                                case "setontable":
                                case "severity":
                                case "severitycolumn":
                                case "severitycolumnindex":
                                case "showreadaskpi":
                                case "space":
                                case "subtitle":
                                case "view":
                                case "viewimpact":
                                case "volatile":
                                case "xpos": // Not in documentation anymore
                                case "ypos": // Not in documentation anymore
                                    break;

                                default:
                                    {
                                        resultMsg.Add(new ValidationResult
                                        {
                                            Line = Convert.ToInt32(LineNum),
                                            ErrorId = 2901,
                                            DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                            DescriptionParameters = new object[] { "ColumnOption", optionName },
                                            TestName = "CheckAttributesContent",
                                            Severity = Severity.Minor
                                        });
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            #endregion Param.ArrayOptions

            #region Param.CRC.Type

            // Param.CRC.Type options: separator is semicolon.
            XmlNodeList xnlParamCrcType = xDoc.SelectNodes("/slc:Protocol/slc:Param/slc:CRC/slc:Type[@options]", xmlNsm);
            foreach (XmlNode paramCrcType in xnlParamCrcType)
            {
                LineNum = paramCrcType.Attributes?["QA_LNx"]?.InnerXml;
                string options = paramCrcType.Attributes?["options"]?.InnerXml?.ToLower();
                if (String.IsNullOrEmpty(options)) { continue; }

                string[] combinations = { "ones complement", "or totaloffset", "ones complement;or totaloffset", "or totaloffset;ones complement" };
                if (!combinations.Contains(options))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 2923,
                        DescriptionFormat = "Unknown CRC Type option {0}. Expected semicolon separated combination of \"ones complement\" and \"or totaloffset\"",
                        DescriptionParameters = new object[] { options },
                        TestName = "CheckAttributesContent",
                        Severity = Severity.Major
                    });
                }
            }

            #endregion Param.CRC.Type

            #region Param.Display

            // Param.Display.Parametersview options: pipe-separated list of options.
            XmlNodeList xnlParametersView = xDoc.SelectNodes("/slc:Protocol/slc:Param/slc:Display/slc:ParametersView", xmlNsm);
            foreach (XmlNode xnParametersView in xnlParametersView)
            {
                LineNum = xnParametersView.Attributes?["QA_LNx"].InnerXml;

                XmlNode xnType = xnParametersView.Attributes?["type"];
                if (xnType != null)
                {
                    string sType = xnType.InnerXml;
                    string[] asTypes = { "column", "pie", "row", "stackedarea" };
                    if (!asTypes.Contains(sType.ToLower()))
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 2924,
                            DescriptionFormat = "Unknown ParametersView type {0}. Expected 'Column', 'Pie', 'Row' or 'StackedArea'.",
                            DescriptionParameters = new object[] { sType },
                            TestName = "CheckAttributesContent",
                            Severity = Severity.Major
                        });
                    }
                }

                XmlNode xnOptions = xnParametersView.Attributes?["options"];
                if (xnOptions != null)
                {
                    string sOptions = xnOptions.InnerXml.ToLower();
                    string[] asOptions = sOptions.Split('|');
                    foreach (string optionName in asOptions)
                    {
                        if (!optionName.StartsWith("height=", StringComparison.InvariantCultureIgnoreCase))
                        {
                            resultMsg.Add(new ValidationResult
                            {
                                Line = Convert.ToInt32(LineNum),
                                ErrorId = 2901,
                                DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                DescriptionParameters = new object[] { "ParametersView", optionName },
                                TestName = "CheckAttributesContent",
                                Severity = Severity.Minor
                            });
                        }
                    }
                }
            }

            // Param.Display.Parametersview.Parameters.Parameter options: not yet implemented

            #endregion Param.Display

            #region Param.Interprete.Type

            // Param.Interprete.Type (trim)
            XmlNodeList xnlParamInterpreteType = xDoc.SelectNodes("/slc:Protocol/slc:Param/slc:Interprete/slc:Type[@trim]", xmlNsm);
            foreach (XmlNode xnParamInterpreteType in xnlParamInterpreteType)
            {
                LineNum = xnParamInterpreteType.Attributes?["QA_LNx"].InnerXml;
                XmlNode xnTrim = xnParamInterpreteType.Attributes?["trim"];
                if (xnTrim == null) { continue; }

                string sTrim = xnTrim.InnerXml;
                string[] trims = { "left", "right", "left;right", "right;left" };
                if (!trims.Contains(sTrim.ToLower()))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 2925,
                        DescriptionFormat = "Unknown Interprete type trim {0}. Expected semicolon separated combination of 'left' and 'right'.",
                        DescriptionParameters = new object[] { sTrim },
                        TestName = "CheckAttributesContent",
                        Severity = Severity.Major
                    });
                }
            }

            #endregion Param.Interprete.Type

            // Param.Measurement.Discreets.Discreet dependency values: semicolon separated list
            // Param.Measurement.Discreets.Discreet options

            #region Param.Measurement.Type

            // Param.Measurement.Type options
            XmlNodeList xnlParamMeasurementType = xDoc.SelectNodes("/slc:Protocol/slc:Param/slc:Measurement/slc:Type[@options]", xmlNsm);
            foreach (XmlNode xnParamMeasurementType in xnlParamMeasurementType)
            {
                LineNum = xnParamMeasurementType.Attributes?["QA_LNx"].InnerXml;
                string type = xnParamMeasurementType.InnerXml;
                XmlNode xnOptions = xnParamMeasurementType.Attributes?["options"];
                string sOptions = xnOptions.InnerXml;
                sOptions = new string(sOptions.Where(c => !Char.IsWhiteSpace(c)).ToArray());

                switch (type.ToLower())
                {
                    case "number":
                        {
                            // If number: time, time:minute, time:hour
                            string[] asOptionsNumeric = { "time", "time:minute", "time:hour" };
                            if (!asOptionsNumeric.Contains(sOptions.ToLower()))
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 2926,
                                    DescriptionFormat = "Unknown option for Measurement Type number. Possible values are 'time', 'time:minute' or 'time:hour'.",
                                    DescriptionParameters = null,
                                    TestName = "CheckAttributesContent",
                                    Severity = Severity.Major
                                });
                            }
                        }
                        break;

                    case "string":
                        {
                            // If string: lines, case, options: semicolon separated
                            string[] asCaseString = { "upper", "lower" };

                            string lines = xnParamMeasurementType.Attributes?["lines"]?.InnerXml;
                            if (!String.IsNullOrEmpty(lines))
                            {
                                if (!Int32.TryParse(lines, out int iLines))
                                {
                                    resultMsg.Add(new ValidationResult
                                    {
                                        Line = Convert.ToInt32(LineNum),
                                        ErrorId = 2927,
                                        DescriptionFormat = "Attribute {0} value '{1}' is not formatted correctly. Expected integer number.",
                                        DescriptionParameters = new object[] { "Lines", lines },
                                        TestName = "CheckAttributesContent",
                                        Severity = Severity.Major
                                    });
                                }
                            }

                            string caseAttribute = xnParamMeasurementType.Attributes?["case"]?.InnerXml.ToLower();
                            if (!String.IsNullOrEmpty(caseAttribute) && !asCaseString.Contains(caseAttribute))
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 2928,
                                    DescriptionFormat = "Case value is not correctly formatted. Expected 'upper' or 'lower'.",
                                    DescriptionParameters = null,
                                    TestName = "CheckAttributesContent",
                                    Severity = Severity.Major
                                });
                            }

                            // Options tag
                            string[] asOptions = sOptions.Split(';');
                            foreach (string option in asOptions)
                            {
                                string[] asContent = option.Split('=');
                                string optionName = asContent[0].ToLower();
                                switch (optionName)
                                {
                                    case "hscroll":
                                    case "fixedfont":
                                    case "password":
                                        break;

                                    case "number":
                                        {
                                            string value = asContent[1];
                                            string[] checkValues = { "true", "false" };
                                            if (!checkValues.Contains(value))
                                            {
                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 2929,
                                                    DescriptionFormat = "Number value is not correctly formatted. Expected \"true\" or \"false\".",
                                                    DescriptionParameters = null,
                                                    TestName = "CheckAttributesContent",
                                                    Severity = Severity.Major
                                                });
                                            }
                                        }
                                        break;

                                    case "tab":
                                        {
                                            string sValues = asContent[1];
                                            if (!String.IsNullOrEmpty(sValues))
                                            {
                                                string[] asValues = sValues.Split(',');
                                                foreach (string value in asValues)
                                                {
                                                    if (Int32.TryParse(value, out int iValue)) { continue; }

                                                    resultMsg.Add(new ValidationResult
                                                    {
                                                        Line = Convert.ToInt32(LineNum),
                                                        ErrorId = 2930,
                                                        DescriptionFormat = "Tab option value not correctly formatted. Expected int or comma separated list of int values.",
                                                        DescriptionParameters = null,
                                                        TestName = "CheckAttributesContent",
                                                        Severity = Severity.Major
                                                    });
                                                }
                                            }
                                        }
                                        break;

                                    default:
                                        {
                                            resultMsg.Add(new ValidationResult
                                            {
                                                Line = Convert.ToInt32(LineNum),
                                                ErrorId = 2931,
                                                DescriptionFormat = "Unknown option {0} for Measurement Type String.",
                                                DescriptionParameters = new object[] { optionName },
                                                TestName = "CheckAttributesContent",
                                                Severity = Severity.Major
                                            });
                                        }
                                        break;
                                }
                            }
                        }
                        break;

                    case "title":
                        {
                            // If title: options: semicolon separated
                            string[] asCheckValues = { "begin", "end", "end;connect", "begin;connect" };
                            if (!asCheckValues.Contains(sOptions.ToLower()))
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 2901,
                                    DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                    DescriptionParameters = new object[] { "Measurement Type", sOptions },
                                    TestName = "CheckAttributesContent",
                                    Severity = Severity.Minor
                                });
                            }
                        }
                        break;

                    case "analog":
                        {
                            // If analog: options: hscroll
                            if (!String.Equals(sOptions, "hscroll", StringComparison.OrdinalIgnoreCase))
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 2901,
                                    DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                    DescriptionParameters = new object[] { "ArrayOptions", sOptions },
                                    TestName = "CheckAttributesContent",
                                    Severity = Severity.Minor
                                });
                            }
                        }
                        break;

                    case "table":
                        {
                            string[] asOptions = sOptions.Split('=');
                            string optionName = asOptions[0].ToLower();
                            string optionValue = asOptions[1].ToLower();

                            // Get columns in table definition
                            string tablePid = xnParamMeasurementType.ParentNode.ParentNode.Attributes?["id"].InnerXml;
                            Dictionary<string, string> columnPids = GetColumnPids(xDoc, tablePid);
                            int displayedColumns = 0;

                            if (optionName == "tab")
                            {
                                // Correct option for Type table, continue test
                                string[] asContent = optionValue.Split(',');
                                foreach (string content in asContent)
                                {
                                    string[] asInternal = content.Split(':');
                                    string internalName = asInternal[0];
                                    string internalValue = String.Empty;
                                    if (asInternal.Length == 2)
                                    {
                                        internalValue = asInternal[1];
                                    }

                                    switch (internalName)
                                    {
                                        case "columns":
                                            {
                                                string[] asColumns = internalValue.Split('-');
                                                displayedColumns = asColumns.Length;

                                                foreach (string sColumn in asColumns)
                                                {
                                                    string trimColumn = sColumn.Trim();
                                                    string[] asColumnDetails = trimColumn.Split('|');
                                                    string columnPID = asColumnDetails[0];
                                                    string columnIndex = String.Empty;
                                                    if (asColumnDetails.Length == 2)
                                                    {
                                                        columnIndex = asColumnDetails[1];
                                                    }

                                                    // Check that values are numbers. This will also cause an error if illegal separators have been used.
                                                    if (!Int32.TryParse(columnPID, out int iColumnPid))
                                                    {
                                                        resultMsg.Add(new ValidationResult
                                                        {
                                                            Line = Convert.ToInt32(LineNum),
                                                            ErrorId = 2932,
                                                            DescriptionFormat = "Column Parameter ID {0} is not correctly formatted. Expected integer number.",
                                                            DescriptionParameters = new object[] { columnPID },
                                                            TestName = "CheckAttributesContent",
                                                            Severity = Severity.Major
                                                        });
                                                    }
                                                    else
                                                    {
                                                        if (!ParameterIdSet.Contains(columnPID))
                                                        {
                                                            resultMsg.Add(new ValidationResult
                                                            {
                                                                Line = Convert.ToInt32(LineNum),
                                                                ErrorId = 2952,
                                                                DescriptionFormat = "Parameter with ID {0} not found.",
                                                                DescriptionParameters = new object[] { columnPID },
                                                                TestName = "CheckAttributesContent",
                                                                Severity = Severity.Major
                                                            });
                                                        }
                                                    }

                                                    if (columnIndex != String.Empty)
                                                    {
                                                        if (!Int32.TryParse(columnIndex, out int iColumnIndex))
                                                        {
                                                            resultMsg.Add(new ValidationResult
                                                            {
                                                                Line = Convert.ToInt32(LineNum),
                                                                ErrorId = 2933,
                                                                DescriptionFormat = "Column Index {0} is not correctly formatted. Expected integer number.",
                                                                DescriptionParameters = new object[] { columnIndex },
                                                                TestName = "CheckAttributesContent",
                                                                Severity = Severity.Major
                                                            });
                                                        }

                                                        // Check that index - pid combination is correct, and that they are in table definition.
                                                        if (columnPids.Keys.Contains(columnPID))
                                                        {
                                                            string index = String.Empty;
                                                            if (columnIndex != index)
                                                            {
                                                                // Pid-index combination is incorrect, generate an error.
                                                                resultMsg.Add(new ValidationResult
                                                                {
                                                                    Line = Convert.ToInt32(LineNum),
                                                                    ErrorId = 2934,
                                                                    DescriptionFormat = "Combination of Parameter ID {0} with table index {1} is incorrect. Check table definition.",
                                                                    DescriptionParameters = new object[] { columnPID, columnIndex },
                                                                    TestName = "CheckAttributesContent",
                                                                    Severity = Severity.Major
                                                                });
                                                            }
                                                        }
                                                        else
                                                        {
                                                            // Column pid is in column order, but not in table definition, generate an error.
                                                            resultMsg.Add(new ValidationResult
                                                            {
                                                                Line = Convert.ToInt32(LineNum),
                                                                ErrorId = 2935,
                                                                DescriptionFormat = "Parameter ID {0} not found in table definition.",
                                                                DescriptionParameters = new object[] { columnPID },
                                                                TestName = "CheckAttributesContent",
                                                                Severity = Severity.Major
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                            break;

                                        case "lines":
                                            {
                                                if (!Int32.TryParse(internalValue, out int iLines))
                                                {
                                                    resultMsg.Add(new ValidationResult
                                                    {
                                                        Line = Convert.ToInt32(LineNum),
                                                        ErrorId = 2927,
                                                        DescriptionFormat = "Attribute {0} value '{1}' is not formatted correctly. Expected integer number.",
                                                        DescriptionParameters = new object[] { "Lines", optionValue },
                                                        TestName = "CheckAttributesContent",
                                                        Severity = Severity.Major
                                                    });
                                                }
                                            }
                                            break;

                                        case "width":
                                            {
                                                string[] asWidth = internalValue.Split('-');
                                                int iWidths = asWidth.Length;

                                                // Check that length equals number of columns
                                                if (iWidths != displayedColumns)
                                                {
                                                    resultMsg.Add(new ValidationResult
                                                    {
                                                        Line = Convert.ToInt32(LineNum),
                                                        ErrorId = 2936,
                                                        DescriptionFormat = "The number of width items does not match the number of columns items.",
                                                        DescriptionParameters = null,
                                                        TestName = "CheckAttributesContent",
                                                        Severity = Severity.Major
                                                    });
                                                }

                                                // Check that all values are numeric
                                                foreach (string sWidth in asWidth)
                                                {
                                                    if (!Int32.TryParse(sWidth, out int iWidth))
                                                    {
                                                        resultMsg.Add(new ValidationResult
                                                        {
                                                            Line = Convert.ToInt32(LineNum),
                                                            ErrorId = 2927,
                                                            DescriptionFormat = "Attribute {0} value '{1}' is not formatted correctly. Expected integer number.",
                                                            DescriptionParameters = new object[] { "Width", sWidth },
                                                            TestName = "CheckAttributesContent",
                                                            Severity = Severity.Major
                                                        });
                                                    }
                                                }
                                            }
                                            break;

                                        case "sort":
                                            {
                                                string[] asSort = internalValue.Split('-');

                                                // Check that number of sort statements is less or equal than number of columns.
                                                if (asSort.Length > displayedColumns)
                                                {
                                                    resultMsg.Add(new ValidationResult
                                                    {
                                                        Line = Convert.ToInt32(LineNum),
                                                        ErrorId = 2937,
                                                        DescriptionFormat = "The number of sort statements is larger than the number of columns.",
                                                        DescriptionParameters = null,
                                                        TestName = "CheckAttributesContent",
                                                        Severity = Severity.Major
                                                    });
                                                }

                                                // Check that all values are "int" or "string".
                                                foreach (string sort in asSort)
                                                {
                                                    string sLowerCaseSort = sort.ToLower();
                                                    string[] asSortTypes = { "int", "string", "ip" };
                                                    string[] asSortOrders = { "asc", "desc" };
                                                    if (!asSortTypes.Contains(sLowerCaseSort))
                                                    {
                                                        string[] asSortContent = sLowerCaseSort.Split('|');
                                                        if (asSortContent.Length >= 2 && asSortContent.Length < 4) // Order is included
                                                        {
                                                            string sortOrder = asSortContent[1];
                                                            if (!asSortOrders.Contains(sortOrder))
                                                            {
                                                                resultMsg.Add(new ValidationResult
                                                                {
                                                                    Line = Convert.ToInt32(LineNum),
                                                                    ErrorId = 2938,
                                                                    DescriptionFormat = "Unknown or malformed sort statement '{0}'.",
                                                                    DescriptionParameters = new object[] { sort },
                                                                    TestName = "CheckAttributesContent",
                                                                    Severity = Severity.Minor
                                                                });
                                                            }

                                                            if (asSortContent.Length == 3)
                                                            {
                                                                string sortdetail = asSortContent[2];
                                                                if (!Int32.TryParse(sortdetail, out int detail))
                                                                {
                                                                    resultMsg.Add(new ValidationResult
                                                                    {
                                                                        Line = Convert.ToInt32(LineNum),
                                                                        ErrorId = 2938,
                                                                        DescriptionFormat = "Unknown or malformed sort statement '{0}'.",
                                                                        DescriptionParameters = new object[] { sort },
                                                                        TestName = "CheckAttributesContent",
                                                                        Severity = Severity.Minor
                                                                    });
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resultMsg.Add(new ValidationResult
                                                            {
                                                                Line = Convert.ToInt32(LineNum),
                                                                ErrorId = 2938,
                                                                DescriptionFormat = "Unknown or malformed sort statement '{0}'.",
                                                                DescriptionParameters = new object[] { sort },
                                                                TestName = "CheckAttributesContent",
                                                                Severity = Severity.Minor
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                            break;

                                        case "filter":
                                            {
                                                string[] asFilterValues = { "true", "false" };
                                                if (!asFilterValues.Contains(internalValue.ToLower()))
                                                {
                                                    resultMsg.Add(new ValidationResult
                                                    {
                                                        Line = Convert.ToInt32(LineNum),
                                                        ErrorId = 2939,
                                                        DescriptionFormat = "Unknown or malformed filter value '{0}'. Expected 'true' or 'false'.",
                                                        DescriptionParameters = new object[] { internalValue },
                                                        TestName = "CheckAttributesContent",
                                                        Severity = Severity.Minor
                                                    });
                                                }
                                            }
                                            break;

                                        default:
                                            {
                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 2901,
                                                    DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                                    DescriptionParameters = new object[] { "Measurement tab", internalName },
                                                    TestName = "CheckAttributesContent",
                                                    Severity = Severity.Minor
                                                });
                                            }
                                            break;
                                    }
                                }
                            }
                            else // Unknown option for type table
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 2940,
                                    DescriptionFormat = "Unknown or malformed Measurement Type option {0} for Type table.",
                                    DescriptionParameters = new object[] { optionName },
                                    TestName = "CheckAttributesContent",
                                    Severity = Severity.Minor
                                });
                            }
                        }
                        break;
                }
            }

            #endregion Param.Measurement.Type

            #region Param.SNMP.OID

            // Param.SNMP.OID options: semicolon separated
            XmlNodeList xnlParamSnmpOid = xDoc.SelectNodes("/slc:Protocol/slc:Params/slc:Param/slc:SNMP/slc:OID[@options]", xmlNsm);
            foreach (XmlNode xnParamSnmpOid in xnlParamSnmpOid)
            {
                LineNum = xnParamSnmpOid.Attributes?["QA_LNx"].InnerXml;
                XmlNode xnOptions = xnParamSnmpOid.Attributes?["options"];
                if (xnOptions != null)
                {
                    string sOptions = xnOptions.InnerXml.ToLower();
                    if (sOptions != String.Empty)
                    {
                        string trimmed = sOptions.Trim(';');
                        string[] asOptions = trimmed.Split(';');
                        foreach (string option in asOptions)
                        {
                            string[] asContent = option.Split(':');
                            string optionName = asContent[0];
                            switch (optionName)
                            {
                                case "column":
                                case "instance":
                                case "multiplegetnext":
                                case "subtable":
                                case "partialsnmp":
                                    break;

                                case "bulk":
                                case "multipleget":
                                case "multiplegetbulk":
                                    {
                                        if (asContent.Length > 1)    // Bulk options can be used without value specified.
                                        {
                                            if (!Int32.TryParse(asContent[1], out _))
                                            {
                                                resultMsg.Add(new ValidationResult
                                                {
                                                    Line = Convert.ToInt32(LineNum),
                                                    ErrorId = 2947,
                                                    DescriptionFormat = "Option {0} value '{1}' is not formatted correctly, expected integer number.",
                                                    DescriptionParameters = new object[] { optionName, asContent[1] },
                                                    TestName = "CheckAttributesContent",
                                                    Severity = Severity.Major
                                                });
                                            }
                                        }
                                    }
                                    break;

                                default:
                                    {
                                        resultMsg.Add(new ValidationResult
                                        {
                                            Line = Convert.ToInt32(LineNum),
                                            ErrorId = 2901,
                                            DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                            DescriptionParameters = new object[] { "SNMP OID", optionName },
                                            TestName = "CheckAttributesContent",
                                            Severity = Severity.Minor
                                        });
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            #endregion Param.SNMP.OID

            // Param.SNMP.TrapOid

            #region Param.Type

            // Param.Type
            // Distribution: values with colons
            XmlNodeList xnlParamTypeDistribution = xDoc.SelectNodes("/slc:Protocol/slc:Params/slc:Param/slc:Type[@distribution]", xmlNsm);
            foreach (XmlNode xnType in xnlParamTypeDistribution)
            {
                LineNum = xnType.Attributes?["QA_LNx"].InnerXml;
                XmlNode xnDistribution = xnType.Attributes?["distribution"];
                string sDistribution = xnDistribution.InnerXml;
                string[] asDistribution = sDistribution.Split(';');

                foreach (string distribution in asDistribution)
                {
                    string[] asContent = distribution.Split(':');
                    string distributionName = asContent[0];
                    switch (distributionName.ToLower())
                    {
                        case "protocol":
                        case "version":
                        case "pollingip":
                        case "pollingipport":
                        case "busaddress":
                        case "pid":
                        case "remote":
                            break;

                        default:
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 2948,
                                    DescriptionFormat = "Unknown setting {0} in Param Type distribution attribute.",
                                    DescriptionParameters = new object[] { distributionName },
                                    TestName = "CheckAttributesContent",
                                    Severity = Severity.Major
                                });
                            }
                            break;
                    }
                }
            }

            // Options:semicolon separated
            XmlNodeList xnlParamTypeOptions = xDoc.SelectNodes("/slc:Protocol/slc:Params/slc:Param/slc:Type[@options]", xmlNsm);
            foreach (XmlNode xnType in xnlParamTypeOptions)
            {
                LineNum = xnType.Attributes?["QA_LNx"].InnerXml;
                XmlNode xnOptions = xnType.Attributes?["options"];
                string sOptions = xnOptions.InnerXml;
                string[] asOptions = sOptions.Split(';');
                foreach (string option in asOptions)
                {
                    string[] asContent = option.Split('=');
                    string optionName = asContent[0];
                    switch (optionName.ToLower())
                    {
                        case "dynamic snmp get":
                        case "linkalarmvalue":
                        case "ssh username":
                        case "ssh pwd":
                        case "ssh options":
                            break;

                        case "headertrailerlink":
                            // Covered already in new validator
                            break;

                        case "loadoid":
                            {
                                if (String.Equals(xnType.InnerXml, "array", StringComparison.OrdinalIgnoreCase))
                                {
                                    resultMsg.Add(new ValidationResult
                                    {
                                        Line = Convert.ToInt32(LineNum),
                                        ErrorId = 2964,
                                        DescriptionFormat = "loadOID option is not allowed on table parameters.",
                                        DescriptionParameters = null,
                                        TestName = "CheckAttributesContent",
                                        Severity = Severity.Major
                                    });
                                }
                            }
                            break;

                        case "connection":
                            {
                                if (!Int32.TryParse(asContent[1], out int iConnection))
                                {
                                    resultMsg.Add(new ValidationResult
                                    {
                                        Line = Convert.ToInt32(LineNum),
                                        ErrorId = 2947,
                                        DescriptionFormat = "Option {0} value '{1}' is not formatted correctly, expected integer number.",
                                        DescriptionParameters = new object[] { optionName, asContent[1] },
                                        TestName = "CheckAttributesContent",
                                        Severity = Severity.Major
                                    });
                                }
                            }
                            break;

                        case "dimensions":
                            break;

                        case "columntypes":
                            break;

                        default:
                            {
                                // Filter for dynamic ip
                                string regex = @"dynamic\x20(ip\x20\d*)?";
                                if (!Regex.IsMatch(optionName, regex))
                                {
                                    // If no match, create unknown error.
                                    resultMsg.Add(new ValidationResult
                                    {
                                        Line = Convert.ToInt32(LineNum),
                                        ErrorId = 2901,
                                        DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                        DescriptionParameters = new object[] { "Parameter Type", optionName },
                                        TestName = "CheckAttributesContent",
                                        Severity = Severity.Minor
                                    });
                                }
                            }
                            break;
                    }
                }
            }

            #endregion Param.Type
        }

        /// <summary>
        /// Checks the QAction attributes.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <param name="resultMsg">List of results.</param>
        /// <param name="xmlNsm">The namespace.</param>
        private void CheckQActionAttributes(XmlDocument xDoc, List<IValidationResult> resultMsg, XmlNamespaceManager xmlNsm)
        {
            // QAction dllImport: semicolon separated -- check by XSD restriction? ([^\/:*?"<>|;]+\.dll)(;[^\/:*?"<>|;]+\.dll)* (in xsd: ([^\/:*?&quot;&gt;&lt;|;]+\.dll)(;[^\/:*?&quot;&gt;&lt;|;]+\.dll)*)
            // Get all QAction id's
            HashSet<string> qActionIds = new HashSet<string>();
            XmlNodeList xnlQAction = xDoc.SelectNodes("/slc:Protocol/slc:QActions/slc:QAction", xmlNsm);
            foreach (XmlNode xnQaction in xnlQAction)
            {
                XmlNode xnQaId = xnQaction?.Attributes?.GetNamedItem("id");
                if (xnQaId == null)
                {
                    continue; // No error generated as this should be enforced by XSD
                }

                qActionIds.Add(xnQaId.InnerXml);
            }

            XmlNodeList xnlQActionDll = xDoc.SelectNodes("/slc:Protocol/slc:QActions/slc:QAction[@dllImport]", xmlNsm);
            foreach (XmlNode xnQAction in xnlQActionDll)
            {
                LineNum = xnQAction.Attributes?["QA_LNx"].InnerXml;
                XmlNode xnDll = xnQAction.Attributes?["dllImport"];

                if (xnDll == null) { continue; }

                string sDll = xnDll.InnerXml;
                List<string> allDll = sDll.Split(';').ToList();
                foreach (string dll in allDll)
                {
                    const string ProtocolString = "[ProtocolName].[ProtocolVersion].QAction.";
                    if (dll.StartsWith(ProtocolString, StringComparison.InvariantCulture))
                    {
                        string dllid = dll.Substring(ProtocolString.Length, dll.LastIndexOf('.') - ProtocolString.Length);
                        if (qActionIds.Contains(dllid)) { continue; }

                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 2972,
                            DescriptionFormat = "Non existing QAction dll reference {0}.",
                            DescriptionParameters = new object[] { dll },
                            TestName = "CheckAttributesContent",
                            Severity = Severity.Major
                        });
                    }
                }
            }

            // QAction inputParameters
            XmlNodeList xnlQActionInputParameters = xDoc.SelectNodes("/slc:Protocol/slc:QActions/slc:QAction[@inputParameters]", xmlNsm);
            foreach (XmlNode xnQAction in xnlQActionInputParameters)
            {
                LineNum = xnQAction.Attributes?["QA_LNx"].InnerXml;
                XmlNode xnQActionInputParameter = xnQAction.Attributes?["inputParameters"];
                XmlNode xnQActionId = xnQAction.Attributes?["id"];
                string sQActionId = xnQActionId?.InnerXml;
                if (xnQActionInputParameter == null) { continue; }

                string sInputParams = xnQActionInputParameter.InnerXml;
                List<string> lInputParams = sInputParams.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (string inputParam in lInputParams)
                {
                    int inputParamPid;
                    if (!Int32.TryParse(inputParam, out inputParamPid))
                    {
                        continue;
                    }

                    // Is a parameter inside the protocol
                    if (!ParameterInfoDictionary.TryGetValue(inputParamPid, out ParameterInfo parameterInfo))
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 2971,
                            DescriptionFormat = "InputParameter parameter {0} does not exist.",
                            DescriptionParameters = new object[] { inputParam },
                            TestName = "CheckAttributesContent",
                            Severity = Severity.Major
                        });
                    }
                    else
                    {
                        // Check if it's a table
                        if (String.Equals(parameterInfo.Type, "array", StringComparison.OrdinalIgnoreCase))
                        {
                            // Check if it has an Interprete
                            if (String.IsNullOrWhiteSpace(parameterInfo.IntRawType) || String.IsNullOrWhiteSpace(parameterInfo.IntType))
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(parameterInfo.LineNum),
                                    ErrorId = 2973,
                                    DescriptionFormat = "Table ({0}), that is being used as inputParameter on QAction {1}, has no valid Interprete tag defined.",
                                    DescriptionParameters = new object[] { parameterInfo.Pid, sQActionId },
                                    TestName = "CheckAttributesContent",
                                    Severity = Severity.Critical    // This can have a lot of impact on the system.
                                });
                            }
                            else
                            {
                                // Check if it's the correct Interprete
                                if (!String.Equals(parameterInfo.IntRawType, "other", StringComparison.OrdinalIgnoreCase) || !String.Equals(parameterInfo.IntType, "double", StringComparison.OrdinalIgnoreCase))
                                {
                                    resultMsg.Add(new ValidationResult
                                    {
                                        Line = Convert.ToInt32(parameterInfo.LineNum),
                                        ErrorId = 2974,
                                        DescriptionFormat = "Table ({0}), that is being used as inputParameter on QAction {1}, has an incorrect Interprete tag defined. (Should be 'other'-'double')",
                                        DescriptionParameters = new object[] { parameterInfo.Pid, sQActionId },
                                        TestName = "CheckAttributesContent",
                                        Severity = Severity.Critical    // This can have a lot of impact on the system.
                                    });
                                }
                            }
                        }
                    }
                }
            }

            // QAction options
            XmlNodeList xnlQactionOptions = xDoc.SelectNodes("/slc:Protocol/slc:QActions/slc:QAction[@options]", xmlNsm);
            foreach (XmlNode xnQAction in xnlQactionOptions)
            {
                LineNum = xnQAction.Attributes?["QA_LNx"].InnerXml;
                XmlNode options = xnQAction.Attributes?["options"];
                if (options == null) { continue; }

                string sOptions = options.InnerXml;
                string[] asOptions = sOptions.Split(';');
                foreach (string option in asOptions)
                {
                    string[] asContent = option.Split('=');
                    string optionName = asContent[0];
                    switch (optionName.ToLower())
                    {
                        case "binary":
                        case "debug":
                        case "group":
                        case "precompile":
                        case "queued":
                        case "dllname":
                        case "":
                            break;
                        default:
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 2901,
                                    DescriptionFormat = "Unknown or malformed {0} option '{1}'.",
                                    DescriptionParameters = new object[] { "QAction", optionName },
                                    TestName = "CheckAttributesContent",
                                    Severity = Severity.Minor
                                });
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks the relation attributes.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <param name="resultMsg">List of results.</param>
        /// <param name="xmlNsm">The namespace.</param>
        private void CheckRelationAttributes(XmlDocument xDoc, List<IValidationResult> resultMsg, XmlNamespaceManager xmlNsm)
        {
            // Relations.Relation options
            XmlNodeList xnlRelationOptions = xDoc.SelectNodes("/slc:Protocol/slc:Relations/slc:Relation[@options]", xmlNsm);
            List<string> topologyNames = new List<string>();
            foreach (XmlNode xnRelation in xnlRelationOptions)
            {
                LineNum = xnRelation.Attributes?["QA_LNx"].InnerXml;
                string sOption = xnRelation.Attributes?["options"].InnerXml.ToLower();
                if (String.IsNullOrWhiteSpace(sOption)) { continue; }

                string[] asOptions = sOption.Split(':');
                if (asOptions[0] != "includeinalarms")
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 2953,
                        DescriptionFormat = "The first part in the relation option must be 'includeinalarms'.",
                        DescriptionParameters = null,
                        TestName = "CheckAttributesContent",
                        Severity = Severity.Major
                    });
                }

                if (asOptions.Length == 2)
                {
                    if (topologyNames.Contains(asOptions[1]))
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 2954,
                            DescriptionFormat = "Topology name \"{0}\" is not unique.",
                            DescriptionParameters = new object[] { asOptions[1] },
                            TestName = "CheckAttributesContent",
                            Severity = Severity.Major
                        });
                    }
                    else
                    {
                        topologyNames.Add(asOptions[1]);
                    }
                }

                if (asOptions.Length == 3 && !String.Equals(asOptions[2], "righttoplevel", StringComparison.OrdinalIgnoreCase))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 2955,
                        DescriptionFormat = "The third part in the relation option must be 'righttoplevel'.",
                        DescriptionParameters = null,
                        TestName = "CheckAttributesContent",
                        Severity = Severity.Major
                    });
                }
            }
        }

        /// <summary>
        /// Checks the response attributes.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <param name="resultMsg">List of results.</param>
        /// <param name="xmlNsm">The namespace.</param>
        private void CheckResponseAttributes(XmlDocument xDoc, List<IValidationResult> resultMsg, XmlNamespaceManager xmlNsm)
        {
            // Response options
            XmlNodeList xnlResponseOption = xDoc.SelectNodes("/slc:Protocol/slc:Responses/slc:Response[@options]", xmlNsm);
            foreach (XmlNode xnResponse in xnlResponseOption)
            {
                LineNum = xnResponse.Attributes?["QA_LNx"].InnerXml;
                string sOptions = xnResponse.Attributes?["options"]?.InnerXml;
                string sRegex = @"connection:\d+";
                if (sOptions != null && !Regex.IsMatch(sOptions.ToLower(), sRegex))
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 2956,
                        DescriptionFormat = "Response option {0} does not match format 'connection:XX'.",
                        DescriptionParameters = new object[] { sOptions },
                        TestName = "CheckAttributesContent",
                        Severity = Severity.Major
                    });
                }
            }

            // Response.Content optional
            XmlNodeList xnlResponseContentOptional = xDoc.SelectNodes(".//slc:Responses/slc:Response/slc:Content[@optional]", xmlNsm);
            foreach (XmlNode xnContent in xnlResponseContentOptional)
            {
                LineNum = xnContent.Attributes?["QA_LNx"]?.InnerXml;
                string sOptional = xnContent.Attributes?["optional"]?.InnerXml;
                string[] asOptional = sOptional?.Split(';');
                string sRegex = @"\d+\++|\d+\*?";
                int lastnr = -1;
                if (asOptional == null) { continue; }

                foreach (string optional in asOptional)
                {
                    if (!Regex.IsMatch(optional, sRegex))
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 2957,
                            DescriptionFormat = "Optional response definition {0} is not correctly formatted.",
                            DescriptionParameters = new object[] { optional },
                            TestName = "CheckAttributesContent",
                            Severity = Severity.Major
                        });
                    }

                    string sNumber = optional.Trim('*', '+');
                    if (Int32.TryParse(sNumber, out int newnr))
                    {
                        // Check if number is larger than previous
                        if (newnr <= lastnr)
                        {
                            resultMsg.Add(new ValidationResult
                            {
                                Line = Convert.ToInt32(LineNum),
                                ErrorId = 2958,
                                DescriptionFormat = "The order of optional responses is incorrect.{0} comes after {1}.",
                                DescriptionParameters = new object[] { newnr, lastnr },
                                TestName = "CheckAttributesContent",
                                Severity = Severity.Major
                            });
                        }
                    }
                    else
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 2959,
                            DescriptionFormat = "Invalid substring '{0}' in optional response definition.",
                            DescriptionParameters = new object[] { sNumber },
                            TestName = "CheckAttributesContent",
                            Severity = Severity.Major
                        });
                    }

                    lastnr = newnr;
                }
            }
        }
    }
}
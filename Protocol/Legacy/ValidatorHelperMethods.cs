namespace Skyline.DataMiner.CICD.Validators.Protocol.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    internal partial class ProtocolChecks
    {
        private const string _Uri = "http://www.skyline.be/protocol";

        /// <summary>
        /// Check if any characters not allowed as element or protocol names are present in a string.
        /// </summary>
        /// <param name="s">String to test.</param>
        /// <returns>List of not allowed characters used in the string.</returns>
        public List<char> CheckBadCharacters(string s)
        {
            char[] notAllowed = { '<', '>', ':', '"', '/', '\\', '|', '?', '*', ';', '°' };
            List<char> badChars = new List<char>();
            foreach (char ch in notAllowed)
            {
                if (s.Contains(ch))
                {
                    badChars.Add(ch);
                }
            }

            return badChars;
        }

        /// <summary>
        /// Collects the parameter information.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public void CollectParameterInfo(XmlDocument xDoc)
        {
            // Clear any old data
            ParameterInfoDictionary.Clear();
            GroupsDictionary.Clear();
            ParameterIdSet.Clear();

            // Collect new data
            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            XmlNodeList xnlParam = xDoc.SelectNodes("/slc:Protocol/slc:Params/slc:Param", xmlNsm);
            if (xnlParam != null)
            {
                foreach (XmlNode xnParam in xnlParam)
                {
                    LineNum = xnParam.Attributes["QA_LNx"].InnerXml;

                    XmlNode xnId = xnParam.Attributes.GetNamedItem("id");
                    if (xnId != null)
                    {
                        string sPid = xnParam.Attributes.GetNamedItem("id").InnerXml;
                        ParameterIdSet.Add(sPid);

                        if (Int32.TryParse(sPid, out int iPid))
                        {
                            string sName = String.Empty;
                            string sDescription = String.Empty;
                            string sType = String.Empty;
                            string sIntType = String.Empty;
                            string sIntRawType = String.Empty;
                            string sMType = String.Empty;
                            string sPage = String.Empty;
                            string sColumn = String.Empty;
                            string sRow = String.Empty;
                            string sLineNum = String.Empty;
                            string sLengthType = String.Empty;
                            string sLengthTypeId = String.Empty;

                            int? duplicateAs = null;

                            bool bRTDisplay = false;
                            bool bTrend = false;
                            bool bAlarmed = false;
                            bool bVirtualSource = false;
                            List<int> exports = new List<int>();
                            bool singleExport = false;

                            // Get exports if present
                            string sExport = xnParam.Attributes.GetNamedItem("export")?.InnerXml;
                            if (!String.IsNullOrEmpty(sExport))
                            {
                                string[] sExports = sExport.Split(';');
                                foreach (string export in sExports)
                                {
                                    if (Int32.TryParse(export, out int x))
                                    {
                                        if (!exports.Contains(x))
                                        {
                                            exports.Add(x);
                                        }
                                    }
                                    else if (export == "true")
                                    {
                                        singleExport = true;
                                    }
                                }
                            }

                            string sDuplicateAs = xnParam.Attributes.GetNamedItem("duplicateAs")?.InnerXml;
                            if (!String.IsNullOrEmpty(sExport) && Int32.TryParse(sDuplicateAs, out int d))
                            {
                                duplicateAs = d;
                            }

                            // Get name
                            XmlNode xnName = xnParam.SelectSingleNode("./slc:Name", xmlNsm);
                            if (xnName != null)
                            {
                                LineNum = xnName.Attributes["QA_LNx"].InnerXml;
                                sName = xnName.InnerXml;
                            }

                            // Get description
                            XmlNode xnDescription = xnParam.SelectSingleNode("./slc:Description", xmlNsm);
                            if (xnDescription != null)
                            {
                                LineNum = xnDescription.Attributes["QA_LNx"].InnerXml;
                                sDescription = xnDescription.InnerXml;
                            }

                            // Get parameter type + virtual attribute
                            XmlNode xnType = xnParam.SelectSingleNode("./slc:Type", xmlNsm);
                            if (xnType != null)
                            {
                                LineNum = xnType.Attributes["QA_LNx"].InnerXml;
                                sType = xnType.InnerXml.ToLower();
                                XmlNode xnVirtual = xnType.Attributes["virtual"];
                                if (xnVirtual != null)
                                {
                                    string sv = xnVirtual.InnerText;
                                    if (sv.ToLower() == "source")
                                    {
                                        bVirtualSource = true;
                                    }
                                }
                            }

                            // Get parameter RTDisplay
                            XmlNode xnRTDisplay = xnParam.SelectSingleNode("./slc:Display/slc:RTDisplay", xmlNsm);
                            if (xnRTDisplay != null)
                            {
                                LineNum = xnRTDisplay.Attributes["QA_LNx"].InnerXml;
                                bRTDisplay = Boolean.TryParse(xnRTDisplay.InnerText, out bool result) && result; // In case of empty RTDisplay tag
                            }

                            // Get trending
                            XmlNode xnTrend = xnParam.Attributes["trending"];
                            if (xnTrend != null)
                            {
                                string trending = xnTrend.InnerXml;
                                bTrend = Boolean.Parse(trending);
                            }
                            else
                            {
                                // When trending isn't defined, it takes the value from the RTDisplay tag.
                                if (bRTDisplay)
                                {
                                    bTrend = bRTDisplay;
                                }

                                // Capture write and write bit => Default false when write parameter.
                                if (sType.ToLower().Contains("write"))
                                {
                                    bTrend = false;
                                }
                            }

                            // Get interprete type
                            XmlNode xnIntType = xnParam.SelectSingleNode("./slc:Interprete/slc:Type", xmlNsm);
                            if (xnIntType != null)
                            {
                                LineNum = xnIntType.Attributes["QA_LNx"].InnerXml;
                                sIntType = xnIntType.InnerXml.ToLower();
                            }

                            // Get interprete raw type
                            XmlNode xnIntRawType = xnParam.SelectSingleNode("./slc:Interprete/slc:RawType", xmlNsm);
                            if (xnIntRawType != null)
                            {
                                LineNum = xnIntRawType.Attributes["QA_LNx"].InnerXml;
                                sIntRawType = xnIntRawType.InnerXml.ToLower();
                            }

                            // Get parameter measurement type
                            XmlNode xnMType = xnParam.SelectSingleNode("./slc:Measurement/slc:Type", xmlNsm);
                            if (xnMType != null)
                            {
                                LineNum = xnMType.Attributes["QA_LNx"].InnerXml;
                                sMType = xnMType.InnerXml.ToLower();
                            }

                            // Get parameter lengthType
                            XmlNode xnLengthType = xnParam.SelectSingleNode("./slc:Interprete/slc:LengthType", xmlNsm);
                            if (xnLengthType != null)
                            {
                                LineNum = xnLengthType.Attributes["QA_LNx"].InnerXml;
                                sLengthType = xnLengthType.InnerXml.ToLower();

                                // Get parameter lengthType ID
                                XmlNode xnLengthTypeId = xnLengthType.SelectSingleNode("./@id", xmlNsm);
                                if (xnLengthTypeId != null)
                                {
                                    sLengthTypeId = xnLengthTypeId.InnerXml;
                                }
                            }

                            // Get parameter LineNumber
                            XmlNode xnLineNum = xnParam.Attributes["QA_LNx"];
                            if (xnLineNum != null)
                            {
                                sLineNum = xnLineNum.InnerXml;
                            }

                            // Get alarm monitored
                            XmlNode xnAlarm = xnParam.SelectSingleNode("./slc:Alarm/slc:Monitored", xmlNsm);
                            if (xnAlarm != null)
                            {
                                string monitored = xnAlarm.InnerXml;
                                bAlarmed = Convert.ToBoolean(monitored);
                            }

                            // Get parameter positions
                            XmlNodeList xnlPosition = xnParam.SelectNodes("./slc:Display/slc:Positions/slc:Position", xmlNsm);
                            if (xnlPosition.Count != 0)
                            {
                                List<Position> poslist = new List<Position>();
                                foreach (XmlNode xnPosition in xnlPosition)
                                {
                                    LineNum = xnPosition.Attributes["QA_LNx"].InnerXml;
                                    XmlNode xnPage = xnPosition.SelectSingleNode("./slc:Page", xmlNsm);
                                    if (xnPage != null)
                                    {
                                        LineNum = xnPage.Attributes["QA_LNx"].InnerXml;
                                        sPage = xnPage.InnerXml;
                                    }

                                    XmlNode xnRow = xnPosition.SelectSingleNode("./slc:Row", xmlNsm);
                                    if (xnRow != null)
                                    {
                                        LineNum = xnRow.Attributes["QA_LNx"].InnerXml;
                                        sRow = xnRow.InnerXml;
                                    }

                                    XmlNode xnColumn = xnPosition.SelectSingleNode("./slc:Column", xmlNsm);
                                    if (xnColumn != null)
                                    {
                                        LineNum = xnColumn.Attributes["QA_LNx"].InnerXml;
                                        sColumn = xnColumn.InnerXml;
                                    }

                                    if (!String.IsNullOrEmpty(sPage) && !String.IsNullOrEmpty(sRow) && !String.IsNullOrEmpty(sColumn)) // Position data is OK
                                    {
                                        if (exports.Count == 0 && !singleExport)
                                        {
                                            poslist.Add(new Position(sPage, sRow, sColumn));
                                        }
                                        else
                                        {
                                            if (singleExport)
                                            {
                                                Position position = new Position
                                                {
                                                    Page = sPage,
                                                    Row = sRow,
                                                    Column = sColumn,
                                                    Export = -1
                                                };
                                                poslist.Add(position);
                                            }
                                            else
                                            {
                                                foreach (int export in exports)
                                                {
                                                    // Need to use new position, else the same object will be added to list twice.
                                                    Position position = new Position
                                                    {
                                                        Page = sPage,
                                                        Row = sRow,
                                                        Column = sColumn,
                                                        Export = export
                                                    };

                                                    poslist.Add(position);
                                                }
                                            }
                                        }
                                    }
                                    else // No position data, add positions for exports
                                    {
                                        if (singleExport)
                                        {
                                            Position position = new Position
                                            {
                                                Export = -1
                                            };
                                            poslist.Add(position);
                                        }

                                        if (exports.Count > 0)
                                        {
                                            foreach (int export in exports)
                                            {
                                                // Need to use new position, else the same object will be added to list multiple times.
                                                Position newposition = new Position
                                                {
                                                    Export = export
                                                };
                                                poslist.Add(newposition);
                                            }
                                        }
                                    }
                                }

                                Position emptyPosition = new Position();
                                while (poslist.Count > 1 && poslist.Contains(emptyPosition))
                                {
                                    poslist.Remove(emptyPosition);
                                }

                                ParameterInfo pi2 = new ParameterInfo(iPid, sName, sDescription, sType, sIntType, sIntRawType, sMType,
                                    bRTDisplay, sLineNum, sLengthType, sLengthTypeId, bTrend, bAlarmed, bVirtualSource, xnParam,
                                    poslist.ToArray());

                                pi2.DuplicateAs = duplicateAs;

                                if (ParameterInfoDictionary.ContainsKey(Convert.ToInt32(sPid)))
                                {
                                    continue;
                                }

                                ParameterInfoDictionary.Add(Convert.ToInt32(sPid), pi2);
                            }
                            else
                            {
                                List<Position> poslist = new List<Position>();

                                if (singleExport)
                                {
                                    Position position = new Position
                                    {
                                        Export = -1
                                    };
                                    poslist.Add(position);
                                }

                                if (exports.Count > 0)
                                {
                                    foreach (int export in exports)
                                    {
                                        Position newposition = new Position
                                        {
                                            Export = export
                                        };
                                        poslist.Add(newposition);
                                    }
                                }

                                ParameterInfo pi = new ParameterInfo(iPid, sName, sDescription, sType, sIntType, sIntRawType, sMType,
                                    bRTDisplay, sLineNum, sLengthType, sLengthTypeId, bTrend, bAlarmed, bVirtualSource, xnParam,
                                    poslist.ToArray());

                                pi.DuplicateAs = duplicateAs;

                                if (ParameterInfoDictionary.ContainsKey(Convert.ToInt32(sPid)))
                                {
                                    continue;
                                }

                                ParameterInfoDictionary.Add(Convert.ToInt32(sPid), pi);
                            }
                        }
                    }
                }
            }

            XmlNodeList xnlGroup = xDoc.SelectNodes("/slc:Protocol/slc:Groups/slc:Group", xmlNsm);
            foreach (XmlNode xnGroup in xnlGroup)
            {
                if (xnGroup != null)
                {
                    LineNum = xnGroup.Attributes["QA_LNx"].InnerXml;
                    string groupId = xnGroup.Attributes.GetNamedItem("id").InnerXml;

                    if (GroupsDictionary.ContainsKey(Convert.ToInt32(groupId)))
                    {
                        continue;
                    }

                    GroupsDictionary.Add(Convert.ToInt32(groupId), xnGroup);
                }
            }
        }

        /// <summary>
        /// Gets all tables.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of tables.</returns>
        public List<XmlNode> GetAllTables(XmlDocument xDoc)
        {
            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            XmlNodeList xnlTables = xDoc.SelectNodes("slc:Protocol/slc:Params/slc:Param[slc:Type='array']", xmlNsm);
            XmlNodeList xnlTables2 = xDoc.SelectNodes("slc:Protocol/slc:Params/slc:Param[slc:Type='ARRAY']", xmlNsm);
            var allTables = xnlTables.Cast<XmlNode>().Concat(xnlTables2.Cast<XmlNode>()).ToList();
            return allTables;
        }

        /// <summary>
        /// Gets the duplicated parameters.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> GetDuplicatedParams(XmlDocument xDoc) // M
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();

            DuplicateParameterDictionary.Clear();

            XmlNodeList xnlParam = xDoc.GetElementsByTagName("Param");
            foreach (XmlNode xnParam in xnlParam)
            {
                LineNum = xnParam.Attributes?["QA_LNx"]?.InnerXml;
                string sPid = xnParam.Attributes?.GetNamedItem("id")?.InnerText;

                if (Int32.TryParse(sPid, out int iPid))
                {
                    string sDuplicateAs = xnParam.Attributes?.GetNamedItem("duplicateAs")?.InnerText;
                    if (!String.IsNullOrEmpty(sDuplicateAs))
                    {
                        string[] asDuplicateAsPids = sDuplicateAs.Split(',');
                        foreach (string sDuplicateAsPid in asDuplicateAsPids)
                        {
                            if (Int32.TryParse(sDuplicateAsPid, out int iDuplicateAsPid))
                            {
                                DuplicateParameterDictionary.Add(iDuplicateAsPid, iPid);
                            }
                            else
                            {
                                resultMsg.Add(new ValidationResult
                                {
                                    Line = Convert.ToInt32(LineNum),
                                    ErrorId = 1001,
                                    DescriptionFormat = "Internal Application Error : Error in {0}. Could not parse sDuplicateAsPid '{1}' to an integer.",
                                    DescriptionParameters = new object[] { "GetDuplicatedParams", sDuplicateAsPid },
                                    TestName = "GetDuplicatedParams",
                                    Severity = Severity.Critical
                                });
                            }
                        }
                    }
                }
            }

            return resultMsg;
        }

        /// <summary>
        /// Gets the real pid.
        /// </summary>
        /// <param name="iDuplicatedPid">The duplicated pid.</param>
        /// <returns>The real pid.</returns>
        public int GetRealPid(int iDuplicatedPid)
        {
            if (!DuplicateParameterDictionary.TryGetValue(iDuplicatedPid, out int iRealPid))
            {
                iRealPid = iDuplicatedPid;
            }

            return iRealPid;
        }

        /// <summary>
        /// Determines whether the specified string is an <see cref="Int32"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>
        ///   <c>true</c> if the specified string is an integer; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInteger(string s)
        {
            return Int32.TryParse(s, out int i);
        }

        /// <summary>
        /// Loads the line numbers.
        /// </summary>
        /// <returns>The protocol document.</returns>
        public XmlDocument LoadWithLineNums(string xml) // S
        {
            // Add line numbers
            XDocument xInput = XDocument.Parse(xml, LoadOptions.SetLineInfo);
            XElement root = xInput.Root;
            XNamespace skylineProtocol = "http://www.skyline.be/protocol";

            // Add root namespace if needed
            if (root.GetDefaultNamespace() != skylineProtocol)
            {
                root.Name = skylineProtocol.GetName(root.Name.LocalName);
                foreach (XElement el in root.Descendants())
                {
                    if (el.GetDefaultNamespace() != skylineProtocol)
                    {
                        el.Name = skylineProtocol.GetName(el.Name.LocalName);
                    }

                    if (el.NodeType == XmlNodeType.Element)
                    {
                        // Add lineInfo
                        IXmlLineInfo ili = el;
                        if (ili.HasLineInfo())
                        {
                            int line = ili.LineNumber;
                            XAttribute xline = new XAttribute("QA_LNx", line);
                            el.Add(xline);
                        }
                    }
                }
            }
            else
            {
                foreach (XElement xni in xInput.Descendants())
                {
                    if (xni.NodeType == XmlNodeType.Element)
                    {
                        // Add lineInfo
                        IXmlLineInfo ili = xni;
                        if (ili.HasLineInfo())
                        {
                            int line = ili.LineNumber;
                            XAttribute xline = new XAttribute("QA_LNx", line);
                            xni.Add(xline);
                        }
                    }
                }
            }

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xInput.ToString());

            return xDoc;
        }

        /// <summary>
        /// Returns a Dictionary with (PID, index) pairs. For columns in the type tag, indexes are auto-incremented, skipping indexes defined in ColumnOptions.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <param name="tablePid">Table Parameter ID.</param>
        /// <param name="resultMsg">Optional resultMsg for errors, should only be used when calling from 17xx tests.</param>
        /// <returns>Dictionary with (PID, index) pairs.</returns>
        private Dictionary<string, string> GetColumnPids(XmlDocument xDoc, string tablePid, List<IValidationResult> resultMsg = default(List<IValidationResult>))
        {
            Dictionary<string, string> columnPids = new Dictionary<string, string>();

            // Add xmlNameSpaceManager
            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            XmlNode xnParam = ParameterInfoDictionary[Convert.ToInt32(tablePid)].Element;

            if (xnParam == null) { return columnPids; }

            LineNum = xnParam.Attributes?["QA_LNx"].InnerXml;

            // Add id's in ColumnOption tags to pidsToCheck List
            XmlNodeList xnlColumnOptions = xnParam.SelectNodes(".//slc:ColumnOption", xmlNsm);
            foreach (XmlNode xnColumnOption in xnlColumnOptions)
            {
                LineNum = xnColumnOption.Attributes?["QA_LNx"].InnerXml;
                string columnOptionPid = xnColumnOption.Attributes?["pid"]?.InnerXml;
                if (columnOptionPid == null) { continue; }

                string columnOptionIdx = xnColumnOption.Attributes?["idx"]?.InnerXml;
                if (columnOptionIdx == null) { continue; }

                if (!columnPids.Keys.Contains(columnOptionPid))
                {
                    columnPids.Add(columnOptionPid, columnOptionIdx);
                }
                else if (resultMsg != default(List<IValidationResult>)) // Generate error only once
                {
                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(LineNum),
                        ErrorId = 1703,
                        DescriptionFormat = "Table Column Parameter {0} is added to table {1} more than once.",
                        DescriptionParameters = new object[] { columnOptionPid, tablePid },
                        TestName = "CheckTableColumnParams",
                        Severity = Severity.Critical
                    });
                }
            }

            // Add id's in type tag to columnPids List
            XmlNode xnType = xnParam.SelectSingleNode("slc:Type", xmlNsm);
            if (xnType != null && String.Equals(xnType.InnerXml, "array", StringComparison.OrdinalIgnoreCase) && xnType.Attributes?["id"] != null)
            {
                LineNum = xnType.Attributes?["QA_LNx"].InnerXml;
                string sId = xnType.Attributes["id"].InnerXml;
                string[] ids = sId.Split(';');
                int counter = 0;
                foreach (string id in ids)
                {
                    if (String.IsNullOrEmpty(id)) { continue; }

                    while (columnPids.Values.Contains(counter.ToString()))
                    {
                        counter++;
                    }

                    if (!columnPids.Keys.Contains(id))
                    {
                        columnPids.Add(id, counter.ToString());
                    }
                    else if (resultMsg != default(List<IValidationResult>))
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(LineNum),
                            ErrorId = 1703,
                            DescriptionFormat = "Table Column Parameter {0} is added to table {1} more than once.",
                            DescriptionParameters = new object[] { id, tablePid },
                            TestName = "CheckTableColumnParams",
                            Severity = Severity.Critical
                        });
                    }

                    counter++;
                }
            }

            return columnPids;
        }

        /// <summary>
        /// Get a list of exported table pids for main tables.
        /// </summary>
        /// <param name="xDoc">Skyline DataMiner protocol XmlDocument.</param>
        /// <returns>List of exported tables.</returns>
        private Dictionary<string, string> GetDveTables(XmlDocument xDoc) // M
        {
            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);

            Dictionary<string, string> exportedTables = new Dictionary<string, string>();

            XmlNodeList xnlType = xDoc.GetElementsByTagName("Type");
            XmlNode xnType = xnlType.Item(0);
            LineNum = xnType?.Attributes?["QA_LNx"]?.InnerXml;
            string typeOptions = xnType?.Attributes?.GetNamedItem("options")?.InnerXml;

            if (!String.IsNullOrEmpty(typeOptions) && typeOptions.Contains("exportProtocol"))
            {
                string[] options = typeOptions.Split(';');

                foreach (string option in options)
                {
                    string[] optionDetails = option.Split(':');
                    if (optionDetails.Length >= 3 && optionDetails[0] == "exportProtocol")
                    {
                        exportedTables.Add(optionDetails[2], optionDetails[1]);
                    }
                }
            }

            XmlNodeList xnlDveProtocol = xDoc.SelectNodes("slc:Protocol/slc:DVEs/slc:DVEProtocols/slc:DVEProtocol", xmlNsm);
            foreach (XmlNode dveProtocol in xnlDveProtocol)
            {
                LineNum = dveProtocol.Attributes?["QA_LNx"]?.InnerXml;
                string name = dveProtocol.Attributes?["name"]?.InnerXml;
                string tablePid = dveProtocol.Attributes?["tablePID"]?.InnerXml;
                if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(tablePid))
                {
                    exportedTables.Add(tablePid, name);
                }
            }

            return exportedTables;
        }

        /// <summary>
        /// Gets the inner XML or CData.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Inner XML as string.</returns>
        private string GetInnerXmlOrCData(XmlNode node)
        {
            // Get string or CDATA content
            string content = String.Empty;
            if (!node.HasChildNodes) // No child nodes, just return innerXML
            {
                return node.InnerXml;
            }

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Comment)
                {
                    continue; // We're not interested in comments
                }

                if (child.NodeType == XmlNodeType.CDATA)
                {
                    content = child.InnerText;
                    break; // First CDATA node will be used (there should only be one)
                }
                else // No CDATA content, just read innerXml
                {
                    content = node.InnerXml;
                    break;
                }
            }

            return content;
        }

        /// <summary>
        /// Gets the read pid for a write pid.
        /// </summary>
        /// <param name="writePid">Write parameter id.</param>
        /// <returns>Corresponding read parameter id, -1 if no read parameter id is found.</returns>
        private int GetReadPid(int writePid)
        {
            int readPid = -1; // Default if no write exists.
            string writeDescription = ParameterInfoDictionary[writePid].Description;
            ParameterInfo piRead = ParameterInfoDictionary.Values.FirstOrDefault(pi => pi.Description == writeDescription && pi.Type == "read");
            if (piRead != null)
            {
                readPid = Convert.ToInt32(piRead.Pid);
            }

            return readPid;
        }

        /// <summary>
        /// Gets the table foreign keys.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="xmlNsm">The namespace.</param>
        /// <returns>List of table ids.</returns>
        private List<int> GetTableForeignKeys(XmlNode table, XmlNamespaceManager xmlNsm)
        {
            List<int> keys = new List<int>();

            // Return empty list immediately if node is not an element
            if (table.NodeType != XmlNodeType.Element)
            {
                return keys;
            }

            // Find foreignKeys
            // Find tables with foreignKey to exported table that are not in a relation, in this case the table will be exported.
            XmlNodeList xnlCOoptions = table.SelectNodes("./slc:ArrayOptions/slc:ColumnOption/@options", xmlNsm);
            if (xnlCOoptions != null)
            {
                foreach (XmlNode options in xnlCOoptions)
                {
                    string sOptions = options.InnerXml;
                    if (!String.IsNullOrEmpty(sOptions))
                    {
                        string[] asAllOptions = sOptions.Split(new[] { sOptions[0] }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string option in asAllOptions)
                        {
                            if (option.StartsWith("foreignkey=", StringComparison.InvariantCultureIgnoreCase))
                            {
                                string fkTable = option.Substring("foreignkey=".Length);
                                if (Int32.TryParse(fkTable, out int iTable))
                                {
                                    keys.Add(iTable);
                                }
                            }
                        }
                    }
                }
            }

            return keys;
        }

        /// <summary>
        /// Gets the table measurement pids.
        /// </summary>
        /// <param name="tableParam">The table parameter.</param>
        /// <param name="bDuplicatedAsToMain">If set to <c>true</c> [Duplicated as to main].</param>
        /// <returns>HashSet of Pids.</returns>
        private HashSet<int> GetTableMeasurementPids(XmlNode tableParam, bool bDuplicatedAsToMain = true)
        {
            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(tableParam.OwnerDocument.NameTable);
            xmlNsm.AddNamespace("slc", _Uri);
            HashSet<int> result = new HashSet<int>();

            // Get all column parameters in measurement
            XmlAttribute measurementTypeOptions = (XmlAttribute)tableParam.SelectSingleNode("./slc:Measurement/slc:Type/@options", xmlNsm);
            if (measurementTypeOptions == null)
            {
                return result;
            }

            LineNum = measurementTypeOptions.OwnerElement?.Attributes["QA_LNx"].InnerXml;
            string options = measurementTypeOptions.InnerXml;
            options = options.TrimStart(';');
            string[] asOptions = options.Split(';');
            foreach (string s in asOptions)
            {
                const string Tab = "tab=";
                if (s.StartsWith(Tab, StringComparison.InvariantCulture))
                {
                    string taboption = s.Substring(s.IndexOf(Tab, StringComparison.InvariantCulture) + Tab.Length);
                    string[] asTabOptions = taboption.Split(',');
                    foreach (string stab in asTabOptions)
                    {
                        const string Col = "columns:";
                        if (stab.StartsWith(Col, StringComparison.InvariantCulture))
                        {
                            string columns = stab.Substring(stab.IndexOf(Col, StringComparison.InvariantCulture) + Col.Length);
                            string[] asColumns = columns.Split('-');
                            foreach (string column in asColumns)
                            {
                                string sPid;
                                if (column.Contains('|'))
                                {
                                    // Format PID|IDX is used
                                    sPid = column.Substring(0, column.IndexOf('|'));
                                }
                                else
                                {
                                    // Content is parameter id
                                    sPid = column;
                                }

                                if (Int32.TryParse(sPid, out int iPid))
                                {
                                    int iRealPid;
                                    if (bDuplicatedAsToMain)
                                    {
                                        // If parameter id a duplicated parameter, run check on original parameter
                                        iRealPid = GetRealPid(iPid);
                                    }
                                    else
                                    {
                                        iRealPid = iPid;
                                    }

                                    result.Add(iRealPid);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the write pid for a read pid.
        /// </summary>
        /// <param name="readPid">Read parameter id.</param>
        /// <returns>Corresponding write parameter id, -1 if no write parameter id is found.</returns>
        private int GetWritePid(int readPid)
        {
            int writePid = -1; // Default if no write exists.
            if (ParameterInfoDictionary.ContainsKey(readPid))
            {
                string readDescription = ParameterInfoDictionary[readPid].Description;
                ParameterInfo piWrite = ParameterInfoDictionary.Values.FirstOrDefault(pi => pi.Description == readDescription && pi.Type == "write");
                if (piWrite != null)
                {
                    writePid = Convert.ToInt32(piWrite.Pid);
                }
            }

            return writePid;
        }
    }
}
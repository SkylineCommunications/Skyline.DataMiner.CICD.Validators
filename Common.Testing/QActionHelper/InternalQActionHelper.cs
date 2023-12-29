namespace Common.Testing.QActionHelper
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml.Linq;
	using Skyline.DataMiner.Scripting;

	/// <summary>
    /// QActionHelper without the logging to a log file.
    /// </summary>
    public static class InternalQActionHelper
    {
        #region "Public Methods"
        
        /// <summary>
        /// Helper method for DIS.
        /// </summary>
        /// <param name="xmlProtocolContent">XML Protocol as a string.</param>
        /// <param name="sourceCode">Resulting source code of helper. <see langword="null"/> if helper could not be created.</param>
        /// <returns>List with XMLParseError objects.</returns>
        public static List<XMLParseError> CreateProtocolQActionHelperFromString(string xmlProtocolContent, out string sourceCode)
        {
            return CreateProtocolQActionHelperFromString(xmlProtocolContent, true, true, out sourceCode);
        }

        /// <summary>
        /// Helper method for DIS.
        /// </summary>
        /// <param name="xmlProtocolContent">XML Protocol as a string.</param>
        /// <param name="generateXMLComments"></param>
        /// <param name="compileAssembly"></param>
        /// <param name="sourceCode">Resulting source code of helper. <see langword="null"/> if helper could not be created.</param>
        /// <returns>List with XMLParseError objects.</returns>
        private static List<XMLParseError> CreateProtocolQActionHelperFromString(string xmlProtocolContent, bool generateXMLComments, bool compileAssembly, out string sourceCode)
        {
            //create XmlDocument with protocol
            sourceCode = null;
            XDocument xmlProtocol = null;

            try
            {
                xmlProtocol = XDocument.Parse(xmlProtocolContent, LoadOptions.SetLineInfo);
            }
            catch (Exception e)
            {
                //create List for XMLParseErrors
                List<XMLParseError> parseErrors = new List<XMLParseError>();
                //add Error Message to parseErrors
                parseErrors.Add(new XMLParseError(e.Message));
                return parseErrors;
            }

            string protocolName;
            string protocolVersion;

            return CreateProtocolQActionHelper(xmlProtocol, generateXMLComments, compileAssembly, out sourceCode, out protocolName, out protocolVersion);
        }

        #endregion

        #region "Create QAction Helper"

        /// <summary>
        /// Generates the source code string from the specified protocol.
        /// </summary>
        /// <param name="xmlProtocol">XML Protocol as a string</param>
        /// <param name="generateXMLComments"></param>
        /// <param name="compileAssembly"></param>
        /// <param name="logErrors"></param>
        /// <param name="sourceCode">The resulting source code.</param>
        /// <returns>List with XMLParseError objects</returns>
        private static List<XMLParseError> CreateProtocolQActionHelper(XDocument xmlProtocol, bool generateXMLComments, bool compileAssembly, out string sourceCode, out string protocolName, out string protocolVersion)
        {
            //create List for XMLParseErrors
            sourceCode = null;
            protocolName = null;
            protocolVersion = null;

            List<XMLParseError> parseErrors = new List<XMLParseError>();

            #region "Parsing of Protocol"

            #region Protocol General

            //declare Namespace
            XNamespace ns = QActionHelper.GetAttributeValue(xmlProtocol.Root, "xmlns");

            //extract Name and Version of protocol
            protocolName = QActionHelper.GetElementValue(xmlProtocol.Root, ns + "Name");
            if (protocolName == String.Empty)
                parseErrors.Add(new XMLParseError("Invalid Protocol Name", QActionHelper.GetLine(xmlProtocol.Root, ns + "Name")));

            protocolVersion = QActionHelper.GetElementValue(xmlProtocol.Root, ns + "Version");
            if (protocolVersion == String.Empty)
                parseErrors.Add(new XMLParseError("Invalid Protocol Version", QActionHelper.GetLine(xmlProtocol.Root, ns + "Version")));
            #endregion

            #region Params

            //List with Parameters
            var parameters = new List<ProtocolParameter>();
            var parameterIds = new HashSet<int>();
            var parameterNames = new HashSet<string>();

            //List with Tables
            List<ProtocolTable> tables = new List<ProtocolTable>();

            //extract Parameter nodes
            string parameterName, parameterType, parameterIdStr;
            int parameterId;
            List<int> parameterDuplicateAsIDs = new List<int>();
            XElement elementParams = QActionHelper.GetElement(xmlProtocol.Root, ns + "Params");

            if (elementParams != null)
            {
                foreach (XElement parameterNode in QActionHelper.GetElements(elementParams, ns + "Param"))
                {
                    ProtocolParameter parameter = new ProtocolParameter();

                    //extract Name from Parameter
                    parameterName = QActionHelper.GetElementValue(parameterNode, ns + "Name");
                    if (parameterName == String.Empty)
                    {
                        parseErrors.Add(new XMLParseError("Invalid Param Name", QActionHelper.GetLine(parameterNode, ns + "Name")));
                        continue;
                    }
                    parameterName = Utils.CleanIdentifier(Utils.ConvertStringToCaseType(parameterName, CaseType.CamelCase));

                    //parameter duplicateAs
                    parameterDuplicateAsIDs.Clear();
                    string parameterDuplicateAsStr = QActionHelper.GetAttributeValue(parameterNode, "duplicateAs");
                    string[] parameterDuplicateAsParts = parameterDuplicateAsStr.Split(',');
                    foreach (var item in parameterDuplicateAsParts)
                    {
                        int dupId = 0;
                        if (Int32.TryParse(item, out dupId))
                            parameterDuplicateAsIDs.Add(dupId);
                    }

                    //extract Type from Parameter
                    parameterType = QActionHelper.GetElementValue(parameterNode, ns + "Type").ToLower();
                    if (parameterType == String.Empty)
                    {
                        parseErrors.Add(new XMLParseError("Invalid Param Type", QActionHelper.GetLine(parameterNode, ns + "Type")));
                        continue;
                    }
                    parameter.Type = parameterType;

                    //add suffix type
                    if (!parameterType.Equals("read") && !parameterType.Equals("write") && !parameterType.Equals("array"))
                        parameterName = parameterName + "_" + Utils.CleanIdentifier(parameterType);

                    //extract Id from Parameter
                    parameterIdStr = QActionHelper.GetAttributeValue(parameterNode, "id");
                    if (parameterIdStr == String.Empty || !Int32.TryParse(parameterIdStr, out parameterId))
                    {
                        parseErrors.Add(new XMLParseError("Invalid Param Id", QActionHelper.GetLine(parameterNode)));
                        continue;
                    }
                    if (parameterIds.Contains(parameterId))
                    {
                        parseErrors.Add(new XMLParseError("Duplicate Param Id (" + parameterId + ")", QActionHelper.GetLine(parameterNode)));
                        continue;
                    }
                    parameter.Id = parameterId;

                    //check protected dataminer keywords
                    List<string> protectedKeywords = new List<string>() { "dataminerid", "elementid", "elementname", "userinfo", "columns", "index", "key", "columncount" };
                    if (protectedKeywords.Exists(x => x.Equals(parameterName.ToLower())))
                        parameterName = parameterName + "_" + parameterId;

                    parameter.OriginalName = parameterName;

                    //add pid-suffix to read/write param
                    if (parameterType.Equals("read") || parameterType.Equals("write"))
                    {
                        parameterName = parameterName + "_" + parameterId;
                        parameter.AddedPidSuffix = true;
                    }
                    else
                    {
                        //rename Parameter if already exists
                        if (parameterNames.Contains(parameterName))
                        {
                            ProtocolParameter presentParameter = parameters.SingleOrDefault(x => x.Name.Equals(parameterName));
                            if (presentParameter != null)
                            {
                                presentParameter.Name = presentParameter.OriginalName + "_" + presentParameter.Id;
                                presentParameter.AddedPidSuffix = true;
                            }

                            parameterName = parameterName + "_" + parameterId;
                            parameter.AddedPidSuffix = true;
                        }
                    }

                    parameter.Name = parameterName;

                    #region Measurement

                    //extract Measurement
                    XElement parameterMeasurement = QActionHelper.GetElement(parameterNode, ns + "Measurement");
                    if (parameterMeasurement != null)
                    {
                        XElement discreets = QActionHelper.GetElement(parameterMeasurement, ns + "Discreets");
                        if (discreets != null)
                        {
                            string intellisenseDiscreets = "";
                            string discreetDisplay = "";
                            string discreetValue = "";
                            foreach (XElement discreet in QActionHelper.GetElements(discreets, ns + "Discreet"))
                            {
                                discreetDisplay = QActionHelper.GetElementValue(discreet, ns + "Display");
                                discreetValue = QActionHelper.GetElementValue(discreet, ns + "Value");
                                intellisenseDiscreets = intellisenseDiscreets + discreetDisplay + " = " + discreetValue + ", ";
                            }
                            if (intellisenseDiscreets.Length > 2)
                                intellisenseDiscreets = intellisenseDiscreets.Substring(0, intellisenseDiscreets.Length - 2);

                            parameter.IntellisenseDiscreets = intellisenseDiscreets;
                        }
                    }

                    #endregion

                    #region Interprete

                    //extract Interprete
                    XElement interprete = QActionHelper.GetElement(parameterNode, ns + "Interprete");
                    if (interprete != null)
                    {
                        XElement exceptions = QActionHelper.GetElement(interprete, ns + "Exceptions");
                        if (exceptions != null)
                        {
                            string intellisenseExceptions = "";
                            string exceptionDisplay = "";
                            string exceptionValue = "";
                            foreach (XElement exception in QActionHelper.GetElements(exceptions, ns + "Exception"))
                            {
                                exceptionDisplay = QActionHelper.GetElementValue(exception, ns + "Display");
                                exceptionValue = QActionHelper.GetElementValue(exception, ns + "Value");
                                intellisenseExceptions = intellisenseExceptions + exceptionDisplay + " = " + exceptionValue + ", ";
                            }
                            if (intellisenseExceptions.Length > 2)
                                intellisenseExceptions = intellisenseExceptions.Substring(0, intellisenseExceptions.Length - 2);

                            parameter.IntellisenseExceptions = intellisenseExceptions;
                        }
                    }

                    #endregion

                    #region Array

                    //check if it is an array
                    if (parameterType.Equals("array"))
                    {
                        ProtocolTable table = new ProtocolTable();
                        table.Id = parameterId;
                        table.Name = parameterName;

                        //extract columns with indexes and Parameters
                        string columnIdStr, paramIdStr, optionsStr;
                        int columnId = 0;
                        int paramId = 0;
                        int tableIndex = 0;
                        //check ArrayOptions
                        XElement arrayOptions = parameterNode.Element(ns + "ArrayOptions");
                        if (arrayOptions != null)
                        {
                            string indexAttribute = QActionHelper.GetAttributeValue(arrayOptions, "index");
                            if (indexAttribute != String.Empty || Int32.TryParse(indexAttribute, out tableIndex))
                            {
                                foreach (XElement columnOption in QActionHelper.GetElements(arrayOptions, ns + "ColumnOption"))
                                {
                                    columnIdStr = QActionHelper.GetAttributeValue(columnOption, "idx");
                                    if (columnIdStr == String.Empty || !Int32.TryParse(columnIdStr, out columnId))
                                    {
                                        parseErrors.Add(new XMLParseError("Invalid ColumnOption idx", QActionHelper.GetLine(columnOption)));
                                        continue;
                                    }

                                    paramIdStr = QActionHelper.GetAttributeValue(columnOption, "pid");
                                    if (paramIdStr == String.Empty || !Int32.TryParse(paramIdStr, out paramId))
                                    {
                                        parseErrors.Add(new XMLParseError("Invalid ColumnOption pid", QActionHelper.GetLine(columnOption)));
                                        continue;
                                    }

                                    optionsStr = QActionHelper.GetAttributeValue(columnOption, "options");
                                    if (optionsStr != String.Empty)
                                    {
                                        int foreignKey = 0;
                                        foreach (string option in optionsStr.Split(optionsStr[0]))
                                        {
                                            if (option.StartsWith("foreignKey=", StringComparison.OrdinalIgnoreCase))
                                            {
                                                if (!Int32.TryParse(option.Split('=')[1], out foreignKey))
                                                {
                                                    parseErrors.Add(new XMLParseError("Invalid ColumnOption ForeignKey", QActionHelper.GetLine(columnOption)));
                                                    continue;
                                                }

                                                if (!table.Parents.ContainsKey(paramId))
                                                    table.Parents.Add(paramId, foreignKey);
                                                else
                                                {
                                                    parseErrors.Add(new XMLParseError("Multiple ForeignKey relations are defined", QActionHelper.GetLine(columnOption)));
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                    if (table.Columns.ContainsKey(columnId))
                                    {
                                        parseErrors.Add(new XMLParseError("Duplicate ColumnOption idx", QActionHelper.GetLine(columnOption)));
                                        continue;
                                    }
                                    else
                                    {
                                        table.Columns.Add(columnId, paramId);
                                    }
                                }
                                columnId = 0;
                            }
                        }
                        //check Type id attribute
                        string typeId = QActionHelper.GetAttributeValue(parameterNode.Element(ns + "Type"), "id");
                        if (typeId != String.Empty)
                        {
                            foreach (string typeIdParam in typeId.Split(';'))
                            {
                                if (!Int32.TryParse(typeIdParam, out paramId))
                                {
                                    parseErrors.Add(new XMLParseError("Invalid Type id", QActionHelper.GetLine(parameterNode.Element(ns + "Type"))));
                                    continue;
                                }
                                if (!table.Columns.ContainsValue(paramId))
                                {
                                    foreach (KeyValuePair<int, int> tableColumn in table.Columns.Skip(columnId))
                                    {
                                        if (tableColumn.Key == columnId)
                                            columnId++;
                                        else
                                            break;
                                    }
                                    table.Columns.Add(columnId, paramId);
                                    columnId++;
                                }
                            }
                        }
                        table.Index = tableIndex;
                        tables.Add(table);
                    }

                    #endregion

                    #region Duplicated Params

                    if (parameterIds.Contains(parameter.Id))
                    {
                        parseErrors.Add(new XMLParseError("Duplicate Param", QActionHelper.GetLine(parameterNode)));
                        continue;
                    }
                    else
                    {
                        parameters.Add(parameter);
                        parameterIds.Add(parameter.Id);
                        parameterNames.Add(parameter.OriginalName);

                        //also add duplicated params
                        int i = 1;

                        foreach (var id in parameterDuplicateAsIDs)
                        {
                            if (parameterIds.Contains(id))
                            {
                                parseErrors.Add(new XMLParseError("DuplicateAs Param with id " + id + " already exits", QActionHelper.GetLine(parameterNode)));
                                continue;
                            }

                            var dupParam = new ProtocolParameter();
                            dupParam.Id = id;
                            dupParam.OriginalName = Utils.CleanIdentifier("view" + i + "_" + parameter.OriginalName);
                            dupParam.Name = Utils.CleanIdentifier("view" + i + "_" + parameter.Name);
                            dupParam.IntellisenseDiscreets = parameter.IntellisenseDiscreets;
                            dupParam.IntellisenseExceptions = parameter.IntellisenseExceptions;
                            dupParam.Type = parameter.Type;

                            parameters.Add(dupParam);
                            parameterIds.Add(dupParam.Id);
                            parameterNames.Add(dupParam.OriginalName);

                            i++;
                        }
                    }

                    #endregion
                }

                #region PostProcessing

                var parametersByOriginalName = parameters.GroupBy(x => x.OriginalName).ToDictionary(x => x.Key, x => x.ToList());
                var readWriteParameters = parameters.Where(x => (x.Type.Equals("read") || x.Type.Equals("write")) && x.AddedPidSuffix).ToList();

                foreach (ProtocolParameter readWriteParameter in readWriteParameters)
                {
                    if (readWriteParameter.Type.Equals("read"))
                    {
                        if (parametersByOriginalName[readWriteParameter.OriginalName].Any(x => !x.AddedPidSuffix && x.Type.Equals("read")))
                            continue;
                    }
                    else if (readWriteParameter.Type.Equals("write"))
                    {
                        if (parametersByOriginalName[readWriteParameter.OriginalName].Any(x => !x.AddedPidSuffix && x.Type.Equals("write")))
                            continue;
                    }

                    ProtocolParameter dupReadWriteParam = new ProtocolParameter();
                    dupReadWriteParam.Id = readWriteParameter.Id;
                    dupReadWriteParam.OriginalName = readWriteParameter.OriginalName;
                    dupReadWriteParam.Name = readWriteParameter.OriginalName;
                    dupReadWriteParam.IntellisenseDiscreets = readWriteParameter.IntellisenseDiscreets;
                    dupReadWriteParam.IntellisenseExceptions = readWriteParameter.IntellisenseExceptions;
                    dupReadWriteParam.Type = readWriteParameter.Type;

                    parameters.Add(dupReadWriteParam);
                    parametersByOriginalName[readWriteParameter.OriginalName].Add(dupReadWriteParam);
                }

                //sort params on Id
                parameters = parameters.OrderBy(x => x.Id).ToList();

                #endregion
            }

            #endregion

            #endregion

            #region "Generating of SourceCode

            //generate sourcecode only if there are no errors
            if (parseErrors.Count == 0)
            {
                #region "PreProcessing"
                var parametersById = parameters.ToLookup(x => x.Id);
                var parametersByOriginalName = parameters.ToLookup(x => x.OriginalName);
                var tableColumnParameterIds = new HashSet<int>();

                foreach (ProtocolTable table in tables)
                {
                    foreach (KeyValuePair<int, int> tableColumn in table.Columns)
                    {
                        tableColumnParameterIds.Add(tableColumn.Value);
                        table.ColumnsParameters.Add(tableColumn.Key, parametersById[tableColumn.Value].ToList());
                    }
                }

                foreach (ProtocolTable table in tables)
                {
                    if (table.Parents != null && table.Parents.Count > 0)
                    {
                        ProtocolTable parentTable;
                        foreach (KeyValuePair<int, int> parent in table.Parents)
                        {
                            parentTable = tables.SingleOrDefault(x => x.Id == parent.Value);
                            if (parentTable != null)
                            {
                                parentTable.Children.Add(parent.Key, table.Id);
                            }
                        }
                    }
                }

                #endregion

                #region "Static Parameters"

                //create StringBuilder for static Parameter
                StringBuilder sbSourceCodeStaticParameter = new StringBuilder();

                //open static class Parameter
                sbSourceCodeStaticParameter.AppendLine("public static class Parameter" + Environment.NewLine + "{");

                //add constants for referencing read Parameters
                foreach (ProtocolParameter parameter in parameters.Where(x => x.Type.Equals("read")))
                {
                    if (!tableColumnParameterIds.Contains(parameter.Id))
                    {
                        if (generateXMLComments)
                        {
                            sbSourceCodeStaticParameter.AppendLine($"\t/// <summary>PID: {parameter.Id} | Type: {parameter.Type}</summary>");
                        }
                        if (parameter.AddedPidSuffix && parametersByOriginalName[parameter.OriginalName].Any(x => x.Type.Equals("read") && !x.AddedPidSuffix))
                            sbSourceCodeStaticParameter.AppendLine("\t[EditorBrowsable(EditorBrowsableState.Never)]");

                        sbSourceCodeStaticParameter.AppendLine("\tpublic const int " + parameter.Name + " = " + parameter.Id + ";");
                    }
                }

                //add constants for referencing write Parameters
                sbSourceCodeStaticParameter.AppendLine("\tpublic class Write");
                sbSourceCodeStaticParameter.AppendLine("\t{");

                foreach (ProtocolParameter parameter in parameters.Where(x => x.Type.Equals("write")))
                {
                    if (!tableColumnParameterIds.Contains(parameter.Id))
                    {
                        if (generateXMLComments)
                        {
                            sbSourceCodeStaticParameter.AppendLine($"\t\t/// <summary>PID: {parameter.Id} | Type: {parameter.Type}</summary>");
                        }
                        if (parameter.AddedPidSuffix && parametersByOriginalName[parameter.OriginalName].Any(x => x.Type.Equals("write") && !x.AddedPidSuffix))
                            sbSourceCodeStaticParameter.AppendLine("\t\t[EditorBrowsable(EditorBrowsableState.Never)]");

                        sbSourceCodeStaticParameter.AppendLine("\t\tpublic const int " + parameter.Name + " = " + parameter.Id + ";");
                    }
                }

                sbSourceCodeStaticParameter.AppendLine("\t}");

                //add static Tables
                foreach (ProtocolTable table in tables)
                {
                    sbSourceCodeStaticParameter.AppendLine("\tpublic class " + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase));
                    sbSourceCodeStaticParameter.AppendLine("\t{");

                    if (generateXMLComments)
                    {
                        sbSourceCodeStaticParameter.AppendLine($"\t\t/// <summary>PID: {table.Id}</summary>");
                    }
                    sbSourceCodeStaticParameter.AppendLine("\t\tpublic const int tablePid = " + table.Id + ";");
                    if (generateXMLComments)
                    {
                        sbSourceCodeStaticParameter.AppendLine($"\t\t/// <summary>IDX: {table.Index}</summary>");
                    }
                    sbSourceCodeStaticParameter.AppendLine("\t\tpublic const int indexColumn = " + table.Index + ";");

                    if (table.ColumnsParameters.Count(x => x.Key == table.Index) == 1)
                    {
                        if (generateXMLComments)
                        {
                            sbSourceCodeStaticParameter.AppendLine(
                                $"\t\t/// <summary>PID: {table.ColumnsParameters.Single(x => x.Key == table.Index).Value.FirstOrDefault().Id}</summary>");
                        }
                        sbSourceCodeStaticParameter.AppendLine("\t\tpublic const int indexColumnPid = " + table.ColumnsParameters.Single(x => x.Key == table.Index).Value.FirstOrDefault().Id + ";");
                    }

                    //add Pid for columns
                    sbSourceCodeStaticParameter.AppendLine("\t\tpublic class Pid");
                    sbSourceCodeStaticParameter.AppendLine("\t\t{");
                    foreach (KeyValuePair<int, List<ProtocolParameter>> tableColumn in table.ColumnsParameters)
                    {
                        foreach (ProtocolParameter tableColumnProtocolParameter in tableColumn.Value)
                        {
                            if (tableColumnProtocolParameter != null && !tableColumnProtocolParameter.Type.Equals("write"))
                            {
                                if (tableColumnProtocolParameter.Type.Equals("read"))
                                {
                                    if (generateXMLComments)
                                    {
                                        sbSourceCodeStaticParameter.AppendLine(
                                            $"\t\t\t/// <summary>PID: {tableColumnProtocolParameter.Id} | Type: {tableColumnProtocolParameter.Type}</summary>");
                                    }
                                    if (tableColumnProtocolParameter.AddedPidSuffix && parametersByOriginalName[tableColumnProtocolParameter.OriginalName].Any(x => x.Type.Equals("read") && !x.AddedPidSuffix))
                                        sbSourceCodeStaticParameter.AppendLine("\t\t\t[EditorBrowsable(EditorBrowsableState.Never)]");

                                    sbSourceCodeStaticParameter.AppendLine("\t\t\tpublic const int " + tableColumnProtocolParameter.Name + " = " + tableColumnProtocolParameter.Id + ";");
                                }
                            }
                        }
                    }

                    sbSourceCodeStaticParameter.AppendLine("\t\t\tpublic class Write");
                    sbSourceCodeStaticParameter.AppendLine("\t\t\t{");
                    foreach (KeyValuePair<int, List<ProtocolParameter>> tableColumn in table.ColumnsParameters)
                    {
                        foreach (ProtocolParameter tableColumnProtocolParameter in tableColumn.Value)
                        {
                            if (tableColumnProtocolParameter != null && tableColumnProtocolParameter.Type.Equals("write"))
                            {
                                if (generateXMLComments)
                                {
                                    sbSourceCodeStaticParameter.AppendLine(
                                        $"\t\t\t\t/// <summary>PID: {tableColumnProtocolParameter.Id} | Type: {tableColumnProtocolParameter.Type}</summary>");
                                }
                                if (tableColumnProtocolParameter.AddedPidSuffix && parametersByOriginalName[tableColumnProtocolParameter.OriginalName].Any(x => x.Type.Equals("write") && !x.AddedPidSuffix))
                                    sbSourceCodeStaticParameter.AppendLine("\t\t\t\t[EditorBrowsable(EditorBrowsableState.Never)]");

                                sbSourceCodeStaticParameter.AppendLine("\t\t\t\tpublic const int " + tableColumnProtocolParameter.Name + " = " + tableColumnProtocolParameter.Id + ";");
                            }
                        }
                    }
                    sbSourceCodeStaticParameter.AppendLine("\t\t\t}");

                    sbSourceCodeStaticParameter.AppendLine("\t\t}");

                    //add Idx for columns
                    sbSourceCodeStaticParameter.AppendLine("\t\tpublic class Idx");
                    sbSourceCodeStaticParameter.AppendLine("\t\t{");
                    int columnId = 0;
                    bool columnAdded = false;
                    foreach (KeyValuePair<int, List<ProtocolParameter>> tableColumn in table.ColumnsParameters)
                    {
                        foreach (ProtocolParameter tableColumnProtocolParameter in tableColumn.Value)
                        {
                            if (tableColumnProtocolParameter != null)
                            {
                                if (tableColumnProtocolParameter.Type.Equals("read"))
                                {
                                    if (generateXMLComments)
                                    {
                                        sbSourceCodeStaticParameter.AppendLine(
                                            $"\t\t\t/// <summary>IDX: {columnId} | Type: {tableColumnProtocolParameter.Type}</summary>");
                                    }
                                    if (tableColumnProtocolParameter.AddedPidSuffix && parametersByOriginalName[tableColumnProtocolParameter.OriginalName].Any(x => x.Type.Equals("read") && !x.AddedPidSuffix))
                                        sbSourceCodeStaticParameter.AppendLine("\t\t\t[EditorBrowsable(EditorBrowsableState.Never)]");

                                    sbSourceCodeStaticParameter.AppendLine("\t\t\tpublic const int " + tableColumnProtocolParameter.Name + " = " + columnId + ";");
                                }
                                columnAdded = true;
                            }
                        }
                        if (columnAdded)
                        {
                            columnId++;
                            columnAdded = false;
                        }
                    }
                    sbSourceCodeStaticParameter.AppendLine("\t\t}");

                    sbSourceCodeStaticParameter.AppendLine("\t}");
                }

                //close static class Parameter
                sbSourceCodeStaticParameter.AppendLine("}");

                #endregion

                #region "SLProtocolExt"

                //create StringBuilder for extended SLProtocol and ConcreteSLProtocol
                StringBuilder sbSourceCodeSLProtocolExt = new StringBuilder();

                //Add the WriteParameters class.
                sbSourceCodeSLProtocolExt.AppendLine("public class WriteParameters");
                sbSourceCodeSLProtocolExt.AppendLine("{");

                foreach (ProtocolParameter parameter in parameters)
                {
                    if (!parameter.Type.Equals("write") || parameter.AddedPidSuffix)
                    {
                        continue;
                    }

                    if (generateXMLComments)
                    {
                        sbSourceCodeSLProtocolExt.Append($"\t/// <summary>PID: {parameter.Id}  | Type: {parameter.Type}");
                        if (!String.IsNullOrEmpty(parameter.IntellisenseDiscreets))
                            sbSourceCodeSLProtocolExt.Append($" | DISCREETS: {parameter.IntellisenseDiscreets}");
                        if (!String.IsNullOrEmpty(parameter.IntellisenseExceptions))
                            sbSourceCodeSLProtocolExt.Append($" | EXCEPTIONS: {parameter.IntellisenseExceptions}");
                        sbSourceCodeSLProtocolExt.AppendLine("</summary>");
                    }
                    sbSourceCodeSLProtocolExt.Append("\tpublic System.Object " + Utils.ConvertStringToCaseType(parameter.Name, CaseType.PascalCase) + " {");
                    sbSourceCodeSLProtocolExt.Append("get { return Protocol.GetParameter(" + parameter.Id + "); }");
                    sbSourceCodeSLProtocolExt.Append("set { Protocol.SetParameter(" + parameter.Id + ", value); }");
                    sbSourceCodeSLProtocolExt.AppendLine("}");
                }
                //create constructor WriteParameters
                sbSourceCodeSLProtocolExt.AppendLine("\tpublic SLProtocolExt Protocol;");
                sbSourceCodeSLProtocolExt.AppendLine("\tpublic WriteParameters(SLProtocolExt protocol)");
                sbSourceCodeSLProtocolExt.AppendLine("\t{");
                sbSourceCodeSLProtocolExt.AppendLine("\t\tProtocol = protocol;");
                sbSourceCodeSLProtocolExt.AppendLine("\t}");

                //close class WriteParameters
                sbSourceCodeSLProtocolExt.AppendLine("}");

                //create SLProtocolExt interface
                sbSourceCodeSLProtocolExt.AppendLine("public interface SLProtocolExt : SLProtocol" + Environment.NewLine + "{");

                //Add every table.
                foreach (ProtocolTable table in tables)
                {
                    if (generateXMLComments)
                    {
                        sbSourceCodeSLProtocolExt.AppendLine($"\t/// <summary>PID: {table.Id}</summary>");
                    }
                    sbSourceCodeSLProtocolExt.AppendLine("\t" + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase) + "QActionTable " + table.Name + " { get; set; }");
                }

                //Add every parameter.
                foreach (ProtocolParameter parameter in parameters)
                {
                    if (parameter.Type.Equals("array"))
                        continue;

                    if (parameter.Type.Equals("write") &&
                        !parameter.AddedPidSuffix &&
                        parametersByOriginalName[parameter.OriginalName].Any(x => x.Type.Equals("read")))
                    {
                        continue;
                    }

                    sbSourceCodeSLProtocolExt.Append("\tobject " + Utils.ConvertStringToCaseType(parameter.Name, CaseType.PascalCase) + " { ");
                    sbSourceCodeSLProtocolExt.Append("get; ");
                    sbSourceCodeSLProtocolExt.Append("set; ");
                    sbSourceCodeSLProtocolExt.AppendLine("}");
                }

                //Add the write parameters class.
                sbSourceCodeSLProtocolExt.AppendLine("\tWriteParameters Write { get; set; }");

                //close interface SLProtocolExt
                sbSourceCodeSLProtocolExt.AppendLine("}");

                //extend with ConcreteSLProtocolExt
                sbSourceCodeSLProtocolExt.AppendLine("public class ConcreteSLProtocolExt : ConcreteSLProtocol, SLProtocolExt" + Environment.NewLine + "{");

                //create instance variable for each table
                foreach (ProtocolTable table in tables)
                {
                    if (generateXMLComments)
                    {
                        sbSourceCodeSLProtocolExt.AppendLine($"\t/// <summary>PID: {table.Id}</summary>");
                    }
                    sbSourceCodeSLProtocolExt.AppendLine("\tpublic " + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase) + "QActionTable " + table.Name + " { get; set; }");
                }

                //add property for each Parameter (not array)
                foreach (ProtocolParameter parameter in parameters)
                {
                    if (parameter.Type.Equals("array"))
                        continue;

                    string getString = "get { return GetParameter(" + parameter.Id + "); }";
                    string setString = "set { SetParameter(" + parameter.Id + ", value); }";

                    if (parameter.Type.Equals("write") && !parameter.AddedPidSuffix)
                    {
                        if (parametersByOriginalName[parameter.OriginalName].Any(x => x.Type.Equals("read")))
                            continue;
                        getString = "get { return Write." + Utils.ConvertStringToCaseType(parameter.Name, CaseType.PascalCase) + "; }";
                        setString = "set { Write." + Utils.ConvertStringToCaseType(parameter.Name, CaseType.PascalCase) + " = value; }";
                    }
                    if (generateXMLComments)
                    {
                        sbSourceCodeSLProtocolExt.Append($"\t/// <summary>PID: {parameter.Id}  | Type: {parameter.Type}");
                        if (!String.IsNullOrEmpty(parameter.IntellisenseDiscreets))
                            sbSourceCodeSLProtocolExt.Append($" | DISCREETS: {parameter.IntellisenseDiscreets}");
                        if (!String.IsNullOrEmpty(parameter.IntellisenseExceptions))
                            sbSourceCodeSLProtocolExt.Append($" | EXCEPTIONS: {parameter.IntellisenseExceptions}");
                        sbSourceCodeSLProtocolExt.AppendLine("</summary>");
                    }
                    if (parameter.AddedPidSuffix && parametersByOriginalName[parameter.OriginalName].Any(x => x.Type.Equals("read") && !x.AddedPidSuffix))
                        sbSourceCodeSLProtocolExt.AppendLine("\t[EditorBrowsable(EditorBrowsableState.Never)]");

                    sbSourceCodeSLProtocolExt.Append("\tpublic System.Object " + Utils.ConvertStringToCaseType(parameter.Name, CaseType.PascalCase) + " {");
                    sbSourceCodeSLProtocolExt.Append(getString);
                    sbSourceCodeSLProtocolExt.Append(setString);
                    sbSourceCodeSLProtocolExt.AppendLine("}");
                }

                //add write params.
                sbSourceCodeSLProtocolExt.AppendLine("\tpublic WriteParameters Write { get; set; }");

                //create constructor
                sbSourceCodeSLProtocolExt.AppendLine("\tpublic ConcreteSLProtocolExt()");
                sbSourceCodeSLProtocolExt.AppendLine("\t{");
                //instantiate each table instance variable
                foreach (ProtocolTable table in tables)
                {
                    sbSourceCodeSLProtocolExt.AppendLine("\t\t" + table.Name + " = new " + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase) + "QActionTable(this, " + table.Id + ", \"" + table.Name + "\");");
                }
                sbSourceCodeSLProtocolExt.AppendLine("\t\tWrite = new WriteParameters(this);");
                sbSourceCodeSLProtocolExt.AppendLine("\t}");

                //close class ConcreteSLProtocolExt
                sbSourceCodeSLProtocolExt.AppendLine("}");
                #endregion

                #region "QActionTables"

                //create StringBuilder for QActionTableRows
                StringBuilder sbSourceCodeQActionTables = new StringBuilder();

                foreach (ProtocolTable table in tables)
                {
                    //extend QActionTable
                    if (generateXMLComments)
                        sbSourceCodeQActionTables.AppendLine($"/// <summary>IDX: {table.Index}</summary>");
                    sbSourceCodeQActionTables.AppendLine("public class " + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase) + "QActionTable : QActionTable, IEnumerable<" + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase) + "QActionRow>" + Environment.NewLine + "{");

                    //create empty constructor
                    sbSourceCodeQActionTables.AppendLine("\tpublic " + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase) + "QActionTable(SLProtocol protocol, int tableId, string tableName) : base(protocol, tableId, tableName) { }");
                    sbSourceCodeQActionTables.AppendLine("\tIEnumerator IEnumerable.GetEnumerator() { return (IEnumerator) GetEnumerator(); }");
                    sbSourceCodeQActionTables.AppendLine("\tpublic IEnumerator<" + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase) + "QActionRow> GetEnumerator() { return new QActionTableEnumerator<" + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase) + "QActionRow>(this); }");

                    //close QActionTableRow
                    sbSourceCodeQActionTables.AppendLine("}");
                }

                #endregion

                #region "QActionTableRows"

                //create StringBuilder for QActionTableRows
                StringBuilder sbSourceCodeQActionTableRows = new StringBuilder();

                foreach (ProtocolTable table in tables)
                {
                    //extend QActionTableRow
                    if (generateXMLComments)
                        sbSourceCodeQActionTableRows.AppendLine($"/// <summary>IDX: {table.Index}</summary>");
                    sbSourceCodeQActionTableRows.AppendLine("public class " + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase) + "QActionRow : QActionTableRow" + Environment.NewLine + "{");

                    //add property for each column
                    int columnId = 0;
                    bool columnAdded = false;

                    List<ProtocolParameter> filteredParameters = table.Columns.Values.SelectMany(x => parametersById[x]).ToList();
                    List<string> duplicateParameterNames = filteredParameters.GroupBy(x => x.Name).Where(y => y.Skip(1).Any()).Select(y => y.Key).ToList();
                    foreach (KeyValuePair<int, List<ProtocolParameter>> tableColumn in table.ColumnsParameters)
                    {
                        foreach (ProtocolParameter tableColumnProtocolParameter in tableColumn.Value)
                        {
                            if (tableColumnProtocolParameter != null && !duplicateParameterNames.Contains(tableColumnProtocolParameter.Name))
                            {
                                if (generateXMLComments)
                                {
                                    sbSourceCodeQActionTableRows.AppendLine(
                                        $"\t/// <summary>PID: {tableColumnProtocolParameter.Id} | Type: {tableColumnProtocolParameter.Type}</summary>");
                                }
                                if (tableColumnProtocolParameter.AddedPidSuffix && parametersByOriginalName[tableColumnProtocolParameter.OriginalName].Any(x => x.Type.Equals("read") && !x.AddedPidSuffix))
                                    sbSourceCodeQActionTableRows.AppendLine("\t[EditorBrowsable(EditorBrowsableState.Never)]");

                                sbSourceCodeQActionTableRows.AppendLine("\tpublic System.Object " + Utils.ConvertStringToCaseType(tableColumnProtocolParameter.Name, CaseType.PascalCase) + " { get { if (base.Columns.ContainsKey(" + columnId + ")) { return base.Columns[" + columnId + "]; } else { return null; } } set { if (base.Columns.ContainsKey(" + columnId + ")) { base.Columns[" + columnId + "] = value; } else { base.Columns.Add(" + columnId + ", value); } } }");

                                columnAdded = true;
                            }
                        }
                        if (columnAdded)
                        {
                            columnId++;
                            columnAdded = false;
                        }
                    }

                    //create empty constructor
                    sbSourceCodeQActionTableRows.AppendLine("\tpublic " + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase) + "QActionRow() : base(" + table.Index + ", " + table.Columns.Count + ") { }");
                    //create constructor with Object[]
                    sbSourceCodeQActionTableRows.AppendLine("\tpublic " + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase) + "QActionRow(System.Object[] oRow) : base(" + table.Index + ", " + table.Columns.Count + ", oRow) { }");

                    //create implicit operators
                    sbSourceCodeQActionTableRows.AppendLine("\tpublic static implicit operator " + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase) + "QActionRow(System.Object[] source) { return new " + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase) + "QActionRow(source); }");
                    sbSourceCodeQActionTableRows.AppendLine("\tpublic static implicit operator System.Object[](" + Utils.ConvertStringToCaseType(table.Name, CaseType.PascalCase) + "QActionRow source) { return source.ToObjectArray(); }");

                    //create GetParentRows methods
                    if (table.Parents != null && table.Parents.Count > 0)
                    {
                        ProtocolTable parentTable = null;
                        foreach (KeyValuePair<int, int> parentTableIds in table.Parents)
                        {
                            parentTable = tables.SingleOrDefault(x => x.Id == parentTableIds.Value);
                            if (parentTable != null && parameters.Any(x => x.Id == parentTableIds.Key))
                            {
                                ProtocolParameter parameter = parametersById[parentTableIds.Key].FirstOrDefault(x => !x.AddedPidSuffix);
                                if (parameter == null)
                                    parameter = parametersById[parentTableIds.Key].FirstOrDefault(x => x.AddedPidSuffix);
                                sbSourceCodeQActionTableRows.AppendLine("\tpublic System.Object[] GetParentRow" + Utils.ConvertStringToCaseType(parentTable.Name, CaseType.PascalCase) + Utils.ConvertStringToCaseType(parameter.Name, CaseType.PascalCase) + "(SLProtocol protocol) { return (System.Object[])protocol.GetRow(" + parentTableIds.Value + ", (System.String)" + Utils.ConvertStringToCaseType(parameter.Name, CaseType.PascalCase) + "); }");
                            }
                        }
                    }

                    //create GetChildKeys methods
                    if (table.Children != null && table.Children.Count > 0)
                    {
                        ProtocolTable childTable = null;
                        foreach (KeyValuePair<int, int> childTableIds in table.Children)
                        {
                            childTable = tables.SingleOrDefault(x => x.Id == childTableIds.Value);
                            if (childTable != null && parameters.Any(x => x.Id == childTableIds.Key))
                            {
                                ProtocolParameter parameter = parametersById[childTableIds.Key].FirstOrDefault(x => !x.AddedPidSuffix);
                                if (parameter == null)
                                    parameter = parametersById[childTableIds.Key].FirstOrDefault(x => x.AddedPidSuffix);
                                sbSourceCodeQActionTableRows.AppendLine("\tpublic System.String[] GetChildKeys" + Utils.ConvertStringToCaseType(childTable.Name, CaseType.PascalCase) + Utils.ConvertStringToCaseType(parameter.Name, CaseType.PascalCase) + "(SLProtocol protocol) { return (System.String[])protocol.NotifyProtocol(196, " + childTableIds.Key + ", Key); }");
                            }
                        }
                    }

                    //close QActionTableRow
                    sbSourceCodeQActionTableRows.AppendLine("}");
                }

                #endregion

                #region "Combining SourceCode"

                //create new StringBuilder for all sourcecode
                StringBuilder sbSourceCode = new StringBuilder();

                //add dllImport statements for QActionHelper processed by SLManagedScripting (uses same format as dllImport in protocol QAction)
                if (!compileAssembly && !generateXMLComments)
                {
                    sbSourceCode.AppendLine("dllImport=\"System.Data.Linq.dll\"" + Environment.NewLine);
                }
                //referencedAssemblies.Add("System.Data.Linq.dll");

                //add using statements for QActionHelper
                sbSourceCode.AppendLine("using System.ComponentModel;");
                sbSourceCode.AppendLine("using System.Collections;");
                sbSourceCode.AppendLine("using System.Collections.Generic;");
                sbSourceCode.AppendLine("using System.Linq;" + Environment.NewLine);

                //open namespace
                sbSourceCode.AppendLine("namespace Skyline.DataMiner.Scripting" + Environment.NewLine + "{");

                //add static Parameter
                sbSourceCode.Append(sbSourceCodeStaticParameter.ToString());

                //add extensions to SLProtocol and ConcreteSLProtocol
                sbSourceCode.Append(sbSourceCodeSLProtocolExt.ToString());

                //add QATables
                sbSourceCode.Append(sbSourceCodeQActionTables.ToString());

                //add QATableRows
                sbSourceCode.Append(sbSourceCodeQActionTableRows.ToString());

                //close namespace
                sbSourceCode.AppendLine("}");

                sourceCode = sbSourceCode.ToString();
                #endregion
            }
            #endregion

            return parseErrors.OrderBy(x => x.Line).ToList<XMLParseError>();
        }

        #endregion
    }
}
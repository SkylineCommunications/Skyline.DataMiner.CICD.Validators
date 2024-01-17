namespace ProtocolTests.SchemaGenerator
{
    using System;
    using System.Reflection;
    using System.Xml;

    public class SchemaGenerator
    {
        private const string TargetNamespace = "http://www.skyline.be/validatorProtocolUnitTest";

        private readonly string inputFile;

        private XmlDocument doc;
        private XmlNamespaceManager nsmgr;

        public SchemaGenerator(string inputFile)
        {
            if (String.IsNullOrEmpty(inputFile))
            {
                throw new ArgumentException("Input file parameter must not be null or empty.", nameof(inputFile));
            }

            this.inputFile = inputFile;
        }

        /// <summary>
        /// Creates XML Schema to be used for creating unit tests for the protocol validator.
        /// </summary>
        public string CreateSchema()
        {
            try
            {
                doc = new XmlDocument();
                doc.Load(inputFile);

                nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");

                UpdateNamespaces();

                UpdateSchemaLocation();

                MakeElementsOptional();
                MakeAttributesOptional();

                RemoveKeysAndKeyReferences();

                return doc.OuterXml;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return "";
            }
        }

        /// <summary>
        /// Updates the namespaces.
        /// </summary>
        private void UpdateNamespaces()
        {
            XmlNodeList targetNamespaceAttr = doc.DocumentElement.SelectNodes("./@targetNamespace");
            if (targetNamespaceAttr.Count > 0)
            {
                foreach (XmlNode attr in targetNamespaceAttr)
                {
                    attr.Value = TargetNamespace;
                }
            }

            XmlAttribute disnsAttr = doc.CreateAttribute("xmlns:dis");
            disnsAttr.Value = TargetNamespace;

            XmlAttribute xmlnsAttr = doc.CreateAttribute("xmlns");
            xmlnsAttr.Value = TargetNamespace;

            doc.DocumentElement.Attributes.Append(xmlnsAttr);
            doc.DocumentElement.Attributes.Append(disnsAttr);
        }

        private void UpdateSchemaLocation()
        {
            XmlNodeList elementList = doc.DocumentElement.SelectNodes(".//xs:include", nsmgr);

            var assembly = typeof(SchemaGenerator).Assembly;
            var buildConfigurationName = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration;

            var projectName = assembly.GetName().Name;
            foreach (XmlNode node in elementList)
            {
                var result = node.Attributes["schemaLocation"];
                if (result != null)
                {
                    result.Value = $@".\..\{projectName}\bin\{buildConfigurationName}\net472\Skyline\XSD\" + result.Value;
                }
            }
        }

        /// <summary>
        /// Makes the elements optional.
        /// </summary>
        private void MakeElementsOptional()
        {
            XmlNodeList elementList = doc.DocumentElement.SelectNodes(".//xs:element", nsmgr);

            foreach (XmlNode node in elementList)
            {
                XmlNodeList minOccursAttrs = node.SelectNodes("./@minOccurs");
                if (minOccursAttrs.Count > 0)
                {
                    bool isHttpRequestNode = IsHttpRequestNode(node);
                    bool isExposerNode = IsExposerNode(node);

                    if (!isHttpRequestNode && !isExposerNode)
                    {
                        foreach (XmlNode minOccursAttr in minOccursAttrs)
                        {
                            if (!minOccursAttr.Value.Equals("0"))
                            {
                                minOccursAttr.Value = "0";
                            }
                        }
                    }
                }
                else
                {
                    bool isRootNode = node.Attributes["name"].Value == "Protocol" && node.ParentNode.Name == "xs:schema";

                    if (!isRootNode)
                    {
                        XmlAttribute minOccursAttr = doc.CreateAttribute("minOccurs");
                        minOccursAttr.Value = "0";

                        node.Attributes.Append(minOccursAttr);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the specified node is an Exposer node, as these cannot be made optional as otherwise the content model would become ambiguous rendering the XML Schema invalid.
        /// </summary>
        /// <returns></returns>
        private bool IsExposerNode(XmlNode node)
        {
            bool result = false;

            string name = node.Attributes["name"].Value;

            if (name.Equals("Exposer"))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified node is an HTTP request node, as these cannot be made optional as otherwise the content model would become ambiguous rendering the XML Schema invalid.
        /// </summary>
        /// <returns></returns>
        private bool IsHttpRequestNode(XmlNode node)
        {
            bool result = false;

            string name = node.Attributes["name"].Value;

            var typeAttr = node.Attributes["type"];

            if (typeAttr != null)
            {
                string type = node.Attributes["type"].Value;

                if ((name.Equals("Headers") || name == "Parameters" || name == "Data") && (type == "HttpRequestHeaders" || type == "HttpRequestData" || type == "HttpRequestParameters"))
                {
                    result = true;
                }
            }

            return result;
        }


        /// <summary>
        /// Makes the attributes optional.
        /// </summary>
        private void MakeAttributesOptional()
        {
            XmlNodeList attributeList = doc.DocumentElement.SelectNodes(".//xs:attribute", nsmgr);

            foreach (XmlNode node in attributeList)
            {
                XmlNodeList minOccursAttrs = node.SelectNodes("./@use");
                if (minOccursAttrs.Count > 0)
                {
                    foreach (XmlNode minOccursAttr in minOccursAttrs)
                    {
                        if (!minOccursAttr.Value.Equals("optional"))
                        {
                            minOccursAttr.Value = "optional";
                        }
                    }
                }
                else
                {
                    XmlAttribute minOccursAttr = doc.CreateAttribute("use");
                    minOccursAttr.Value = "optional";

                    node.Attributes.Append(minOccursAttr);
                }
            }
        }

        /// <summary>
        /// Removes the key reference constraints.
        /// </summary>
        private void RemoveKeysAndKeyReferences()
        {
            XmlNodeList keyRefList = doc.DocumentElement.SelectNodes(".//xs:keyref", nsmgr);

            foreach (XmlNode node in keyRefList)
            {
                XmlNode parent = node.ParentNode;
                parent.RemoveChild(node);
            }

            XmlNodeList keyList = doc.DocumentElement.SelectNodes(".//xs:key", nsmgr);

            foreach (XmlNode node in keyList)
            {
                XmlNode parent = node.ParentNode;
                parent.RemoveChild(node);
            }
        }
    }
}

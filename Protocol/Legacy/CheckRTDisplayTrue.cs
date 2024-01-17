namespace Skyline.DataMiner.CICD.Validators.Protocol.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    internal partial class ProtocolChecks
    {
        /// <summary>
        /// Checks the RTDisplay.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <returns>List of results.</returns>
        public List<IValidationResult> CheckRTDisplayTrue(XmlDocument xDoc) // M
        {
            List<IValidationResult> resultMsg = new List<IValidationResult>();
            XmlNamespaceManager xmlNsm = new XmlNamespaceManager(xDoc.NameTable);
            xmlNsm.AddNamespace("slc", "http://www.skyline.be/protocol");

            // Checks for parameters that MUST have RTDisplay = true
            RtdCheckDependencyValues(xDoc, xmlNsm, ref resultMsg);

            return resultMsg;
        }

        /// <summary>
        /// Check on DependencyValues.
        /// </summary>
        /// <param name="xDoc">The protocol document.</param>
        /// <param name="xmlNsm">The namespace.</param>
        /// <param name="rtdAllowedParams">Set of allowed parameters.</param>
        /// <param name="resultMsg">List of results.</param>
        private void RtdCheckDependencyValues(XmlDocument xDoc, XmlNamespaceManager xmlNsm, ref List<IValidationResult> resultMsg)
        {
            XmlNodeList xnlParams = xDoc.SelectNodes("slc:Protocol/slc:Params/slc:Param[slc:Measurement/slc:Discreets/slc:Discreet/@dependencyValues]", xmlNsm);
            foreach (XmlNode xnParam in xnlParams)
            {
                LineNum = xnParam.Attributes?["QA_LNx"].InnerXml;

                // Check Parameter Type (only write parameters)
                XmlNode xnParamType = xnParam.SelectSingleNode("./slc:Type", xmlNsm);
                string sParamType = xnParamType?.InnerXml;
                if (!String.Equals(sParamType, "write"))
                {
                    string sLineNum = xnParamType?.Attributes?["QA_LNx"].InnerXml ?? LineNum;

                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(sLineNum),
                        ErrorId = 5302,
                        DescriptionFormat = "DependencyValues is only allowed on write parameters.",
                        DescriptionParameters = null,
                        TestName = "RtdCheckDependencyValues",
                        Severity = Severity.Major
                    });
                }

                // Get Parameter Name
                string sParamName = xnParam.SelectSingleNode("./slc:Name", xmlNsm)?.InnerXml;

                if (!String.IsNullOrWhiteSpace(sParamName) && sParamName.EndsWith("_ContextMenu"))
                {
                    continue;
                }

                // No ContextMenu
                // Check if Discreets has dependencyId attribute
                XmlNode xnDiscreets = xnParam.SelectSingleNode("./slc:Measurement/slc:Discreets", xmlNsm);
                string sDependencyId = xnDiscreets?.Attributes["dependencyId"]?.InnerXml;
                if (xnDiscreets == null || sDependencyId == null)
                {
                    string sLineNum = xnDiscreets?.Attributes?["QA_LNx"].InnerXml ?? LineNum;

                    resultMsg.Add(new ValidationResult
                    {
                        Line = Convert.ToInt32(sLineNum),
                        ErrorId = 5301,
                        DescriptionFormat = "Required attribute '{0}' is missing.",
                        DescriptionParameters = new object[] { "dependencyId" },
                        TestName = "RtdCheckDependencyValues",
                        Severity = Severity.Major
                    });
                }

                // Check if all discreets have the dependencyValues attribute
                XmlNodeList xnDiscreetList = xnDiscreets?.ChildNodes;
                foreach (XmlNode xnDiscreet in xnDiscreetList)
                {
                    string sLineNum = xnDiscreet?.Attributes?["QA_LNx"].InnerXml ?? LineNum;

                    // Get dependencyValues attribute
                    string sDependencyValues = xnDiscreet?.Attributes?["dependencyValues"]?.InnerXml;

                    if (String.IsNullOrWhiteSpace(sDependencyValues))
                    {
                        resultMsg.Add(new ValidationResult
                        {
                            Line = Convert.ToInt32(sLineNum),
                            ErrorId = 5301,
                            DescriptionFormat = "Required attribute '{0}' is missing.",
                            DescriptionParameters = new object[] { "dependencyValues" },
                            TestName = "RtdCheckDependencyValues",
                            Severity = Severity.Major
                        });
                    }
                }

                // No need to check the value from the DependencyValues attribute as it can contain anything.
                // Maybe later => If DependencyId refers to Discreet parameter => Check if dependencyValues match with one of the discreets?
            }
        }
    }
}
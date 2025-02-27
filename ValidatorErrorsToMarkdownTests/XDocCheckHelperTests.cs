using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown.Tests
{
    [TestClass()]
    public class XDocCheckHelperTests
    {
        [TestMethod()]
        public void GetCheckDescriptionTest_NoTemplateNoOverride()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description>
                                                <Format>Invalid prefix '{1}' in 'Protocol/Name' tag. Current value '{0}'.</Format>
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagValue</InputParameter>
                                                    <InputParameter id=""1"">invalidPrefix</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);
            XDocCheckHelper helper = new(element, null);

            // Act
            var result = helper.GetCheckDescription(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Invalid prefix '{invalidPrefix}' in 'Protocol/Name' tag. Current value '{tagValue}'.", result);
        }

        [TestMethod()]
        public void GetCheckDescriptionTest_NoTemplateOverride()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description>
                                                <Format>Invalid prefix '{1}' in 'Protocol/Name' tag. Current value '{0}'.</Format>
                                                <InputParameters>
                                                    <InputParameter id=""0"" value=""Protocol"">tagValue</InputParameter>
                                                    <InputParameter id=""1"">invalidPrefix</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);
            XDocCheckHelper helper = new(element, null);

            // Act
            var result = helper.GetCheckDescription(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Invalid prefix '{invalidPrefix}' in 'Protocol/Name' tag. Current value 'Protocol'.", result);
        }

        [TestMethod()]
        public void GetCheckDescriptionTest_TemplateOverride()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"" value=""Protocol"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            string xmlTemplateContent = @"  <DescriptionTemplates>
                                                <DescriptionTemplate id=""1000"">
                                                    <Name>MissingTag</Name>
                                                    <Format>Missing tag '{0}'.</Format>
                                                    <InputParameters>
                                                        <InputParameter id=""0"">tagName</InputParameter>
                                                    </InputParameters>
                                                </DescriptionTemplate>
                                            </DescriptionTemplates>";
            XElement element = XElement.Parse(xmlContent);
            XElement template = XElement.Parse(xmlTemplateContent);
            DescriptionTemplates templates = new(template);
            XDocCheckHelper helper = new(element, templates);

            // Act
            var result = helper.GetCheckDescription(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Missing tag 'Protocol'.", result);
        }

        [TestMethod()]
        public void GetCheckDescriptionTest_TemplateNoOverride()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            string xmlTemplateContent = @"  <DescriptionTemplates>
                                                <DescriptionTemplate id=""1000"">
                                                    <Name>MissingTag</Name>
                                                    <Format>Missing tag '{0}'.</Format>
                                                    <InputParameters>
                                                        <InputParameter id=""0"">tagName</InputParameter>
                                                    </InputParameters>
                                                </DescriptionTemplate>
                                            </DescriptionTemplates>";
            XElement element = XElement.Parse(xmlContent);
            XElement template = XElement.Parse(xmlTemplateContent);
            DescriptionTemplates templates = new(template);
            XDocCheckHelper helper = new(element, templates);

            // Act
            var result = helper.GetCheckDescription(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Missing tag '{tagName}'.", result);
        }

        [TestMethod()]
        public void GetCheckDescriptionTest_TemplateNoOverrideOnlyInputTemplate()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000""></Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            string xmlTemplateContent = @"  <DescriptionTemplates>
                                                <DescriptionTemplate id=""1000"">
                                                    <Name>MissingTag</Name>
                                                    <Format>Missing tag '{0}'.</Format>
                                                    <InputParameters>
                                                        <InputParameter id=""0"">tagName</InputParameter>
                                                    </InputParameters>
                                                </DescriptionTemplate>
                                            </DescriptionTemplates>";
            XElement element = XElement.Parse(xmlContent);
            XElement template = XElement.Parse(xmlTemplateContent);
            DescriptionTemplates templates = new(template);
            XDocCheckHelper helper = new(element, templates);

            // Act
            var result = helper.GetCheckDescription(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Missing tag '{tagName}'.", result);
        }

        [TestMethod()]
        public void GetCheckNameTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);
            XDocCheckHelper helper = new(element, null);

            // Act
            var result = helper.GetCheckName();

            // Assert
            Assert.AreEqual("CheckProtocolTag", result);
        }

        [TestMethod()]
        public void GetCheckIdTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);
            XDocCheckHelper helper = new(element, null);

            // Act
            var result = helper.GetCheckId();

            // Assert
            Assert.AreEqual("1", result);
        }

        [TestMethod()]
        public void GetCheckSeverityTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);

            // Act
            var result = XDocCheckHelper.GetCheckSeverity(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Critical", result);
        }

        [TestMethod()]
        public void GetCheckCertaintyTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);

            // Act
            var result = XDocCheckHelper.GetCheckCertainty(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Certain", result);
        }

        [TestMethod()]
        public void GetCheckErrorMessageIdTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);

            // Act
            var result = XDocCheckHelper.GetCheckErrorMessageId(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("5", result);
        }

        [TestMethod()]
        public void GetCheckErrorMessageNameTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);

            // Act
            var result = XDocCheckHelper.GetCheckErrorMessageName(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("InvalidPrefix", result);
        }

        [TestMethod()]
        public void GetCheckGroupDescriptionTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription>Unrecommended chars in some parameter names.</GroupDescription>
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);

            // Act
            var result = XDocCheckHelper.GetCheckGroupDescription(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Unrecommended chars in some parameter names.", result);
        }

        [TestMethod()]
        public void GetCheckSourceTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);

            // Act
            var result = XDocCheckHelper.GetCheckSource(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Validator", result);
        }

        [TestMethod()]
        public void GetCheckFixImpactTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);

            // Act
            var result = XDocCheckHelper.GetCheckFixImpact(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("NonBreaking", result);
        }

        [TestMethod()]
        public void HasCheckErrorMessageCodeFixTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Remove invalid prefix and trim if needed.]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);

            // Act
            var result = XDocCheckHelper.HasCheckErrorMessageCodeFix(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod()]
        public void GetCheckHowToFixTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[Use one of the following constructors:[NewLine]- XmlSerializer.XmlSerializer(Type)[NewLine]- XmlSerializer.XmlSerializer(Type, String)]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);

            // Act
            var result = XDocCheckHelper.GetCheckHowToFix(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual($"Use one of the following constructors:{Environment.NewLine}- XmlSerializer.XmlSerializer(Type){Environment.NewLine}- XmlSerializer.XmlSerializer(Type, String)", result);
        }

        [TestMethod()]
        public void GetCheckExampleCodeTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[]]></HowToFix>
                                            <ExampleCode><![CDATA[object[] elementInfo = new object[] { elementId, ipPort, multipleSet, instance, connectionId, setCommunityString, enableRetries, agentId };[NewLine]object[] oidInfo = new object[] { new object[] { oid, newValue, snmpType } };[NewLine][NewLine]object[] result = (object[])protocol.NotifyProtocol(292/*NT_SNMP_SET*/, elementInfo, oidInfo);]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);

            // Act
            var result = XDocCheckHelper.GetCheckExampleCode(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("object[] elementInfo = new object[] { elementId, ipPort, multipleSet, instance, connectionId, setCommunityString, enableRetries, agentId };" + Environment.NewLine + "object[] oidInfo = new object[] { new object[] { oid, newValue, snmpType } };" + Environment.NewLine + Environment.NewLine + "object[] result = (object[])protocol.NotifyProtocol(292/*NT_SNMP_SET*/, elementInfo, oidInfo);", result);
        }

        [TestMethod()]
        public void GetCheckDetailsTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[Skyline recommends the following structure for driver pages:[NewLine]- General[NewLine]- -----------[NewLine]- Data Page(s)[NewLine]- -----------[NewLine]- WebInterface]]></Details>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);

            // Act
            var result = XDocCheckHelper.GetCheckDetails(element.Descendants("ErrorMessage").FirstOrDefault());

            // Assert
            Assert.AreEqual("Skyline recommends the following structure for driver pages:" + Environment.NewLine + "- General" + Environment.NewLine + "- -----------" + Environment.NewLine + "- Data Page(s)" + Environment.NewLine + "- -----------" + Environment.NewLine + "- WebInterface", result);
        }

        [TestMethod()]
        public void GetCheckAutoFixWarningsTest()
        {
            // Arrange
            string xmlContent = @"  <Check id=""1"">
                                        <Name namespace=""Protocol"">CheckProtocolTag</Name>
                                        <ErrorMessage id=""5"">
                                            <Name>InvalidPrefix</Name>
                                            <GroupDescription />
                                            <Description templateId=""1000"">
                                                <InputParameters>
                                                    <InputParameter id=""0"">tagName</InputParameter>
                                                </InputParameters>
                                            </Description>
                                            <Severity>Critical</Severity>
                                            <Certainty>Certain</Certainty>
                                            <Source>Validator</Source>
                                            <FixImpact>NonBreaking</FixImpact>
                                            <HasCodeFix>True</HasCodeFix>
                                            <HowToFix><![CDATA[]]></HowToFix>
                                            <ExampleCode><![CDATA[]]></ExampleCode>
                                            <Details><![CDATA[]]></Details>
                                            <AutoFixWarnings>
									            <AutoFixWarning autoFixPopup=""true"">Double check the use of the Parameter class in QActions.</AutoFixWarning>
                                                <AutoFixWarning autoFixPopup=""false"">Double check the use of the (Get/Set)ParameterByName methods in QActions.</AutoFixWarning>
                                            </AutoFixWarnings>
                                        </ErrorMessage>
                                    </Check>";
            XElement element = XElement.Parse(xmlContent);

            // Act
            var result = XDocCheckHelper.GetCheckAutoFixWarnings(element.Descendants("ErrorMessage").FirstOrDefault());
            // Assert
            Assert.AreEqual("Double check the use of the Parameter class in QActions.", result[0]);
            Assert.AreEqual("Double check the use of the (Get/Set)ParameterByName methods in QActions.", result[1]);
        }
    }
}
namespace ProtocolTests.Protocol.QActions.QAction.CSharpCheckUnrecommendedConstructor
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CSharpCheckUnrecommendedConstructor;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CSharpCheckUnrecommendedConstructor();

        #region Valid Checks

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedConstructor_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedConstructor_UnrecommendedXmlSerializerConstructor()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "UnrecommendedXmlSerializerConstructor",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.UnrecommendedXmlSerializerConstructor(null, null, null, "System.Xml.Serialization", "XmlSerializer(XmlTypeMapping)", "2"),
                    Error.UnrecommendedXmlSerializerConstructor(null, null, null, "System.Xml.Serialization", "XmlSerializer(Type, Type[])", "3"),
                    Error.UnrecommendedXmlSerializerConstructor(null, null, null, "System.Xml.Serialization", "XmlSerializer(Type, XmlAttributeOverrides)", "4"),
                    Error.UnrecommendedXmlSerializerConstructor(null, null, null, "System.Xml.Serialization", "XmlSerializer(Type, XmlRootAttribute)", "5"),
                    Error.UnrecommendedXmlSerializerConstructor(null, null, null, "System.Xml.Serialization", "XmlSerializer(Type, XmlAttributeOverrides, Type[], XmlRootAttribute, String)", "6"),
                    Error.UnrecommendedXmlSerializerConstructor(null, null, null, "System.Xml.Serialization", "XmlSerializer(Type, XmlAttributeOverrides, Type[], XmlRootAttribute, String, String)", "7"),
                    Error.UnrecommendedXmlSerializerConstructor(null, null, null, "System.Xml.Serialization", "XmlSerializer(Type, XmlAttributeOverrides, Type[], XmlRootAttribute, String, String, Evidence)", "8"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedConstructor_UnrecommendedXmlSerializerConstructor()
        {
            // Create ErrorMessage
            var message = Error.UnrecommendedXmlSerializerConstructor(null, null, null, "unrecommendedConstructor", "constructorNamespace", "qactionId");

            var expected = new ValidationResult
            {
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = "Constructor 'unrecommendedConstructor' ('constructorNamespace') is unrecommended. QAction ID 'qactionId'.",
                Details = "As mentioned on Microsoft docs (https://learn.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer):" + Environment.NewLine + "To increase performance, the XML serialization infrastructure dynamically generates assemblies to serialize and deserialize specified types. The infrastructure finds and reuses those assemblies. This behavior occurs only when using the following constructors:" + Environment.NewLine + "- XmlSerializer.XmlSerializer(Type)" + Environment.NewLine + "- XmlSerializer.XmlSerializer(Type, String)" + Environment.NewLine + "If you use any of the other constructors, multiple versions of the same assembly are generated and never unloaded, which results in a memory leak and poor performance. The easiest solution is to use one of the previously mentioned two constructors.",
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot check = new CSharpCheckUnrecommendedConstructor();

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedConstructor_CheckCategory() => Generic.CheckCategory(check, Category.QAction);

        [TestMethod]
        public void QAction_CSharpCheckUnrecommendedConstructor_CheckId() => Generic.CheckId(check, CheckId.CSharpCheckUnrecommendedConstructor);
    }
}
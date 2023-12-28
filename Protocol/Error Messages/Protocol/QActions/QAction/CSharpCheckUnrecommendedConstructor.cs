namespace SLDisValidator2.Tests.Protocol.QActions.QAction.CSharpCheckUnrecommendedConstructor
{
    using System;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using SLDisValidator2.Common;
    using SLDisValidator2.Interfaces;

    internal static class Error
    {
        internal static IValidationResult UnrecommendedXmlSerializerConstructor(IValidate test, IReadable referenceNode, IReadable positionNode, string typeUnrecommendedConstructor, string constructorNamespace, string qactionId)
        {
            return new ValidationResult
            {
                Test = test,
                CheckId = CheckId.CSharpCheckUnrecommendedConstructor,
                ErrorId = ErrorIds.UnrecommendedXmlSerializerConstructor,
                FullId = "3.36.1",
                Category = Category.QAction,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.NonBreaking,
                GroupDescription = "",
                Description = String.Format("Constructor '{0}' ('{1}') is unrecommended. QAction ID '{2}'.", typeUnrecommendedConstructor, constructorNamespace, qactionId),
                HowToFix = "Use one of the following constructors:" + Environment.NewLine + "- XmlSerializer.XmlSerializer(Type)" + Environment.NewLine + "- XmlSerializer.XmlSerializer(Type, String)",
                ExampleCode = "",
                Details = "As mentioned on Microsoft docs (https://learn.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer):" + Environment.NewLine + "To increase performance, the XML serialization infrastructure dynamically generates assemblies to serialize and deserialize specified types. The infrastructure finds and reuses those assemblies. This behavior occurs only when using the following constructors:" + Environment.NewLine + "- XmlSerializer.XmlSerializer(Type)" + Environment.NewLine + "- XmlSerializer.XmlSerializer(Type, String)" + Environment.NewLine + "If you use any of the other constructors, multiple versions of the same assembly are generated and never unloaded, which results in a memory leak and poor performance. The easiest solution is to use one of the previously mentioned two constructors.",
                HasCodeFix = false,

                PositionNode = positionNode,
                ReferenceNode = referenceNode,
            };
        }
    }

    internal static class ErrorIds
    {
        public const uint UnrecommendedXmlSerializerConstructor = 1;
    }

    public static class CheckId
    {
        public const uint CSharpCheckUnrecommendedConstructor = 36;
    }
}
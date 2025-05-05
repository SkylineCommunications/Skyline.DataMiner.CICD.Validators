---
uid: Validator_3_36_1
---

# CSharpCheckUnrecommendedConstructor

## UnrecommendedXmlSerializerConstructor

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

As mentioned on Microsoft docs (https://learn.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer):
To increase performance, the XML serialization infrastructure dynamically generates assemblies to serialize and deserialize specified types. The infrastructure finds and reuses those assemblies. This behavior occurs only when using the following constructors:
- XmlSerializer.XmlSerializer(Type)
- XmlSerializer.XmlSerializer(Type, String)
If you use any of the other constructors, multiple versions of the same assembly are generated and never unloaded, which results in a memory leak and poor performance. The easiest solution is to use one of the previously mentioned two constructors.

<!-- Uncomment to add example code -->
<!--### Example code-->

# Skyline.DataMiner.CICD.Validators.Protocol.Features

## About

This NuGet package holds the code to identify which features are being used inside the connector. Do note that this is not a full list of all the features.

### About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exist. In addition, you can leverage DataMiner Development Packages to build you own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

### About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.

## Getting Started

### Creation of ProtocolInputData

```csharp
string protocolXml = ...;
Microsoft.CodeAnalysis.Solution solution = ...;

Parser parser = new Parser(protocolXml);
XmlDocument document = parser.Document;
ProtocolModel model = new ProtocolModel(document);

QActionCompilationModel qactionCompilationModel = new QActionCompilationModel(model, solution);

var protocolInputData = new ProtocolInputData(model, document, qactionCompilationModel);

```

### Run VersionChecker

```csharp
CancellationToken cancellationToken = ...;
var protocolInputData = ...;

var result = VersionChecker.GetUsedFeatures(protocolInputData, cancellationToken);

```

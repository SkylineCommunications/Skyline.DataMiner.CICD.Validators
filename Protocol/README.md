# Skyline.DataMiner.CICD.Validators.Protocol

## About

This NuGet package holds the code to validate DataMiner connectors.

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

### Creation of ValidatorSettings

```csharp

DataMinerVersion minimumSupportedDataMinerVersion = ...;
IUnitList unitList = ...; // See Skyline.DataMiner.XmlSchemas.Protocol NuGet

var validatorSettings = new ValidatorSettings(minimumSupportedDataMinerVersion, unitList);

// Optionally you can specify the expected provider for the CheckProviderTag check.
validatorSettings.ExpectedProvider = "My Company Name";

```

#### Limited set of checks

If you want to validate a limited set of checks, you can specify those in the ValidatorSettings:

```csharp

Category category = Category.Protocol;
uint checkId = Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckProtocolTag.CheckId.CheckProtocolTag;

validatorSettings.WithTestToExecute((category, checkId));

```

### Validator

```csharp
CancellationToken cancellationToken = ...;
var protocolInputData = ...;
var validatorSettings = ...;

Validator validator = new Validator();

IList<IValidationResult> results = validator.RunValidate(protocolInputData, validatorSettings, cancellationToken);

```

### Major Change Checker

```csharp
CancellationToken cancellationToken = ...;
var oldProtocolInputData = ...;
var newProtocolInputData = ...;
var validatorSettings = ...;

Validator validator = new Validator();

IList<IValidationResult> results = validator.RunCompare(newProtocolInputData, oldProtocolInputData, validatorSettings, cancellationToken);

```

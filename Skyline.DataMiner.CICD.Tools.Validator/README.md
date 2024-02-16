# Skyline.DataMiner.CICD.Tools.Validator

## About

Validates a DataMiner artifact.

> **Note**
> Currently this tool only supports DataMiner protocol Visual Studio solutions.
> 
> Protocol packages (.dmprotocol) are not supported yet.

> **Note**
> Usage of this tool is tracked through non-personal metrics provided through a single https call on each use.
>
> These metrics may include, but are not limited to, the frequency of use and the primary purposes for which the Software is employed (e.g., automation, protocol analysis, visualization, etc.). By using the Software, you agree to allow Skyline to collect and analyze such metrics for the purpose of improving and enhancing the Software.

### About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exist. In addition, you can leverage DataMiner Development Packages to build you own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

### About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.

## Getting Started

In a command line terminal, install the tool using the following command:

```console
dotnet tool install -g Skyline.DataMiner.CICD.Tools.Validator
```
To validate a protocol Visual Studio solution, the following command can be used:

```console
dataminer-validator validate-protocol-solution --solution-file-path "<pathToSlnFile>" --results-output-directory-path "<FolderPath>"
```

To obtain more information about all the options, the following command can be use:

```console
dataminer-validator validate-protocol-solution -h
```

Note: By default, a build operation will be performed. If you do not want this (e.g. because the tool is being used as part of a CI/CD pipeline where one of the previous steps is a build step, you can disable it by setting the `--perform-build` option to `false`.
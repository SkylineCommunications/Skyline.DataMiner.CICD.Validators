# Skyline.DataMiner.CICD.Tools.Validator

## About

Validates and compares DataMiner artifacts.

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

### Validating a protocol solution

To validate a protocol Visual Studio solution, use the following command:

```console
dataminer-validator validate protocol-solution --solution-path "<pathToSlnFile>" --output-directory "<FolderPath>"
```

To obtain more information about all the options:

```console
dataminer-validator validate protocol-solution -h
```

### Comparing a protocol solution with a previous version

To compare a protocol solution with its previous version (using the Major Change Checker), use the following command:

```console
dataminer-validator compare protocol-solution --solution-path "<pathToSlnFile>" --output-directory "<FolderPath>" --catalog-id "<catalogId>" --catalog-api-key "<apiKey>"
```

The tool can automatically retrieve the previous version from the DataMiner Catalog, or you can provide a path to a previous protocol.xml file using the `--previous-protocol-xml-path` option.

Whenever a previous version is available (either resolved from the Catalog or supplied via `--previous-protocol-xml-path`), the compare command will **also run the validator against that previous version** and write the results to a separate output file (default name: `previousValidatorsResults`). The file name can be overridden via `--previous-validate-output-file-name` (`-pvofn`). When the current version is an initial version (e.g. `1.0.0.1`, or `X.0.0.1` without a `BasedOn` attribute), no previous version is fetched and this extra file is not produced.

In the same scenario the compare command also produces an **XML-based validation of the current version** (default name: `currentValidatorResults`, overridable via `--current-validate-output-file-name` / `-cvofn`). This file uses the same validation scope as the previous-version file (bare `protocol.xml`, no QAction C# compilation), so the two are directly comparable in CI gates that diff current vs previous validator results. This file is independent of the regular `validate protocol-solution` command output, which remains the authoritative solution-based validation.

When the current protocol version's revision is `1` (e.g. `1.0.0.1`, `2.0.0.1`, `2.1.0.1`, `1.2.3.1`), the Major Change Checker pass is **skipped** regardless of whether a `BasedOn` attribute is set or a previous version was resolved. The output `MajorChangeCheckerResults.json` is still written, but with `Skipped: true` and a `SkippedReason` describing why. The previous/current XML-based validate output files are still produced as long as a previous version is available, so downstream tooling can still evaluate validator-result deltas for revision-1 versions with a `BasedOn`.

> **Note**
> Validation of the previous version runs against the bare `protocol.xml` only, so QAction C# checks are skipped. The same limitation applies to the regular compare logic and to the XML-based current-version validation produced by the compare command.

To obtain more information about all the options:

```console
dataminer-validator compare protocol-solution -h
```

### Additional options

Both commands support the following common options:
- `--output-file-name` / `-ofn`: Name of the results file (without extension)
- `--output-format` / `-of`: Output format(s): JSON, XML, HTML (default: JSON and HTML)
- `--include-suppressed` / `-is`: Include suppressed results in the output

For the validate command:
- `--perform-restore` / `-pr`: Perform a dotnet restore operation (default: true)
- `--restore-timeout` / `-rt`: Timeout for the restore operation in milliseconds (default: 300000)
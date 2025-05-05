---
uid: Validator_3_33_3
---

# CSharpSLProtocolGetParameters

## HardCodedPid

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

SLProtocol.GetParameters is used to get current values of standalone parameters.
Make sure to provide it with a uint array of existing standalone parameter IDs.
Using Parameter class is recommended.

### Example code

protocol.GetParameters(new uint[] { Parameter.ParameterName, Parameter.ParameterName2 });

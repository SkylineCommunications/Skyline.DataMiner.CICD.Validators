---
uid: Validator_3_8_2
---

# CSharpSLProtocolSetRow

## HardCodedPid

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

SLProtocol.SetRow is used to update the values of a table row.
Make sure to provide it with an ID of a table parameter that exists.
Using Parameter class is recommended.

### Example code

protocol.SetRow(Parameter.TableName.tablePid, key, row);

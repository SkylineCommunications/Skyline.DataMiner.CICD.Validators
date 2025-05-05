---
uid: Validator_3_20_1
---

# CSharpNotifyDataMinerNTUpdatePortsXml

## DeltIncompatible

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.
See Example code.

More information about the syntax can be found in the DataMiner Development Library.

### Example code

string updateConfig = changeType + ";" + elementId + ";" + parameterId + ";" + agentId;
string updateValue = inputs + ";" + outputs;

int result = (int) protocol.NotifyDataMinerQueued(128/*NT_UPDATE_PORTS_XML*/, updateConfig, updateValue);

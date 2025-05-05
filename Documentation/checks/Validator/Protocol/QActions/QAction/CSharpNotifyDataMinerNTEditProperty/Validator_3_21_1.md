---
uid: Validator_3_21_1
---

# CSharpNotifyDataMinerNTEditProperty

## DeltIncompatible

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.
See Example code.

More information about the syntax can be found in the DataMiner Development Library.

### Example code

string propertyLocation = "element:"+ elementId + ":" + agentId;
string[] propertyDetails = new string[3] {"DeviceKey", "read-write", "2100"};

protocol.NotifyDataMinerQueued(62/*NT_EDIT_PROPERTY*/ , propertyLocation, propertyDetails);

---
uid: Validator_3_27_1
---

# CSharpNotifyDataMinerNTGetElementName

## DeltIncompatible

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.
See Example code.

More information about the syntax can be found in the DataMiner Development Library.

### Example code

uint[] elementDetails = new uint[] { agentId, elementId };
string elementName = (string) protocol.NotifyDataMiner(144/*NT_GET_ELEMENT_NAME */, elementDetails, null);

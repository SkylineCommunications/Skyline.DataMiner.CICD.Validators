---
uid: Validator_3_16_1
---

# CSharpNotifyDataMinerNTSetElementState

## DeltIncompatible

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.
See Example code.

More information about the syntax can be found in the DataMiner Development Library.

### Example code

uint[] elementDetails = new uint[] { elementId, state, deleteOptions, dmaID };
protocol.NotifyDataMiner(115 /*NT_SET_ELEMENT_STATE*/ , elementDetails, null);

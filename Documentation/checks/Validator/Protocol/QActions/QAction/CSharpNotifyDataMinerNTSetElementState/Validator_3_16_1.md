---
uid: Validator_3_16_1
---

# CSharpNotifyDataMinerNTSetElementState

## DeltIncompatible

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.
See Example code.

More information about the syntax can be found in the DataMiner Development Library.

### Example code

uint[] elementDetails = new uint[] { elementId, state, deleteOptions, dmaID };
protocol.NotifyDataMiner(115 /*NT_SET_ELEMENT_STATE*/ , elementDetails, null);

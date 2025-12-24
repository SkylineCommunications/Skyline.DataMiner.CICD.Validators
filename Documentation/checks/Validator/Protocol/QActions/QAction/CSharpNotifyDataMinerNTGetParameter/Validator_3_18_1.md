---
uid: Validator_3_18_1
---

# CSharpNotifyDataMinerNTGetParameter

## DeltIncompatible

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.
See Example code.

More information about the syntax can be found in the DataMiner Development Library.

### Example code

uint[] ids = new uint[] { dmaID, elementID, parameterID };
object[] result = (object[])protocol.NotifyDataMiner(73/*NT_GET_PARAMETER*/, ids, null);

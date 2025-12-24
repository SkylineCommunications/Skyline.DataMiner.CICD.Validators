---
uid: Validator_3_17_1
---

# CSharpNotifyDataMinerNTSetAlarmState

## DeltIncompatible

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.
See Example code.

More information about the syntax can be found in the DataMiner Development Library.

### Example code

object elementDetails = new uint[] {elementID, state, dmaID};
object maskDetails = new string[] {maskType, comment};

protocol.NotifyDataMiner(116 /* NT_SET_ALARM_STATE */, elementDetails, maskDetails);

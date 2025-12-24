---
uid: Validator_3_19_1
---

# CSharpNotifyDataMinerNTAssignAlarmTemplate

## DeltIncompatible

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.
See Example code.

More information about the syntax can be found in the DataMiner Development Library.

### Example code

uint[] elementDetails = { agentId, elementId };
string[] alarmTemplate = new string[] { "Alarm Template 1" };

protocol.NotifyDataMiner(117/*NT_ASSIGN_ALARM_TEMPLATE*/, elementDetails, alarmTemplate);

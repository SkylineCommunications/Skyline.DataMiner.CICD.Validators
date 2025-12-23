---
uid: Validator_3_30_1
---

# CSharpNotifyProtocolNTSnmpGet

## DeltIncompatible

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.
See Example code.

More information about the syntax can be found in the DataMiner Development Library.

### Example code

object[] elementInfo = new object[] { elementId, ipPort, multipleGet, instance, connectionId, getCommunityString, splitErrors, agentId };
string[] oidInfo = new string[] { "1.3.6.1.2.1.1.4.0" };

object[] result = (object[])protocol.NotifyProtocol(295/*NT_SNMP_GET*/, elementInfo, oidInfo);

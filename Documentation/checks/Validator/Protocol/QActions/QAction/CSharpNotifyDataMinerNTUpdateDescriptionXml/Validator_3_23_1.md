---
uid: Validator_3_23_1
---

# CSharpNotifyDataMinerNTUpdateDescriptionXml

## DeltIncompatible

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.
See Example code.

More information about the syntax can be found in the DataMiner Development Library.

### Example code

uint[] elementDetails = new uint[] { agentId, elementId };
object[] updates = new object[] { update1, update2 };

int result = (int) protocol.NotifyDataMinerQueued(127/*NT_UPDATE_DESCRIPTION_XML */ , elementDetails, updates);

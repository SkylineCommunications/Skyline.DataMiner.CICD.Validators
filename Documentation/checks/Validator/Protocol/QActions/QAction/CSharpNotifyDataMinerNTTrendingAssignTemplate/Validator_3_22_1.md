---
uid: Validator_3_22_1
---

# CSharpNotifyDataMinerNTTrendingAssignTemplate

## DeltIncompatible

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.
See Example code.

More information about the syntax can be found in the DataMiner Development Library.

### Example code

uint[] elementDetails = { agentId, elementId };
string[] trendTemplate = new string[] { "Template 1" };

protocol.NotifyDataMiner(14 /*NT_TRENDING_ASSIGN_TEMPLATE*/, elementDetails, trendTemplate);

---  
uid: Validator_3_25_1  
---

# CSharpNotifyDataMinerNTGetValue

## DeltIncompatible

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.  
See Example code.  
More information about the syntax can be found in the DataMiner Development Library.

### Example code

```xml
uint[] elementDetails = new uint[] { agentId, elementId };
int parameterId = 120;

object[] result = (object[]) protocol.NotifyDataMiner(69 /*NT_GET_VALUE*/, elementDetails, parameterId);
```

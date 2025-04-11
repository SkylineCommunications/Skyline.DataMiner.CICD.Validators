---  
uid: Validator_3_24_1  
---

# CSharpNotifyDataMinerNTAssignSimulation

## DeltIncompatible

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.  
See Example code.  
More information about the syntax can be found in the DataMiner Development Library.

### Example code

```xml
uint[] elementDetails = new uint[] { agentId, elementId };
bool assignSimulation = false;

protocol.NotifyDataMinerQueued(76 /*NT_ASSIGN_SIMULATION*/ , elementDetails, assignSimulation);
```

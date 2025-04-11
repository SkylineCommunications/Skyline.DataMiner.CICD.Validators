---  
uid: Validator_3_18_1  
---

# CSharpNotifyDataMinerNTGetParameter

## DeltIncompatible

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.  
See Example code.  
More information about the syntax can be found in the DataMiner Development Library.

### Example code

```xml
uint[] ids = new uint[] { dmaID, elementID, parameterID };
object[] result = (object[])protocol.NotifyDataMiner(73/*NT_GET_PARAMETER*/, ids, null);
```

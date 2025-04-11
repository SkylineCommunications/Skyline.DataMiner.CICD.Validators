---  
uid: Validator_3_26_1  
---

# CSharpNotifyDataMinerNTGetAlarmInfo

## DeltIncompatible

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.  
See Example code.  
More information about the syntax can be found in the DataMiner Development Library.

### Example code

```xml
uint[] elementInfo = new uint[] { dmaId, elementId };
uint[] parameterIds = new uint[] { 100, 300 };

object[] result = (object[]) protocol.NotifyDataMiner(48 /* NT_GET_ALARM_INFO */, elementInfo, parameterIds);
```

---  
uid: Validator_3_28_1  
---

# CSharpNotifyDataMinerNTServiceSetVdx

## DeltIncompatible

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.  
See Example code.  
More information about the syntax can be found in the DataMiner Development Library.

### Example code

```xml
string serviceInfo = dmaId + "/" + serviceId;
string serviceVdx = "Visio|1";

protocol.NotifyDataMiner(232 /*NT_SERVICE_SET_VDX*/ , serviceInfo, serviceVdx);
```

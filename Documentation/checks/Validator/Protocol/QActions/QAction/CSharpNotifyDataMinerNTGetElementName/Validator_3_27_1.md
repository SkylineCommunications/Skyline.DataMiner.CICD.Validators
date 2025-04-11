---  
uid: Validator_3_27_1  
---

# CSharpNotifyDataMinerNTGetElementName

## DeltIncompatible

### Details

To make this call DELT compatible, the DMA ID needs to be provided as argument.  
See Example code.  
More information about the syntax can be found in the DataMiner Development Library.

### Example code

```xml
uint[] elementDetails = new uint[] { agentId, elementId };
string elementName = (string) protocol.NotifyDataMiner(144/*NT_GET_ELEMENT_NAME */, elementDetails, null);
```

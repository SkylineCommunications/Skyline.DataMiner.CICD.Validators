---  
uid: Validator_3_33_3  
---

# CSharpSLProtocolGetParameters

## HardCodedPid

### Details

SLProtocol.GetParameters is used to get current values of standalone parameters.  
Make sure to provide it with a uint array of existing standalone parameter IDs.  
Using Parameter class is recommended.

### Example code

```xml
protocol.GetParameters(new uint[] { Parameter.ParameterName, Parameter.ParameterName2 });
```

---  
uid: Validator_3_7_2  
---

# CSharpSLProtocolSetParameter

## HardCodedPid

### Details

SLProtocol.SetParameter is used to update the value of a standalone parameter.  
Make sure to provide it with an ID of a standalone parameter that exists.  
Using Parameter class is recommended.

### Example code

```xml
protocol.SetParameter(Parameter.ParameterName, value);
```

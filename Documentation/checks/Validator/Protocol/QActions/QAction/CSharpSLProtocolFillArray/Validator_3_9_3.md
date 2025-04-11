---  
uid: Validator_3_9_3  
---

# CSharpSLProtocolFillArray

## HardCodedPid

### Details

SLProtocol.FillArray is used to update a table with new values.  
Make sure to provide it with an ID of a table parameter that exists.  
Using Parameter class is recommended.

### Example code

```xml
protocol.FillArray(Parameter.TableName.tablePid, ..);
```

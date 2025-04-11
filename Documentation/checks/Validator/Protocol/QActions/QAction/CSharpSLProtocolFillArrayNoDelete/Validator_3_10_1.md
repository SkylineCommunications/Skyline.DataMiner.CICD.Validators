---  
uid: Validator_3_10_1  
---

# CSharpSLProtocolFillArrayNoDelete

## NonExistingParam

### Details

SLProtocol.FillArrayNoDelete is used to update a table with new values.  
Make sure to provide it with an ID of a table parameter that exists.  
Using Parameter class is recommended.

### Example code

```xml
protocol.FillArrayNoDelete(Parameter.TableName.tablePid, ..);
```

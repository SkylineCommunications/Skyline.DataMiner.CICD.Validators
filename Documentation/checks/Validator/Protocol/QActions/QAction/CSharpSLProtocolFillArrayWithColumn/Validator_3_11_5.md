---  
uid: Validator_3_11_5  
---

# CSharpSLProtocolFillArrayWithColumn

## HardCodedColumnPid

### Details

SLProtocol.FillArrayWithColumn is used to update the values of a column.  
Make sure to provide it with an ID of a column parameter that exists.  
Using Parameter class is recommended.

### Example code

```xml
protocol.FillArrayWithColumn(Parameter.TableName.tablePid, Parameter.TableName.Pid.ColumnName, keys, values);
```

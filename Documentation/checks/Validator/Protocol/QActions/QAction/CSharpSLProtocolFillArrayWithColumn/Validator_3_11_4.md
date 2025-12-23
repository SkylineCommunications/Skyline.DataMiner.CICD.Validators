---
uid: Validator_3_11_4
---

# CSharpSLProtocolFillArrayWithColumn

## HardCodedTablePid

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

SLProtocol.FillArrayWithColumn is used to update the values of a column.
Make sure to provide it with an ID of a table parameter that exists.
Using Parameter class is recommended.

### Example code

protocol.FillArrayWithColumn(Parameter.TableName.tablePid, Parameter.TableName.Pid.ColumnName, keys, values);

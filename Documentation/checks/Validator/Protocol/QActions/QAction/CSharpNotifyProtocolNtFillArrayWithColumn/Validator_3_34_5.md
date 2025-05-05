---
uid: Validator_3_34_5
---

# CSharpNotifyProtocolNtFillArrayWithColumn

## HardCodedTablePid

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

NotifyProtocol(220/*NT_FILL_ARRAY_WITH_COLUMN*/, ...) is used to update the values of column(s).
Make sure to provide it with an ID of a table parameter that exists.
Using Parameter class is recommended.

### Example code

object[] columnPids = new object[]
{
 Parameter.TableName.tablePid,
 Parameter.TableName.Pid.ColumnName
};
object[] columnValues = new object[]
{
 keys,
 values
};
protocol.NotifyProtocol((int)NotifyType.NT_FILL_ARRAY_WITH_COLUMN, columnPids, columnValues);

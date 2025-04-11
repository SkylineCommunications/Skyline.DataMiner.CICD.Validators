---  
uid: Validator_3_34_8  
---

# CSharpNotifyProtocolNtFillArrayWithColumn

## ColumnManagedByProtocolItem

### Details

Some columns are fully managed by protocol items and therefore cannot be updated from QActions.  
Examples:  
\- Multi\-threaded timers with following ping options: rttColumn, timestampColumn, jitterColumn, latencyColumn, packetLossRateColumn.  
\- Merge actions (result destination).  
\- ...

---  
uid: Validator_3_15_3  
---

# CSharpCheckUnrecommendedMethod

## UnrecommendedSlProtocolSetParameterIndex

### Details

'SLProtocol.SetParameterIndex' method is used to set the value of a specific cell in a table.  
The problem with this call is that it relies on an indexer to identify the row for which a cell value needs to be updated.  
However, the order of rows in element tables is not guaranteed.  
Meaning, using an index (row position) is not ideal.  
Instead, it is recommended to use the 'SLProtocol.SetParameterIndexByKey' method which relies on the primary key of the rows.

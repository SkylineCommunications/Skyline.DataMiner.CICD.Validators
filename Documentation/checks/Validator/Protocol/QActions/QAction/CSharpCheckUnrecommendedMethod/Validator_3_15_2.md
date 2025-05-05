---
uid: Validator_3_15_2
---

# CSharpCheckUnrecommendedMethod

## UnrecommendedSlProtocolGetParameterIndex

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

'SLProtocol.GetParameterIndex' method is used to get the value of a specific cell in a table.
The problem with this call is that it relies on an indexer to identify the row for which a cell value needs to be retrieved.
However, the order of rows in element tables is not guaranteed.
Meaning, using an index (row position) is not ideal.

Instead, it is recommended to use the 'SLProtocol.GetParameterIndexByKey' method which relies on the primary key of the rows.

<!-- Uncomment to add example code -->
<!--### Example code-->

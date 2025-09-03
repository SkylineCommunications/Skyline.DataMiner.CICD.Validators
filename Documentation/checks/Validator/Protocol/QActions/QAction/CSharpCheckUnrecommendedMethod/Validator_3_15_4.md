---
uid: Validator_3_15_4
---

# CSharpCheckUnrecommendedMethod

## UnrecommendedSlProtocolSetParametersIndex

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

'SLProtocol.SetParametersIndex' method is used to set the value of a specific cells in a table.
The problem with this call is that it relies on an indexer to identify the rows for which a cells values need to be updated.
However, the order of rows in element tables is not guaranteed.
Meaning, using an index (row position) is not ideal.

Instead, it is recommended to use the 'SLProtocol.SetParametersIndexByKey' method which relies on the primary key of the rows.

<!-- Uncomment to add example code -->
<!--### Example code-->

---
uid: Validator_2_78_2
---

# CheckVolatileTables

## SuggestedVolatileOption

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

Volatile tables reduce DB usage by keeping rows only in memory.
Safe when the table has no saved/alarmed/state columns, relations, DVE usage, DCF, or trending.
Note: Making an existing table volatile may require recreating the element to ensure stale data is cleared.

### Example code

<Param id="1000">
 <Name>ValidVolatileTable</Name>
 <Type>array</Type>
 <ArrayOptions index="0" options=";volatile;">
  <ColumnOption idx="0" pid="1001" type="retrieved" options=""/>
  <ColumnOption idx="1" pid="1002" type="retrieved" options=""/>
 </ArrayOptions>
</Param>

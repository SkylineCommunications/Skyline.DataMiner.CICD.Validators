---
uid: Validator_4_2_5
---

# CheckParamTag

## ObsoleteSuffixTable

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

Suffix 'table' is used in 'Group/Content/Param' element to indicate the provided parameter represents a table to be polled.
This results in a number of GetNext request operations to retrieve the instances followed by a number of Get request operations to retrieve the table data column by column. This way of polling is inefficient and can some times lead to corrupted data.

To improve on this, the new suffix 'tablev2' should be used and will result in a number of getBulk request operations to retrieve the table data row by row.

Note that these suffixes can only be used if the group is called by a multi-threaded timer.

### Example code

<Param>100:tablev2</Param>

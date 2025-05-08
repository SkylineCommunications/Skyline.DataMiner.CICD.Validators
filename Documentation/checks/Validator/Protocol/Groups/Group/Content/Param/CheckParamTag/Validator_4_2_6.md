---
uid: Validator_4_2_6
---

# CheckParamTag

## SuffixRequiresMultiThreadedTimer

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

Within 'Group/Content/Param' element, following suffixes are allowed:
 - Single: If ":single" is appended after the parameter ID, this parameter will be retrieved via a separate SNMP Get request.
 - Instance: Indicates that this parameter holds the instance value. The following parameters in the group will use the value retrieved by this parameter as the instance.
 - table: (Deprecated) Indicates that the requested parameter represents a table. Use tablev2 instead.
 - tablev2: Indicates that the requested parameter represents a table.
 - getnext: Performs a GetNext request.

However, all those are meant to poll data via a multi-threaded timer so a group containing such suffixes can only be called from multi-threaded timers.

### Example code

<Param>100:tablev2</Param>

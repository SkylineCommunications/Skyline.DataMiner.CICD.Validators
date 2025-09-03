---
uid: Validator_2_47_2
---

# CheckOidTagIdAttrCombo

## InvalidCombo

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

The SNMP/OID@id attribute needs to refer to a standalone read Param and can be used in following situations:
- Subtables: the id should refer to a parameter that will allow to filter on the list of instances to be polled.
- Tables with filtered rows: each column should contain an id attribute that refers to a parameter that will allow to filter on the instances to be polled.
- Dynamic OID: the id should refer to a parameter that will allow to dynamically define the full OID.
Except for subtables, the id attribute only makes sense in case a wildcard is present in the OID tag value.

<!-- Uncomment to add example code -->
<!--### Example code-->

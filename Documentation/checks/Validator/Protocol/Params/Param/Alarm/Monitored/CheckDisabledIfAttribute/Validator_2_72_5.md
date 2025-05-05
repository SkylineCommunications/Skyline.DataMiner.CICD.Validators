---
uid: Validator_2_72_5
---

# CheckDisabledIfAttribute

## ReferencedParamWrongType

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

Monitored@disabledIf attribute should follow the following format 'pid,value' where:
- pid: refers to the ID of an existing parameter.
  The referenced Param is expected to:
    - Have RTDisplay tag set to 'true'.
- value: correspond to the value of the referenced parameter which will cause the monitoring to be disabled.
  When using discreet values, it is only possible to set a condition on the discreet value, not on the display value.

<!-- Uncomment to add example code -->
<!--### Example code-->

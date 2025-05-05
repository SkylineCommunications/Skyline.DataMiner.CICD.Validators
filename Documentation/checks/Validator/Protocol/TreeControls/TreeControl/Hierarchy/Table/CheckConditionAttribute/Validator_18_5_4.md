---
uid: Validator_18_5_4
---

# CheckConditionAttribute

## NonExistingId

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

Hierarchy/Table@condition attribute should have one of the following format:
- {conditionColumnPid}:{conditionValue}
- {conditionColumnPid}:{conditionValue};filter:fk={fkColumnPid}
where:
- {conditionColumnPid} refers to a column which should have its RTDisplay set to true.
- {conditionValue} allows to specify the value to be present in column referred by {conditionColumnPid} for the condition to match.

<!-- Uncomment to add example code -->
<!--### Example code-->

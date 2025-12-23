---
uid: Validator_18_5_6
---

# CheckConditionAttribute

## MissingValueInAttribute_Sub

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

Hierarchy/Table@condition attribute should have one of the following format:
- {conditionColumnPid}:{conditionValue}
- {conditionColumnPid}:{conditionValue};filter:fk={fkColumnPid}
where:
- {conditionColumnPid} refers to a column which should have its RTDisplay set to true.
- {conditionValue} allows to specify the value to be present in column referred by {conditionColumnPid} for the condition to match.

<!-- Uncomment to add example code -->
<!--### Example code-->

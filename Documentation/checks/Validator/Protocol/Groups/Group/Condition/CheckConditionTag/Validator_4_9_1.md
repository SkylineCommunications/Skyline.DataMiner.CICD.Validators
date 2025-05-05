---
uid: Validator_4_9_1
---

# CheckConditionTag

## InvalidCondition

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

A 'Group/Condition' should always contain a statement returning a boolean.
See DDL for more information.

Here are a few examples of mistakes covered by this error:
- Empty condition.
- Malformed condition:
  - The 'id:' placeholder used to retrieve a parameter value is incorrectly defined.
  - The number of opening & closing parenthesis is not matching.
  - '&&', '||' is used instead of 'AND', 'OR'.
- Condition that is not a boolean expression.
- Fully hard-coded boolean expression (No reference to any parameter value).
- etc.

<!-- Uncomment to add example code -->
<!--### Example code-->

---
uid: Validator_5_8_5
---

# CheckIdAttribute

## DuplicatedId

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

The id attribute is used internally as the identifier for each trigger.
It is therefore mandatory and needs to follow a number of rules:
- Each trigger should have a unique id.
- Should be an unsigned integer.
- Only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).

<!-- Uncomment to add example code -->
<!--### Example code-->

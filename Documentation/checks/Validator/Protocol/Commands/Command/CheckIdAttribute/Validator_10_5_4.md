---
uid: Validator_10_5_4
---

# CheckIdAttribute

## InvalidValue

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

The id attribute is used internally as the identifier for each command.
It is therefore mandatory and needs to follow a number of rules:
- Each command should have a unique id.
- Should be an unsigned integer.
- Only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).

<!-- Uncomment to add example code -->
<!--### Example code-->

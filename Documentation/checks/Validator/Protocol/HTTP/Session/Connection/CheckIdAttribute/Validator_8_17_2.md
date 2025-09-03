---
uid: Validator_8_17_2
---

# CheckIdAttribute

## EmptyAttribute

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

The id attribute is used internally as the identifier for each connection within a session.
It is therefore mandatory and needs to follow a number of rules:
- Each connection within a session should have a unique id.
- Should be an unsigned integer.
- Only plain numbers are allowed (no leading signs, no leading zeros, no scientific notation, etc).

<!-- Uncomment to add example code -->
<!--### Example code-->

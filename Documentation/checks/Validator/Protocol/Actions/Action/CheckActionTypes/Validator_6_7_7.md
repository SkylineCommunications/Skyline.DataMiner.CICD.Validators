---
uid: Validator_6_7_7
---

# CheckActionTypes

## ExcessiveTypeIdOrTypeValueAttribute

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

Following cases require either a 'Action/Type@id' or a 'Action/Type@value' attribute (one or the other, not both):
- On pair:
    - set next: define the 'time to wait after pair' value (in ms) either by referencing the ID of a parameter containing the (dynamic) value via 'Type@id' or by hard coding the value via the 'Type@value' attribute.

<!-- Uncomment to add example code -->
<!--### Example code-->

---
uid: Validator_2_1_14
---

# CheckIdAttribute

## RTDisplayExpectedOnSpectrumParam

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

Parameters with ID in the range [64 000, 64 299] are considered as spectrum parameters.
Such spectrum Param should have its RTDisplay set to true.

### How to fix

Double check if the parameter is required or not. If so, add RTDisplay. If not, remove it.

<!-- Uncomment to add example code -->
<!--### Example code-->

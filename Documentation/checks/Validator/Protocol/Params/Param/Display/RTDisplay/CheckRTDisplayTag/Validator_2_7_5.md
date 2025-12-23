---
uid: Validator_2_7_5
---

# CheckRTDisplayTag

## RTDisplayUnexpected

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

This protocol doesn't contain anything that would justify the need of the RTDisplay tag being true.

### How to fix

Double check if this Param requires RTDisplay for reasons that are outside the scope of this connector (Visios, automation scripts, etc).

- If so, suppress this result and explain why RTDisplay is required via the suppression comment.
- If not, remove the full Display tag containing this RTDisplay tag.

<!-- Uncomment to add example code -->
<!--### Example code-->

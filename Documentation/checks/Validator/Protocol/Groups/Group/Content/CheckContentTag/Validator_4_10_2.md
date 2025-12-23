---
uid: Validator_4_10_2
---

# CheckContentTag

## MixedTypes

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

Depending on the Group/Type, the Group/Content can only contain certain tags:
- 'poll': Can contain multiple instances of one of the below tags but not a mix of them:
    - 'Param'
    - 'Pair'
    - 'Session'
- 'action' / 'poll action': Can only contain Action tags.
- 'trigger' / 'poll trigger': Can only contain Trigger tags.

<!-- Uncomment to add example code -->
<!--### Example code-->

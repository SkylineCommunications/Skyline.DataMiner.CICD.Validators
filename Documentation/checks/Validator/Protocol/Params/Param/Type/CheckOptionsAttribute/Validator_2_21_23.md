---
uid: Validator_2_21_23
---

# CheckOptionsAttribute

## HeaderTrailerConnectionShouldBeValid

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

When a connection is specified then this header/trailer parameter will only be taken into account when the connection ID matches.
If the connection id is being specified on a different connection type, like SNMP, then it makes no sense to specify the parameter as header/trailer type on such a connection type.

### Example code

<Type options="headerTrailerLink=1;connection=0">

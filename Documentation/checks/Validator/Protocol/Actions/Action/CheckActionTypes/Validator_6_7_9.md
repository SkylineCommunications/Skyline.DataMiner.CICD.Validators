---
uid: Validator_6_7_9
---

# CheckActionTypes

## NonExistingConnectionRefInTypeNrAttribute

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

The 'Action/Type@nr' attribute should refer to an existing connection ID (0-based) in following cases:
- On group:
    - 'set' / 'set with wait' : allowing to set multiple SNMP parameters in one go. The refered connection should be of type SNMP (snmp, snmpV2 or snmpV3).

<!-- Uncomment to add example code -->
<!--### Example code-->

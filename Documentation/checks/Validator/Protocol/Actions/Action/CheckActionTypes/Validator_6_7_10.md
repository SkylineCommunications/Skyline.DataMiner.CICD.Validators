---
uid: Validator_6_7_10
---

# CheckActionTypes

## UnsupportedConnectionTypeDueTo

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

The 'Action/Type@nr' attribute should refer to the ID (0-based) of an existing connection in following cases:
- On group:
    - 'set' / 'set with wait' : allowing to set multiple SNMP parameters in one go. The refered connection should be of type SNMP (snmp, snmpV2 or snmpV3).

<!-- Uncomment to add example code -->
<!--### Example code-->

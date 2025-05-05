---
uid: Validator_1_26_1
---

# CheckConnectionPingGroups

## InvalidPingGroupType

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

When to define a poll group:
If a protocol has, at least, one group of type "poll" (no matter on which connection), then, the main connection should have a ping group defined in the protocol.

How to define a poll group:
No matter the (1st) connection type, if a group with id="-1" is defined, it will be the ping group.
Otherwise:
    - SNMP: the first group defined in the XML.
    - (smart-)serial: 
        - The pair with ping attribute set to true.
        - If no such pair, the pair with lowest ID.

<!-- Uncomment to add example code -->
<!--### Example code-->

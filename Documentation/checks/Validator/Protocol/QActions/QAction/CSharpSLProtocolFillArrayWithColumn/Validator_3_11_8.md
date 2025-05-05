---
uid: Validator_3_11_8
---

# CSharpSLProtocolFillArrayWithColumn

## UnrecommendedSetOnSnmpParam

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

It is typically unrecommended to update SNMP parameters from within a protocol.
Indeed, SNMP parameters should typically strictly be updated via polling.

Exceptions can sometimes be made when we update SNMP parameters based on received traps which contains, without any possible doubt, the new value.

Side note: There are alternatives to poll SNMP parameters in an efficient way when, for example, a cell or column or row needs to be updated but you don't want to poll the entire table:
- See various snmpSet options
- See dynamicSnmpGet feature.
- ...

<!-- Uncomment to add example code -->
<!--### Example code-->

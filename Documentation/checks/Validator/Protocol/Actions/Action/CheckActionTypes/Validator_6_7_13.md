---  
uid: Validator_6_7_13  
---

# CheckActionTypes

## UnsupportedGroupParamWithoutSnmp

### Details

The 'Action\/On@id' attribute should refer to the ID of an existing Group in following cases:  
\- On group:  
    \- 'set' \/ 'set with wait' : allowing to set multiple SNMP parameters in one go. The group content should refer to SNMP parameters Param\/Type 'write' and SNMP\/Enabled 'true'.

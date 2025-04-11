---  
uid: Validator_6_7_5  
---

# CheckActionTypes

## NonExistingParamRefInTypeIdAttribute

### Details

The 'Action\/Type@id' attribute should refer to existing parameter ID in following cases:  
\- On pair:  
    \- timeout: ID of the parameter that holds the timeout value (in ms).  
    \- set next: (optional) ID of the parameter containing the 'time to wait after pair' value (in ms).

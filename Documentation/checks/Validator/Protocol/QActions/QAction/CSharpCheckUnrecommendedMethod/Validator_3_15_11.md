---  
uid: Validator_3_15_11  
---

# CSharpCheckUnrecommendedMethod

## UnrecommendedNotifyProtocolNT\_GET\_KEY\_POSITION

### Details

'SLProtocol.NotifyProtocol(163\/\*NT\_GET\_KEY\_POSITION\*\/, ...)' method is now considered obsolete.  
Instead of relying on row positions, working directly with calls relying on primary keys is recommended  
Examples:  
 \- Use 'SLProtocol.GetParameterIndexByKey()' instead of 'SLProtocol.GetParameterIndex()'.

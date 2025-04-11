---  
uid: Validator_3_15_10  
---

# CSharpCheckUnrecommendedMethod

## UnrecommendedNotifyProtocolNT\_GET\_DATA

### Details

'SLProtocol.NotifyProtocol(60\/\*NT\_GET\_DATA\*\/, ...)' method is now considered unrecommended.  
Instead, the wrapper method 'SLProtocol.GetData()' is recommended.  
If the intention was only to check if the parameter is empty, then 'SLProtocol.IsEmpty()' is recommended.

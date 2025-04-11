---  
uid: Validator_3_15_19  
---

# CSharpCheckUnrecommendedMethod

## UnrecommendedNotifyProtocolNT\_ARRAY\_ROW\_COUNT

### Details

'SLProtocol.NotifyProtocol(195\/\*NT\_ARRAY\_ROW\_COUNT\*\/, ...)' method is now considered unrecommended.  
Instead, the wrapper method 'SLProtocol.RowCount()' is recommended.  
If the intention is to loop over rows based on the result, using a call to get columns straight away is recommended.

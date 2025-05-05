---
uid: Validator_3_15_10
---

# CSharpCheckUnrecommendedMethod

## UnrecommendedNotifyProtocolNT_GET_DATA

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

'SLProtocol.NotifyProtocol(60/*NT_GET_DATA*/, ...)' method is now considered unrecommended.

Instead, the wrapper method 'SLProtocol.GetData()' is recommended.

If the intention was only to check if the parameter is empty, then 'SLProtocol.IsEmpty()' is recommended.

<!-- Uncomment to add example code -->
<!--### Example code-->

---
uid: Validator_3_15_19
---

# CSharpCheckUnrecommendedMethod

## UnrecommendedNotifyProtocolNT_ARRAY_ROW_COUNT

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

'SLProtocol.NotifyProtocol(195/*NT_ARRAY_ROW_COUNT*/, ...)' method is now considered unrecommended.

Instead, the wrapper method 'SLProtocol.RowCount()' is recommended.

If the intention is to loop over rows based on the result, using a call to get columns straight away is recommended.

<!-- Uncomment to add example code -->
<!--### Example code-->

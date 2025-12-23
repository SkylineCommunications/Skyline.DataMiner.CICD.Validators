---
uid: Validator_3_15_11
---

# CSharpCheckUnrecommendedMethod

## UnrecommendedNotifyProtocolNT_GET_KEY_POSITION

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

'SLProtocol.NotifyProtocol(163/*NT_GET_KEY_POSITION*/, ...)' method is now considered obsolete.

Instead of relying on row positions, working directly with calls relying on primary keys is recommended

Examples:
 - Use 'SLProtocol.GetParameterIndexByKey()' instead of 'SLProtocol.GetParameterIndex()'.

<!-- Uncomment to add example code -->
<!--### Example code-->

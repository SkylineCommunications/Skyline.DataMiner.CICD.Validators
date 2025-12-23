---
uid: Validator_3_33_2
---

# CSharpSLProtocolGetParameters

## NonExistingParam

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

SLProtocol.GetParameters is used to get current values of standalone parameters.
Make sure to provide it with a uint array of existing standalone parameter IDs.
Using Parameter class is recommended.

### Example code

protocol.GetParameters(new uint[] { Parameter.ParameterName, Parameter.ParameterName2 });

---
uid: Validator_2_39_2
---

# CheckDisplayKey

## DisplayColumnSameAsPK

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

The excessive [ArrayOptions@displayColumn](https://docs.dataminer.services/develop/schemadoc/Protocol/Protocol.Params.Param.ArrayOptions-displayColumn.html) attribute definition has no added value and bad impact on the performance. We recommend to:

- Remove displayColumn option in any case as using it is considered bad practice.
- if a specific Display Key is needed, use [NamingFormat](https://docs.dataminer.services/develop/schemadoc/Protocol/Protocol.Params.Param.ArrayOptions.NamingFormat.html) tag to define it.

<!-- Uncomment to add example code -->
<!--### Example code-->

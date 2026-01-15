---
uid: Validator_2_78_1
---

# CheckVolatileTables

## SuggestedVolatileOption

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

The validator didn't find anything within the protocol that required the table data to be stored in database so we suggest to make use of the [volatile](https://docs.dataminer.services/develop/schemadoc/Protocol/Protocol.Params.Param.ArrayOptions-options.html#volatile) option. However, note that the validator cannot be sure so you'll need to double check if data needs to be persistent or not.

- if so, suppress this validator check.
- if not, use the [volatile](https://docs.dataminer.services/develop/schemadoc/Protocol/Protocol.Params.Param.ArrayOptions-options.html#volatile) option in order to spare precious resources.

<!-- Uncomment to add example code -->
<!--### Example code-->

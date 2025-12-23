---
uid: Validator_2_78_1
---

# CheckVolatileTables

## SuggestedVolatileOption

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

If this error message is raised by the validator, it means that nothing within the protocol code requires the need of storing the table data into the database. The results has the Certainty property as "Uncertain" because the validator can't be sure if data needs to be stored or not, it can only be sure that none of the protocol feature known to require saved data is present in the connector. It's now up to the developer to double check if the data really needs to be stored or not. If so, the validator result should be suppressed with explanation of why the data needs to be stored. If not, the [volatile option](https://docs.dataminer.services/develop/schemadoc/Protocol/Protocol.Params.Param.ArrayOptions-options.html#volatile) should be added.

<!-- Uncomment to add example code -->
<!--### Example code-->

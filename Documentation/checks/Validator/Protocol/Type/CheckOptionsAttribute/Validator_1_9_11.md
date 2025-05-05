---
uid: Validator_1_9_11
---

# CheckOptionsAttribute

## NonExistingId

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

Within the 'Protocol/Type@options' attribute, the 'exportProtocol' option allows to define which DVE protocols should be made and based on what parameter DVE elements should be created.

The exportProtocol option is expected to have the following format: "exportProtocol:[protocolName]:[DveTablePid]" optionally followed by ":noElementPrefix" where:
- [protocolName] should be the name of the DVE protocol to be created.
    - Note that we recommend the DVE protocol name to start with: "[DveParentProtocolName] - "
- [DveTablePid] should be the PID of the table responsible for creating DVE elements. The referred Param is expected to:
    - Be of type 'array'.
    - Have its RTDisplay tag set to true.

<!-- Uncomment to add example code -->
<!--### Example code-->

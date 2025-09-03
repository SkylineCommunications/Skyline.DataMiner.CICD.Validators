---
uid: Validator_2_77_1
---

# CheckSaveAttribute

## UnrecommendedSavedReadParam

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

Having a parameter being both saved and polled from the data-source seems inconsistent.

- A saved read parameter is typically used for configurations on the DataMiner element side so that user configuration can persist across restarts.
- A polled parameter will typically never need to be saved as we rely on the fact that the newer value will be polled again shortly after an element restart, no matter if such a parameter is a data parameter or a configuration parameter on the data-source side.

<!-- Uncomment to add example code -->
<!--### Example code-->

---
uid: Validator_2_77_1
---

# CheckSaveAttribute

## UnrecommendedSavedReadParam

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

Having a parameter being both saved and polled from the data-source seems inconsistent.

- A saved read parameter is typically used for configurations on the DataMiner element side so that user configuration can persist across restarts.
- A polled parameter will typically never need to be saved as we rely on the fact that the newer value will be polled again shortly after an element restart, no matter if such a parameter is a data parameter or a configuration parameter on the data-source side.

### How to fix

Depending on the use-case, you should either remove the save option or include another parameter within your response.

- Is your parameter corresponding to a user configuration on the DataMiner element side -> don't use it in your response.
- Is your parameter corresponding to data or configuration retrieved from your data-source -> no need to save it.

There might be some use-cases where you need to use a DataMiner element configuration parameter as a filter/validation on whether a data-source response should be accepted or not. In such cases, you will indeed want to save such configuration parameter but then, you should copy its value to a fixed (and non-saved) parameter and include it within your response so that the user configuration does not get overwritten by the data-source response.

<!-- Uncomment to add example code -->
<!--### Example code-->
